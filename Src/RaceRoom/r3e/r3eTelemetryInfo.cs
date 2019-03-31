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
using SimFeedback.telemetry.r3e.Data;
using System;
using System.Reflection;

namespace SimFeedback.telemetry.r3e
{
    public sealed class r3eTelemetryInfo : EventArgs, TelemetryInfo
    {
        private readonly Shared _teleData;

        public r3eTelemetryInfo(Shared telemetryData)
        {
            _teleData = telemetryData;
        }

        public TelemetryValue TelemetryValueByName(string name)
        {
            object data;

            TelemetryValue tv;
            switch (name)
            {
                default:
                    int arrayIndexPos = -1;
                    int squareBracketPos = name.IndexOf('[');
                    if (squareBracketPos != -1)
                    {
                        int.TryParse(name.Substring(squareBracketPos+1, 1), out arrayIndexPos);
                        name = name.Substring(0, squareBracketPos);
                    }
                    Type eleDataType = typeof(Shared);
                    PropertyInfo propertyInfo;
                    FieldInfo fieldInfo = eleDataType.GetField(name);
                    if (fieldInfo != null)
                    {
                        data = fieldInfo.GetValue(_teleData);
                        if (arrayIndexPos != -1 && data.GetType().IsArray)
                        {
                            float[] array = (float[])data;
                            data = array[arrayIndexPos];
                        }
          
                    }
                    else if ((propertyInfo = eleDataType.GetProperty(name)) != null)
                    {
                        data = propertyInfo.GetValue(_teleData, null);
                    }
                    else
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    break;
            }



            tv = new r3eTelemetryValue(name, data);
            object value = tv.Value;
            if (value == null)
            {
                throw new UnknownTelemetryValueException(name);
            }
            return tv;   
        }
    }
}
