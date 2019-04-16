using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModuleDataCompression;

namespace CompressionCSharpFnc
{
    public static class CompressionCSharpFnc
    {
        [FunctionName("CompressionCSharpFnc")]
        public static async Task Run(
            [IoTHubTrigger("messages/events",ConsumerGroup="$Default", Connection = "IoTHubConnectionString")]EventData message,
            [Blob("test-out/{sys.randguid}", FileAccess.Write, Connection="OriginWebJobs")] Stream output,
            ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body)}");
            var fncResult = CompressionClass.Decompress(message.Body.ToArray());
            log.LogInformation($"Decompressed: {Encoding.UTF8.GetString(fncResult)}");  
            await output.WriteAsync(fncResult, 0, fncResult.Length);
        }
    }
}