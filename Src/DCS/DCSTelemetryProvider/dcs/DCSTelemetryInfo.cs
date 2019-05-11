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
using System;
using System.Reflection;

namespace SimFeedback.telemetry.dcs
{
    internal class DCSTelemetryInfo : TelemetryInfo
    {
        private DCSData telemetryData;
        private DCSData lastTelemetryData;
        private Session session;

        public DCSTelemetryInfo(DCSData telemetryData, DCSData lastTelemetryData, Session session)
        {
            this.telemetryData = telemetryData;
            this.lastTelemetryData = lastTelemetryData;
            this.session = session;
        }

        private float PitchRoc
        {
            get
            {
                return (telemetryData.pitchrate - lastTelemetryData.pitchrate) / (telemetryData.time - lastTelemetryData.time);
            }
        }

        private float PitchRate
        {
            get
            {
                return telemetryData.pitchrate;
            }
        }

        private float RollRoc
        {
            get
            {
                return (telemetryData.rollrate - lastTelemetryData.rollrate) / (telemetryData.time - lastTelemetryData.time);
            }
        }

        private float RollRate
        {
            get
            {
                return telemetryData.rollrate;
            }
        }

        private float YawRoc
        {
            get
            {
                return (telemetryData.yawrate - lastTelemetryData.yawrate) / (telemetryData.time - lastTelemetryData.time);
            }
        }

        private float YawRate
        {
            get
            {
                return telemetryData.yawrate;
            }
        }

        private float Roll
        {
            get
            {
                return (float)(Math.Cos(telemetryData.pitch) * Math.Sin(telemetryData.roll));
            }
        }

        private float Pitch
        {
            get
            {
                return (float)Math.Sin(telemetryData.pitch);
            }
        }

        private float Yaw
        {
            get
            {
                return (float)Math.Sin(telemetryData.yaw);
            }
        }


        public TelemetryValue TelemetryValueByName(string name)
        {
            TelemetryValue tv;
            switch (name)
            {
                case "roll":
                    tv = new DCSTelemetryValue("roll", Roll);
                    break;

                case "pitch":
                    tv = new DCSTelemetryValue("pitch", Pitch);
                    break;

                case "yaw":
                    tv = new DCSTelemetryValue("yaw", Yaw);
                    break;

                case "rollrate":
                    tv = new DCSTelemetryValue("rollrate", RollRate);
                    break;

                case "pitchrate":
                    tv = new DCSTelemetryValue("pitchrate", PitchRate);
                    break;

                case "yawrate":
                    tv = new DCSTelemetryValue("yawrate", YawRate);
                    break;

                case "rollroc":
                    tv = new DCSTelemetryValue("rollroc", RollRoc);
                    break;

                case "pitchroc":
                    tv = new DCSTelemetryValue("pitchroc", PitchRoc);
                    break;

                case "yawroc":
                    tv = new DCSTelemetryValue("yawroc", YawRoc);
                    break;

                default:
                    object data;
                    Type eleDataType = typeof(DCSData);
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
                    tv = new DCSTelemetryValue(name, data);
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