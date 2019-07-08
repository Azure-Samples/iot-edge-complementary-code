# Overview

This repo contains sample code that demonstrates how to build an Azure IoT Edge compression module to compress messages sent to Azure IoT Hub from other Azure IoT Edge modules on the same Edge device or other downstream devices. It includes an Azure IoT Edge deployment solution to build the Azure IoT Edge _CompressionModule_, as well as an Azure Function project to receive messages from Azure IoT Hub, decompress them and write them to Azure Blob Storage.  

The samples also demonstrates how code can be shared across cloud and Edge projects. The compression library code is used both by the Edge _CompressionModule_ and the Azure Function.

Versions of sample code are provided in both C# and Node.js.  The _csharp_ and _node_ folders are the root folders for the C# and Node versions of this sample respectively.  The root folders contain three directories, _edge_, _cloud_ and _shared_.  The _edge_ folder contains the Azure IoT Edge deployment solution to build the _CompressionModule_, while the _cloud_ folder contains the Azure Functions project to receive messages from Azure IoT Hub, decompress the messages and write the messages to Azure Blob Storage.  The _shared_ folder contains the compression and decompression code used by both the _CompressionModule_ and Azure Function.  

The sample code is configured to build and run in [Visual Studio Code](https://code.visualstudio.com/).  Visual Studio Code is a Microsoft cross platform code editor, which also includes powerful developer tooling, like IntelliSense code completion and debugging.  Visual Studio Code is available on Windows, Mac, and Linux and supports C# and Node debugging for Azure IoT Edge modules.

There are two [Visual Studio Code](https://code.visualstudio.com/) workspace files, _edge.code-workspace_ and _cloud.code-workspace_ in the language root folders. After installing the development environment prerequisites (below), the Edge and cloud workspaces can be opened in Visual Studio Code with the corresponding _.workspace_ files.

# Prerequisites
## Development machine prerequisites

The Edge and cloud samples are designed to be built and run together on a development machine in [Visual Studio Code](https://code.visualstudio.com/) using [Docker CE](https://docs.docker.com/install/), the [Azure IoT EdgeHub Dev Tool](https://pypi.org/project/iotedgehubdev/) and the [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2). 

### Language SDK's
- [.NET Core 2.1 SDK](https://www.microsoft.com/net/download) - only required for C# version of sample
- [Node.js (8.5 or above)](https://nodejs.org) - required for Node version of sample and the Azure Functions Core Tools
- [Python (2.7/3.6 or above) and Pip](https://www.python.org/) - required for Azure IoT EdgeHub Dev Tool

### Docker
- [Docker Community Edition](https://docs.docker.com/install/) - required for Azure IoT Edge module development, deployment and debugging

### Visual Studio Code and extensions
Install [Visual Studio Code](https://code.visualstudio.com/) first and then add the following extensions:

- [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) (only required for C# version of sample)
- [Azure IoT Tools](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-tools)
- [Azure Functions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)

### Azure IoT EdgeHub Dev Tool
[Azure IoT EdgeHub Dev Tool](https://pypi.org/project/iotedgehubdev/) - local development environment to debug, run, and test your IoT Edge solution. 

1. Make sure Python and Pip (2.7/3.6 or above) are installed and in the path
2. Install **[iotedgehubdev](https://pypi.org/project/iotedgehubdev/)** with Pip:

   ```cmd
   pip install --upgrade iotedgehubdev
   ```

###  Azure Functions Core Tools

[Azure Functions Core Tools]() - version of the Azure Functions runtime for local development computer. It also provides commands to create functions, connect to Azure, and deploy function projects.

1. Make sure Node.js (8.5 or above) is installed and in the path

2. Install **[azure-functions-core-tools](https://www.npmjs.com/package/azure-functions-core-tools)** with npm:

    ```bash
    npm install -g azure-functions-core-tools
    ```
### Azure Storage Emulator

[AzureStorageEmulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) (optional, Windows only) - provides a local environment that emulates the Azure Blob, Queue, and Table services for development purposes.

Azure Blob Storage is required for the Azure Functions runtime for internal state management.  The Azure Function in this sample also writes decompressed messages to an Azure Storage account.

When running this sample locally on Windows, the Azure Storage Emulator can be used instead of creating an Azure Storage account.

## Azure Prerequistes 

### Azure IoT Hub Service
To run the samples, you will need an Azure subscription and a provisioned Azure IoT Hub service.. Every Azure subscription allows one free F1 tier Azure IoT Hub.  The F1 tier Azure IoT is sufficient for this sample. 

[Create an IoT hub using the Azure portal](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-send-telemetry-dotnet#create-an-iot-hub)

### Azure Storage 
The Azure Functions runtime requires an Azure Storage account for internal state management.  The Azure Function in this sample also writes decompressed messages to an Azure Storage account.  An Azure Storage account is only required if Azure Storage Emulator is not being used.

[Create an Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-quickstart-create-account?tabs=azure-portal)

**Note**: For a real application, you would typically create separate storage account for Azure Functions state management and application output.

### Azure Functions App (optional)
This sample only requires the local Azure Functions Core Tools.  An Azure Functions application is only required if you wish to run the sample in Azure.

[Create an Azure Functions App](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function#create-a-function-app)


# Running the Sample

The Azure IoT Edge CompressionModule solution and Azure Functions app are designed to be run side-by-side in two instances of Visual Studio Code.  

To run the sample, open the _edge.code-workspace_ and _cloud.code-workspace_ files in Visual Studio code under either the _csharp_ or _node_ language root folders, and follow the additional instructions in the *readme.md* files located in the _edge_ and _cloud_ folders in Visual Studio code workspaces. 
 



