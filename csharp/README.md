The C# code is broken into 3 folders along with a test data folder:

1) cloud - This code is for the Azure Function which receives a message using the Compression Library and writes it to Azure Blob Storage.
2) edge - This code is for the Azure IoT Edge device which consists of the IoT Edge modules as shown in the architecture diagram.  One module which sends the data, and the Compression module which demonstrates use of the library and routes the compressed data to the IoT Hub.
3) shared - This is the Compression library code.  Since this code is not hosted in a package manager, this allows both the Functions code and the IoT Edge code to use the same library.  There are caveats around this and both the Function and the IoT Edge require unique steps to use a repository with this structure.
4) messages - This contains test data messages that are played back by the edge message simulator.

Each of these folders contains a further README which outlines their installation, use and idiosyncracies.