//
// Copyright (c) 2019 Rausch IT
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
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FH4
{
    public class FH4TelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;                                               // the polling thread, reads telemetry data and sends TelemetryUpdated events
        private const int PORTNUM = 4844;      // Server Port
        private IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread


        public FH4TelemetryProvider() : base()
        {
            Author = "saxxon66";
            Version = "v0.1";
            BannerImage = @"img\banner_fh4.png"; // Image shown on top of the profiles tab
            IconImage = @"img\fh4.png";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 60;     // the update frequency in samples per second
        }

        public override string Name { get { return "fh4"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing FH4TelemetryProvider");
        }

        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(FH4Data));
        }

        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting FH4TelemetryProvider");
                isStopped = false;
                t = new Thread(Run);
                t.Start();
            }
        }

        public override void Stop()
        {
            if (!isStopped)
            {
                LogDebug("Stopping FH4TelemetryProvider");
                isStopped = true;
                if (t != null) t.Join();
            }
        }

        private void Run()
        {
            FH4Data data;
            Session session = new Session();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, PORTNUM));

            Log("Listener started (port: " + PORTNUM.ToString() + ") FH4TelemetryProvider.Thread");

            while (!isStopped)
            {
                try
                {
                    // get data from game, 
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            IsRunning = false;
                            IsConnected = false;
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                    else
                    {
                        IsConnected = true;
                    }

                    Byte[] received = socket.Receive(ref _senderIP);
                    data = new FH4Data(received);

                    if (data.IsRaceOn == 1)
                    {
                        IsRunning = true;

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new FH4TelemetryInfo(data, session));
                        RaiseEvent(OnTelemetryUpdate, args);
                    } else
                    {
                        IsRunning = false;
                    }

                    sw.Restart();
                    //Thread.Sleep(SamplePeriod);


                }
                catch (Exception)
                {
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }

            socket.Close();
            IsConnected = false;
            IsRunning = false;
        }
    }

}
