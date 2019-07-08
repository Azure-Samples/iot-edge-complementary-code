# Introduction 

This (Azure) IoT Edge module project serves to illustrate a pattern for separating business logic from Node package logic 
and a way to include package tests. It provides compression/decompression code using zlib with gzip which can be reused
in the Cloud to perform the complementary operation (ie. compress on device and decompress in the cloud).  It also provides
Dockerfiles along with the `.gitignore` and `.dockerignore` to deploy the module without code bloat in the Dockerfile
being shipped to the device.  The complementary operation is designed such that with input A, and functions F, F', 
A will be transformed to A' and can be recovered to its original state. A->F(A)->A'->F'(A')->A

# Getting Started

1. How to use

    To use this module, copy the directory into your VS Code IoT Edge Solution `\modules` folder.
    The routes for this module are
    
    - `/modules/CompressionNodeModule/inputs/compressMessage`
    - `/modules/CompressionNodeModule/inputs/decompressMessage`
    
    The routes correspond to the `compressMessage` and `decompressMessage` functions in the `compress-message.js` file.  The outputs are:

    - `/modules/CompressionNodeModule/outputs/compressMessageOutput`
    - `/modules/CompressionNodeModule/outputs/decompressMessageOutput`
    
    To use the complementary operation in the cloud, copy the `compress-message.js` file to another Node.js 
    application such an Azure Function using Node with IoTHub Trigger and include it in a require 
    statement as shown in `app.js`.

2.	Software dependencies

    The primary dependency for this module is the [Azure IoT Edge OSS project](https://github.com/Azure/iotedge).

    The `package.json` includes the packages necessary for testing (`chai`, `sinon-chai`, `mocha-sinon`).

3.  Version

    Current & released version is 0.0.1 which includes basic business logic to pass an IoT Edge message through app.js
    without modification into the compress or decompress function and send the modified message back to the edgeHub for 
    routing as described in the deployment file.

# Build and Test

Run `npm install` from the directory source to download the packages into this directory.

To test the `compress-message.js` functions, in VS Code the 'Mocha Tests' option is included for the debugger.
Otherwise, from the command line use `mocha test`.  The `.dockerignore` file will remove the tests from 
the Dockerfile for you.

# Contribute
To contribute to this code either file an issue with the details of the bug along with repro steps or 
make a pull request explaining the issue which you've run into that it addresses along with repro steps.