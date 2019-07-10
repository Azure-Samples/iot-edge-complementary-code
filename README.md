Complementary Code Pattern for Azure IoT Edge Modules & Cloud Applications

Core Pattern: 

In developing IoT solutions, there is often a need to perform one operation on the device and another complementary operation in the cloud or vice versa. Complementary operations such as compression and decompression, batching and shredding, encryption and decryption, encoding and decoding allow data to be transformed in one location and fully recovered in the other.  For example, compression/decompression enable sending smaller amounts of data and batch/shred enable sending multiple messages in one transmission.  The pattern established by the compression/decompression code can be incorporated in any one to one complementary operation.  Further, since the complementary code is packaged and separated from the IoT Edge module logic the operations can be incorporated into new modules or used in Azure applications by including the library.

Architecture Diagram:

!['Architecture Diagram of IoT Edge and Cloud interaction'](./arch.png)"Architecture Diagram"

The separation of business logic and library logic allows for four primary benefits:

1)	Business Logic: By appropriately determining the separation of business logic and library logic, either can be modified independently of the other.  For example, if an improved compression algorithm or application specific algorithm (ie. images/video vs text) the library could be updated.  On the other hand, if preprocessing of the data due to increased compliance requirements changes, the library does not need to be updated while the business logic does.  Additional libraries could be added and used by the business logic without updating the library.
2)	Business Artifact Repository: By creating a library, code can be used across business units and applications.  Libraries which benefit all IoT devices such as compression/decompression and batch/shred, which assist with reducing bandwidth usage or improving data transmission, can be managed and distributed in one location.  Any library which could benefit or be used by more than one application would apply here.  Further, libraries can be hosted and maintained in a central repository.
3)	Unit Testing:  IoT Edge code presents a particularly interesting unit test case.  If the code is bundled into the business logic instead of separated out, it becomes extremely challenging to test.  The reason for this is the need to mock the IoT Edge Module Client before doing any testing of the actual code.  By separating the code into a library, unit testing can take a familiar pattern once again.  When incorporating the code directly, rather than from a registry, one caution to keep in mind, while creating the unit tests in the same code folder as the module, is to ensure that you do not include the module in the Docker deployment of the edge code.  In C#, the unit test code exists in a separate folder from the library code, so this should not be an issue since the library does not have a deployment.json file.  In Node, however, add the tests folder to a “.dockerignore” file in the same folder as the module.
4)	Business Logic Bidirectionality: Finally, by creating a library which contains both of the operations, it becomes easy to add the library to code on the device and in the cloud to perform the complementary operation.  In the example above, compression/decompression are bundled in a single library so (1) the complementary operations are easily identified, (2) they can be unit tested together to ensure that input through the chain results in the same output as entered and (3) the code can be shared on device and in the cloud without the need to create additional code for the operation.

# Sample Overview

This repo contains sample code that demonstrates how to build an Azure IoT Edge compression module to compress messages sent to Azure IoT Hub from other Azure IoT Edge modules on the same Edge device or other downstream devices. It includes an Azure IoT Edge deployment solution to build the Azure IoT Edge _CompressionModule_, as well as an Azure Function project to receive messages from Azure IoT Hub, decompress them and write them to Azure Blob Storage.  

The samples demonstrates how code can be shared across cloud and Edge projects in C# and Node. The compression library code is used both by the Edge _CompressionModule_ and the Azure Function.

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
 





