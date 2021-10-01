using System;
using System.Runtime.InteropServices;

namespace dotnet_app
{
    class Program
    {
        [DllImport("libc.so.6")] static extern int puts(string str);
        [DllImport("./hello_cpp/build/libhello.so")] static extern void Hello();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from C#");

            puts("Hello World from puts() of C");

            Hello();
        }
    }
}
