using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Alan.Utils.ExtensionMethods;

namespace Alan.Utils.UtililyMethods
{
    /// <summary>
    /// Http相关的方法
    /// </summary>
    public class HttpUtils
    {
        /// <summary>
        /// 表单文件
        /// </summary>
        public class FormFileParam
        {
            /// <summary>
            /// 数据
            /// </summary>
            public byte[] Data { get; private set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; private set; }
            /// <summary>
            /// 表单名
            /// </summary>
            public string ParamName { get; private set; }

            public FormFileParam() { }

            public FormFileParam(string fileName, string paramName, byte[] data)
            {
                this.FileName = fileName;
                this.ParamName = paramName;
                this.Data = data;
            }

            public FormFileParam(string fileFullPath)
            {
                this.Data = File.ReadAllBytes(fileFullPath);
                this.ParamName = Path.GetFileNameWithoutExtension(fileFullPath);
                this.FileName = Path.GetFileName(fileFullPath);
            }

        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="file">wen</param>
        /// <param name="contentType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] UploadFile(
            string url,
            FormFileParam file,
            string contentType,
            Dictionary<string, string> data)
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            List<byte> requestBytes = new List<byte>();


            if (data != null)
            {
                string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (KeyValuePair<string, string> dict in data)
                {
                    requestBytes.AddRange(boundaryBytes);
                    string formItem = String.Format(formDataTemplate, dict.Key, dict.Value);
                    requestBytes.AddRange(Encoding.UTF8.GetBytes(formItem));
                }
            }
            requestBytes.AddRange(boundaryBytes);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            var header = String.Format(headerTemplate, file.ParamName, file.FileName, contentType);
            var headerBytes = Encoding.UTF8.GetBytes(header);
            requestBytes.AddRange(headerBytes);

            requestBytes.AddRange(file.Data);

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            requestBytes.AddRange(trailer);

            var requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes.ToArray(), 0, requestBytes.Count);
            requestStream.Close();

            var rep = request.GetResponse();
            using (Stream stream = rep.GetResponseStream())
            {
                if (stream == null) return null;
                return stream.ExRead();
            }
        }



        private string Url { get; set; }
        private string Data { get; set; }
        private string HttpMethod { get; set; }

        public HttpUtils(string url, string data, string method)
        {
            this.Url = url;
            this.Data = data;
            this.HttpMethod = method ?? "GET";
        }

        public static HttpUtils Get(string url, string data, string method)
        {
            return new HttpUtils(url, data, method);
        }

        public T RequestAsModel<T>()
            where T : class
        {
            var rep = this.RequestAsString();
            return rep.ExJsonToEntity<T>();
        }


        public string RequestAsString()
        {
            var bytes = this.Request(null);
            return Encoding.UTF8.GetString(bytes);
        }


        public byte[] Request(Action<Func<string, string>> getHeaders)
        {
            WebRequest req = WebRequest.Create(this.Url);

            if (this.HttpMethod.ToUpper() == "POST")
            {
                req.Method = "POST";
                using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
                    writer.Write(this.Data);
            }

            List<byte> buffers = new List<byte>();
            using (var rep = req.GetResponse())
            {
                using (Stream reader = rep.GetResponseStream())
                {
                    if (reader == null) return null;
                    byte[] buffer = new byte[1024];
                    int readLength;
                    do
                    {
                        readLength = reader.Read(buffer, 0, buffer.Length);
                        buffers.AddRange(buffer.Take(readLength));
                    } while (readLength > 0);
                }

                if (getHeaders != null)
                {
                    getHeaders((key) => rep.Headers[key]);
                }
            }

            return buffers.ToArray();
        }

        public System.Tuple<string, byte[]> DownloadFile()
        {
            var fileName = String.Empty;
            var bytes = this.Request(getHeader =>
            {
                var disposition = getHeader("Content-Disposition");
                if (String.IsNullOrWhiteSpace(disposition))
                {
                    var contentType = (getHeader("Content-Type") ?? "").Split('/');
                    if (contentType.Length > 1) fileName = String.Format("{0}.{1}", Guid.NewGuid(), contentType[1]);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(disposition) || disposition.IndexOf("filename=\"") == -1) return;
                    var fileNames = disposition.Replace("\"", "").Split('=');
                    if (fileNames.Length != 2) return;
                    fileName = fileNames[1];
                }
            });
            if (String.IsNullOrWhiteSpace(fileName)) return Tuple.Create(Guid.NewGuid().ToString(), bytes);
            return System.Tuple.Create(fileName, bytes);
        }

    }
}
