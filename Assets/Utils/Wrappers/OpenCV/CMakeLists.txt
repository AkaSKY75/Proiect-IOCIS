cmake_minimum_required(VERSION 3.12)
project(OpenCV_Wrapper)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)

#3include_directories("${PROJECT_SOURCE_DIR}/SDL")
# add_subdirectory(SDL)

FIND_PACKAGE(OpenCV REQUIRED core imgcodecs)

add_library(opencv_wrapper SHARED opencv_wrapper.cpp)
target_include_directories(opencv_wrapper PRIVATE ${OpenCV_INCLUDE_DIRS})
target_link_libraries(opencv_wrapper ${OpenCV_LIBS})