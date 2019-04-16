using Xunit;
using DataCompression;
// using Microsoft.Azure.Devices.Client;

namespace CompressionTests
{
    public class CompressionTests
    {
        // private static Message emptyTestMsg = new Message();
        private static byte[] emptyMsgData = new byte[]{};

        //decompressedContentData is the string "this is a message." in decimal.
        private static byte[] decompressedContentData = new byte[] {116, 104, 105, 115, 32, 105, 115, 32, 
            97, 32, 109, 101, 115, 115, 97, 103, 101, 46};

        //compressedContentData is the decompressedContentData result using the gzip library.
        private static byte[] compressedContentData = new byte[] {31, 139, 8, 0, 0, 0, 0, 0, 0, 11, 43, 201, 200, 44,
            86, 0, 162, 68, 133, 220, 212, 226, 226, 196, 244, 84, 61, 0, 249, 210, 81, 69, 18, 0, 0, 0};

        // private static Message decompressedMsg = new Message(decompressedContentData);
        // private static Message compressedMsg = new Message(compressedContentData);


        [Fact]
        public void Compression_Function_Empty_Message_Data_Returns_Empty_Message_Data(){
            Assert.Equal(emptyMsgData,DataCompression.CompressionClass.Compress(emptyMsgData));
        }
        
        [Fact]
        public void Compression_Function_Message_Data_Is_Returned_With_Compressed_Data(){
            Assert.Equal(compressedContentData, DataCompression.CompressionClass.Compress(decompressedContentData));
        }

        [Fact]
        public void Decompression_Function_Empty_Message_Data_Returns_Empty_Message_Data(){
            Assert.Equal(emptyMsgData,DataCompression.CompressionClass.Decompress(emptyMsgData));
        }

        [Fact]
        public void Decompression_Function_Compressed_Message_Data_Is_Returned_As_Decompressed_Message_Data(){
            var contents = DataCompression.CompressionClass.Compress(decompressedContentData);
            Assert.Equal(decompressedContentData,DataCompression.CompressionClass.Decompress(contents));
        }
    }
}
