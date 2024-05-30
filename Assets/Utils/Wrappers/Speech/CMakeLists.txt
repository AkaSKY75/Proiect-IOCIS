cmake_minimum_required(VERSION 3.10)
project(SpeechWrapper LANGUAGES CXX)
set(CMAKE_CXX_STANDARD 17)
# Set the output directory for the DLL
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# Find Python
find_package(Python3 COMPONENTS Interpreter Development REQUIRED)
if (NOT Python3_FOUND)
    message(FATAL_ERROR "Python3 not found")
endif()

# Include directories
include_directories(${Python3_INCLUDE_DIRS})

# Create the DLL
add_library(speech_wrapper SHARED speech_wrapper.cpp)

# Link against Python libraries
target_link_libraries(speech_wrapper PRIVATE ${Python3_LIBRARIES})