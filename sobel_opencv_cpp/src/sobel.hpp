#include <opencv2/opencv.hpp>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

extern "C"
{
    void processImage(int width, int height, int depth, int channels, int step, uchar *imagePointer);
}
