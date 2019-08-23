using System.IO;
using System.IO.Compression;

namespace DataCompression
{
    public class CompressionClass
    {
        public static byte[] Compress(byte[] data)
        {
            if (data.Length == 0)
            {
                return data;
            }

            using (MemoryStream compressedDataStream = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(compressedDataStream, CompressionMode.Compress))
                {
                    zip.Write(data, 0, data.Length);
                }
                return compressedDataStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            if (data.Length == 0)
            {
                return data;
            }

            using (MemoryStream decompressedDataStream = new MemoryStream())
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

