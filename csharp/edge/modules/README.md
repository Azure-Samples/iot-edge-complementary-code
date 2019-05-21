# Introduction 

This (Azure) IoT Edge module project serves to illustrate a pattern for separating business logic from Node package logic and a way to include package tests. 

It provides compression/decompression code using zlib with gzip which can be reused in the Cloud to perform the complementary operation (ie. compress on device and decompress in the cloud).  

The complementary operation is designed such that with input A, and functions F, F', A will be transformed to A' and can be recovered to its original state. A->F(A)->A'->F'(A')->A

# Getting Started

1. How to use

    To use this module, copy the directory into your VS Code IoT Edge Solution `\modules` folder.
    The routes for this module are
    
    - `/modules/CompressionCSharpModule/inputs/compressMessage`
    - `/modules/CompressionCSharpModule/inputs/decompressMessage`
    
    The routes correspond to the `compressMessage` and `decompressMessage` functions in the `ModuleDataCompression.cs` file with the
    following output routes:

    - `/modules/CompressionCSharpModule/outputs/compressMessageOutput`
    - `/modules/CompressionCSharpModule/outputs/decompressMessageOutput`
    
    To use the complementary operation in the cloud, copy the `ModuleDataCompression.cs` file to another C# 
    application such an Azure Function using C# with IoTHub Trigger and include it in a `using` 
    statement as shown in `Program.cs`.

2.	Software dependencies

    The primary dependency for this module is the [Azure IoT Edge OSS project](https://github.com/Azure/iotedge).

    The `CompressionCSharpModule.csproj` file includes the other packages which are normally included with an Azure IoT Edge module.

    The accompanying tests use Xunit.

3.  Version

    Current & released version is 0.0.1 which includes basic business logic to pass an IoT Edge message through Program.cs
    without modification into the compress or decompress function and send the modified message back to the edgeHub for
    routing as described in the deployment file.

# Build and Test

To test the `ModuleDataCompression.cs` functionality, within the __CompressionCSharpModuleTests__ folder, run `dotnet test` from the command line or use the VS Code Debugger functionality from [OmniSharp](https://github.com/OmniSharp/omnisharp-vscode/wiki).

The _launch.json_ Help and _Run/Debug Unit Tests_ pages are particularly helpful.

# Contribute

To contribute to this code either file an issue with the details of the bug along with repro steps or 
make a pull request explaining the issue which you've run into that it addresses along with repro steps.