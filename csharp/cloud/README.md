# Overview 

This is the README file for the C# *cloud* Visual Studio workspace (*cloud.code-workspace*), part of the C#/.NET Core version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *edge* workspace (*edge.code-workspace*) located in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

This workspace contains 2 folders:

1. cloud- This folder contains an Azure Functions project which builds the Azure Function (*CompressionFnc*) shown in the architecture diagram.  The *CompressionFnc* serves as the cloud complement to the Azure IoT Edge compression module (*CompressionModule*).  It demonstrates decompressing a compressed message which sent from the *CompressionModule* through your Azure IoT Edge Hub service and writing the decompressed message to Azure Blob Storage.

2. shared - This folder contains two .NET library projects which are use in both the *edge* and *cloud* workspaces - *Compression* and *CompressionTests*.  

   *Compression* is a .NET Standard library project, used in both the Azure IoT Edge solution in the *edge* workspace and the Azure Functions project in the *cloud* workspace.  The *Compression* library itself is very simple and is intended for demonstration of the Complementary Code pattern.  It leverages the *GZipStream* compression class, included in the .NET Core Framework. 
   
   *CompressionTests* is a .NET Core *xUnit.net* unit test project.  The *Unit Testing Complementary Code* section of this document describes how to incorporate unit tests into the Azure Functions App local development loop and build process.

# Sharing Complementary Code in C#/.NET

The method for sharing code between an Azure IoT Edge module and an Azure Function project varies according to the code platform and associated options for publishing and importing code. 

.NET projects can leverage external code via direct references to another project or via references to downloaded NuGet packages. The *CompressionFnc* Azure Functions project uses a direct project reference to leverage code in the *Compression* library project, located in the *shared/Compression* folder.  Below is the line from the *CompressionFnc.csproj* which references the *Compression.csproj*:

```xml
  <ItemGroup>
    <ProjectReference Include="..\shared\Compression\Compression.csproj" />
  </ItemGroup>
```

At build time, the .NET compiler copies the *Compression* library binaries to the binary output folder of the *CompressionFnc*.  

# Unit Testing Complementary Code

As explained in the top level README.md in this repo, the Complementary Code pattern enables unit testing of shared code.  For the C#/.NET Core version of the sample, a *CompressionTests* *xUnit.net* unit test project is included in the *shared/CompressionTests* folder.  This sample shows several ways to run the unit tests in this project, both as part of the inner development loop and build pipeline.

The Visual Studio Code C# language extension recognizes *xUnit.net* projects and enables interactive running debugging of individual unit test methods.

![xunit debug image](C:/ComplementaryCode/csharp/edge/xunit.png)

Unfortunately, the C# language extension only activates on the primary folder in a Visual Studio code workspace.  To get the C# language extension to enable the interactive unit test support, switch the primary folder in Visual Studio code from *cloud* to *shared* by selecting the folder button ![folder icon](C:/ComplementaryCode/csharp/edge/folder.png)on the Visual Studio Code status bar.

There is also a *test* task in the Visual Studio Code *tasks.json* configuration file in the *cloud* folder. This task invokes the *dotnet test* command on the *CompressionTests* project in the Visual Studio Code interactive terminal.  To run the *test* task, search for *Tasks: Run Task* from the Visual Studio Code command palette and select *test* from the task list.

# Configure Azure Functions development environment

After installing prerequisites, there is one additional step to configure your development environment before building and running the sample. The *CompressionFnc* Azure Function requires 3 connection strings to run - *AzureWebJobsStorage*, *IoTHubEventHubEndpoint* and *OutputBlobConnectionString*.  

*AzureWebJobsStorage* is a built-in connection string that the Azure Functions runtime uses to access a special storage account which it uses for state management. It is required for all Azure Function types except HTTP triggered functions. When you create an Azure Function App via the the Azure Portal or the Visual Studio Code Azure Functions extension, an Azure Storage account is automatically created for the Function App and the *AzureWebJobsStorage* connection string is set in the [Azure Functions applications settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob).  The *AzureWebJobsStorage* must be set manually when running a function locally with the Azure Functions Core Tools using either the Azure Storage Emulator connection string (*UseDevelopmentStorage=true*) or the connection string for the Azure Storage Account created during the prerequisites setup.

The *CompressionFnc* receives compressed messages from your Azure IoT Hub service via the [Azure Functions IoT Hub binding](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-iot).  Azure Functions accesses Azure IoT Hub messages at the the Azure IoT Hub's built-in [Event Hub compatible endpoint](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-read-builtin#read-from-the-built-in-endpoint).  *IoTHubEventHubEndpoint* is the connection string for the Azure IoT Hub's Event Hub compatible endpoint.  It can be found in the Azure Portal: 

1. Sign in to the [Azure portal](https://portal.azure.com/) and navigate to your IoT hub
2. Click *Built-in endpoints*
3. Copy the value of  *Event Hub-compatible endpoint* under *Events*

The *CompressionFnc* decompresses messages and writes them to an Azure Blob Storage account using the [Azure Functions Blob Storage binding](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob).  The last connection string, *OutputBlobConnectionString* is the connection string of an Azure Blob Storage account where the *CompressionFnc* will write decompressed messages received from your Azure IoT Hub.  

> **Note:** For production, separate Azure Storage accounts should be used for *AzureWebJobsStorage* and *OutputBlobConnectionString*.  For development, the same Azure Blob Storage account can be used for both, or the Azure Storage Emulator can be used for both on Windows. 

Connection strings and other secrets should not be stored in application code or any file that is checked into source control.   The recommended way to pass connection string and other secrets to an Azure Function is through environment variables.  

Azure Function bindings can implicitly access connection string via environment variables , as shown in the *CompressionFnc* code below which references the *IoTHubEventHubEndpoint* and *OutputBlobConnectionString* environment variables:


```csharp
       ...
       [FunctionName("CompressionFnc")]
        public static async Task Run(
            [IoTHubTrigger("messages/events", ConsumerGroup = "$Default", Connection = "IoTHubEventHubEndpoint")]EventData message,
            [Blob("test-out/{sys.randguid}", FileAccess.Write, Connection = "OutputBlobConnectionString")] Stream output,
            ILogger log)
        {
        ...
```
User code in an Azure Function can also retrieve environment variables explicitly using language/platform specific API's.  C#/.NET functions can use the *Environment.GetEnvironmentVariable* API.

Environment variables are set differently for local development with the Azure Functions Core Tools vs running in the Azure Functions App service runtime.   

When you deploy your function to your Azure Functions App service, environment variables, including connection strings, are configured as [Azure Functions applications settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob) in the Azure Portal.  Azure Functions application settings are encrypted, stored and managed by the Azure App Service platform, which hosts Azure Functions.  

When running Azure Functions in your local development environment with the Azure Functions Core Tools, environment variables are set in a special development-only settings file, *local.settings.json*.  Since this file contains secrets, it should always be excluded from source control. Therefore, it is included in *.gitignore* in this sample repo.  A template *local.settings.json.temp* is provided as a template, which can be renamed to *local.settings.json*.  After renaming, update the *AzureWebJobsStorage* and *OutputBlobConnectionString* values to either the Emulator connection string or the connection string for the Azure Storage account you created during the prerequisite step.  Update the *IoTHubEventHubEndpoint* to value you copied earlier in this section.

> **Note:** The Visual Studio Code Azure Functions extension can optionally publish your *local.settings.json* values to your Azure Function App after you deploy, using the instructions in [Publish application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code#publish-application-settings).   However, be sure not to publish application settings which use the Azure Storage Emulator.  The Azure Storage Emulator setting (*UseDevelopmentStorage=true*) will cause an error when your function executes in your Azure Function App.   Also, you will get a warning that there is already a *AzureWebJobsStorage* setting that was setup as part of the Azure Function App creation.  If you use different Azure Storage account for local development and your Azure Function App, each will maintain their own cursor reading messages from your Azure IoT Hub.

# Build and run the sample

This section provides instructions for building building and running the sample in the Azure Functions local runtime.  The sample can also be pushed to your Azure Function App by following the instructions in the article [Publish the project to Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code#publish-the-project-to-azure).

There are several Visual Studio tasks created by the Azure Functions extension - *build*, *clean release*, *publish* and *func*. The *func* task will run your functions locally in the Azure Functions Core Tools.  To run these tasks, search for *Tasks: Run Task* from the Visual Studio Code command palette and select the from the task list.

The easiest way to run your functions is to simply start your functions in the Visual Studio Code debugger.   Select the Debug icon in the Activity Bar on the side of Visual Studio Code.  You should see *Attach to C# Functions* in the Debug configuration dropdown. Start your debug session by pressing F5 or selecting the play button next to the Configuration dropdown.

You should see Azure Functions local runtime status messages in the Visual Studio integrated terminal.  Once the *CompressionFnc* starts reading messages from your Azure IoT Hub, you should see output showing the decompressed messages.  The messages are several aviation weather reports for New York area airports in XML format.

The *CompressionFnc* also writes decompressed messages to either the Azure Storage Emulator or your Azure Storage account.  The messages are written to output blobs in the format *"test-out/{sys.randguid}"*.  Azure Storage Explorer can be also used to download and view the output blobs.