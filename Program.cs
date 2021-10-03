using System;
// Import C++ libraries and functions
using System.Runtime.InteropServices;
// Open, manipulate, and save images 
using System.Drawing;
// Communicate with serial port
using System.IO.Ports;

namespace dotnet_app
{
    class Program
    {
        // Load processImage function from `libsobel.so` shared (dynamic) library
        [DllImport("./sobel_opencv_cpp/build/libsobel.so")] static extern unsafe void processImage(int width, int height, int depth, int channels, int step, byte* imagePointer);

        // Serial Port
        static bool isSerialPortAvailable;
        static SerialPort mySerialPort;
        static string defaultSerialPortName = "/dev/ttyACM0";

        static void Main(string[] args)
        {
            // Open a sample image file for imgae processing
            Bitmap bmp = new Bitmap(Image.FromFile("monarch.jpg"));
            // Create a copy of input image to be used by processImageByCsharp
            Bitmap bmpBackup = new Bitmap(bmp);

            // Image processing by OpenCV in C++
            processImageByCpp(bmp);
            bmp.Save("tmp/output-opencv.jpg");

            // Image processing by C#
            processImageByCsharp(bmpBackup);
            bmpBackup.Save("tmp/output-csharp.jpg");

            // Open the serial port
            mySerialPort = new SerialPort();
            mySerialPort.BaudRate = 9600;
            isSerialPortAvailable = openMySerialPort(defaultSerialPortName);

            if (isSerialPortAvailable)
            {
                while (true)
                {
                    if (mySerialPort.BytesToRead > 0)
                    {
                        string line = mySerialPort.ReadLine();
                        Console.WriteLine(line);
                    }
                }
            }
            else
            {
                Console.WriteLine("Unable to open the serial port.");
            }
        }

        private static bool openMySerialPort(string serialPortName)
        {
            string[] portNames = SerialPort.GetPortNames();

            if (portNames.GetLength(0) == 0)
            {
                // There is no serial port available
                return false;
            }
            else
            {
                foreach (string portName in portNames)
                {
                    if (portName == serialPortName)
                    {
                        // Found the serial port name
                        mySerialPort.PortName = serialPortName;
                        if (!mySerialPort.IsOpen)
                        {
                            mySerialPort.Open();
                            // Desired serial port opened successfully
                            return true;
                        }
                        else
                        {
                            // Desired serial port is busy
                            return false;
                        }
                    }
                }

                // Desired serial port name is not found
                return false;
            }
        }

        private static void processImageByCpp(Bitmap bmp)
        {
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
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

            bmp.UnlockBits(bitmapData);
        }


        private static void processImageByCsharp(Bitmap bmp)
        {
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
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

            bmp.UnlockBits(bitmapData);
        }
    }
}
