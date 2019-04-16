using System;
using System.IO;
using System.IO.Compression;

namespace DataCompression {

    public class CompressionClass {
        public static byte[] Compress(byte[] data){
                if(data.Length == 0) {
                    return data;
                }

                using (MemoryStream compressedDataStream = new MemoryStream(data.Length))
                {
                    using (GZipStream zip = new GZipStream(compressedDataStream, CompressionMode.Compress, leaveOpen: true))
                    {
                        zip.Write(data, 0, data.Length);
                    }
                    return compressedDataStream.ToArray();    
                }
        }

        public static byte[] Decompress(byte[] data){
            if(data.Length == 0){
                return data;
            }

            byte[] gzipSize = new byte[4];
            gzipSize[0] = data[data.Length -4];
            gzipSize[1] = data[data.Length -3]; 
            gzipSize[2] = data[data.Length -2];
            gzipSize[3] = data[data.Length -1];

            using (MemoryStream decompressedDataStream =new MemoryStream(BitConverter.ToInt32(gzipSize,0)))
            {
                using (MemoryStream dataStream = new MemoryStream(data, writable: false))
                {
                    using (GZipStream zip = new GZipStream(dataStream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(decompressedDataStream);
                    }
                }
            return decompressedDataStream.ToArray();
            }
        }
    }
}

