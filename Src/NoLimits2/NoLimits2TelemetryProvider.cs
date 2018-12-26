//
// Copyright (c) 2018 Rausch IT
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
//
//
using SimFeedback.log;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimFeedback.telemetry.NoLimits2
{
    /// <summary>
    /// No Limits 2 - Roller Coaster Simulator TelemetryProvider for SimFeedback
    /// </summary>
    public sealed class NoLimits2TelemetryProvider : AbstractTelemetryProvider
    {
        private TcpClient tcpClient;    // TCP client instance to connect to the game
        private const int Port = 15151; // TCP port, we use a localhost ip lookup 127.0.0.1.
        private int RequestId = 1;      // Request number increments by each reqwest send to the server
        private bool isStopped = true;  // flag to control the polling thread
        private Thread t;               // The polling thread

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public NoLimits2TelemetryProvider() : base()
        {
            Author = "saxxon66";
            Version = "v0.1";
            BannerImage = @"img\banner_nl2.png"; // Image shown on top of the profiles tab
            IconImage = @"img\nl2.jpg";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "nl2"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(NL2API));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting NoLimits2TelemetryProvider");
                isStopped = false;
                t = new Thread(Run);
                t.Start();
            }
        }

        /// <summary>
        /// Stop the polling thread
        /// </summary>
        public override void Stop()
        {
            LogDebug("Stopping NoLimits2TelemetryProvider");
            isStopped = true;
            if (t != null) t.Join();
        }


        #region Connect
        /// <summary>
        /// Connect to the TCP Server of No Limits 2
        /// </summary>
        private bool Connect()
        {
            var ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            // We are looking for ipv4 addresses only
            IPAddress ipAddress = null;
            foreach (var ip in ipHostInfo.AddressList) {
                if(ip.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    ipAddress = ip;
                    break;
                }
            }

            if (ipAddress != null)
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ipAddress, Port);
                if (tcpClient.Connected)
                {
                    LogDebug("Connected to " + ipAddress.ToString());
                    return true;
                }
                else
                {
                    LogDebug("Can not connect to ip " + ipAddress.ToString());
                    return false;
                }
            } else
            {
                LogDebug("Can not get ip v4 address");
                return false;
            }
        }
        #endregion

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!isStopped)
            {
                try
                {
                    // check if we are connected, otherwise try to connect
                    if (!IsConnected)
                    {
                        IsConnected = Connect();
                        continue;
                    }

                    NetworkStream stream = tcpClient.GetStream();
                    byte[] reqData = NL2API.CreateRequest(RequestId++);
                    stream.Write(reqData, 0, reqData.Length);

                    var reader = new BinaryReader(stream, Encoding.GetEncoding("iso-8859-1"));
                    byte[] resData = readMessage(reader);

                    NL2API telemetryData = new NL2API();
                    telemetryData.ParseResponse(resData);

                    if(!telemetryData.Paused && telemetryData.Onboard && telemetryData.InPlay)
                    {
                        IsRunning = true;
                    }

                    if (IsRunning)
                    {
                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new NL2TelemetryInfo(telemetryData));
                        RaiseEvent(OnTelemetryUpdate, args);
                    }
                    else if (sw.ElapsedMilliseconds > 500)
                    {
                        IsRunning = false;
                    }

                    Thread.Sleep(SamplePeriod);
                }
                catch (Exception e)
                {
                    LogError("NoLimits2TelemetryProvider Exception while processing data", e);
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }

            IsConnected = false;
            IsRunning = false;

            // Close the connection
            try
            {
                if(tcpClient != null)
                    tcpClient.Close();
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Read the data from the stream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] readMessage(BinaryReader input)
        {
            int prefix = input.Read();
            if (prefix != (int)'N')
            {
                if (prefix != -1)
                {
                    throw new Exception("Invalid message received");
                }
                else
                {
                    throw new Exception("No data from server");
                }
            }

            int b1 = input.Read();
            if (b1 == -1)
            {
                throw new Exception("No data from server");
            }
            int b2 = input.Read();
            if (b2 == -1)
            {
                throw new Exception("No data from server");
            }
            int b3 = input.Read();
            if (b3 == -1)
            {
                throw new Exception("No data from server");
            }
            int b4 = input.Read();
            if (b4 == -1)
            {
                throw new Exception("No data from server");
            }

            int b5 = input.Read();
            if (b5 == -1)
            {
                throw new Exception("No data from server");
            }

            int b6 = input.Read();
            if (b6 == -1)
            {
                throw new Exception("No data from server");
            }

            int b7 = input.Read();
            if (b7 == -1)
            {
                throw new Exception("No data from server");
            }

            int b8 = input.Read();
            if (b8 == -1)
            {
                throw new Exception("No data from server");
            }

            int extraSize = NL2API.decodeUShort16((byte)b7, (byte)b8);

            byte[] bytes = new byte[10 + extraSize];

            bytes[0] = (byte)prefix;
            bytes[1] = (byte)b1;
            bytes[2] = (byte)b2;
            bytes[3] = (byte)b3;
            bytes[4] = (byte)b4;
            bytes[5] = (byte)b5;
            bytes[6] = (byte)b6;
            bytes[7] = (byte)b7;
            bytes[8] = (byte)b8;

            for (int i = 0; i < extraSize; ++i)
            {
                int b = input.Read();
                if (b == -1)
                {
                    throw new Exception("No data from server");
                }
                bytes[9 + i] = (byte)b;
            }

            int postfix = input.Read();
            if (postfix != (int)'L')
            {
                if (postfix != -1)
                {
                    throw new Exception("Invalid message received");
                }
                else
                {
                    throw new Exception("No data from server");
                }
            }

            bytes[9 + extraSize] = (byte)postfix;

            return bytes;
        }
    }
}
