using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace dotnet_app
{
    class Program
    {
        [DllImport("./hello_cpp/build/libhello.so")] static extern void Hello();

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

            Console.WriteLine(String.Format("{0}, {1}, {2}", step, bmp.Width, bmp.Height));

            //SobelOpenCV((IntPtr)imagePointer1, bmp.Height, bmp.Width);

            // Unlock the bits.
            bmp.UnlockBits(bitmapData);

            bmp.Save("output.jpg");

            Hello();
        }
    }
}

// https://stackoverflow.com/questions/39579398/opencv-how-to-create-mat-from-uint8-t-pointer