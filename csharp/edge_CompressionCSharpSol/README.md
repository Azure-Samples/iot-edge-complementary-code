This is the Azure IoT Edge Compression Module Sample which includes the code for the Filter Module and the Compression Module.  For a high level overview of the pattern used in this module in conjunction with an Azure Function, see the top level readme of this repository.  

Prerequisites

Azure IoT Hub
Azure IoT Edge
Docker
VS Code (optional - although this README is written from the perspective of it installed)
VS Code Azure IoT Edge Extension (??)

The Azure IoT Edge tutorial covers installation and configuration of these products.

Description

This project follows the Azure IoT Edge tutorial for creating a filter module.  The code included here is identical to what the tutorial describes and is only included for ease of setup.  IoT EdgeHub routes send a message from the filter module to the compression module using the /compressMessage route.  This route (and its corresponding /decompressMessage) instructs the module in which function to apply to the incoming data (compressMessage or decompressMessage, respectively).  The Compression module relies on the Gzip compression and decompression in the /shared/Compression/DataCompression.cs file.  

Why use a /shared folder?
Since the compression and decompression code is used both by the Edge and Cloud solutions (since they are complementary operations), rather than copying and pasting the code into two directories, it exists at the same level as both of these.  In order to use code at this level, the normal Docker build process for the Compression Module requires some modification.
1) CompressionCSharpModule.csproj includes a reference to the csproj file in the directory.
2) The build context for the Docker build step must be elevated.  In the module.json file, on line 16, "contextPath" elevates the build context out to the csharp root directory.   
3) The .dockerignore file ignores the Azure Function code in the cloud_CompressionCSharpFnc folder to reduce the build context sent to Docker (reduction from ~46MB to 3MB).
4) In the CompressionCSharpModule folder, each Dockerfile has been modified to COPY the entire directory that is not ignored by the Dockerfile and build the project.  The primary difference between this and the default Dockerfile can be observed by comparing with the same Dockerfile extension (.amd64, .amd64.debug) in the FilterCSharpModule folder.
5) This provides a model for testing the package code that is separated out from the modules and function code.  When this was only an IoT Edge Solution, the CompressionCSharpTests\ folder existed within the modules\ folder alongside the CompressionCSharpModule\ folder and had to be excluded with a .dockerignore file.  This makes it more straightforward to integrate with a package manager.

IoT Edge Input Handler Pattern

