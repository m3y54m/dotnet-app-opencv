#include <opencv2/opencv.hpp>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

extern "C"
{
    void SobelOpenCV(uchar *imageData, int height, int width);
}
