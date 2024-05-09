cmake_minimum_required(VERSION 3.12)
project(OpenCV_Wrapper)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)

FIND_PACKAGE(OpenCV REQUIRED gapi imgcodecs dnn)

add_library(opencv_wrapper SHARED opencv_wrapper.cpp)
target_include_directories(opencv_wrapper PRIVATE ${OpenCV_INCLUDE_DIRS})
target_link_libraries(opencv_wrapper ${OpenCV_LIBS})