# Overview 
This is the README file for the C# *edge* Visual Studio workspace (*edge.code-workspace*),  part of the C#/.NET Core version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *cloud* workspace (*cloud.code-workspace*) in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

------


> **Warning:**
> When you first load the <b>edge</b> workspace in Visual Studio Code, the installed extensions will present messages letting you know that there are undefined environment variables in the deployment manifests and the C# projects need to be restored.  It is OK to dismiss these warnings.

>The Azure IoT Edge extension presents a dialog with the message "Please set registry credential to .env file." Selecting the *OK* button will create a blank .env file, but the message will continue until the environment variable are defined in the *.env* file. Setting the required environment variables  is covered later in the *Set environment variables* step when you configure your development environment.

> The C# language extension presents a dialog with the message "There are unresolved dependencies.  Please execute the restore command to continue." Selecting the *Restore* button will execute the *dotnet restore* command on all of the .NET project files in the workspace.  This will eliminate the warning on subsequent loads, but the local file system restore and build is not used when running a deployment in the Azure IoT Edge Simulator, since each Azure IoT Edge Module is built in its own Docker container.

------

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

After installing prerequisites, make sure to follow the instructions in the [EdgeDevelopment.md](../../EdgeDevelopment.md) to configure your development environment for building the samples.  

This workspace contains 3 folders:

1. edge - This folder contains an Azure IoT Edge solution consists of the two IoT Edge modules shown in the architecture diagram - the compression module (*CompressionModule*) and a message simulator module (*MessageSimulatorModule*).  The *MessageSimulatorModule* generates messages which are compressed by the *CompressionModule* and forwarded to to your Azure IoT Hub service.

   The *CompressionModule* is designed to illustrate the Complementary Code pattern to share logic between Edge components and the Cloud.  The *CompressionModule* project file references the *Compression* .NET library, located in the *shared/Compression* folder. 

   For demonstration, *MessageSimulatorModule* is also included  in the Azure IoT Edge solution.  This module simply plays back sample test messages from the *messages* folder in this workspace and sends them to the *CompressionModule* using an Azure IoT Edge route.

2. shared - This folder contains two .NET library projects - Compression and CompressionTests.  Compression is the compression library code.  Since this code is not hosted in a package manager, this allows both the Functions code and the IoT Edge code to use the same library.   The *Compression* library uses the *GZipStream* compression class, included in the .NET Core Framework. 

3. messages - This folder contains test data messages that are played back by the message simulator module.

This sample assumes basic familiarity with Azure IoT Edge Modules and how to build them with Visual Studio Code.  For an introduction to building an IoT Edge Modules in C#, refer to 
[Tutorial: Develop a C# IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-csharp-module).

Azure IoT Edge Solutions in Visual Studio Code are organized by a root folder containing the [Azure IoT Edge Deployment manifests](https://docs.microsoft.com/en-us/azure/iot-edge/module-composition) (*deployment.template.json* and *deployment.debug.template.json*) and a *modules* subfolder, which contains a folder for each module included in the deployment manifest the manifest.  Each module folder contains a module metadata file (*module.json*), a Dockerfile for each support platform + debug/release combination,  and the language specific code for building the module (C#,Node.js, Python, etc.).  ???In addition to the compression module , this Azure IoT Edge Solution builds a module used to  .  Each module is built as a .NET Core console application and includes a .NET Core project file.??? 

The *Azure IoT Edge* Visual Studio Code extension recognizes this structure and activates when the root folder is opened in Visual Studio code or as a workspace folder in Visual Studio Code.

Azure IoT Edge routes are defined in the deployment manifests.  This solution has two routes defined - *SimulatorToCompression* and *CompressionToIoTHub*.  The *SimulatorToCompression* route instructs the Azure IoT Edge Hub to route message from all outputs of the *MessageSimulatorModule* to the *compressMessage* input of the *CompressionModule*.  Code in the *CompressionModule* binds the *compressMessage* input to its *CompressMessage* method.  There is also a *decompressMesage* method bound to the *DecompressMessage* method, but it is included only for illustration and is not used in the sample.  The second route, *CompressionToIoTHub*, instructs the Azure IoT Edge Hub to send all output messages to the parent Azure IoT Hub.  

# Sharing code in C#/.NET Core Azure IoT Edge Modules

The method for sharing code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. 

.NET projects can leverage external code via direct references to another project or via references to downloaded NuGet packages. The *CompressionModule* uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder.  Below is the line from the *CompressionModule.csproj* which references the *Compression.csproj*:

```xml
 <ItemGroup>
    <ProjectReference Include="..\..\..\shared\Compression\Compression.csproj" />
  </ItemGroup>
```
At build time, the .NET compiler copies the *Compression* library binaries to the binary output folder of the *CompressionModule*.  However, the .NET compiler must have access to the *Compression* library folder.  When building on the local file system, this isn't a problem. However, Azure IoT Edge modules are not built on the local file system.  They are built in a Docker container, using a Dockerfile.  By default, the *docker build* command issued by the Azure IoT Edge extension uses the module's folder for its root context. For example, the default Docker context for the *CompressionModule* is the *CompressionModule* folder.  Docker can't access files outside of its root context, so, by default, .NET compiler would be unable to resolve the *Compression* library reference when building in a Docker container.  

The solution is to raise the docker context so that it has access to folders above the individual module folders.  The module metadata file (*module.json*) supports a Docker context setting, *contextPath*, which is passed to the Docker build command. Below is the line from the *module.json* file which sets the context.

```json
        "contextPath" : "../../../"
```
This setting is used in both *CompressionModule* and *MessageSimulatorModule* to raise the Docker context to the *csharp* root folder of the repo.  This allows the *CompressionModule* Dockerfile to access the *Compression* library and the *MessageSimulatorModule* Dockerfile to access the sample messages in the *messages* folder.

However, there is one problem with having the Docker build context at this level.  During a build, the Docker client sends all of the files in the Docker context to the Docker server in a tar file.  When running the Docker server locally, this overhead may not be noticeable, but its still overhead in the build process.  Therefore, it should be efficient as possible. 

With the Docker context at the *csharp* folder level, all the code under the *cloud* subfolder is incorrectly included in the Docker context.  

Luckily, there is an easy solution to filter out unwanted files from the Docker build context.  Docker supports a *.dockerignore* file, which contains a list of excluded directories and file patterns.  Below is the a line from the *.dockerignore* file in the *csharp* directory which excludes the *cloud* folder from the Edge module docker builds.

```
cloud/*
```

# Debugging C# Azure IoT Edge Modules in Visual Studio Code

For .NET Core/C#, Azure IoT Edge modules are built as .NET Core console applications.  However, the console application must instantiate a *ModuleClient* object to start and run as an Azure IoT Edge module and route messages.  In order to instantiate a *ModuleClient* object, the module must be able to connect to the Azure IoT Edge runtime or the Azure IoT Edge Simulator.

Visual Studio Code supports debugging Azure IoT Edge modules with the Azure IoT Edge Simulator using two different techniques - debugging without a container, and debugging in attach mode with the Azure IoT Edge Simulator.

  

  This section provides instructions for building building and running the sample in the Simulator, and optionally attaching the VSCode debugger to the running modules.  The sample can also be pushed to your container registry and deployed to actual Edge device, by following the instructions in the [Tutorial: Develop a C# IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-csharp-module).


Azure IoT Edge Solutions contain manifests for both debug (*deployment.debug.template.json*) and release container builds (*deployment.template.json*).  Depending on the selection of the debug or release version, and the selected Azure IoT Edge platform, the Azure IoT Edge extension selects the appropriate Dockerfile in each module's directory.  For example, the Azure IoT Edge extension uses the *Dockerfile.amd64.debug* Dockerfile when the *deployment.debug.template.json* is used and the *amd64* platform is selected.

For .NET Core/C#, the debug version of the amd64 Dockerfile (*Dockerfile.amd64.debug*) installs the *vsdbg* cross-platform .NET debugger, builds a debug version of the .NET console application, and includes unit test code in the container.

# Build and Run the Sample


1) Set module to debug mode (optional)

	- Enable debugger wait code
	
		Debugging Azure IoT modules running in the simulator requires starting the debug build module, and subsequently attaching the vsdbg debugger from Visual Studio Code.  Because of this 2 step process, module initialization usually takes place before the debugger is available.  Both .NET modules in this sample use a debugger wait code strategy to stop program flow execution until the debugger attaches.  This allows debugging of module initialization code.  Below is the code that is added to the module initialization method (*Program.Init*).
		```csharp
	          static async Task Init(bool debug = false)
	          {
	  #if DEBUG            
	              while (debug && !Debugger.IsAttached)
	              {
	                  Console.WriteLine("Module waiting for debugger to attach...");
	                  await Task.Delay(1000);
	              };
	  #endif
		```
	
		To enable this debugger wait code, a debug=true Boolean value must be passed to the Init method from the program's Main entry point.
	
		```csharp
	          static async Task Main(string[] args)
	          {
	              await Init(true);
	
	              // Wait until the app unloads or is cancelled
	              var cts = new CancellationTokenSource();
	              AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
	              Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
	              await WhenCancelled(cts.Token);
	          }
		```
	
	- Set a breakpoint in Visual Studio code at any point after the debugger wait code using the F9 key or clicking to the left of the code line number.
	
	>**Note:** Each Visual Studio Code instance can only debug one module, so its recommended to enable this debugger wait code only only one module at a time.



2. Select the deployment.debug.template.json in the Explorer pane of Visual Studio Code and right click to display the context menu. Select Build and Run IoT Solution in Simulator.

3. Attach debugger to module (optional)

	To bring up the Debug view, select the Debug icon in the Activity Bar on the side of VS Code. 

	In order to start a debug session, first select the configuration named Launch Program using the Configuration drop-down in the Debug view. Once you have your launch configuration set, start your debug session with F5.

# Complementary Code Unit Testing

# Deploying to Azure





