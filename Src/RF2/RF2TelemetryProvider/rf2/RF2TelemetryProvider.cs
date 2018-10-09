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
using System.Threading;

namespace SimFeedback.telemetry.rf2
{
    /// <summary>
    /// RFactor2 TelemetryProvider
    /// </summary>
    public sealed class RF2TelemetryProvider : AbstractTelemetryProvider
    {
        private const string sharedMemoryFile = @"Local\RF2SF";         // the name of the shared memory file
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;                                               // the polling thread, reads telemetry data and sends TelemetryUpdated events

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public RF2TelemetryProvider() : base()
        {
            Author = "saxxon66";
            Version = "v0.1";
            BannerImage = @"img\banner_rf2.png"; // Image shown on top of the profiles tab
            IconImage = @"img\rf2.png";          // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "rf2"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing RF2TelemetryProvider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(RF2Data));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting RF2TelemetryProvider");
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
            LogDebug("Stopping RF2TelemetryProvider");
            isStopped = true;
            if(t != null)t.Join();
        }

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            RF2Data lastTelemetryData = new RF2Data();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!isStopped)
            {
                try
                {
                    // get data from game, 
                    // if an exception will be thrown, we could not retrieve the data because the game 
                    // is not running or something went wrong
                    RF2Data telemetryData = (RF2Data)readSharedMemory(typeof(RF2Data), sharedMemoryFile);
                    // otherwise we are connected
                    IsConnected = true;

                    if (telemetryData.time != lastTelemetryData.time)
                    {
                        IsRunning = true;

                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new RF2TelemetryInfo(telemetryData, lastTelemetryData));
                        RaiseEvent(OnTelemetryUpdate, args);

                        lastTelemetryData = telemetryData;
                    }
                    else if(sw.ElapsedMilliseconds > 500)
                    {
                        IsRunning = false;
                    }

                    Thread.Sleep(SamplePeriod);
                }
                catch (Exception e)
                {
                    LogDebug("RF2TelemetryProvider Exception while processing data", e);
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }
            }

            IsConnected = false;
            IsRunning = false;
        }

    }
}
