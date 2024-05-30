#include <Python.h>
#include <filesystem>
#include <fstream>

extern "C" __declspec(dllexport) int RunPythonScript(void *errorLogPtr) {
   std::filesystem::path errorLogPath(static_cast<char*>(errorLogPtr));
   std::ofstream errorLogFile((errorLogPath / "error_log.txt").string(), std::ios::app);
   try{
        Py_Initialize();

            if (!Py_IsInitialized()) {
                throw new std::exception("Python could not be initialized.");
            }

            // Example Python code using transformers and PyTorch
            const char* code = R"(
        import torch
        from transformers import pipeline

        classifier = pipeline("sentiment-analysis")

        result = classifier("I love using transformers library!")
        print(result)
            )";

            // Run the Python code
            PyRun_SimpleString(code);

            Py_Finalize();
            return 0;
   }
   catch(std::exception e){
            errorLogFile << "std::exception occured: " << e.what() << std::endl;
    }
       catch(...){
            errorLogFile << "Unknown exception occured " << std::endl;
    }
    return -1;
}