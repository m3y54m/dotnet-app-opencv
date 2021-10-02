# Make Libraries with CMake

Creating a shared or static library using CMake

[How to create a shared library with cmake?](https://stackoverflow.com/questions/17511496/how-to-create-a-shared-library-with-cmake)

All the instructions needed to build the libraries and executables are described in `CMakeLists.txt` file.

In order to generate the Makefile and other files used to
build this project in a directory called `build` first go to the root of this repository and run this command:

```console
cmake -S src -B build
```

Now generate the libraries and executables in `build` directory:

```console
cmake --build build
```

For more information go to https://github.com/m3y54m/library-with-cmake
