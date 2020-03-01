using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace QM
{
    /// <summary>
    /// Class responsible for sending and reciving data from client.
    /// </summary>
    public class Server
    {
        private string ServerContentFolderPath { get; set; }
        private IManager Manager { get; set; }
        private HttpListener httpListener;
        private readonly Random random;

        public Server(IManager manager, string[] prefixes, string serverContentFolderPath)
        {
            httpListener = new HttpListener();
            random = new Random();
            this.Manager = manager;
            foreach (string prefix in prefixes)
            {
                httpListener.Prefixes.Add(prefix);
            }
            ServerContentFolderPath = serverContentFolderPath;
        }

        /// <summary>
        /// Function starts HttpListener.
        /// </summary>
        public async void StartAsync()
        {
            if (httpListener.IsListening)
                return;

            httpListener.Start();
            PowerHelper.ForceSystemAwake();
            while (httpListener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await httpListener.GetContextAsync();
                    await Task.Run(() => ProcessRequestAsync(context));
                }
                catch(HttpListenerException)
                {
                    break;
                }
                catch(Exception e)
                {
                    // Unexepected error occured
                    Debug.WriteLine(e.Message);
                    break;
                }
            }
        }

        /// <summary>
        /// Function stops HttpListener
        /// </summary>
        public void Stop()
        {
            if (httpListener.IsListening)
            {
                httpListener.Stop();
                PowerHelper.ResetSystemDefault();
            }
        }

        /// <summary>
        /// Close httpListener
        /// </summary>
        public void Close()
        {
            httpListener.Close();
        }

        /// <summary>
        /// Gets one of the local IPv4 addresses of network interface type specified in application settings.
        /// </summary>
        /// <returns>Local IPv4 address of selected interface.</returns>
        /// source=https://stackoverflow.com/questions/6803073/get-local-ip-address
        public string GetLocalIPv4()
        {
            string output = "";
            NetworkInterfaceType type = ServiceProvider.GetApplicationSettings.NetworkInterfaceType;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Basic async function used by server. Responsible for responding to all client request.
        /// Uses Test.AddRecord() method to validate and process data send by client (data send as html-form, method=post, action="/").
        /// </summary>
        /// <param name="context"></param>
        async void ProcessRequestAsync(HttpListenerContext context)
        {
            byte[] dataToSend;
            string fileName;
            string fileExtension;
            string path = ServerContentFolderPath;
            try
            {
                fileName = Path.GetFileName(context.Request.RawUrl);
                fileExtension = Path.GetExtension(fileName);
            }
            catch (ArgumentException)
            {
                fileName = "";
                fileExtension = "";
            }

            if (fileName == null || fileName == "")//Load main page
            {
                fileName = "index.html";
                path = Path.Combine(path, fileName);
                context.Response.ContentType = "text/html";
            }
            else //Change path and set expected response.ContentType 
            {
                switch (fileExtension)
                {
                    case ".js":
                        path = Path.Combine(ServerContentFolderPath, @"Scripts/", fileName);
                        context.Response.ContentType = "text/javascript";
                        break;
                    case ".css":
                        path = Path.Combine(ServerContentFolderPath, @"Content/", fileName);
                        context.Response.ContentType = "text/css";
                        break;
                    case ".json":
                        path = Path.Combine(ServerContentFolderPath, @"Json/", $"data{random.Next() % Manager.NumberOfGroups}.json");
                        context.Response.ContentType = "application/json";
                        break;
                    case ".svg":
                        path = Path.Combine(ServerContentFolderPath, @"Images/",fileName);
                        context.Response.ContentType = "image/svg+xml";
                        break;
                    default:
                        path = Path.Combine(path, "index.html");
                        context.Response.ContentType = "text/html";
                        break;
                }
            }

            if (context.Request.HasEntityBody)
            {
                var ip = context.Request.RemoteEndPoint.Address.ToString();
                NameValueCollection nameValueCollection;

                try
                {
                    // Read and parse data send by a client
                    var recivedData = context.Request.InputStream;
                    StreamReader streamReader = new StreamReader(recivedData);
                    string queryString = streamReader.ReadToEnd();
                    streamReader.Close();
                    nameValueCollection = HttpUtility.ParseQueryString(queryString);
                }
                catch (Exception)
                {
                    nameValueCollection = null;
                }

                // Calling the thread-safe function AddRecord() of current TestManager instance
                string responseString = Manager.AddRecord(nameValueCollection, ip); 

                if (responseString == null) 
                {
                    // Error occured
                    dataToSend = Encoding.UTF8.GetBytes("BadRequest error. Please refresh main page.");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else 
                {
                    // Load custom template for successful response 
                    string pageToSend = File.ReadAllText(Path.Combine(ServerContentFolderPath, "info.txt"));
                    // Insert response string into template
                    pageToSend = pageToSend.Replace("***InsertStringHere***", responseString);
                    context.Response.ContentType = "text/html";
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    dataToSend = Encoding.UTF8.GetBytes(pageToSend);
                }
            }
            else if (File.Exists(path)) 
            {
                // If there was no data included for server to process just send back from path variable
                dataToSend = File.ReadAllBytes(path);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                // When resource cannot be found
                dataToSend = Encoding.UTF8.GetBytes("Nie znaleziono zasobu.");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            // Fill necassary fields in response you send back to client
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = dataToSend.Length;
            using (Stream s = context.Response.OutputStream)
                await s.WriteAsync(dataToSend, 0, dataToSend.Length);
            context.Response.Close();
        }
    }
}
