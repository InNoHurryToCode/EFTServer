﻿using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using EFTLauncher.Utility;

namespace EFTLauncher.ServerLogic
{
    class ServerResponseListener
    {
        private volatile string domain;     // server address
        private Thread thread;              // request listener thread
        private volatile bool threadHandle; // thread status

        public ServerResponseListener(string domain)
        {
            // get address
            this.domain = domain;
            Logger.Log("INFO: Server domain: " + this.domain);
        }

        public void Initialize()
        {
            if (thread != null)
            {
                return;
            }

            // log status
            Logger.Log("INFO: Initializing http listener");

            // create http listener thread
            threadHandle = true;
            thread = new Thread(ListenerThread);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Terminate()
        {
            if (thread == null || !threadHandle || Thread.CurrentThread == thread)
            {
                return;
            }

            // log status
            Logger.Log("INFO: Joining http listener thread");

            // terminate listener thread
            threadHandle = false;

            if (!thread.Join(1000))
            {
                Logger.Log("ALERT: Thread failed to join, aborting thread");
                thread.Abort();
            }

            thread = null;
        }

        private void ListenerThread()
        {
            // initialize listener
            Logger.Log("INFO: Initializing http listener thread");
            HttpListener httpListener = new HttpListener();

            // check domain
            string charToCheck = domain.Substring(domain.Length - 1);
            if (charToCheck != @"/")
            {
                domain += @"/";
            }

            // listen to domain
            httpListener.Prefixes.Add(domain);
            httpListener.Start();

            // listener thread loop
            Logger.Log("INFO: Entering http listener thread loop");
            while (threadHandle)
            {
                HttpListenerContext context = httpListener.GetContext();

                // receive request
                HttpListenerRequest request = context.Request;
                string requestText;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        requestText = readStream.ReadToEnd();
                    }
                }
                Logger.Log("INFO: Recieved request from " + request.Url);

                // get response to request
                string responseText = GetResponseText(requestText);

                // initiaize response response
                HttpListenerResponse response = context.Response;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;

                // send response
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }

            // terminate listener
            httpListener.Stop();
            Logger.Log("INFO: Terminated http listener thread");
        }

        private string GetResponseText(string requestText)
        {
            return "";
        }
    }
}
