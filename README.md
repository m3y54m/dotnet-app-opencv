# .NET 5.0 Desktop App for Embedded Developers :)

An embedded developer friendly project for .NET 5.0 with C# in Linux capable of:

- Calling an OpenCV-based C++ function in a C# program
- Communication with a serial port

https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio-code

## Commands

### Create the app

```console
dotnet new console --framework net5.0
```

### Create `libsobel.so` C shared library

```console
cmake -S sobel_opencv_cpp/src -B sobel_opencv_cpp/build
cmake --build sobel_opencv_cpp/build
```
### Install .NET dependencies

-   Add support for Bitmap images

    ```console
    dotnet add package System.Drawing.Common --version 5.0.2
    sudo apt install libgdiplus
    ```

-   Add support for SerialPort communication

    ```console
    dotnet add package System.IO.Ports --version 5.0.1
    ```

### Run the app

```console
dotnet run
```


