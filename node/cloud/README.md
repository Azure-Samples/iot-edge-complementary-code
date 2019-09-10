---
page_type: sample
languages:
- nodejs
products:
- azure-functions
- vs-code
---

# Complementary Code pattern sample - Node.js *cloud* workspace

This is the README file for the Node.js *cloud* Visual Studio workspace (*cloud.code-workspace*), part of the Node.js version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *edge* workspace (*edge.code-workspace*) located in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

## Contents

Outline the file contents of the repository. It helps users navigate the codebase, build configuration and any related assets.

This workspace contains 2 folders:

| File/folder       | Description                               |
|-------------------|-------------------------------------------|
| `cloud`           | Azure Functions project                   |
| `shared`          | shared compression library and unit tests |

This workspace contains 2 folders:

- cloud- This folder contains an Azure Functions project which builds the Azure Function (*CompressionFnc*) shown in the architecture diagram.  The *CompressionFnc* serves as the cloud complement to the Azure IoT Edge compression module (*CompressionModule*).  It demonstrates decompressing a compressed message which sent from the *CompressionModule* through your Azure IoT Edge Hub service and writing the decompressed message to Azure Blob Storage.

- shared - This folder contains a *compression* node package, which also contains a unit test folder (*compression/test*).  The *compression* package is used by both the Azure Functions App project in this workspace and the Azure IoT Edge solution in the *edge* workspace. The *compression* package is included to demonstrate source code sharing between edge and cloud code.  It actually uses the [node *zlib* package](https://nodejs.org/dist/latest/docs/api/zlib.html) to do the compression.

## Prerequisites

The prerequisites for this code sample are included in the top level README.md of this repo.

## Setup

### Configure Azure Functions development environment

After installing prerequisites, there is one additional step to configure your development environment before building and running the sample. The *CompressionFnc* Azure Function requires 3 connection strings to run - *AzureWebJobsStorage*, *IoTHubEventHubEndpoint* and *OutputBlobConnectionString*.  

*AzureWebJobsStorage* is a built-in connection string that the Azure Functions runtime uses to access a special storage account which it uses for state management. It is required for all Azure Function types except HTTP triggered functions. When you create an Azure Function App via the the Azure Portal or the Visual Studio Code Azure Functions extension, an Azure Storage account is automatically created for the Function App and the *AzureWebJobsStorage* connection string is set in the [Azure Functions applications settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob).  The *AzureWebJobsStorage* must be set manually when running a function locally with the Azure Functions Core Tools using either the Azure Storage Emulator connection string (*UseDevelopmentStorage=true*) or the connection string for the Azure Storage Account created during the prerequisites setup.

The *CompressionFnc* receives compressed messages from your Azure IoT Hub service via the [Azure Functions IoT Hub binding](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-iot).  Azure Functions accesses Azure IoT Hub messages at the the Azure IoT Hub's built-in [Event Hub compatible endpoint](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-read-builtin#read-from-the-built-in-endpoint).  *IoTHubEventHubEndpoint* is the connection string for the Azure IoT Hub's Event Hub compatible endpoint.  It can be found in the Azure Portal: 

1. Sign in to the [Azure portal](https://portal.azure.com/) and navigate to your IoT hub
2. Click *Built-in endpoints*
3. Copy the value of  *Event Hub-compatible endpoint* under *Events*

The *CompressionFnc* decompresses messages and writes them to an Azure Blob Storage account using the [Azure Functions Blob Storage binding](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob).  The last connection string, *OutputBlobConnectionString* is the connection string of an Azure Blob Storage account where the *CompressionFnc* will write decompressed messages received from your Azure IoT Hub.  

> **Note:** For production, separate Azure Storage accounts should be used for *AzureWebJobsStorage* and *OutputBlobConnectionString*.  For development, the same Azure Blob Storage account can be used for both, or the Azure Storage Emulator can be used for both on Windows. 

Connection strings and other secrets should not be stored in application code or any file that is checked into source control.   The recommended way to pass connection string and other secrets to an Azure Function is through environment variables.  

Azure Function bindings can implicitly access connection string via environment variables , as shown in the *function.json* configuration file for *CompressionFnc*, which references the *IoTHubEventHubEndpoint* and *OutputBlobConnectionString* environment variables:


```json
{
  "disabled": false,
  "bindings": [
    {
      "type": "eventHubTrigger",
      "name": "IoTHubMessage",
      "direction": "in",
      "eventHubName": "messages/events",
      "connection": "IoTHubEventHubEndpoint",
      "cardinality": "one",
      "consumerGroup": "$Default",
      "dataType": "binary"
    },
    {
      "name": "outputBlob",
      "type": "blob",
      "path": "test-out/{rand-guid}.json",
      "connection": "OutputBlobConnectionString",
      "direction": "out"      
    }
  ]
}
```
User code in an Azure Function can also retrieve environment variables explicitly using language/platform specific API's.  Node.js functions can use the *process.env* API.

Environment variables are set differently for local development with the Azure Functions Core Tools vs running in the Azure Functions App service runtime.   

When you deploy your function to your Azure Functions App service, environment variables, including connection strings, are configured as [Azure Functions applications settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob) in the Azure Portal.  Azure Functions application settings are encrypted, stored and managed by the Azure App Service platform, which hosts Azure Functions.  

When running Azure Functions in your local development environment with the Azure Functions Core Tools, environment variables are set in a special development-only settings file, *local.settings.json*.  Since this file contains secrets, it should always be excluded from source control. Therefore, it is included in *.gitignore* in this sample repo.  A template *local.settings.json.temp* is provided as a template, which can be renamed to *local.settings.json*.  After renaming, update the *AzureWebJobsStorage* and *OutputBlobConnectionString* values to either the Emulator connection string or the connection string for the Azure Storage account you created during the prerequisite step.  Update the *IoTHubEventHubEndpoint* to value you copied earlier in this section.

> **Note:** The Visual Studio Code Azure Functions extension can optionally publish your *local.settings.json* values to your Azure Function App after you deploy, using the instructions in [Publish application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code#publish-application-settings).   However, be sure not to publish application settings which use the Azure Storage Emulator.  The Azure Storage Emulator setting (*UseDevelopmentStorage=true*) will cause an error when your function executes in your Azure Function App.   Also, you will get a warning that there is already a *AzureWebJobsStorage* setting that was setup as part of the Azure Function App creation.  If you use different Azure Storage account for local development and your Azure Function App, each will maintain their own cursor reading messages from your Azure IoT Hub.

## Running the sample

This section provides instructions for building building and running the sample in the Azure Functions local runtime.  The sample can also be pushed to your Azure Function App by following the instructions in the article [Publish the project to Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code#publish-the-project-to-azure).

There is a Visual Studio Code task created by the Azure Functions extension - *func: host start*, which will run your functions locally in the Azure Functions Core Tools.  This task also runs the *npm install* task a dependency. To run these tasks directly, search for *Tasks: Run Task* from the Visual Studio Code command palette and select from the task list.

The start your function in debugging mode, select the Debug icon in the Activity Bar on the side of Visual Studio Code.  You should see *Attach to Node Functions* in the Debug configuration dropdown. Start your debug session by pressing F5 or selecting the play button next to the Configuration dropdown.  This first runs the function with the *func: host start* task and the Visual Studio Code Node debugger to the function.

You should see Azure Functions local runtime status messages in the Visual Studio Code integrated terminal.  Once the *CompressionFnc* starts reading messages from your Azure IoT Hub, you should see output showing the decompressed messages.  The messages are several aviation weather reports for New York area airports in XML format.

The *CompressionFnc* also writes decompressed messages to either the Azure Storage Emulator or your Azure Storage account.  The messages are written to output blobs in the format *"test-out/{sys.randguid}.xml"*.  Azure Storage Explorer can be also used to download and view the output blobs.

## Key concepts

### Sharing Complementary Code in Node.js

The method for sharing Complementary code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. 

Node.js application typically share code via npm packages.  Packages are usually  published to package registries, either the public [npm registry](https://www.npmjs.com/) or a private registry.  A JavaScript file can also simply reference code in another file, even if its not installed as a node package.  However, this is not considered good practice since the file dependency isn't reflected in the *package.json* file.  

As of npm version 2.0.0, it's possible to reference a local npm package on the file system directly - without the need for a package registry.  The *CompressionFnc* uses a *file://* package reference to leverage code in the *compression* library project, located in the *shared/compression* folder.  Below is the package.json from the *CompressionFnc*:

```json
{
    "name": "CompressionFnc",
    "version": "0.0.1",
    "dependencies": {
        "@azure/event-hubs": "^1.0.8",
        "compression": "file:../shared/compression"
    }
}
```

At package installation time (*npm install*), npm  copies the *compression* package files to the *node_modules*  folder of the *CompressionModule*.  However, npm must have access to the *compression* library folder.  When running *npm install* on the local file system, this isn't a problem. However, Azure IoT Edge modules are not built on the local file system.  They are built in a Docker container, using a Dockerfile.  By default, the *docker build* command issued by the Azure IoT Edge extension passes the module's folder as the PATH (root context) argument. For example, the default Docker context for the *CompressionModule* is the *CompressionModule* folder.  Docker can't access files outside of its root context, so, by default, npm would be unable to resolve the *compression* package reference when building the *CompressionModule* Docker container.  

The solution is to raise the docker context so that it has access to folders above the individual module folders.  The module metadata file (*module.json*) supports an optional Docker context setting, *contextPath*, which is passed as the PATH argument to to the *docker build* command to set the context. Below is the line from the *module.json* file which raised the context.

```json
        "contextPath" : "../../../"
```

This setting is used in both *CompressionModule* and *MessageSimulatorModule* to raise the Docker context to the *node* root folder of the repo.  This allows the *CompressionModule* Dockerfile to access the *Compression* library and the *MessageSimulatorModule* Dockerfile to access the sample messages in the *messages* folder.

However, there is one problem with having the Docker build context at this level.  During a build, the Docker client sends all of the files in the Docker context to the Docker server in a tar file.  With the Docker context at the *node* folder level, all the code under the *cloud* subfolder is incorrectly included in the Docker context.  When running the Docker server locally, this overhead may not be noticeable, but it is still unnecessary overhead in the build process.  To be as efficient as possible the *cloud* folder should not be included in the Docker context.

Luckily, there is an easy solution to filter out unwanted files from the Docker build context.  Docker supports a *.dockerignore* file, which contains a list of excluded directories and file patterns.  Below is the a line from the *.dockerignore* file in the *csharp* directory which excludes the *cloud* folder from the Edge module docker builds (and all *node_modules* folders since these will be restored during the container image creation).

```
cloud/*
**/node_modules/*
```

### Unit Testing Complementary Code

As explained in the top level README.md in this repo, the Complementary Code pattern enables unit testing of shared code.  For the Node.js version of the sample, the *compression* package includes *mocha* unit tests in the *shared/compression/tests* folder.  This sample shows several ways to run the unit tests in this project, both as part of the inner development loop and build pipeline.

JavaScript language support is built into Visual Studio Code.  However, *mocha* test support is not included by default, although there are a number of mocha extensions available in the Visual Studio Code marketplace to add mocha test support to the UI.  Visual Studio Code does automatically detect package.json scripts and adds them as tasks that can be run via the built-in Visual Studio Code *Tasks* feature.  The *compression* package includes a *test* script to invoke the *mocha*  test.  To run the *test* script as a task, search for *Tasks: Run Task* from the Visual Studio Code command palette and select *test* from the task list.


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
