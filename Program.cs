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

            // Create a copy of input image to be used by processImageByCsharp
            Bitmap bmpBackup = new Bitmap(bmp);
            
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0,
                                                                bmp.Width, bmp.Height),
                                                                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                                                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                int step = bitmapData.Stride;
                int width = bitmapData.Width;
                int height = bitmapData.Height;
                int channels = 3;
                int depth = 0; // CV_8U

                byte* imagePointer = (byte*)bitmapData.Scan0;

                processImage(width, height, depth, channels, step, imagePointer);
            }
            // Unlock the bits.
            bmp.UnlockBits(bitmapData);
            bmp.Save("output-opencv.jpg");
            
            // Image processing by C#
            processImageByCsharp(bmpBackup);

            bmpBackup.Save("output-csharp.jpg");
        }

        private static void processImageByCsharp(Bitmap bmp)
        {
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0,
                                                                bmp.Width, bmp.Height),
                                                                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                                                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            IntPtr imagePointer = bitmapData.Scan0;
            int step = bitmapData.Stride;
            int width = bitmapData.Width;
            int height = bitmapData.Height;
            int channels = 3;

            // Convert Colored image to Grayscale
            unsafe
            {
                byte* imagePtrByte = (byte*)imagePointer;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int b = imagePtrByte[0];
                        int g = imagePtrByte[1];
                        int r = imagePtrByte[2];

                        int avg = (b + g + r) / 3;

                        b = g = r = avg;

                        imagePtrByte[0] = (byte)b;
                        imagePtrByte[1] = (byte)g;
                        imagePtrByte[2] = (byte)r;

                        imagePtrByte += channels;
                    }

                    imagePtrByte += step - (width * channels);
                }
            }

            // Unlock the bits
            bmp.UnlockBits(bitmapData);
        }
    }
}
