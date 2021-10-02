using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace dotnet_app
{
    class Program
    {
        [DllImport("./sobel_opencv_cpp/build/libsobel.so")] static extern unsafe void processImage(int width, int height, int depth, int channels, int step, byte * imagePointer);
        

        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(Image.FromFile("monarch.jpg"));
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0,
                                                                bmp.Width, bmp.Height),
                                                                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                                                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            IntPtr imagePointer = bitmapData.Scan0;
            int step = bitmapData.Stride;
            int width = bitmapData.Width;
            int height = bitmapData.Height;
            int channels = 3;
            int depth = 0; // CV_8U

            unsafe
            {
                byte* imagePtrByte = (byte*)imagePointer;

                // for (int i = 0; i < height; i++)
                // {
                //     for (int j = 0; j < width; j++)
                //     {
                //         int b = imagePtrByte[0];
                //         int g = imagePtrByte[1];
                //         int r = imagePtrByte[2];

                //         int avg = (b + g + r) / 3;

                //         b = g = r = avg;

                //         imagePtrByte[0] = (byte)b;
                //         imagePtrByte[1] = (byte)g;
                //         imagePtrByte[2] = (byte)r;

                //         imagePtrByte += channels;
                //     }

                //     imagePtrByte += step - (width * channels);
                // }
                processImage(width, height, depth, channels, step, imagePtrByte);

            }

            // Unlock the bits.
            bmp.UnlockBits(bitmapData);

            bmp.Save("output.jpg");
        }
    }
}

// https://stackoverflow.com/questions/39579398/opencv-how-to-create-mat-from-uint8-t-pointer