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
namespace SimFeedback.telemetry
{
    /// <summary>
    /// Interface for a Telemetry Value.
    /// </summary>
    public interface TelemetryValue
    {
        /// <summary>
        /// The Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The unit like m/s, KMH, G, ...
        /// </summary>
        string Unit { get; set; }

        /// <summary>
        /// The value itself
        /// </summary>
        object Value { get; set; }
    }


    public abstract class AbstractTelemetryValue : TelemetryValue
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public abstract object Value { get; set; }
    }
}
