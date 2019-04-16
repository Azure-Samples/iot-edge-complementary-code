# Introduction 

This Azure function project serves as the Cloud complement to the IoT Edge Compression Module code.  It demonstrates decompressing a compressed message which is sent from the IoT Edge module through Edge Hub and writing the decompressed message to Azure Blob Storage.

The `compress-message.js` file is identical to the one found within the IoT Edge Module project since it uses the complementary operation.  This code could be reversed to send a compressed message from the Cloud to the Edge Compression Module which would decompress it.  For further information, see the IoT Edge project README.md.

# Getting Started

1. How to use

    To use this Azure function, copy the directory and open it within VS Code.

    You will need to update the value in `local.settings.json.temp`.  After updating, remove `.temp` from the filename (it is added with `.temp` to 
    provide a way to upload the necessary placeholders without exposing secrets).

    You will need to add a Blob named `test-out` in your storage account, or a name of your choosing.  If you choose something different, update the `function.json` file where *"test-out/{rand-guid}.json"* appears, in the bindings section on line 17, to "path": *"<your blob name>/{rand-guid}.json"*,

2.	Software dependencies

    To use this code locally, you will need to have [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local) installed.

    From this command line in this directory, `func start` with run the function and process messages that have been sent to IoT Hub from the Compression Module and write the output of the message to the specified blob storage with a random guid name. 

    The `launch.json` file has the necessary information to run the debug experience in VS Code.

3.  Version

    Current & released version is 0.0.1 which includes complementary logic to process a compressed IoT Edge message with `index.js`
    using the same package as the IoT Edge module.  This code does not provide a mechanism to perform the opposite operation (compress message in the cloud and decompress on device) which would require a different function.

# Build and Test

To test the `compress-message.js` functionality, you can use the tests within the IoT Edge __CompressionNodeModule/tests__. Instructions for how
those tests are run with the IoT Edge CompressionNodeModule are included in the README.md in that project in the _Build and Test_ section.  You will
need to copy the *tests* folder into *CompressionFnc* and the packages required for testing from `package.json` and `npm install` them from the command line.

# Contribute

To contribute to this code either file an issue with the details of the bug along with repro steps or 
make a pull request explaining the issue which you've run into that it addresses along with repro steps.