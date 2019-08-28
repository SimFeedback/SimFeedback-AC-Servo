// KartKraft Telemetry Provider
// Uses UDP (default port 5000) 
// Requires Telemetry to be switched on KartKraft
// Telemetry light in SimFeedback only active once you're on track 
// Data comes as binary stream, KartKraft's API is in the KartKraft/ folder, a bunch of keys omitted
//
// Plugin Authored by Steely
//
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
using FlatBuffers;
using SimFeedback.log;
using SimFeedback.telemetry.kartkraft;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimFeedback.telemetry.KartKraftTelemetryProvider
{
    public sealed class KartKraftTelemetryProvider : AbstractTelemetryProvider
    {
        private const int PORTNUM = 5000;      // Server Port
        private const string IP = "127.0.0.1";  // Server IP
        IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        private bool isStopped = true;          // flag to control the polling thread
        private Thread t;                       // the polling thread, reads telemetry data and sends TelemetryUpdated events

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public KartKraftTelemetryProvider() : base()
        {
            Author = "steely with a little help from his friends";
            Version = "v0.1";
            BannerImage = @"img\banner_kartkraft.png"; // Image shown on top of the profiles tab
            IconImage = @"img\kartkraft.png";           // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 60;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "kartkraft"; } }

        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(KartKraftData));
        }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
        }


        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting KartKraftProvider");
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
            LogDebug("Stopping KartKraftProvider");
            isStopped = true;
            if (t != null) t.Join();
        }

        #region Connect
        /// <summary>
        /// Connect to the TCP Server of No Limits 2
        /// </summary>
        private void Run()
        {
            //TODO
            KartKraftData lastTelemetryData = new KartKraftData();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, PORTNUM));

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
                            //Thread.Sleep(1000);
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

                    // DCS approach....
                    // Byte[] received = socket.Receive(ref _senderIP);
                    // string resp = Encoding.UTF8.GetString(received);
                    //LogDebug(resp);

                    //KK Approach
                    Byte[] received = socket.Receive(ref _senderIP);
                    ByteBuffer b = new ByteBuffer(received);
                    KartKraft.Frame frame = KartKraft.Frame.GetRootAsFrame(b);

                    KartKraftData telemetryData = parseResponse(frame);

                    IsRunning = true;

                    //TODO
                    TelemetryEventArgs args = new TelemetryEventArgs(
                        new KartKraftTelemetryInfo(telemetryData, lastTelemetryData));
                    RaiseEvent(OnTelemetryUpdate, args);
                    lastTelemetryData = telemetryData;

                    sw.Restart();
                    //Thread.Sleep(SamplePeriod);
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
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

            Log("Listener thread stopped, KartKraftTelemetryProvider.Thread");
        }
        #endregion

        private KartKraftData parseResponse(KartKraft.Frame resp)
        {
            
            KartKraftData outData = new KartKraftData();
            try
            {
                outData.AccelerationX = resp.Motion.Value.AccelerationX;
                outData.AccelerationY = resp.Motion.Value.AccelerationY;
                outData.AccelerationZ = resp.Motion.Value.AccelerationZ;
                outData.Pitch = resp.Motion.Value.Pitch;
                outData.Roll = resp.Motion.Value.Roll;
                outData.Yaw = resp.Motion.Value.Yaw;
                outData.RPM = resp.Dash.Value.Rpm;
                outData.Speed = resp.Dash.Value.Speed;
            }catch(Exception e)
            {
                // sometimes the response object can be null, so we catch them here.
                // LogError(e.ToString()); // nothing wo log, only null values
            }
            return outData;
        }

    }
}
