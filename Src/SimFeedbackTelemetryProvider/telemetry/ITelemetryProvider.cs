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

namespace SimFeedback.telemetry
{
    /// <summary>
    /// The Telemetry Provider is the direct link between SimFeedback and the game.
    /// It will establish the connection (shared memory, UDP, ...) and retrieve the telemetry data.
    /// This telemetry data will be send via TelemetryUpdated events.
    /// It is responsible for maintaining the update frequency (TelemetryUpdateFrequency).
    /// </summary>
    public interface ITelemetryProvider
    {
        /// <summary>
        /// The name of the Telemetry Provider.
        /// This name will be used to identify the provider 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version Info. Optional.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Your name here
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Default Banner Image location.
        /// Image location needs to be relative from the app install root directory.
        /// This image is the banner displayed in the gui.
        /// It can be overriden by the profile and is optional.
        /// Size: 460x80 px
        /// </summary>
        string BannerImage { get; }

        // TODO implement Icons from the TelemetryProvider
        string IconImage { get;  }

        /// <summary>
        /// The update frequency in samples per second.
        /// A nice update rate is 100 samples/second.
        /// </summary>
        int TelemetryUpdateFrequency { get; }

        /// <summary>
        /// True, if the telemetry provider was able to connect to the game.
        /// False, as long as the game is not running or could not be reached.
        /// Everytime this property is changed a Connected or Disconnected Event must be raised.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// True, if the telemetry provider sends TelemetryUpdated events.
        /// False, if the game is running but the car is in pit or in replay mode.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Initializes the telemetry provider.
        /// It is called right after SimFeedback starts and after the active profile or a major configuration parameter 
        /// was changed, like the com port of the controller or the soundcard settings.
        /// At this stage, a configuration can be read or data can be loaded.
        /// Do not try to establish the connection to the game at this time, because the game could not be started allready.
        /// </summary>
        void Init(ILogger logger);

        /// <summary>
        /// Starts the connection to the game and begin to raise the TelemetryUpdated Events.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the connection to the game and no more TelemetryUpdated Events will be raised.
        /// </summary>
        void Stop();

        /// <summary>
        /// A list of all telemetry values provided by the game and supported by the Telemetry Provider.
        /// The Telemetry Provider must make sure, that all values in this list will be provided by the TelemetryUpdated Event.
        /// </summary>
        /// <returns>A list of telemetry value names that this Telemetry Provider supports</returns>
        string[] GetValueList();

        /// <summary>
        /// This Event must be raised if new telemetry data was received.
        /// </summary>
        event EventHandler<TelemetryEventArgs> TelemetryUpdate;

        /// <summary>
        /// This Event must be raised if the connection to the game was astablished.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// This Event must be raised if the connection to the game was lost.
        /// </summary>
        event EventHandler Disconnected;


    }
}
