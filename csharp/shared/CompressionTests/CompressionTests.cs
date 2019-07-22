using Xunit;
using DataCompression;
using System.IO;

namespace CompressionTests
{
    public class CompressionTests 
    {
        private static byte[] emptyMsgData = new byte[]{};
        private byte[] decompressedContentData;
        private byte[] compressedContentData;
     
        public CompressionTests()
        {
            decompressedContentData = File.ReadAllBytes("test/1.xml");
            compressedContentData = File.ReadAllBytes("test/1.xml.gz");
        }

        [Fact]
        public void Compress_Empty_Buffer_Returns_Empty_Buffer()
        {
            Assert.Equal(emptyMsgData, CompressionClass.Compress(emptyMsgData));
        }
        
        [Fact]
        public void Compress_Decompress_Returns_Original_Data()
        {
            var compressedData = CompressionClass.Compress(decompressedContentData);
            Assert.Equal(decompressedContentData, CompressionClass.Decompress(compressedData));
        }

        [Fact]
        public void Decompress_Empty_Data_Returns_Empty_Data()
        {
            Assert.Equal(emptyMsgData, CompressionClass.Decompress(emptyMsgData));
        }

        [Fact]
        public void Decompress_External_GZip_Data_Returns_Original_Data()
        {
            Assert.Equal(decompressedContentData, CompressionClass.Decompress(compressedContentData));
        }
    }
}
