#include <filesystem>
#include <fstream>
#include <iomanip>
#include "opencv2/dnn.hpp"
#include "opencv2/gapi/core.hpp"
#include "opencv2/gapi/gpu/core.hpp"
#include "opencv2/gapi/gpu/imgproc.hpp"
// #include "opencv2/gapi/cpu/gcpukernel.hpp"
#include "opencv2/gapi/infer.hpp"
#include "opencv2/gapi/infer/ie.hpp"
#include "opencv2/gapi/streaming/cap.hpp"
// #include "opencv2/highgui.hpp"
#include "opencv2/imgcodecs.hpp"
#include <string>
#include <sstream>

const char* const encoderXML = "FP16-INT8-action-recognition-0001-encoder.xml";
const char* const encoderBin = "FP16-INT8-action-recognition-0001-encoder.bin";
const char* const decoderXML = "FP16-INT8-action-recognition-0001-decoder.xml";
const char* const decoderBin = "FP16-INT8-action-recognition-0001-decoder.bin";

const char* const actions[] = {
    "abseiling", "air drumming", "answering questions", "applauding", "applying cream", "archery", "arm wrestling", "arranging flowers", "assembling computer", "auctioning",
    "baby waking up", "baking cookies", "balloon blowing", "bandaging", "barbequing", "bartending", "beatboxing", "bee keeping", "belly dancing", "bench pressing", "bending back",
    "bending metal", "biking through snow", "blasting sand", "blowing glass", "blowing leaves", "blowing nose", "blowing out candles", "bobsledding", "bookbinding", "bouncing on trampoline",
    "bowling", "braiding hair", "breading or breadcrumbing", "breakdancing", "brush painting", "brushing hair", "brushing teeth", "building cabinet", "building shed", "bungee jumping",
    "busking", "canoeing or kayaking", "capoeira", "carrying baby", "cartwheeling", "carving pumpkin", "catching fish", "catching or throwing baseball", "catching or throwing frisbee",
    "catching or throwing softball", "celebrating", "changing oil", "changing wheel", "checking tires", "cheerleading", "chopping wood", "clapping", "clay pottery making", "clean and jerk",
    "cleaning floor", "cleaning gutters", "cleaning pool", "cleaning shoes", "cleaning toilet", "cleaning windows", "climbing a rope", "climbing ladder", "climbing tree", "contact juggling",
    "cooking chicken", "cooking egg", "cooking on campfire", "cooking sausages", "counting money", "country line dancing", "cracking neck", "crawling baby", "crossing river", "crying",
    "curling hair", "cutting nails", "cutting pineapple", "cutting watermelon", "dancing ballet", "dancing charleston", "dancing gangnam style", "dancing macarena", "deadlifting",
    "decorating the christmas tree", "digging", "dining", "disc golfing", "diving cliff", "dodgeball", "doing aerobics", "doing laundry", "doing nails", "drawing", "dribbling basketball",
    "drinking", "drinking beer", "drinking shots", "driving car", "driving tractor", "drop kicking", "drumming fingers", "dunking basketball", "dying hair", "eating burger", "eating cake",
    "eating carrots", "eating chips", "eating doughnuts", "eating hotdog", "eating ice cream", "eating spaghetti", "eating watermelon", "egg hunting", "exercising arm",
    "exercising with an exercise ball", "extinguishing fire", "faceplanting", "feeding birds", "feeding fish", "feeding goats", "filling eyebrows", "finger snapping", "fixing hair",
    "flipping pancake", "flying kite", "folding clothes", "folding napkins", "folding paper", "front raises", "frying vegetables", "garbage collecting", "gargling", "getting a haircut",
    "getting a tattoo", "giving or receiving award", "golf chipping", "golf driving", "golf putting", "grinding meat", "grooming dog", "grooming horse", "gymnastics tumbling",
    "hammer throw", "headbanging", "headbutting", "high jump", "high kick", "hitting baseball", "hockey stop", "holding snake", "hopscotch", "hoverboarding", "hugging", "hula hooping",
    "hurdling", "hurling (sport)", "ice climbing", "ice fishing", "ice skating", "ironing", "javelin throw", "jetskiing", "jogging", "juggling balls", "juggling fire",
    "juggling soccer ball", "jumping into pool", "jumpstyle dancing", "kicking field goal", "kicking soccer ball", "kissing", "kitesurfing", "knitting", "krumping", "laughing",
    "laying bricks", "long jump", "lunge", "making a cake", "making a sandwich", "making bed", "making jewelry", "making pizza", "making snowman", "making sushi", "making tea",
    "marching", "massaging back", "massaging feet", "massaging legs", "massaging person's head", "milking cow", "mopping floor", "motorcycling", "moving furniture", "mowing lawn",
    "news anchoring", "opening bottle", "opening present", "paragliding", "parasailing", "parkour", "passing American football (in game)", "passing American football (not in game)",
    "peeling apples", "peeling potatoes", "petting animal (not cat)", "petting cat", "picking fruit", "planting trees", "plastering", "playing accordion", "playing badminton",
    "playing bagpipes", "playing basketball", "playing bass guitar", "playing cards", "playing cello", "playing chess", "playing clarinet", "playing controller", "playing cricket",
    "playing cymbals", "playing didgeridoo", "playing drums", "playing flute", "playing guitar", "playing harmonica", "playing harp", "playing ice hockey", "playing keyboard",
    "playing kickball", "playing monopoly", "playing organ", "playing paintball", "playing piano", "playing poker", "playing recorder", "playing saxophone", "playing squash or racquetball",
    "playing tennis", "playing trombone", "playing trumpet", "playing ukulele", "playing violin", "playing volleyball", "playing xylophone", "pole vault", "presenting weather forecast",
    "pull ups", "pumping fist", "pumping gas", "punching bag", "punching person (boxing)", "push up", "pushing car", "pushing cart", "pushing wheelchair", "reading book", "reading newspaper",
    "recording music", "riding a bike", "riding camel", "riding elephant", "riding mechanical bull", "riding mountain bike", "riding mule", "riding or walking with horse",
    "riding scooter", "riding unicycle", "ripping paper", "robot dancing", "rock climbing", "rock scissors paper", "roller skating", "running on treadmill", "sailing", "salsa dancing",
    "sanding floor", "scrambling eggs", "scuba diving", "setting table", "shaking hands", "shaking head", "sharpening knives", "sharpening pencil", "shaving head", "shaving legs",
    "shearing sheep", "shining shoes", "shooting basketball", "shooting goal (soccer)", "shot put", "shoveling snow", "shredding paper", "shuffling cards", "side kick",
    "sign language interpreting", "singing", "situp", "skateboarding", "ski jumping", "skiing (not slalom or crosscountry)", "skiing crosscountry", "skiing slalom", "skipping rope",
    "skydiving", "slacklining", "slapping", "sled dog racing", "smoking", "smoking hookah", "snatch weight lifting", "sneezing", "sniffing", "snorkeling", "snowboarding", "snowkiting",
    "snowmobiling", "somersaulting", "spinning poi", "spray painting", "spraying", "springboard diving", "squat", "sticking tongue out", "stomping grapes", "stretching arm",
    "stretching leg", "strumming guitar", "surfing crowd", "surfing water", "sweeping floor", "swimming backstroke", "swimming breast stroke", "swimming butterfly stroke",
    "swing dancing", "swinging legs", "swinging on something", "sword fighting", "tai chi", "taking a shower", "tango dancing", "tap dancing", "tapping guitar", "tapping pen",
    "tasting beer", "tasting food", "testifying", "texting", "throwing axe", "throwing ball", "throwing discus", "tickling", "tobogganing", "tossing coin", "tossing salad",
    "training dog", "trapezing", "trimming or shaving beard", "trimming trees", "triple jump", "tying bow tie", "tying knot (not on a tie)", "tying tie", "unboxing", "unloading truck",
    "using computer", "using remote controller (not gaming)", "using segway", "vault", "waiting in line", "walking the dog", "washing dishes", "washing feet", "washing hair",
    "washing hands", "water skiing", "water sliding", "watering plants", "waxing back", "waxing chest", "waxing eyebrows", "waxing legs", "weaving basket", "welding", "whistling",
    "windsurfing", "wrapping present", "wrestling", "writing", "yawning", "yoga", "zumba"
};

namespace custom {

    // Action encoder recognition: takes one Mat, returns another Mat
    G_API_NET(ActionEncoder, <cv::GMat(cv::GMat)>, "action-encoder-recognition");

    // Action decoder recognition: takes one Mat, returns another Mat
    G_API_NET(ActionDecoder, <cv::GMat(cv::GMat)>, "action-decoder-recognition");

}  // namespace custom

extern "C" __declspec(dllexport) int DetectGesture(void* shouldRunImageProcPtr, void* frameWasChangedPtr, void* errorLogPath, void* modelsPath, void* pixels) {
    try {
        cv::GComputation encoder([]() {
            cv::GMat in;
            cv::GMat detections = cv::gapi::infer<custom::ActionEncoder>(in);
            cv::GMat frame = cv::gapi::copy(in);
            return cv::GComputation(cv::GIn(in), cv::GOut(frame, detections));
        });

        cv::GComputation decoder([]{
            cv::GMat in;
            cv::GMat predictedActions = cv::gapi::infer<custom::ActionDecoder>(in);
            return cv::GComputation(cv::GIn(in), cv::GOut(predictedActions));
        });

        std::filesystem::path mPath (static_cast<char*>(modelsPath));
        auto encNet = cv::gapi::ie::Params<custom::ActionEncoder> {
            (mPath / encoderXML).string(),
            (mPath / encoderBin).string(),
            "GPU"
        };

        auto decNet = cv::gapi::ie::Params<custom::ActionDecoder> {
            (mPath / decoderXML).string(),
            (mPath / decoderBin).string(),
            "GPU"
        };

        auto networks = cv::gapi::networks(encNet, decNet); 

        const int decoderInputSize[] = {1, 16, 512};

        cv::Mat imgMat;
        cv::Mat encoderOutput, decoderOutput;
        cv::Mat decoderInput = cv::Mat(3, decoderInputSize, CV_32F);

        std::stringstream message;
        auto cc = encoder.compileStreaming(cv::compile_args(networks, cv::gapi::combine(cv::gapi::core::gpu::kernels(), cv::gapi::imgproc::gpu::kernels())));
        auto decoderGraph = decoder.compile(cv::descr_of(decoderInput), cv::compile_args(networks, cv::gapi::combine(cv::gapi::core::gpu::kernels(), cv::gapi::imgproc::gpu::kernels())));
        auto in_src = cv::gapi::wip::make_src<cv::gapi::wip::GCaptureSource>(0, std::map<int, double>{{cv::CAP_PROP_FRAME_WIDTH, 640.0}, {cv::CAP_PROP_FRAME_HEIGHT, 480.0}});
        uint8_t frames = 0;
        
        cc.setSource(cv::gin(in_src));
        cc.start();
        while(cc.running()) {
            auto output = cv::gout(imgMat, encoderOutput);
            const auto& shouldRunImageProc = *(static_cast<bool*>(shouldRunImageProcPtr));
            auto& frameWasChanged = *(static_cast<bool*>(frameWasChangedPtr));
            if (cc.pull(std::move(output)) && shouldRunImageProc == true) {
                if (frameWasChanged == false) {
                    cv::cvtColor(imgMat, imgMat, cv::COLOR_BGR2RGB);
                    cv::putText(imgMat, message.str(), {0, 50}, cv::FONT_HERSHEY_TRIPLEX, 1.0, {0, 255, 0}, 2);
                    cv::flip(imgMat, imgMat, 0);
                    std::memcpy(pixels, imgMat.data, imgMat.total() * imgMat.elemSize());
                    frameWasChanged = true;
                    if(frames != 16) {
                        std::memcpy(decoderInput.data + frames * 512 * 4, encoderOutput.data, 512 * 4);
                        ++frames;
                    } else {
                        frames = 0;
                        decoderGraph(decoderInput, decoderOutput);
                        float maxConfidence = 0.0;
                        size_t element = 0;
                        for (size_t i = 0; i < decoderOutput.total(); ++i) {
                            const auto& actualConfidence = decoderOutput.ptr<float>()[i];
                            if (actualConfidence > maxConfidence) {
                                maxConfidence = actualConfidence;
                                element = i;
                            }
                        }
                        message.str(std::string());
                        message << actions[element]  << " ( " << std::fixed << std::setprecision(2) << maxConfidence << " )";
                    }
                }
            } else {
                break;
            }
        }
        cc.stop();
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
    return -1;
}
