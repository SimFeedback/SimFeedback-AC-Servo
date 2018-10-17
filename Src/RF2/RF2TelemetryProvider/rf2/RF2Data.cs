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
using System.Runtime.InteropServices;

namespace SimFeedback.telemetry.rf2
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct RF2Data
    {
        public float time;         // time since started      
        public float pitch;        // pitch in radians (+ up)
        public float roll;         // roll in radians (+ right)
        public float accelX;       // local lateral acceleration (+ left) in m/s^2
        public float accelY;       // local vertical acceleration (+ up) in m/s^2
        public float accelZ;       // local longitudinal acceleration (+ back) in m/s^2
        public float rotAccelX;    // local pitch rotational acceleration (+ up) in radians/sec^2
        public float rotAccelY;    // local yaw rotational acceleration (+ right) in radians/sec^2
        public float rotAccelZ;    // local roll rotational acceleration (+ right) in radians/sec^2
        public float velX;         // local lateral velocity (+ left) in m/s
        public float velY;         // local vertical velocity (+ up) in m/s
        public float velZ;         // local longitudinal velocity (+ back) in m/s
        public float rotVelX;      // local pitch rotational velocity (+ up) in radians/sec
        public float rotVelY;      // local yaw rotational velocity (+ right) in radians/sec
        public float rotVelZ;      // local roll rotational velocity (+ right) in radians/sec


        public float speed;        // speed in m/s
        public float rpm;		   // engine rounds in r/min

        public float Heave
        {
            get { return (accelY / 9.81f); }
        }

        public float Surge
        {
            get { return (accelZ / 9.81f); }
        }

        public float Sway
        {
            get { return (accelX / 9.81f); }
        }

        public float PitchDeg
        {
            get { return pitch * 57.296f;  }
        }

        public float RollDeg
        {
            get { return roll * 57.296f; }
        }

        public float SlipAngle
        {
            get
            {
                float slipAngle = 0.0f;
                if (speed > 5)
                {
                    // Porsche GT3 Cup
                    // Fahrzeug Länge: 4.564
                    // Radstand: 1.980 x 2.456
                    slipAngle = (float)(Math.Atan(
                        (velX - rotVelY * (1.980f / 2))
                        /
                        (velZ - rotVelY * (2.456f / 2))
                        ) * 180 / Math.PI);
                }
                return slipAngle;
            }
        }
    };
}
