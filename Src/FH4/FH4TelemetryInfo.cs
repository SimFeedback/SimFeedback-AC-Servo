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
using SimFeedback.telemetry;
using System;
using System.Reflection;

namespace FH4
{
    class FH4TelemetryInfo : TelemetryInfo
    {
        private FH4Data telemetryData;
        private Session session;

        public FH4TelemetryInfo(FH4Data data, Session session)
        {
            telemetryData = data;
            this.session = session;
        }

        private FH4TelemetryValue SurgeLowPass()
        {
            LowpassFilter lp = (LowpassFilter)session.Get("SurgeLowPass", new LowpassFilter());
            float data = (float)lp.firstOrder_lowpassFilter(telemetryData.AccelerationZ, 0.1);
            FH4TelemetryValue tv = new FH4TelemetryValue("Surge", data);
            return tv;
        }

        public TelemetryValue TelemetryValueByName(string name)
        {
            FH4TelemetryValue tv;
            switch (name)
            {
                case "Surge":
                    tv = SurgeLowPass();
                    break;
                default:
                    object data;
                    Type eleDataType = typeof(FH4Data);
                    PropertyInfo propertyInfo;
                    FieldInfo fieldInfo = eleDataType.GetField(name);
                    if (fieldInfo != null)
                    {
                        data = fieldInfo.GetValue(telemetryData);
                    }
                    else if ((propertyInfo = eleDataType.GetProperty(name)) != null)
                    {
                        data = propertyInfo.GetValue(telemetryData, null);
                    }
                    else
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    tv = new FH4TelemetryValue(name, data);
                    object value = tv.Value;
                    if (value == null)
                    {
                        throw new UnknownTelemetryValueException(name);
                    }

                    break;
            }

            return tv;
        }

    }
}
