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
namespace SimFeedback.extension
{
    public interface SimFeedbackExtensionFacade
    {
        /// <summary>
        /// Write message to log file INFO level.
        /// </summary>
        /// <param name="msg">The message</param>
        void Log(string msg);

        /// <summary>
        /// Write message to log file DEBUG level.
        /// </summary>
        /// <param name="msg">The message</param>
        void LogDebug(string msg);

        /// <summary>
        /// Write message to log file ERROR level.
        /// </summary>
        /// <param name="msg">The message</param>
        void LogError(string msg);

        /// <summary>
        /// Start the system.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the system.
        /// </summary>
        void Stop();

        /// <summary>
        /// System is running
        /// </summary>
        /// <returns>true it it is running otherwise false</returns>
        bool IsRunning();

        /// <summary>
        /// TelemetryProvider is connected to the game.
        /// </summary>
        /// <returns>true if connected otherwise false</returns>
        bool IsTelemetryProviderConnected();

        /// <summary>
        /// Increments the overall Intensity 
        /// </summary>
        void IncrementOverallIntensity();

        /// <summary>
        /// Decrements the overall Intensity 
        /// </summary>
        void DecrementOverallIntensity();

        /// <summary>
        /// All effects will be disabled
        /// </summary>
        void DisableAllEffects();

        /// <summary>
        /// All effects will be enabled
        /// </summary>
        void EnableAllEffects();

        /// <summary>
        /// Saves the configuration, us ICustomConfig
        /// </summary>
        void SaveConfig();

    }
}