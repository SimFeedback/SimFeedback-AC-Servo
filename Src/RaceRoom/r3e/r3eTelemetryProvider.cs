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
using SimFeedback.telemetry.r3e.Data;
using System;
using System.Diagnostics;
using System.Threading;

namespace SimFeedback.telemetry.r3e
{
    class r3eTelemetryProvider : AbstractTelemetryProvider
    {
        private const string sharedMemoryFile = Constant.SharedMemoryName; // the name of the shared memory file
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;                                               // the polling thread, reads telemetry data and sends TelemetryUpdated events


        public r3eTelemetryProvider() : base()
        {
            Author = "saxxon66";
            Version = "v1.2";
            BannerImage = @"img\banner_r3e.png"; // Image shown on top of the profiles tab
            IconImage = @"img\r3e.jpg";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "r3e"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            logger.Log("Initializing RaceRoom Racing Experience (R3E) Telemetry Provider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(Shared));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting RaceRoom Racing Experience (R3E)");
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
            LogDebug("Stopping RaceRoom Racing Experience (R3E)");
            isStopped = true;
            if (t != null) t.Join();
        }

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            Shared telemetryData;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!isStopped)
            {

                try
                {
                    telemetryData = (Shared)readSharedMemory(typeof(Shared), sharedMemoryFile);
                    IsConnected = true;

                    if (telemetryData.GamePaused != 1 && telemetryData.GameInMenus != 1 && telemetryData.Player.GameSimulationTicks != 0)
                    {
                        IsRunning = true;
                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new r3eTelemetryInfo(telemetryData));
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
                    LogDebug("RaceRoom Racing Experience (R3E) TelemetryProvider Exception while processing data", e);
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }

            }

            IsConnected = false;
            IsRunning = false;

            Log("polling stopped, RaceRoom Racing Experience (R3E) TelemetryProvider");
        }


    }
}
