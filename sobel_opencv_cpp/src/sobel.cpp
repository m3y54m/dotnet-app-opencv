#include "sobel.hpp"

using namespace cv;
using namespace std;

void processImage(int width, int height, int depth, int channels, int step, uchar *imagePointer)
{
    int type = CV_MAKETYPE(depth, channels);

    // Create a 'new' Mat object using Mat data addressed by 'imagePointer'
    Mat tmp = Mat(height, width, type, imagePointer, step);

    // ------------------------------------------------------
    // Main image processing operations are done here:
    // [Sobel filter]
    // ------------------------------------------------------
    // Convert to grayscale
    cvtColor(tmp, tmp, COLOR_BGR2GRAY);

    // Generate grad_x and grad_y
    Mat grad_x, grad_y;
    Mat abs_grad_x, abs_grad_y;
    int scale = 1;
    int delta = 0;

    // Gradient X
    Sobel(tmp, grad_x, depth, 1, 0, 3, scale, delta, BORDER_DEFAULT);
    convertScaleAbs(grad_x, abs_grad_x, 1, 0);

    // Gradient Y
    Sobel(tmp, grad_y, depth, 0, 1, 3, scale, delta, BORDER_DEFAULT);
    convertScaleAbs(grad_y, abs_grad_y, 1, 0);

    /// Total Gradient (approximate)
    addWeighted(abs_grad_x, 2.0, abs_grad_y, 2.0, 0, tmp);    

    // To keep the color space of the result image the same as input (BGR)
    cvtColor(tmp, tmp, COLOR_GRAY2BGR);
    // ------------------------------------------------------

    // Overwrite data of 'tmp' Mat to the address of 'imagePointer'
    memcpy(imagePointer, tmp.data, width * height * channels * sizeof(uchar));
}
