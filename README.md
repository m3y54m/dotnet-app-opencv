# .NET 5.0 Desktop App for Linux

A getting started project for .NET 5.0 with C# in Linux

https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio-code

## Commands

### Create the app

```console
dotnet new console --framework net5.0
```

### Create `libhello.so` C shared library

```console
cmake -S hello_cpp/src -B hello_cpp/build
cmake --build hello_cpp/build
```

### Run the app

```console
dotnet run
```