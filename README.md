# .NET 5.0 Desktop App for Linux

A getting started project for .NET 5.0 with C# in Linux + OpenCV C++

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

### Run the app

```console
dotnet run
```

dotnet add package System.Drawing.Common --version 5.0.2
sudo apt install libgdiplus
