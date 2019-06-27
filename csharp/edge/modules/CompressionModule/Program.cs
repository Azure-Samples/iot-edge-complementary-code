using System;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using DataCompression;
using System.Diagnostics;

namespace CompressionModule
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await Init();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            await WhenCancelled(cts.Token);
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(bool debug = false)
        {
#if DEBUG            
            while (debug && !Debugger.IsAttached)
            {
                Console.WriteLine("Module waiting for debugger to attach...");
                await Task.Delay(1000);
            };
#endif
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("compressMessage", CompressMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("decompressMessage", DecompressMessage, ioTHubModuleClient);
        }

        /// <summary>
        /// This Compress message method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> CompressMessage(Message message, object userContext)
        {
            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain expected values");
            }

            await moduleClient.CompleteAsync(message);

            byte[] messageBytes = message.GetBytes(); //returns the BodyStream of the message as bytes

            Console.WriteLine($"Received message with body size: {messageBytes.Length}");

            if (messageBytes.Length > 0)
            {
                var compressedMessageData = CompressionClass.Compress(messageBytes);
                var outMessage = new Message(compressedMessageData);
                // copy properties from original message to new message
                foreach (var prop in message.Properties)
                {
                    outMessage.Properties.Add(prop.Key, prop.Value);
                }
                // add a new property indicating the message is compressed with gzip so the backend can handle compressed and non-compressed messages 
                outMessage.Properties.Add("compression", "gzip");
                await moduleClient.SendEventAsync("compressMessageOutput", outMessage);
                Console.WriteLine($"Sent compressed message with body size: {compressedMessageData.Length}");
            }
            else
            {
                var outMessage = new Message(messageBytes);
                foreach (var prop in message.Properties)
                {
                    outMessage.Properties.Add(prop.Key, prop.Value);
                }
                await moduleClient.SendEventAsync("compressMessageOutput", outMessage);
                Console.WriteLine("Message had no body and was passed as-is.");
            }
            return MessageResponse.None;
        }


        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> DecompressMessage(Message message, object userContext)
        {
            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            await moduleClient.CompleteAsync(message);

            byte[] messageBytes = message.GetBytes();
            if (message.Properties.Contains(new System.Collections.Generic.KeyValuePair<string, string>("compression", "gzip")))
            {
                Console.WriteLine($"Received compressed message with body size: {messageBytes.Length}");

                if (messageBytes.Length > 0)
                {
                    var decompressedMessageData = CompressionClass.Decompress(messageBytes);
                    var outMessage = new Message(decompressedMessageData);
                    foreach (var prop in message.Properties)
                    {
                        outMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    await moduleClient.SendEventAsync("decompressMessageOutput", outMessage);
                    Console.WriteLine($"Sent decompressed message sent with body size: {decompressedMessageData.Length}");
                }
                else
                {
                    //If message body is 0 then pass through with no changes
                    var outMessage = new Message(messageBytes);
                    foreach (var prop in message.Properties)
                    {
                        outMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    await moduleClient.SendEventAsync("compressMessageOutput", outMessage);
                    Console.WriteLine("Message had no body and was passed as-is.");
                }
            }
            else
            { 
                var outMessage = new Message(messageBytes);
                foreach (var prop in message.Properties)
                {
                    outMessage.Properties.Add(prop.Key, prop.Value);
                }
                await moduleClient.SendEventAsync("decompressMessageOutput", outMessage);
                Console.WriteLine("Message was not compressed and was passed as-is.");
            }
            return MessageResponse.None;
        }
    }
}
