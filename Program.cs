using System;
// Import C++ libraries and functions
using System.Runtime.InteropServices;
// Open, manipulate, and save images 
using System.Drawing;
// Communicate with serial port
using System.IO.Ports;
// Thread.Sleep and Multi-threading
using System.Threading;
// Queues
using System.Collections.Generic;

namespace dotnet_app
{
    class Program
    {
        // Load processImage function from `libsobel.so` shared (dynamic) library
        [DllImport("./sobel_opencv_cpp/build/libsobel.so")] static extern unsafe void processImage(int width, int height, int depth, int channels, int step, byte* imagePointer);

        // Serial Port
        static SerialPort mySerialPort;
        const string defaultSerialPortName = "/dev/ttyACM0";

        // Serial Port Data Processing
        static Queue<byte> queueSerialPortBytes = new Queue<byte>();
        private static object lockQueue = new object();
        static AutoResetEvent someSerialPortDataIsReady = new AutoResetEvent(false);
        static Thread serialPortThread;

        static bool quitCommand = false;


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

            // Serial port communication
            mySerialPort = new SerialPort();
            mySerialPort.BaudRate = 9600;
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(addSerialPortDataToQueue);
            bool isSerialPortAvailable = openMySerialPort(defaultSerialPortName);

            // Create a new thread for processing serial port incoming data byte by byte
            serialPortThread = new Thread(getSerialPortDataFromQueue);

            if (isSerialPortAvailable)
            {
                serialPortThread.Start();

                while (!quitCommand)
                {
                    // This is used to check the `q` (quit command) in main thread
                    char command = Console.ReadKey().KeyChar;

                    if (command.Equals('q'))
                    {
                        quitCommand = true;
                    }
                }

                serialPortThread.Join();
                mySerialPort.Close();
                Console.WriteLine("\rQuit signal is received.");
            }
            else
            {
                Console.WriteLine("Unable to open the serial port.");
            }
        }

        private static void addSerialPortDataToQueue(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (mySerialPort.BytesToRead > 0)
                {
                    // Get a chunk of bytes available in serial port buffer
                    byte[] chunk = new byte[mySerialPort.BytesToRead];
                    mySerialPort.Read(chunk, 0, chunk.Length);

                    // Add the chunk of bytes to queue
                    lock (lockQueue)
                    {
                        for (int i = 0; i < chunk.Length; i++)
                            queueSerialPortBytes.Enqueue(chunk[i]);
                    }

                    someSerialPortDataIsReady.Set();
                }
            }
            catch
            {
                //...
            }
        }

        //This thread processes the stored chunks doing the less locking possible
        private static void getSerialPortDataFromQueue(object state)
        {
            while (!quitCommand)
            {
                someSerialPortDataIsReady.WaitOne();

                lock (lockQueue)
                {
                    while (queueSerialPortBytes.Count > 0)  // process each single byte
                    {
                        // get (pop) one byte from queue of serial port received bytes
                        byte byteFromSerialPort = queueSerialPortBytes.Dequeue();

                        // All application-specific processing of serial port bytes is done here
                        prossesSerialPortByte(byteFromSerialPort);
                    }
                }
            }
        }

        private static void prossesSerialPortByte(byte inputByte)
        {
            // Print each received char
            Console.Write(Convert.ToChar(inputByte));
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
