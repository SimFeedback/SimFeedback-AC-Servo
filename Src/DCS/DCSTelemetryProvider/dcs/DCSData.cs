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
namespace SimFeedback.telemetry.dcs
{
    internal class DCSData
    {
        public float time;      // sec
        public float pitch;     // rad
        public float pitchrate; // rad per sec
        public float pitchroc; // rad per (sec*sec)
        public float roll;      // rad
        public float rollrate;  // rad per sec
        public float rollroc;  // rad per (sec*sec)
        public float yaw;       // rad
        public float yawrate;   // rad per sec
        public float yawroc;   // rad per (sec*sec)
        public float sway;      // G
        public float heave;     // G
        public float surge;     // G
        public float airspeed;  // kmh
        public float aoa;      // rad
    }
}