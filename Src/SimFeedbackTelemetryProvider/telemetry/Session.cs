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
using System.Collections.Generic;

namespace SimFeedback.telemetry
{
    /// <summary>
    /// A threadsave Session container for storing stateful information.
    /// You need to implement the session handling by your TelemetryProvider.
    /// </summary>
    public class Session
    {
        Dictionary<string, object> _dict = new Dictionary<string, object>();

        /// <summary>
        /// Stores a value by key
        /// </summary>
        /// <param name="key">the name of the variable</param>
        /// <param name="value">the value to be stored in this session</param>
        public void Set(string key, object value)
        {
            lock (_dict)
            {
                _dict[key] = value;
            }
        }

        /// <summary>
        /// Retrieve a value vrom the session by key.
        /// </summary>
        /// <param name="key">the name of the variable</param>
        /// <param name="defaultValue">The default value, optional, if teh value is not in the session or is null</param>
        /// <returns>The value if exists or null. If a defaultValue is given that will be returned in case of null.</returns>
        public object Get(string key, object defaultValue=null )
        {
            lock (_dict)
            {
                try
                {
                    if (_dict.ContainsKey(key))
                        return _dict[key];
                    else
                        return defaultValue;
                }
                catch (KeyNotFoundException) { return defaultValue; }
            }

        }
    }
}
