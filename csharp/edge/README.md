# Overview 
This folder contains an Azure IoT Edge Solution which builds an Azure IoT Edge compression module, a part of the C#/.NET Core version of the Complementary Code Pattern sample.  For a high level overview of the both the pattern and the sample, along with **prerequites** for building and running the sample, start with the top level [README.md](../../README.md) in this repository.

Azure IoT Edge Solutions in Visual Studio Code are organized by a root folder containing the Azure IoT Edge Deployment manifests (*deployment.template.json* and *deployment.debug.template.json*) and a *modules* subfolder, which contains the module manifests (*module.json*), Dockerfiles and language specific code for building modules listed in the manifest.  The Azure IoT Edge Visual Studio Code extension recognizes this structure and activates when the root folder is opened in Visual Studio code or as a workspace folder in Visual Studio Code.

This sample assumes basic familiarity with Azure IoT Edge Modules and how to build them with Visual Studio Code.  For an introduction to building an IoT Edge Modules in C#, refer to 
[Tutorial: Develop a C# IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-csharp-module).

This Azure IoT Edge Solution builds two modules - *CompressionModule* and *MessageSimulatorModule*, which are located under the *modules* folder.  Each module is built as a .NET Core console application and includes a .NET Core project file. 

The *CompressionModule* is designed to illustrate the Complementary Code pattern to share logic between Edge components and the Cloud.  The *CompressionModule* project file references the *Compression* .NET library, located in the *shared/Compression* folder.  The *Compression* library module relies on the Gzip compression and decompression the xxx NuGet package.  For a description of the shared compression code, see the [README.md](..\shared\Compression\README.md) in the *shared/Compression* folder of this workspace. 

For demonstration, *MessageSimulatorModule* is also included  in the Azure IoT Edge solution.  This module plays back sample test messages from the *messages* folder in this workspace and sends them to the compression module using an Azure IoT Edge route. 

Azure IoT Edge routes are defined in the deployment manifests.  This solution has two routes defined - *SimulatorToCompression* and *CompressionToIoTHub*.  The *SimulatorToCompression* route instructs the Azure IoT Edge Hub to route message from all outputs of the *MessageSimulatorModule* to the *compressMessage* input of the *CompressionModule*.  The code in the *CompressionModule* binds the *compressMessage* route to its compressMessage method.  There is also a *decompressMesage* method bound to the *decompressMessage* method, but it is included only for illustration and not used in the sample.  Finally, the *CompressionToIoTHub* route instructs the Azure IoT Edge Hub to send all output messages to the parent Azure IoT Hub.  

## Sharing code in C#/.NET Core Azure IoT Edge Modules

The method for sharing code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. .NET projects can leverage external code via direct references to another project or via references to NuGet packages. The *CompressionModule* uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder. 

The .NET compiler includes referenced libraries in the binary output folder of the main application.

Referencing another .NET project is relatively simple, but special considerations had to be made in the CompressionModule Dockerfiles to accomodate project files outside of the Azure IoT Edge Solution Folder.

```xml
 <ItemGroup>
    <ProjectReference Include="..\..\..\shared\Compression\Compression.csproj" />
  </ItemGroup>
  ```


```json
        "contextPath" : "../../../"
```        
 


## Build and Run the Sample


1) Set Environment Variables

    The .env.temp file is used as a placeholder for the values for your project.  After filling in the values, remove the .temp extension so you only have .env.  The pattern used here allows you to store environment variables in one place without having them directly listed in the deployment templates and module.json files.  For example in the Compression Module, the module.json file contains $COMPRESSION_MODULE_REGISTRY which gets replaced with the value in the .env file.  In normal setup, that would have the actual value you have entered for your module.  Since this is a shared repository, I have opted for placeholders.  This pattern may be useful  in sharing other modules which you develop later.  

1) Verfiy Docker runtime mode (Windows only)
This sample is built to run in an Ubuntu container and requires a Linux Container runtime. If running on Windows, make sure that that Docker CE is running in the default Linux container mode. 

3) Select your target architecture

    Currently, Visual Studio Code can develop Node.js modules for Linux AMD64 and Linux ARM32v7 devices. You need to select which architecture you're targeting with each solution, because the container is built and run differently for each architecture type. The default is Linux AMD64.

    Open the command palette and search for *Azure IoT Edge: Set Default Target Platform for Edge Solution*, or select the [shortcut icon](./edge-platform.png) in the side bar at the bottom of the window.

    In the command palette, select the target architecture from the list of options. This tutorial uses an Ubuntu docker image as the IoT Edge device, make sure the default amd64 is selected.

1) Open the command paletta and search for *Azure IoT Edge: Setup IoT Edge Simulator*.  Select this
command to configure the Visual Studio Code Azure IoT Edge extension to connect to your Azure IoT Edge service.

Azure IoT Edge Solutions in Visual Studio Code contain a deployment manifest and one or more Azure IoT Edge Modules.  The Visual Studio Code Azure IoT Edge extension recognizes the Azure IoT Edge Solution structure and enables context menu items on the deployment manifest files, deployment.template.json and deployment.debug.template.json.

The debug version of the deployment manifest builds Docker image with the vsdg, the cross-platform Visual Studio Code debugger included, allowing source line debugging of Azure Iot Edge modules running in the Azure IoT Edge Simulator().

Select the deployment.debug.template.json in the Explorer pane of Visual Studio Code and right click to display the context menu. Select Build and Run IoT Solution in Simulator.






 