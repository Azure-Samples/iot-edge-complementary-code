---
page_type: sample
languages:
- csharp
products:
- azure-iot-hub
- azure-iot-edge
- vs-code
---

# Complementary Code pattern sample - Node.js *edge* workspace

This is the README file for the Node.js *edge* Visual Studio workspace (*edge.code-workspace*),  part of the Node.js version of the Complementary Code Pattern sample.  It is intended to be built and run with the companion code in the *cloud* workspace (*cloud.code-workspace*) in the same folder.  Both workspaces can be open simultaneously in different instances of Visual Studio Code.

------


> **Warning:**
> When you first load the *edge* workspace in Visual Studio Code, the Azure IoT Edge extension presents a dialog with the message "Please set registry credential to .env file." Selecting the *Yes* button will create a blank .env file, but the message will continue until the deployment manifest environment variable are defined in the *.env* file. Setting the required environment variables is covered later in the *Set environment variables* step in [EdgeDevelopment.md](../../EdgeDevelopment.md).

------

The top level [README.md](../../README.md) in this repository provides an overview of the Complementary Code sample, including an architecture diagram, along with prerequisites for building and running the sample code.

This sample assumes basic familiarity with Azure IoT Edge Modules and how to build them with Visual Studio Code.  For an introduction to building an IoT Edge Module in Node.js, refer to [Tutorial: Develop and deploy a Node.js IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-node-module).

## Contents

This workspace contains 3 folders:

| File/folder       | Description                                |
|-------------------|--------------------------------------------|
| `edge`            | Azure IoT Edge solution                    |
| `shared`          | shared compression library and unit tests  |
| `messages`        | test messages                              |

- edge - This folder contains an Azure IoT Edge solution consists of the two IoT Edge modules shown in the architecture diagram - the compression module (*CompressionModule*) and a message simulator module (*MessageSimulatorModule*).  The *MessageSimulatorModule* generates messages which are compressed by the *CompressionModule* and forwarded to to your Azure IoT Hub service.

   The *CompressionModule* is designed to illustrate the Complementary Code pattern to share logic between Edge components and the Cloud.  The *CompressionModule* *package.json* file references the *compression* npm file package, located in the *shared/compression* folder. 

   For demonstration, *MessageSimulatorModule* is also included  in the Azure IoT Edge solution.  This module simply plays back sample test messages from the *messages* folder in this workspace and sends them to the *CompressionModule* using an Azure IoT Edge route.

- shared - This folder contains a *compression* node package, which also contains a unit test folder (*compression/test*).  The *compression* package is used by both the Azure Functions App project in this workspace and the Azure IoT Edge solution in the *edge* workspace. The *compression* package is included to demonstrate source code sharing between edge and cloud code.  It actually uses the [node *zlib* package](https://nodejs.org/dist/latest/docs/api/zlib.html) to do the compression.

- messages - This folder contains test data messages that are played back by the *MessageSimulatorModule*.

## Prerequisites

The prerequisites for this code sample are included in the top level README.md of this repo.

## Setup

After installing prerequisites, make sure to follow the instructions in the [EdgeDevelopment.md](../../EdgeDevelopment.md) to configure your development environment for building the samples.  

## Running the sample

This section provides instructions for building building and running the sample in the Simulator, and optionally attaching the Visual Studio Code debugger to the running modules.  The sample can also be pushed to your container registry and deployed to actual Edge device by following the instructions in the [Tutorial: Develop and deploy a Node.js IoT Edge module for Linux devices](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-node-module).


1. Build and Run the solution

   With the Explorer icon in the Visual Studio Code Activity Bar selected, select the *deployment.debug.template.json* file in the Explorer pane and right click to display the context menu. Select *Build and Run IoT Solution in Simulator*.

   This command issues *docker build* command for each module in the selected deployment manifest, and then runs the deployment in the Azure IoT Simulator in solution code by issuing an *iotedgehubdev start* command.  These commands run in the Visual Studio Code integrated terminal.

   The Azure IoT Edge Simulator status messages and any console messages from individual messages are also shown in the Visual Studio integrated terminal.  Once the *MessageSimulatorModule* and *CompressionModule* have been initialized and started, you should see output indicating that compressed messages are being sent to your Azure IoT Hub service:

   ```
   CompressionModule         | Received message with body size: 9709
   CompressionModule         | Sent compressed message with body size: 1327  
   ```

   To stop the Azure IoT Edge Simulator after debugging, search for *Azure IoT Edge: Stop IoT Simulator* in the Visual Studio command palette, or simple press Ctrl+C in the Visual Studio Code integrated terminal. 

2. Attach debugger to module (optional)

    To bring up the Debug view, select the Debug icon in the Activity Bar on the side of Visual Studio Code.  In order to start a debug session, first select the configuration for your target module using the Configuration drop-down in the Debug view. The following configurations are provided for debugging either the *CompressionModule* or the *MessageSimulatorModule* with the Azure IoT Edge Simulator running in solution mode:

    - *CompressionModule Remote Debug (Node.js)*

    - *MessageSimulatorModule Remote Debug (Node.js)*
    
    Once you have your launch configuration set, start your debug session by pressing F5 or selecting the play button next to the Configuration drop-down in the Debug view.
    
    Once the node debugger has connected to the selected Azure IoT Edge Module container, execution should stop if any breakpoints were previously selected.

## Key concepts

### Understanding Azure IoT Edge solutions

Azure IoT Edge Solutions in Visual Studio Code are organized by a root folder containing the [Azure IoT Edge Deployment manifests](https://docs.microsoft.com/en-us/azure/iot-edge/module-composition) (*deployment.template.json* and *deployment.debug.template.json*) and a *modules* subfolder, which contains a folder for each module built by the solution.  

Each module folder contains a module metadata file (*module.json*), a Dockerfile for each support platform + debug/release combination,  and the language specific code for building the module (C#, Node.js, Python, etc.).  Node.js Azure IoT Edge modules are build as Node applications, so each module folder also contains a *package.json* file.  

The Azure IoT Edge Visual Studio Code extension recognizes this Azure IoT Edge solution folder structure and activates when the root folder is opened in Visual Studio code or as a workspace folder in Visual Studio Code.  

An [Azure IoT Edge deployment manifest](https://docs.microsoft.com/en-us/azure/iot-edge/module-composition) is JSON document which describes:

- The IoT Edge agent module twin, which includes three components.
  - The container image for each module that runs on the device.
  - The credentials to access private container registries that contain module images.
  - Instructions for how each module should be created and managed.
- The IoT Edge hub module twin, which includes how messages flow between modules and eventually to IoT Hub.
- Optionally, the desired properties of any additional module twins.

The structure of the deployment manifest is shown below:

```json
{
    "modulesContent": {
        "$edgeAgent": { // required
            "properties.desired": {
                // desired properties of the Edge agent
                // includes the image URIs of all modules
                // includes container registry credentials
            }
        },
        "$edgeHub": { //required
            "properties.desired": {
                // desired properties of the Edge hub
                // includes the routing information between modules, and to IoT Hub
            }
        },
        "module1": {  // optional
            "properties.desired": {
                // desired properties of module1
            }
        },
        "module2": {  // optional
            "properties.desired": {
                // desired properties of module2
            }
        },
        ...
    }
}
```

Each user module included in the deployment is listed under the *$edgeAgent.[properties.desired].modules* key in the deployment manifest.  This information provides information to the IoT Edge agent as to how to start, monitor and configure each module.

> **Note:** The deployment manifest also provides the same information for IoT Edge system modules (*edgeAgent* and *edgeHub*), but the configuration for the system modules is under the *$edgeAgent.[properties.desired].systemModules* key.

Below is the section of *deployment.debug.template.json* for the *CompressionModule*:

```json
    ...
    "modules": {
        "CompressionModule": {
            "version": "1.0",
            "type": "docker",
            "status": "running",
            "restartPolicy": "always",
            "settings": {
                "image": "${MODULES.CompressionModule.debug}",
                "createOptions": {}
            }
        },
        ...
```

The *image* key under the module *settings* can either contain a Docker container registry address and image tag (*mcr.microsoft.com/azureiotedge-simulated-temperature-sensor:1.0*) or a special placeholder in the format shown in the *CompressionModule* sample above.  The Azure IoT Edge extension uses these special placeholders to determine which module containers need to be built locally in the solution.  

The *${MODULES.CompressionModule.debug}* placeholder indicates that the Azure IoT Edge extension should look for a *CompressionModule* subfolder folder in the *modules* folder of the solution.  It then loads the module metadata file (*module.json*) in the *CompressionModule* subfolder to determine which Dockerfile to build.  Below is the module metadata file for the *CompressionModule*:

```json
{
    "$schema-version": "0.0.1",
    "description": "",
    "image": {
        "repository": "$CONTAINER_REGISTRY_ADDRESS/compressionmodule-node",
        "tag": {
            "version": "0.0.1",
            "platforms": {
                "amd64": "./Dockerfile.amd64",
                "amd64.debug": "./Dockerfile.amd64.debug"
             
            }
        },
        "buildOptions": [],
        "contextPath" : "../../../"
    },
    "language": "javascript"
}
```

The Azure IoT Edge extension relies on the currently selected target platform to choose a key under *platforms*.  For this sample, only the *amd64* (Linux) target is supported in *CompressionModule* module metadata file, however many modules support multiple target platforms.  The *.debug*  suffix on the placeholder in the deployment template (*deployment.debug.template.json*) indicates that the Azure IoT Edge extension should use the Dockerfile listed under the *amd64.debug* key.

The *module.json* file also contains keys that are used to construct the *docker build* command along with the Dockerfile name.  The *contextPath* key is used to set the Docker build context, which is explained more in the next section of this document.  The *buildOptions* key can be used to passed additional parameters to *docker build*. 

> Note:  The Azure IoT Edge Simulator uses Docker Compose when running in solution mode.  The *createOptions* key under the module settings in the deployment manifest can be used to pass [*docker create* options](https://docs.docker.com/engine/api/v1.30/#operation/ContainerCreate), such as exposed ports, volume mounts, host configuration, etc., to the Azure IoT Edge Simulator for use in container creation.

The Azure IoT Edge extension creates two deployments manifests, *deployment.template.json* and *deployment.debug.template.json*, when scaffolding a new Azure IoT Edge solution.  The *deployment.debug.template.json* version allows for the creation of a separate Docker images for debugging.  Node.js debug Dockerfile open the node debugger port 9229 in the container and start the Node application with the  *--inspect=0.0.0.0:9229* debugging command line option.  The *CompressionModule* debug Dockerfile in this sample also includes  a step to run the mocha unit test code in the container.

Azure IoT Edge routes are also defined in the deployment manifests, under the *$edgeHub* desired properties.  Below are the routes defined for this sample:
```json
    "$edgeHub": {
      "properties.desired": {
        "schemaVersion": "1.0",
        "routes": {
          "SimulatorToCompression": "FROM /messages/modules/MessageSimulatorModule/outputs/* INTO BrokeredEndpoint(\"/modules/CompressionModule/inputs/compressMessage\")",
          "CompressionToIoTHub": "FROM /messages/modules/CompressionModule/outputs/compressMessageOutput INTO $upstream"
        },
        "storeAndForwardConfiguration": {
          "timeToLiveSecs": 7200
        }
      }
    }
```
The *SimulatorToCompression* route instructs the Azure IoT Edge Hub to route message from all outputs of the *MessageSimulatorModule* to the *compressMessage* input of the *CompressionModule*.  The second route, *CompressionToIoTHub*, instructs the Azure IoT Edge Hub to send all output messages to the parent Azure IoT Hub.  

### Sharing Complementary Code in Node.js

The method for sharing Complementary code between an Azure IoT Edge module and an Azure Function varies according to the code platform and associated options for publishing and importing code. 

Node.js application typically share code via npm packages.  Packages are usually  published to package registries, either the public [npm registry](https://www.npmjs.com/) or a private registry.  A JavaScript file can also simply reference code in another file, even if its not installed as a node package.  However, this is not considered good practice since the file dependency isn't reflected in the *package.json* file.  

As of npm version 2.0.0, it's possible to reference a local npm package on the file system directly - without the need for a package registry.  The *CompressionModule* uses a *file://* package reference to leverage code in the *compression* library project, located in the *shared/compression* folder.  Below is the package.json from the *CompressionModule*:

```json
{
    "name": "compression-module",
    "version": "0.0.1",
    "dependencies": {
        "azure-iot-device": "^1.9.9",
        "azure-iot-device-mqtt": "^1.9.9",
        "compression": "file:../../../shared/compression"
    },
    "devDependencies": {
        "eslint": "^6.1.0"
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

Also, the debug version of the *CompressionModule* Dockerfile (*Dockerfile.amd64.debug*) is setup to build and run the *mocha* unit tests in the *compression* package as part of the Dockerfile build.  This demonstrates how to incorporate unit tests into the Azure IoT Edge module build pipeline, and ensure all relevant unit tests succeed before the Edge module container can be built.

### Debugging Azure IoT Edge Modules in Visual Studio Code

While Node.js Azure IoT Edge modules are built as ordinary Node.js console applications, they cannot be run or debugged directly because they must instantiate a *ModuleClient* object to start and operate as an Azure IoT Edge module and route messages.  In order to instantiate and use a *ModuleClient* object, the module must be able to connect to either the Azure IoT Edge runtime or the Azure IoT Edge Simulator.

While Visual Studio Code can be used to connect to and debug a module running on the real device under the Azure IoT Edge runtime using Docker *ssh* tunneling, this sample only covers debugging with the Azure IoT Edge Simulator.

The Azure IoT Edge Simulator actually supports two modes for running and debugging modules - single module mode and solution mode.  The single module mode allows a module to be run and debugged as an ordinary .NET Core application outside of a Docker container.  While this simplifies the inner development loop, single module mode only supports limited module functionality.  Neither Module Twins or Direct Methods are supported in single module mode, and each message must be manually passed to the module via a special HTTP interface.  And, as the name implies, only single module can be run at a time, so testing module interactions is not possible. A single module debug configuration named "MessageSimulatorModule Local Debug (Node.js)" is, however, included in the *launch.json* Visual Studio Code configuration file. To learn more about Azure IoT Edge Simulator debugging in single module mode, refer to [Debug a module without a container (C#, Node.js, Java)](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-vs-code-develop-module#set-up-iot-edge-simulator-for-single-module-app)

This sample is designed to show module-to-module message flow and edge-to-cloud message flow from from Azure IoT Edge modules to an Azure Function, so it is intended to be run and debugged under the Azure IoT Edge Simulator in solution mode.  When running in solution mode, the Azure IoT Edge Simulator uses Docker Compose to deploy all modules in the deployment manifest to the local Docker server.  

If the modules are built with debug Dockerfiles, which include steps to open the Node debugger port (9229) and start Node with debugger *inspect* flag (*--inspect=0.0.0.0:9229*), Visual Studio Code can then connect to node in debug mode in the running module container.    

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
