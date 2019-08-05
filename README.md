# Complementary Code Pattern for Azure IoT Edge Modules & Cloud Applications

## Core Pattern 
In developing IoT solutions, there is often a need to perform one operation on the device and another complementary operation in the cloud or vice versa. Complementary operations such as compression/decompression, batching/shredding, encryption/decryption, encoding/decoding allow data to be transformed in one location and fully recovered in the other.  For example, compression/decompression enables sending smaller amounts of data and batching/shredding enables sending multiple messages in one transmission. 

In addition to the benefits of code reuse, separating out complementary code helps with unit testing, which can be  difficult for Azure IoT Edge Modules as well as many cloud services.  Azure IoT Edge Modules rely on a *ModuleClient* object to communicate with the Azure IoT Hub service.  The *ModuleClient* object can only be instantiated while the module is executing under the Azure IoT Edge runtime.  Likewise, user code in many cloud services is also difficult to unit test because the code relies on infrastructure that is only present when running in the cloud service environment.  For example, Azure Functions communicate with other Azure services via runtime *bindings* that are surfaced as various runtime objects in the supported languages.  By separating the complementary code and other business logic into a library, unit testing can follow a familiar pattern once again, minimizing the amount of code that can old be exercised through integration testing.

This sample demonstrates the Complementary Code pattern using compression/decompression, as shown below:

!['Architecture Diagram of IoT Edge and Cloud interaction'](./arch.png)

## Sample Overview

This repo contains sample code which demonstrates the Complementary Code pattern with compression/decompression.  

It includes an Azure IoT Edge deployment solution to build a module which compresses messages sent to Azure IoT Hub from other Azure IoT Edge modules on the same Edge device or other downstream devices. For demonstration, the deployment also includes a message simulator module that plays back sample messages stored on disk in a loop.  

The sample includes an Azure Functions project to build an Azure Function to receive messages from Azure IoT Hub, decompress them and write them to Azure Blob Storage.  

Finally, the compression/decompression code is shared across the Azure IoT Edge deployment solution and the Azure Functions solution. 

Versions of sample code are provided in both C#/.NET Core and Node.js.  The _csharp_ and _node_ folders are the root folders for the C#/.NET Core and Node.js versions of this sample respectively.  The root folders contain four directories - _edge_, _cloud_, _shared_, and _messages_.

The _edge_ folder contains the Azure IoT Edge deployment solution to build the compression module, while the _cloud_ folder contains the Azure Functions project to receive messages from Azure IoT Hub, decompress the messages and write the messages to Azure Blob Storage.  The _shared_ folder contains the compression and decompression code used by both the compression module and Azure Function, along with unit tests for the shared code.  The _messages_ folder contains test messages that are played back by the message simulator module, which is included in the Azure IoT Edge deployment solution.

The sample code is configured to build and run in [Visual Studio Code](https://code.visualstudio.com/) (aka VSCode).  Visual Studio Code is a Microsoft cross platform code editor, which also includes powerful developer tooling, like IntelliSense code completion and debugging.  Visual Studio Code is available on Windows, Mac, and Linux and supports C# and Node debugging for Azure IoT Edge modules.

There are two Visual Studio Code workspace files, _edge.code-workspace_ and _cloud.code-workspace_ in the language root folders. After installing the development environment prerequisites (below), the Edge and cloud workspaces can be opened in Visual Studio Code with the corresponding _.workspace_ files.

## Prerequisites
### Development machine prerequisites

The edge and cloud samples are designed to be built and run together on a development machine in Visual Studio Code using Docker CE, the Azure IoT EdgeHub Dev Tool and the Azure Functions Core Tools. Below are the prerequisites to build and run the sample on a local development machine: 

#### Language SDK's
- [.NET Core SDK (2.1 or above)](https://www.microsoft.com/net/download) - only required for C# version of sample
- [Node.js (8.5 or above)](https://nodejs.org) - required for Node version of sample and the Azure Functions Core Tools.
- [Python (2.7/3.6 or above) and Pip](https://www.python.org/) - required for Azure IoT EdgeHub Dev Tool. **Windows users should select the option to add Python to the path.**

#### Docker
- [Docker Community Edition](https://docs.docker.com/install/) - required for Azure IoT Edge module development, deployment and debugging. Docker CE is free, but may require registration with Docker account to download.  Docker on Windows requires Hyper-V support.  Please make sure your Windows version supports Hyper-V.  For Windows 10, Hyper-V is available with the Pro or Enterprise versions.

#### Visual Studio Code and extensions
Install [Visual Studio Code](https://code.visualstudio.com/) first and then add the following extensions:

- [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) (only required for C# version of sample) - provides C# syntax checking, build and debug support
- [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint) (only required for Node.js version of sample) - provides JavaScript syntax checking
- [Azure IoT Tools](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-tools)

    **Note**: Azure IoT Tools is an extension pack that installs 3 extensions that will show up in the Extensions pane in Visual Studio Code - *Azure IoT Hub Toolkit*, *Azure IoT Edge* and *Azure IoT Workbench*.

- [Azure Functions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)

**Note**: Extensions can be installed either via links to the Visual Studio Code Marketplace above or by searching extensions by name in the Marketplace from the Extensions tab in Visual Studio Code.

#### Azure IoT EdgeHub Dev Tool
[Azure IoT EdgeHub Dev Tool](https://pypi.org/project/iotedgehubdev/)  is a version of the Azure IoT Edge runtime for local development machine. 

1. Make sure Python and Pip (2.7/3.6 or above) are installed and in the path
2. Install **[iotedgehubdev](https://pypi.org/project/iotedgehubdev/)** with Pip:

   ```cmd
   pip install --upgrade iotedgehubdev
   ```

####  Azure Functions Core Tools

[Azure Functions Core Tools](https://github.com/Microsoft/vscode-azurefunctions/blob/master/README.md) is a version of the Azure Functions runtime for local development machine. It also provides commands to create functions, connect to Azure, and deploy Azure Function projects.

1. Make sure Node.js (8.5 or above) is installed and in the path

2. Install **[azure-functions-core-tools](https://www.npmjs.com/package/azure-functions-core-tools)** with npm:

    ```bash
    npm install -g azure-functions-core-tools
    ```
#### Azure Storage Emulator (optional, Windows only)

[AzureStorageEmulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)  provides a local environment that emulates the Azure Blob, Queue, and Table services for development purposes.  Use the link for the standalone installer.

Azure Blob Storage is required for the Azure Functions runtime for internal state management.  The Azure Function in this sample also writes decompressed messages to an Azure Storage account. 

When running this sample locally on **Windows**, the Azure Storage Emulator can be used instead of creating an Azure Storage account.

#### Azure Storage Explorer (optional)

[Azure Storage Explorer]() provides a GUI for connecting to and managing Azure storage accounts and Azure Cosmos DB entities.  Azure Storage Emulator can be used to view and download blobs output by the sample, both in your Azure Storage account or the Azure Storage Emulator.

### Azure Prerequisites 

#### Azure IoT Hub Service
To run the samples, you will need an Azure subscription and a provisioned Azure IoT Hub service.. Every Azure subscription allows one free F1 tier Azure IoT Hub.  The F1 tier Azure IoT is sufficient for this sample. 

[Create an IoT hub using the Azure portal](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-send-telemetry-dotnet#create-an-iot-hub)

#### Azure Storage 
The Azure Functions runtime requires an Azure Storage account for internal state management.  The Azure Function in this sample also writes decompressed messages to an Azure Storage account.  An Azure Storage account is only required if Azure Storage Emulator is not being used.

[Create an Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-quickstart-create-account?tabs=azure-portal)

**Note**: For a real application, you would typically create separate storage account for Azure Functions state management and application output.

#### Azure Functions App (optional)
This sample only requires the local Azure Functions Core Tools.  An Azure Functions application is only required if you wish to run the sample in Azure.

[Create an Azure Functions App](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function#create-a-function-app)

#### Azure Container Registry (optional)
This sample can be built and run in the local Azure IoT Edge Simulator without pushing Azure IoT Edge modules to a container registry.  A container registry is only needed when deploying to actual Azure IoT Edge device.  Any Docker container registry can be used, including DockerHub and Azure Container Registry.

[Create an Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal)

## Running the Sample

The Azure IoT Edge solution (*edge* folder) and Azure Functions app (*cloud* folder) are designed to be run side-by-side in two instances of Visual Studio Code.  

To run the sample, open the _edge.code-workspace_ and _cloud.code-workspace_ files in Visual Studio code under either the _csharp_ or _node_ language root folders, and follow the additional instructions in the *README.md* files located in the _edge_ and _cloud_ folders in the Visual Studio code workspaces. 


