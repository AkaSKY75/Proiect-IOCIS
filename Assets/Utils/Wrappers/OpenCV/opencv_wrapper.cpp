#include <filesystem>
#include <fstream>
#include <iomanip>
#include <opencv2/core.hpp>
#include <opencv2/dnn.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/videoio.hpp>
#include <string>
#include <sstream>
#include <utility>
#include <vector>

const char* const humanPose0007XML = "FP16-human-pose-estimation-0007.xml";
const char* const humanPose0007Bin = "FP16-human-pose-estimation-0007.bin";

enum BodyPARTS {
    LEFT_ELBOW = 7,
    RIGHT_ELBOW,
    LEFT_HAND,
    RIGHT_HAND,
    MAX_BODY_PART
};

enum SupportedActions {
    UNKNOWN_ACTION = -1,
    ARMS_UP
};

namespace {
    inline float LogitsToProbability(float logitsVector) {
        return 1/(1+std::exp(-1*(logitsVector)));
    }
}

extern "C" __declspec(dllexport) int DetectGesture(void* shouldRunImageProcPtr, 
            void* frameWasChangedPtr, void* errorLogPath, void* modelsPath, 
            void* pixels, void* detectedGesturePtr, const int width, const int height) {
    try {
        cv::Mat videoFrame;
        cv::Mat audioFrame;
        cv::VideoCapture cap;
        std::vector<int> params {    
                                    cv::CAP_PROP_AUDIO_STREAM, 0,
                                    cv::CAP_PROP_VIDEO_STREAM, 0,
                                    cv::CAP_PROP_AUDIO_DATA_DEPTH, CV_16S,
                                    cv::CAP_PROP_FRAME_WIDTH, width,
                                    cv::CAP_PROP_FRAME_HEIGHT, height,
                                    cv::CAP_PROP_FPS, 30
                                };
        std::vector<cv::String> outputLayerNames = {"heatmaps", "embeddings"};
        std::vector<cv::Mat> outputs;

        cap.open(0, cv::CAP_ANY, params);
        if (!cap.isOpened())
        {
            throw new std::exception("Cannot open video capture!");
            return -1;
        }

        const int audioBaseIndex = (int)cap.get(cv::CAP_PROP_AUDIO_BASE_INDEX);
        const int numberOfChannels = (int)cap.get(cv::CAP_PROP_AUDIO_TOTAL_CHANNELS);
        const auto& shouldRunImageProc = *static_cast<bool*>(shouldRunImageProcPtr);
        auto& frameWasChanged = *static_cast<bool*>(frameWasChangedPtr);

        std::filesystem::path mPath(static_cast<char*>(modelsPath));

        cv::dnn::Net humanPoseEstimation0007Net = cv::dnn::readNetFromModelOptimizer((mPath / humanPose0007XML).string(),
                                                            (mPath / humanPose0007Bin).string());

        humanPoseEstimation0007Net.setPreferableBackend(cv::dnn::DNN_BACKEND_INFERENCE_ENGINE);
        humanPoseEstimation0007Net.setPreferableTarget(cv::dnn::DNN_TARGET_OPENCL);

        const int size[] = {1, 3, 224, 224};
        const uint8_t numKeypoints = 17;
        const uint8_t heatMapWidth = 224;
        const uint8_t heatMapHeight = 224;
        const size_t heatMapSize = heatMapHeight * heatMapWidth;

        const size_t armsUpHorizontalThreshold = 30; // pixels
        uint8_t bodyPartsFound = 0;

        std::map<size_t, std::map<uint8_t, cv::Point>> embeddingClusters;
        humanPoseEstimation0007Net.setInput(cv::Mat(4, size, CV_32F));
        humanPoseEstimation0007Net.forward(outputs, outputLayerNames); // first inference should be made with dummy input for time-taking initializations in memory...

        while(shouldRunImageProc == true)
        {
            if (cap.grab())
            {
                cap.retrieve(videoFrame);
                for (int nCh = 0; nCh < numberOfChannels; nCh++)
                {
                    cap.retrieve(audioFrame, audioBaseIndex+nCh);
                    if (!audioFrame.empty()) {
                        // audioData[nCh].push_back(audioFrame);
                    }
                    // numberOfSamples+=audioFrame.cols;
                }
                if (!videoFrame.empty())
                {
                    if (frameWasChanged == false) {
                        cv::Mat blob;

                        embeddingClusters.clear();
                        bodyPartsFound = 0;

                        videoFrame.convertTo(blob, CV_32F);
                        cv::cvtColor(videoFrame, videoFrame, cv::COLOR_BGR2RGB);
                        blob = cv::dnn::blobFromImage(blob, 1, cv::Size(448, 448));
                        humanPoseEstimation0007Net.setInput(blob);
                        humanPoseEstimation0007Net.forward(outputs, outputLayerNames);
                        for(uint8_t k = 0; k < numKeypoints; ++k) {
                            cv::Mat heatmap(heatMapHeight, heatMapWidth, CV_32F, outputs[0].ptr<float>() + k * heatMapSize);
                            cv::Mat embedding(heatMapHeight, heatMapWidth, CV_32F, outputs[1].ptr<float>() + k * heatMapSize);

                            cv::Point maxLoc;
                            double maxVal;
                            cv::minMaxLoc(heatmap, nullptr, &maxVal, nullptr, &maxLoc);

                            if (maxVal > 0.2) { // Threshold
                                if (k == BodyPARTS::LEFT_ELBOW || k == BodyPARTS::RIGHT_ELBOW ||
                                    k == BodyPARTS::LEFT_HAND  || k == BodyPARTS::RIGHT_HAND) {
                                        bodyPartsFound++;
                                }
                                // size_t clusterId = static_cast<size_t>(embedding.at<float>(maxLoc));
                                maxLoc.x = static_cast<int>(maxLoc.x * (videoFrame.cols / static_cast<float>(heatMapWidth)));
                                maxLoc.y = static_cast<int>(maxLoc.y * (videoFrame.rows / static_cast<float>(heatMapHeight)));
                                cv::circle(videoFrame, maxLoc, 3, cv::Scalar(0, 255, 0), -1);
                                embeddingClusters[/*clusterId*/0].insert({k, maxLoc});
                            }
                        }
                        // cv::imwrite((mPath / "imagine.png").string(), videoFrame);
                        if (bodyPartsFound == BodyPARTS::MAX_BODY_PART % BodyPARTS::LEFT_ELBOW) {
                            // left hand must be upside right elbow for arms up
                            auto verticalLeftDif = embeddingClusters[0][BodyPARTS::LEFT_ELBOW].y -
                                                    embeddingClusters[0][BodyPARTS::LEFT_HAND].y;
                            // right hand must be upside right elbow for arms up
                            auto verticalRightDif = embeddingClusters[0][BodyPARTS::RIGHT_ELBOW].y -
                                                    embeddingClusters[0][BodyPARTS::RIGHT_HAND].y;
                            // x axis diferences between elbows and hands must be close to 0
                            auto horizontalLeftDif = std::abs(embeddingClusters[0][BodyPARTS::LEFT_ELBOW].x - 
                                                            embeddingClusters[0][BodyPARTS::LEFT_HAND].x);
                            auto horizontalRightDif = std::abs(embeddingClusters[0][BodyPARTS::RIGHT_ELBOW].x - 
                                                            embeddingClusters[0][BodyPARTS::RIGHT_HAND].x);

                            auto& detectedGesture = *static_cast<int*>(detectedGesturePtr);
                            detectedGesture = UNKNOWN_ACTION;
                            if (verticalLeftDif > 0 && verticalRightDif > 0 && // hands must be above elbows
                                horizontalLeftDif < armsUpHorizontalThreshold &&
                                horizontalRightDif < armsUpHorizontalThreshold) {
                                    detectedGesture = ARMS_UP;
                            }
                        
                        }
                        cv::flip(videoFrame, videoFrame, -1);
                        std::memcpy(pixels, videoFrame.ptr<const uint8_t>(), videoFrame.total() * videoFrame.elemSize());
                        frameWasChanged = true;
                    }
                }
            } else { break; }
        }

        return 0;
    } catch(cv::Exception e) {
        std::ofstream f(static_cast<char*>(errorLogPath), std::ios_base::app);
        f << "CV exception occurred on `DetectGesture` function: " << e.what() << std::endl;
        f.close();
    } catch(std::exception e) {
        std::ofstream f(static_cast<char*>(errorLogPath), std::ios_base::app);
        f << "std::exception occurred on `DetectGesture` function: " << e.what() << std::endl;
        f.close();
    } catch(...) {
        std::ofstream f(static_cast<char*>(errorLogPath), std::ios_base::app);
        f << "Unknown exception occurred on `DetectGesture` function!" << std::endl;
        f.close();
    }
    auto& frameWasChanged = *(static_cast<bool*>(frameWasChangedPtr));
    frameWasChanged = true;
    return -1;
}
