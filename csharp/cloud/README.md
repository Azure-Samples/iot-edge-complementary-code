# Overview 

This is the README file for the C# *cloud* Visual Studio workspace (*cloud.code-workspace*),  part of the C#/.NET Core version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *edge* workspace (*edge.code-workspace*) in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

After installing prerequisites, make sure to follow the instructions in the [EdgeDevelopment.md](../../EdgeDevelopment.md) to configure your development environment for building the samples.  

This workspace contains 2 folders:

1. cloud- This folder contains an Azure Functions App project  as shown in the architecture diagram.  This Azure function project serves as the Cloud complement to the IoT Edge compression module code.  It demonstrates decompressing a compressed message which is sent from the IoT Edge module through Edge Hub and writing the decompressed message to Azure Blob Storage.

2. shared - This folder contains two .NET library projects - *Compression* and *CompressionTests*.  *Compression* is the compression library code used by both the Azure Fiunctions App project in this workspace and the Azure IoT Edge solution in the *edge* workspace. The *Compression* library uses the *GZipStream* compression class, included in the .NET Core Framework. *CompressionTests* is an xUnit.net unit test application.

   

# Sharing code in C#/.NET Core Azure IoT Edge Modules

The method for sharing code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. 

.NET projects can leverage external code via direct references to another project or via references to downloaded NuGet packages. The *CompressionFnc* Azure Funtions App project uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder.  Below is the line from the *CompressionFnc.csproj* which references the *Compression.csproj*:

```xml
  <ItemGroup>
    <ProjectReference Include="..\shared\Compression\Compression.csproj" />
  </ItemGroup>
```

At build time, the .NET compiler copies the *Compression* library binaries to the binary output folder of the *CompressionFnc*.  

Debugging C# Azure IoT Edge Modules in Visual Studio Code

# Getting Started

1. How to use

    To use this Azure function, copy the directory and open it within VS Code.

    You will need to update the value in `local.settings.json.temp`.  After updating, remove `.temp` from the filename (it is added with `.temp` to 
    provide a way to upload the necessary placeholders without exposing secrets).

    You will need to add a Blob named `test-out` in your storage account, or a name of your choosing.  If you choose something different, update `test-out` 
    in the parameters of the `CompressionCSharpFnc.cs` file.  Instead of *"test-out/{sys.randguid}"* it will be *"<your chosen name>/{sys.randguid}"*.

    


# Build and Test

To test the `ModuleDataCompression.cs` functionality, you can use the tests within the IoT Edge __CompressionCSharpModuleTests__ folder.  Instructions for how
those tests are run with the IoT Edge CompressionCSharpModule are included in the README.md in that project in the _Build and Test_ section.

# Contribute

To contribute to this code either file an issue with the details of the bug along with repro steps or 
make a pull request explaining the issue which you've run into that it addresses along with repro steps.