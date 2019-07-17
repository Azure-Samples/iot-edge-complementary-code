Why use a /shared folder?
Since the compression and decompression code is used both by the Edge and Cloud solutions (since they are complementary operations), rather than copying and pasting the code into two directories, it exists at the same level as both of these.  In order to use code at this level, the normal Docker build process for the Compression Module requires some modification.
1) CompressionModule.csproj includes a reference to the csproj file in the directory.
2) The build context for the Docker build step must be elevated.  In the module.json file, on line 16, "contextPath" elevates the build context out to the csharp root directory.   
3) The .dockerignore file ignores the Azure Function code in the cloud_CompressionCSharpFnc folder to reduce the build context sent to Docker (reduction from ~46MB to 3MB).
4) In the CompressionCSharpModule folder, each Dockerfile has been modified to COPY the entire directory that is not ignored by the Dockerfile and build the project.  
5) This provides a model for testing the package code that is separated out from the modules and function code.  When this was only an IoT Edge Solution, the CompressionCSharpTests\ folder existed within the modules\ folder alongside the CompressionCSharpModule\ folder and had to be excluded with a .dockerignore file.  This makes it more straightforward to integrate with a package manager.
