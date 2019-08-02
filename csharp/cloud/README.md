# Overview 

This is the README file for the C# *cloud* Visual Studio workspace (*cloud.code-workspace*), part of the C#/.NET Core version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *edge* workspace (*edge.code-workspace*) in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

This workspace contains 2 folders:

1. cloud- This folder contains an Azure Functions project, which builds an Azure Function called *CompressionFnc*, as shown in the architecture diagram.  The *CompressionFnc* serves as the cloud complement to the Azure IoT Edge compression module code.  It demonstrates decompressing a compressed message which sent from the *CompressionModule* through your Azure IoT Edge Hub service and writing the decompressed message to Azure Blob Storage.

2. shared - This folder contains two .NET library projects which are use in both the *edge* and *cloud* workspaces - *Compression* and *CompressionTests*.  

   *Compression* is a .NET Standard library project, used in both the Azure IoT Edge solution in the *edge* workspace and the Azure Functions project in the *cloud* workspace.  The *Compression* library itself is very simple and is intended for demonstration of the Complementary Code pattern.  It leverages the *GZipStream* compression class, included in the .NET Core Framework. 
   
   *CompressionTests* is a .NET Core *xUnit.net* unit test project.  The *Unit Testing Complementary Code* section of this document describes how to incorporate unit tests into the Azure Functions App local development loop and build process.

# Sharing Complementary Code in C#/.NET

The method for sharing code between an Azure IoT Edge module and an Azure Function project varies according to the code platform and associated options for publishing and importing code. 

.NET projects can leverage external code via direct references to another project or via references to downloaded NuGet packages. The *CompressionFnc* Azure Functions App project uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder.  Below is the line from the *CompressionFnc.csproj* which references the *Compression.csproj*:

```xml
  <ItemGroup>
    <ProjectReference Include="..\shared\Compression\Compression.csproj" />
  </ItemGroup>
```

At build time, the .NET compiler copies the *Compression* library binaries to the binary output folder of the *CompressionFnc*.  

# Getting Started

1. The Azure Functions runtime requires an Azure Blob Storage account to persist its runtime s

    T use this Azure function, copy the directory and open it within VS Code.

    You will need to update the value in `local.settings.json.temp`.  After updating, remove `.temp` from the filename (it is added with `.temp` to 
    provide a way to upload the necessary placeholders without exposing secrets).

    You will need to add a Blob named `test-out` in your storage account, or a name of your choosing.  If you choose something different, update `test-out` 
    in the parameters of the `CompressionCSharpFnc.cs` file.  Instead of *"test-out/{sys.randguid}"* it will be *"<your chosen name>/{sys.randguid}"*.

    
    
    
    
    https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code#publish-the-project-to-azure
    
    

https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code#publish-to-azure