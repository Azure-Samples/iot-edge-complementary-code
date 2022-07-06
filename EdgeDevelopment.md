# Configure Azure IoT Edge development environment

1. Connect your Azure account to Visual Studio Code

   The *Azure IoT Tools* Visual Studio Code extension pack installs a prerequisite *Azure Account* extension if its not already present.  This extension allows Visual Studio Code to connect to your Azure subscription.  For this sample, Visual Studio Code needs to connect to you Azure IoT Hub service.

   Open the command palette and search for *Azure: Sign In*

   Select this command and you will be prompted to sign into your Azure account in a separate browser window.  After sign-in, you should see *Azure:* followed by your login account in the status bar at the bottom of Visual Studio Code.

2. Connect to your Azure IoT Hub

   There are 2 ways to connect to your Azure IoT Hub from within Visual Studio Code:

   Open the command palette and search for *Azure IoT Hub: Select IoT Hub* 

   ​	**or**

   With the Explorer icon in the Visual Studio Code Activity Bar selected, go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Select the "..." to open the Azure IoT Hub context menu.  From the Context Menu, choose *Select IoT Hub*.  

   Both options will open a selection list of available subscriptions at the top of the Visual Studio window.  After selecting your subscription, all available Azure IoT Hubs in your subscription will be presented in another selection list.  After selecting your Azure IoT Hub, the *AZURE IOT HUB** section in the Explorer pane of Visual Studio Code will be populated with configured Devices and Endpoints.  The Devices list will initially be empty for a new Azure IoT Hub.

3. Create an Azure IoT Edge device 

   This sample is designed to run in the Azure IoT Edge Simulator on a local development machine.  However, the Simulator still connects to your Azure IoT Hub service, and therefore needs an Azure IoT Edge device definition in Azure IoT Hub.  You can create an Azure IoT Edge device in the Azure portal, but its easier from Visual Studio Code with the Azure IoT Edge extension installed.

   There are 2 ways to create an Azure IoT Edge device from Visual Studio Code:

   Open the command palette and search for *Azure IoT Hub: Create IoT Edge Device*. 

   ​	**or**

   With the Explorer icon in the Visual Studio Code Activity Bar selected, go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Select the "..." to open the Azure IoT Hub context menu.  From the Context Menu, choose *Create IoT Edge Device*. 

   Both options will open a prompt for you to enter the name of the device.

   > **Note:** There is also a *Azure IoT Hub: Create Device* command.  This creates a basic IoT device definition, which does not support the Azure IoT Edge Runtime and does not work with the Azure IoT Edge Simulator.

4. Configure Azure IoT Edge Simulator to use your Edge Device identity

   Again, there are 2 ways to create setup the Azure IoT Edge Simulator from within Visual Studio Code 

   Open the palette and search for *Azure IoT Edge: Setup IoT Edge Simulator*.  After selecting the command, a list of devices is displayed.  Select the device you created in the previous step. 

    **or**

   With the Explorer icon in the Visual Studio Code Activity Bar selected, go to the *AZURE IOT HUB* section in the Explorer pane of Visual Studio Code.  Expand the Devices list, and right click on the device you created in the previous step to open the Context Menu.  Select *Setup IoT Edge Simulator* from the Context Menu. 
   
   This command will pass your Edge device credentials to the Azure IoT Edge Simulator via a command in the Terminal Window.
   
   > **Note:** If you try to use the *Setup IoT Edge Simulator* command without first connecting to your Azure IoT Hub, you will instead be prompted to enter the connection string for an Azure IoT Hub device.
   
5. Set environment variables

   The Azure IoT Edge solution deployment manifests (*deployment.template.json* and *deployment.debug.template.json*) and module metadata files (*module.json*) support environment variable substitution.  There are 3 environment variable placeholders used in this sample - *$CONTAINER_REGISTRY_USERNAME*, *$CONTAINER_REGISTRY_PASSWORD* and *$CONTAINER_REGISTRY_ADDRESS*.  These are used to specify your container registry address and login credentials.  To run the code in the Azure IoT Edge Simulator, the *$CONTAINER_REGISTRY_ADDRESS* can be set to the Docker local registry container value of *localhost:5000*.  When using the local registry container value, the $CONTAINER_REGISTRY_USERNAME and $CONTAINER_REGISTRY_PASSWORD are not used.  However, since they are defined in the deployment manifests, they must be defined in order to avoid the "Please set registry credential to .env file." warning message on initial load.  
   To protect secrets, *.env* files should not be included in source control. Therefore, this sample includes a *.env.temp* template file that can be renamed to *.env*  or the values can be copied to your .env file inside the corresponding _csharp_ and _node_ root folders.  To build and run the sample in the Azure IoT Edge Simulator, the following values can be used:

   ```
   CONTAINER_REGISTRY_ADDRESS=localhost:5000
   CONTAINER_REGISTRY_USERNAME=<registry username>
   CONTAINER_REGISTRY_PASSWORD=<registry password>
   ```
   > **Note:** *CONTAINER_REGISTRY_USERNAME* and *CONTAINER_REGISTRY_PASSWORD* are not used with the local registry container (*localhost:5000*), but these variables must be defined with any non-empty value. 

   If you wish to deploy the solution to a real Edge device, make sure to set the values to your container registry.    

6. Verify Docker runtime mode (**Windows only**)

   This sample is built to run in an Ubuntu container and requires a Linux Container runtime. If running on Windows, make sure that that Docker CE is running in the default Linux container mode, not Windows container mode. 

   You can do this by right clicking the Docker icon in the system tray.  If the context menu shows "Switch to Windows containers...", Docker is running in Linux container mode. 

7. Select your target architecture

   Currently, the Azure IoT Edge Visual Studio Code can build Azure IoT modules targeting *amd64* (Linux), *arm32* (Linux) and *windows-amd64*.  The target architecture setting tells the Azure IoT Edge extension which Dockerfile to use in each module directory.  This tutorial only included Dockerfiles that target Linux, so make sure the default *amd64* is selected.

   If *amd64* is not shown next to the Edge icon ![edge icon](./images/edge-icon.png) in the status bar at the bottom of Visual Studio, either select this icon to change the target platform or search for *Azure IoT Edge: Set Default Target Platform for Edge Solution* in the command palette.

