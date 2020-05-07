using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CalculateFunding.Common.Extensions
{
    public static class StringExtensions
    {
        public static byte[] Compress(this string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return new byte[0];
            }

            byte[] data = body.AsUTF8Bytes();

            byte[] zippedBytes;

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(outputStream, CompressionMode.Compress, false))
                {
                    gZipStream.Write(data, 0, data.Length);

                    gZipStream.Flush();

                    outputStream.Flush();
                    zippedBytes = outputStream.ToArray();
                }
            }

            return zippedBytes;
        }

        public static byte[] AsUTF8Bytes(this string text)
        {
            return Encoding.UTF8.GetBytes(text ?? string.Empty);
        }

    }
}
