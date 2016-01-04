using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Alan.Utils.ExtensionMethods
{
    /// <summary>
    /// Stream相关扩展方法
    /// </summary>
    public static class ExStream
    {
        /// <summary>
        /// 读流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ExRead(this Stream stream)
        {
            if (stream == null) return null;
            List<byte> responseBytes = new List<byte>();
            byte[] buffer = new byte[1024];
            int read;
            do
            {
                read = stream.Read(buffer, 0, buffer.Length);
                responseBytes.AddRange(buffer.Take(read));
            } while (read > 0);

            return responseBytes.ToArray();
        }

    }
}
