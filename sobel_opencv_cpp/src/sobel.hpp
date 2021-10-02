#include <opencv2/opencv.hpp>
#include <iostream>

// extern "C" is necessary for C++ libraries
extern "C"
{
    void processImage(int width, int height, int depth, int channels, int step, uchar *imagePointer);
}
