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
    /// TelemetryInfo will bring you the real data from the underlying telemetry data.
    /// It only has one method TelemetryValueByName. You can implement some logik on the names, like
    /// square brackets for array values or new calculated values by a custom name, like SlipAngel.
    /// Maybe you need to calculated the difference between two or more values, TelemetryInfo is the place to implement it.
    /// </summary>
    public interface TelemetryInfo
    {
        /// <summary>
        /// TelemetryValueByName is the main and only method for the effect to get data from the Telemetry Provider.
        /// </summary>
        /// <param name="name">The name of the telemetry value provided by the Telemetry Provider</param>
        /// <returns>A TelemetryValue, what is basically the raw value plus a unit (m/s, m/s^2, ...) and its name.</returns>
        TelemetryValue TelemetryValueByName(string name);
    }
}
