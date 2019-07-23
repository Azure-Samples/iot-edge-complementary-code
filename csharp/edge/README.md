# Overview 
This folder contains the Azure IoT Edge Solution for the Azure IoT Edge compression module, a part of the C#/.NET Core version of the Complementary Code Pattern sample.  For a high level overview of the both the pattern and the sample, along with **prerequites** for building and running the sample, start with the top level [README.md](../../README.md) in this repository.

Azure IoT Edge Solutions in Visual Studio Code are organized by a root folder containing the [Azure IoT Edge Deployment manifests](https://docs.microsoft.com/en-us/azure/iot-edge/module-composition) (*deployment.template.json* and *deployment.debug.template.json*) and a *modules* subfolder, which contains a folder for each module, containing the module metadata file (*module.json*), Dockerfiles and language specific code for building the module.  The Azure IoT Edge Visual Studio Code extension recognizes this structure and activates when the root folder is opened in Visual Studio code or as a workspace folder in Visual Studio Code.

This sample assumes basic familiarity with Azure IoT Edge Modules and how to build them with Visual Studio Code.  For an introduction to building an IoT Edge Modules in C#, refer to 
[Tutorial: Develop a C# IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-csharp-module).

This Azure IoT Edge Solution actually builds two modules - *CompressionModule* and *MessageSimulatorModule*, which are located under the *modules* folder.  Each module is built as a .NET Core console application and includes a .NET Core project file. 

The *CompressionModule* is designed to illustrate the Complementary Code pattern to share logic between Edge components and the Cloud.  The *CompressionModule* project file references the *Compression* .NET library, located in the *shared/Compression* folder.  The *Compression* library uses the *GZipStream* compression class, included in the .NET Core Framework. 

For demonstration, *MessageSimulatorModule* is also included  in the Azure IoT Edge solution.  This module plays back sample test messages from the *messages* folder in this workspace and sends them to the *CompressionModule* using an Azure IoT Edge route. 

Azure IoT Edge routes are defined in the deployment manifests.  This solution has two routes defined - *SimulatorToCompression* and *CompressionToIoTHub*.  The *SimulatorToCompression* route instructs the Azure IoT Edge Hub to route message from all outputs of the *MessageSimulatorModule* to the *compressMessage* input of the *CompressionModule*.  Code in the *CompressionModule* binds the *compressMessage* input to its *CompressMessage* method.  There is also a *decompressMesage* method bound to the *DecompressMessage* method, but it is included only for illustration and is not used in the sample.  The second route, *CompressionToIoTHub*, instructs the Azure IoT Edge Hub to send all output messages to the parent Azure IoT Hub.  

## Sharing code in C#/.NET Core Azure IoT Edge Modules

The method for sharing code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. 

.NET projects can leverage external code via direct references to another project or via references to downloaded NuGet packages. The *CompressionModule* uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder.  Below is the line from the *CompressionModule.csproj* which references the *Compression.csproj*:

```xml
 <ItemGroup>
    <ProjectReference Include="..\..\..\shared\Compression\Compression.csproj" />
  </ItemGroup>
  ```
At build time, the .NET compiler copies the *Compression* library binaries to the binary output folder of the *CompressionModule*.  However, the .NET compiler must have access to the referenced library folder.  When building on the local file system, this isn't a problem. However, Azure IoT Edge Module are not built on the local file system.  They are built in a Docker container, using a Dockerfile.  By default, the *docker build* command issued by the Azure IoT Edge extension uses the module's folder for its root context. For example, the default Docker context for the *CompressionModule* is the *CompressionModule* folder.  Docker can't access files outside of its context, so the .NET compiler would be unable to resolve the *Compression* library reference when building in a Docker container.  

The solution is to raise the docker context so that it has access to folders above the individual module folders.  The module metadata file (*module.json*) supports a Docker context setting, *contextPath*, which is passed to the Docker build command. Below is the line from the *module.json* file which sets the context.

```json
        "contextPath" : "../../../"
```        
This setting is used in both *CompressionModule* and *MessageSimulatorModule* to raise the Docker context to the *csharp* root folder.  This allows the *CompressionModule* Dockerfile to access the *Compression* library and the *MessageSimulatorModule* Dockerfile to access the sample messages in the *messages* folder.

There is one problem with having the Docker build context at this level.  During a build, the Docker client sends all of the files in the Docker context to the Docker server in a tar file.  When running the Docker server locally, this overhead may not be noticable, but its still overhead in the build process.  Therefore, it should be efficient as possible. 

With the Docker context at the *csharp* folder level, all the code under the *cloud* subfolder is incorrectly included in the Docker context.  

Luckily, there is an easy solution to filter out unwanted files from the Docker build context.  Docker supports a *.dockerignore* file, which contains a list of exluded directories and file patterns.  Below is the a line from the *.dockerignore* file in the *csharp* directory which excludes the *cloud* folder from the Edge module docker builds.

```
cloud/*
```

## Development Environment Configuration

1) Handling warnings from Visual Studio Code extensions

    When you first load the edge and cloud workspaces in Visual Studio Code, the installed extensions will present messages letting you know that there are undefined environment variables in the deployment manifests and the C# projects need to be restored.  It is OK to dismiss these warnings.

    The Azure IoT Edge extension presents a dialog with the message "Please set registry credential to .env file." Selecting the OK button will create a blank .env file, but the message will continue until the environement variable are defined in the *.env* file. Setting the required environment variables  is covered below in the *Setup environment variables step*.

    The C# language extension presents a dialog with the message "There are unresolved dependencies.  Please execute the restore command to continue." Selecting the Restore button will execute the *dotnet restore* command on all of the .NET project files in the workspace.  This will eliminate the warning on subsequent loads, but the local file system retore and build is not used when running a deployment in the Azure IoT Edge Simulator, since each module is built in its own the Azure IoT Edge Modules are built in their own Docker containers.

1) Connect your Azure account to Visual Studio Code

    The *Azure IoT Tools* Visual Studio Code extension pack installs a prerequisite *Azure Account* extension if its not already present.  This extension allows Visual Studio Code to connect to your Azure subscription.  For this sample, Visual Studio Code needs to connect to you Azure IoT Hub service.

    Open the command palette and search for *Azure: Sign In*

    Select this command and you will be prompted to sign into your Azure account in a separate browser window.  After sign-in, you should see *Azure:* followed by your login account in the status bar at the bottom of Visual Studio Code.

1) Connect to your Azure IoT Hub

    There are 2 ways to connect to your Azure IoT Hub from within Visual Studio Code:

      Open the command palette and search for *Azure IoT Hub: Select IoT Hub* 
      
      **or**

      Go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Select the ... to open the Azure IoT Hub context menu.  From the Context Menu, choose *Select IoT Hub*.  

    Both options will open a selection list of available subscriptions at the top of the Visual Studio window.  After selecting your subscription, all available Azure IoT Hubs in your subscription will be presented in another selection list.  After selecting your Azure IoT Hub, the *AZURE IOT HUB** section in the Explorer pane of Visual Studio Code will be populated with configured Devices and Endpoints.  The Devices list will initially be empty for a new Azure IoT Hub.

1) Create an Azure IoT Edge device 
    
    This sample is designed to run in the Azure IoT Edge Simulator on a local developement machine.  However, the Simulator still connects to your Azure IoT Hub service, and therefore needs an Azure IoT Edge device definition in Azure IoT Hub.  You can create an Azure IoT Edge device in the Azure portal, but its easier from Visual Studio Code with the Azure IoT Edge extension installed.

    There are 2 ways to create an Azure IoT Edge device from Visual Studio Code:

    Open the command palettle and search for *Azure IoT Hub: Create IoT Edge Device*. 

    **or**
    
    Go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Select the ... to open the Azure IoT Hub context menu.  From the Context Menu, choose *Create IoT Edge Device*. 

    Both options will open a prompt for you to enter the name of the device.

    **Note:** There is also a *Azure IoT Hub: Create Device* command.  This creates a traditional Edge device which does not support the Azure IoT Edge Runtime and does not work with the Azure IoT Edge Simulator.

1) Configure Azure IoT Edge Simulator to use your Edge Device identity

    Again, there are 2 ways to create setup the Azure IoT Edge Simulator from within Visual Studio Code 
    
    Open the palette and search for *Azure IoT Edge: Setup IoT Edge Simulator*.  After selecting the command, a list of devices is displayed.  Select the device you created in the previous step. 

    **or**

    Go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Expand the Devices list, and right click on the device you created in the previous step to open the Context Menu.  Select *Setup IoT Edge Simulator* from the Context Menu. 

    **Note:** If you try to use the *Setup IoT Edge Simulator command without first connecting to your Azure IoT Hub,
    you will instead be prompted to enter the connection string for an Azure IoT Hub device.

1) Set environment variables

    The Azure IoT Edge solution deployment manifests (deployment.json and deployment.debug.json) support environment variable substitution.  contain 3 variablesThe *.env.temp* file is used as a placeholder for the values for your project.  After filling in the values, remove the .temp extension so you only have .env.  The pattern used here allows you to store environment variables in one place without having them directly listed in the deployment templates and module.json files.  For example in the Compression Module, the module.json file contains $COMPRESSION_MODULE_REGISTRY which gets replaced with the value in the .env file.  In normal setup, that would have the actual value you have entered for your module.  Since this is a shared repository, I have opted for placeholders.  This pattern may be useful  in sharing other modules which you develop later.  

1) Verfiy Docker runtime mode (**Windows only**)
This sample is built to run in an Ubuntu container and requires a Linux Container runtime. If running on Windows, make sure that that Docker CE is running in the default Linux container mode. 

3) Select your target architecture

    Currently, Visual Studio Code can develop Node.js modules for Linux AMD64 and Linux ARM32v7 devices. You need to select which architecture you're targeting with each solution, because the container is built and run differently for each architecture type. The default is Linux AMD64.

    Open the command palette and search for *Azure IoT Edge: Set Default Target Platform for Edge Solution*, or select the [shortcut icon](./edge-platform.png) in the side bar at the bottom of the window.

    In the command palette, select the target architecture from the list of options. This tutorial uses an Ubuntu docker image as the IoT Edge device, make sure the default amd64 is selected.

## Build and Run the Sample

The sample deployment can be built and run in the Azure IoT Edge Simulator, using either the debug or non-debug build.  These correspond to the The debug version of the deployment manifest builds Docker image with the vsdg, the cross-platform Visual Studio Code debugger included, allowing source line debugging of Azure Iot Edge modules running in the Azure IoT Edge Simulator().

Azure IoT Edge Solutions in Visual Studio Code contain a deployment manifest and one or more Azure IoT Edge Modules.  The Visual Studio Code Azure IoT Edge extension recognizes the Azure IoT Edge Solution structure and enables context menu items on the deployment manifest files, deployment.template.json and deployment.debug.template.json.


Select the deployment.debug.template.json in the Explorer pane of Visual Studio Code and right click to display the context menu. Select Build and Run IoT Solution in Simulator.

## Debugging Azure IoT Edge Modules

 ## Complementary Code Unit Testing






 