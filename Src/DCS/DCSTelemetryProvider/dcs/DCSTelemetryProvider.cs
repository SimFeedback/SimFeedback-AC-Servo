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
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimFeedback.telemetry.dcs
{
    /// <summary>
    /// DCS TelemetryProvider
    /// You need to replace the C:\Users\[USERNAME]\Saved Games\DCS\Scripts\Export.lua see Plugin dir.
    /// </summary>
    class DCSTelemetryProvider : AbstractTelemetryProvider
    {
        // Port and IP of the game service
        private const int PORTNUM = 6666;      // Server Port
        private const string IP = "127.0.0.1";  // Server IP
        IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        private bool isStopped = true;          // flag to control the polling thread
        private Thread t;                       // the polling thread, reads telemetry data and sends TelemetryUpdated events

        public DCSTelemetryProvider() : base()
        {
            Author = "harryharry + saxxon66";
            Version = "v1.2";
            BannerImage = @"img\banner_dcs.png"; // Image shown on top of the profiles tab
            IconImage = @"img\dcs.png";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "DCS"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing DCS Telemetry Provider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(DCSData));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
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
            isStopped = true;
            if (t != null) t.Join();
        }

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            DCSData lastTelemetryData = new DCSData();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, PORTNUM));

            Session session = new Session();

            Log("Listener thread started (IP: " + IP + ":" + PORTNUM.ToString() + ") DCSTelemetryProvider.Thread");

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
                        else
                        {
                            Thread.Sleep(1);
                        }
                        continue;
                    }
                    else
                    {
                        IsConnected = true;
                    }

                    Byte[] received = socket.Receive(ref _senderIP);
                    string resp = Encoding.UTF8.GetString(received);
                    DCSData telemetryData = ParseReponse(resp);

                    IsRunning = true;

                    TelemetryEventArgs args = new TelemetryEventArgs(
                        new DCSTelemetryInfo(telemetryData, lastTelemetryData, session));
                    RaiseEvent(OnTelemetryUpdate, args);
                    lastTelemetryData = telemetryData;
                    
                    sw.Restart();
                }
                catch (Exception)
                {
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }
            IsConnected = false;
            IsRunning = false;

            try
            {
                socket.Close();
            }
            catch (Exception) { }

            Log("Listener thread stopped, DCSTelemetryProvider.Thread");
        }

        private DCSData ParseReponse(string resp)
        {
            DCSData telemetryData = new DCSData();

            string[] tokens = resp.Split(';');
            if (tokens.Length == 15)
            {
                telemetryData.time = float.Parse(tokens[0], CultureInfo.InvariantCulture);

                telemetryData.pitch = float.Parse(tokens[1], CultureInfo.InvariantCulture);
                telemetryData.roll = float.Parse(tokens[2], CultureInfo.InvariantCulture);
                telemetryData.yaw = float.Parse(tokens[3], CultureInfo.InvariantCulture);

                telemetryData.pitchrate = float.Parse(tokens[4], CultureInfo.InvariantCulture);
                telemetryData.rollrate = float.Parse(tokens[5], CultureInfo.InvariantCulture);
                telemetryData.yawrate = float.Parse(tokens[6], CultureInfo.InvariantCulture);

                telemetryData.pitchroc = float.Parse(tokens[7], CultureInfo.InvariantCulture);
                telemetryData.rollroc = float.Parse(tokens[8], CultureInfo.InvariantCulture);
                telemetryData.yawroc = float.Parse(tokens[9], CultureInfo.InvariantCulture);

                telemetryData.sway = float.Parse(tokens[10], CultureInfo.InvariantCulture);
                telemetryData.heave = float.Parse(tokens[11], CultureInfo.InvariantCulture);
                telemetryData.surge = float.Parse(tokens[12], CultureInfo.InvariantCulture);

                telemetryData.airspeed = float.Parse(tokens[13], CultureInfo.InvariantCulture);
                telemetryData.aoa = float.Parse(tokens[14], CultureInfo.InvariantCulture);
            }

            return telemetryData;
        }
    }
}
