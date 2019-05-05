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
using System;

namespace SimFeedback.log
{
    /// <summary>
    /// Supported Loglevel
    /// </summary>
    public enum LOGLEVEL
    {
        NONE = -1,
        ERROR = 0,
        NORMAL = 1,
        DEBUG = 2
    }

    /// <summary>
    /// Simple Logging Interface
    /// </summary>
    public interface ILogger
    {
        LOGLEVEL Loglevel { get; set; }

        /// <summary>
        /// Writes a message to the log file using loglevel: normal
        /// </summary>
        /// <param name="msg">You message</param>
        void Log(string msg);
        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: normal
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        void Log(string msg, Exception ex);

        /// <summary>
        /// Writes a message to the log file using loglevel: error
        /// </summary>
        /// <param name="msg">You message</param>
        void LogError(string msg);

        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: error
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        void LogError(string msg, Exception ex);

        /// <summary>
        /// Writes a message to the log file using loglevel: debug
        /// </summary>
        /// <param name="msg">You message</param>
        void LogDebug(string msg);

        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: debug
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        void LogDebug(string msg, Exception ex);
    }
}
