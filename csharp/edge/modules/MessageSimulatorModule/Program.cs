using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;

namespace MessageSimulatorModule
{
    public class Program
    {
        private bool _debug;
        public string OutputChannelName { get; private set; }
        public int Interval { get; private set; }
        public string Folder { get; private set; }

        public string Pattern { get; private set; }

        static async Task Main(string[] args)
        {
            var module = new Program();
            await module.Run();
        }

        public Program(bool debug = false)
        {
            _debug = debug;
        }


        public async Task Run()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            // Unloading assembly or Ctrl+C will trigger cancellation token
            AssemblyLoadContext.Default.Unloading += (ctx) => cancellationTokenSource.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cancellationTokenSource.Cancel();

            try
            {
#if DEBUG            
                while (_debug && !Debugger.IsAttached)
                {
                    Console.WriteLine("Module waiting for debugger to attach...");
                    await Task.Delay(1000, cancellationTokenSource.Token);
                };
#endif

                // use environment variable or defaults
                 Interval = 30000;
                int interval;
                if (Int32.TryParse(Environment.GetEnvironmentVariable("Interval"), out interval))
                    Interval = interval;
                OutputChannelName = Environment.GetEnvironmentVariable("OutputChannelName") ?? "output";
                Folder = Environment.GetEnvironmentVariable("Folder") ?? "messages";
                Pattern = Environment.GetEnvironmentVariable("Pattern") ?? "*.xml";

                MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
                ITransportSettings[] settings = { mqttSetting };

                // Open a connection to the Edge runtime
                using (var moduleClient = await ModuleClient.CreateFromEnvironmentAsync(settings))
                {
                    await moduleClient.OpenAsync(cancellationTokenSource.Token);
                    Console.WriteLine("IoT Hub module client initialized.");

                    await moduleClient.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateHandler, moduleClient, cancellationTokenSource.Token);

                    // Fire the DesiredPropertyUpdateHandler manually to read initial values
                    var twin = await moduleClient.GetTwinAsync();
                    await DesiredPropertyUpdateHandler(twin.Properties.Desired, moduleClient);

                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        var files = System.IO.Directory.GetFiles(Folder, Pattern);
                        foreach (var file in files)
                        {
                            try
                            {
                                var data = await ReadAllBytesAsync(file, cancellationTokenSource.Token);
                                await moduleClient.SendEventAsync(OutputChannelName, new Message(data));
                                await Task.Delay(Interval, cancellationTokenSource.Token);
                            }
                            catch (TaskCanceledException)
                            {
                                Console.WriteLine("Asynchronous operation cancelled.");
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Asynchronous operation cancelled.");
            }

            Console.WriteLine("IoT Hub module client exiting.");
        }

        private async Task DesiredPropertyUpdateHandler(TwinCollection desiredProperties, object userContext)
        {
            var moduleClient = userContext as ModuleClient;

            if (desiredProperties.Contains("OutputChannel"))
            {
                OutputChannelName = desiredProperties["OutputChannel"];
            }
            if (desiredProperties.Contains("Interval"))
            {
                Interval = desiredProperties["Interval"];
            }
            if (desiredProperties.Contains("Folder"))
            {
                Folder = desiredProperties["Folder"];
            }
            if (desiredProperties.Contains("Pattern"))
            {
                Pattern = desiredProperties["Pattern"];
            }
            var reportedProperties = new TwinCollection(new JObject(), null);
            reportedProperties["OutputChannel"] = OutputChannelName;
            reportedProperties["Interval"] = Interval;
            reportedProperties["Folder"] = Folder;
            reportedProperties["Pattern"] = Pattern;
            await moduleClient.UpdateReportedPropertiesAsync(reportedProperties);

        }

        public async Task<byte[]> ReadAllBytesAsync(string filePath, CancellationToken cancellationToken)
        {
            const int bufferSize = 4096;
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: bufferSize, useAsync: true))
            {
                byte[] data = new byte[sourceStream.Length];
                int numRead = 0;
                int totalRead = 0;
              
                while ((numRead = await sourceStream.ReadAsync(data, totalRead, Math.Min(bufferSize,(int)sourceStream.Length-totalRead), cancellationToken)) != 0)
                {
                   totalRead += numRead;
                }
                return data;
            }
        }
    }


}