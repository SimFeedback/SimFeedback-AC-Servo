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
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace SimFeedback.telemetry
{
    /// <summary>
    /// An abstract TelemetryProvider to get you started quick.
    /// No need to use it but it can be very handy.
    /// It will take care for some basic things, like start/stop Events, 
    /// reading shared memory files into a data structure and a method to list 
    /// all of the values names from your data struct.
    /// </summary>
    public abstract class AbstractTelemetryProvider : ITelemetryProvider
    {
        
        private readonly SynchronizationContext context;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AbstractTelemetryProvider()
        {
            context = SynchronizationContext.Current;
            Author = "unknown";
            Version = "";
            BannerImage = @"img\banner_simfeedback.png";
            IconImage = @"img\SimFeedback.png";
        }

        #region Abstract Properties
        /// <summary>
        /// <see cref="ITelemetryProvider.GetValueList"/>
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// <see cref="ITelemetryProvider.GetValueList"/>
        /// </summary>
        public abstract string[] GetValueList();
        /// <summary>
        /// <see cref="ITelemetryProvider.Stop"/>
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// <see cref="ITelemetryProvider.Start"/>
        /// </summary>
        public abstract void Start();
        #endregion

        /// <summary>
        /// <see cref="ITelemetryProvider.Init"/>
        /// Call this from your impelementation, if you are overwriting this method, to
        /// set the Logger instance.
        /// </summary>
        public virtual void Init(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// <see cref="ITelemetryProvider.GetValueList"/>
        /// </summary>
        public virtual string Author { get; protected set; }

        /// <summary>
        /// <see cref="ITelemetryProvider.GetValueList"/>
        /// </summary>
        public virtual string Version { get; protected set; }

        /// <summary>
        /// <see cref="ITelemetryProvider.GetValueList"/>
        /// </summary>
        public virtual string BannerImage { get; protected set; }

        /// <summary>
        /// <see cref="ITelemetryProvider.IconImage"/>
        /// </summary>
        public virtual string IconImage { get; protected set; }

        /// <summary>
        /// <see cref="ITelemetryProvider.TelemetryUpdateFrequency"/>
        /// </summary>
        public virtual int TelemetryUpdateFrequency
        {
            get;
            protected set;
        }

        private bool _isConnected = false;
        /// <summary>
        /// <see cref="ITelemetryProvider.IsConnected"/>
        /// Setting this Property will raise Connected or Disconnected events on change.
        /// </summary>
        public virtual bool IsConnected
        {
            get { return _isConnected; }
            protected set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    if (_isConnected)
                        RaiseEvent(FireConnected, new TelemetryEventArgs());
                    else
                        RaiseEvent(FireDisconnected, new TelemetryEventArgs());
                }
            }
        }

        /// <summary>
        /// <see cref="ITelemetryProvider.IsRunning"/>
        /// </summary>
        public virtual bool IsRunning
        {
            get;
            protected set;
        }



        #region Shared Memory
        private byte[] ReadBuffer(Stream memoryMappedViewStream, int size)
        {
            using (var reader = new BinaryReader(memoryMappedViewStream))
            {
                return reader.ReadBytes(size);
            }
        }

        /// <summary>
        /// Read a shared memory file into a data structure.
        /// </summary>
        /// <param name="T">The type of your data structure to read values in</param>
        /// <param name="sharedMemoryFile">name and location of the shared memory file</param>
        /// <returns>The filled data structure, or an exception if something went wrong.</returns>
        /// <remarks>There is no error handling, all exceptions will be thrown so make sure to catch them all.</remarks>
        protected Object readSharedMemory(Type T, string sharedMemoryFile)
        {
            Object data;
            int sharedMemorySize = Marshal.SizeOf(T); ;
            using (var mappedFile = MemoryMappedFile.OpenExisting(sharedMemoryFile))
            {
                using (var memoryMappedViewStream = mappedFile.CreateViewStream())
                {
                    var buffer = ReadBuffer(memoryMappedViewStream, sharedMemorySize);
                    var alloc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    data = Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), T);
                    memoryMappedViewStream.Close();
                    alloc.Free();
                }
            }
            return data;
        }
        #endregion

        #region Events

        /// <summary>
        /// <see cref="ITelemetryProvider.TelemetryUpdate"/>
        /// </summary>
        public event EventHandler<TelemetryEventArgs> TelemetryUpdate;
        protected void OnTelemetryUpdate(TelemetryEventArgs e)
        {
            TelemetryUpdate?.Invoke(this, e);
        }

        /// <summary>
        /// <see cref="ITelemetryProvider.Connected"/>
        /// </summary>
        public event EventHandler Connected;
        private void FireConnected(TelemetryEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        /// <summary>
        /// <see cref="ITelemetryProvider.Disconnected"/>
        /// </summary>
        public event EventHandler Disconnected;
        private void FireDisconnected(TelemetryEventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        /// <summary>
        /// Helper method to raise an event.
        /// </summary>
        /// <example>
        /// Call it like this
        /// <code>
        ///     TelemetryEventArgs args = new TelemetryEventArgs(
        ///         new acTelemetryInfo(telemetryData, telemetryData));
        ///     RaiseEvent(OnTelemetryUpdated, args);
        /// </code>
        /// </example>
        protected void RaiseEvent<T>(Action<T> del, T e) where T : EventArgs
        {
            var callback = new SendOrPostCallback(obj => del(obj as T));

            if (context != null)
            {
                context.Post(callback, e);
            }
            else
            {
                callback.Invoke(e);
            }
        }
        #endregion

        #region Logging
        /// <summary>
        /// local logger instance, set by init
        /// </summary>
        private ILogger Logger;

        /// <summary>
        /// Writes a message to the log file using loglevel: normal
        /// </summary>
        /// <param name="msg">You message</param>
        protected void Log(string msg)
        {
            Logger.Log(msg);
        }

        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: normal
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        protected void Log(string msg, Exception ex)
        {
            Logger.Log(msg, ex);
        }

        /// <summary>
        /// Writes a message to the log file using loglevel: error
        /// </summary>
        /// <param name="msg">You message</param>
        protected void LogError(string msg)
        {
            Logger.LogError(msg);
        }

        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: error
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        protected void LogError(string msg, Exception ex)
        {
            Logger.LogError(msg, ex);
        }

        /// <summary>
        /// Writes a message to the log file using loglevel: debug
        /// </summary>
        /// <param name="msg">You message</param>
        protected void LogDebug(string msg)
        {
            Logger.LogDebug(msg);
        }

        /// <summary>
        /// Writes a message and the message of the exception to the log file using loglevel: debug
        /// </summary>
        /// <param name="msg">Your Message</param>
        /// <param name="ex">Your Exception</param>
        protected void LogDebug(string msg, Exception ex)
        {
            Logger.LogDebug(msg, ex);
        }
        #endregion

        #region Helper

        /// <summary>
        /// The sample period in millis.
        /// </summary>
        public virtual int SamplePeriod { get { return 1000 / TelemetryUpdateFrequency;  } }

        /// <summary>
        /// Will get the list of names of the public memebers and properties by reflection.
        /// </summary>
        /// <param name="T">The type of your storage object/struct</param>
        /// <returns>A list of names</returns>
        protected string[] GetValueListByReflection(Type T)
        {
            // Use reflection to get all public field and property names of the data model
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            MemberInfo[] members = T.GetProperties(bindingFlags).Concat(T.GetFields(bindingFlags).Cast<MemberInfo>()).ToArray();

            List<string> telemetryNameList = new List<string>();
            foreach (var m in members)
            {
                telemetryNameList.Add(m.Name);
            }
            return telemetryNameList.ToArray();
        }
        #endregion
    }
}
