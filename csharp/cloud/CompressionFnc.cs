using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataCompression;
using System;

namespace CompressionFnc
{
    public static class CompressionFnc
    {
        [FunctionName("CompressionFnc")]
        public static async Task Run(
            [IoTHubTrigger("messages/events", ConsumerGroup = "$Default", Connection = "IoTHubEventHubEndpoint")]EventData message,
            [Blob("test-out/{sys.randguid}.xml", FileAccess.Write, Connection = "OutputBlobConnectionString")] Stream output,
            ILogger log)
        {
           
           
            if (message.Properties.Contains(new System.Collections.Generic.KeyValuePair<string, object>("compression", "gzip")))
            {
                var fncResult = CompressionClass.Decompress(message.Body.ToArray());
                log.LogInformation($"Decompressed message: {Encoding.UTF8.GetString(fncResult)}");
            try 
            {
                await output.WriteAsync(fncResult, 0, fncResult.Length);
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
            }
            else
            {
                log.LogInformation($"Received uncompressed message: {Encoding.UTF8.GetString(message.Body.ToArray())}");
                await output.WriteAsync(message.Body.ToArray(), 0, message.Body.Count);
            }
        }
    }
}