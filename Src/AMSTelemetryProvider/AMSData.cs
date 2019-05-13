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
using System.Runtime.InteropServices;

namespace SimFeedback.telemetry.ams
{
    class Constant
    {
        public const string SharedMemoryName = "$rFactorShared$";

    }
    namespace Data
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct rfVec3<Float>
        {
            public Float x;
            public Float y;
            public Float z;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct rfShared
        {

            // Car local-space velocity
            // Unit: Meter per second (m/s)
            public rfVec3<Double> localVel;

            // Car local-space acceleration
            // Unit: Meter per second squared (m/s^2)
            public rfVec3<Double> localAccel;

            // Car body rotation
            public rfVec3<Double> localRot;

            // Car body angular acceleration (torque divided by inertia)
            public rfVec3<Double> localRotAcceleration;

            public Boolean inRealtime;

            public float currentET;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Shared
        {

            public rfShared rfData;
            public Boolean inRealtime;

            public Boolean isInGame
            {
                get
                {
                    return inRealtime;
                }
            }

            public float Time
            {
                get
                {
                    return rfData.currentET;
                }
            }

            // Properties

            public float Pitch
            {
                get
                {
                    return (float)((rfData.localRot.x) * (180 / Math.PI));
                }
            }

            public float PitchVelocity
            {
                get
                {
                    return (float)((rfData.localRotAcceleration.x) * (180 / Math.PI));
                }
            }

            public float Roll
            {
                get
                {
                    return (float)(rfData.localRot.z * (180 / Math.PI));
                }
            }

            public float RollVelocity
            {
                get
                {
                    return (float)((rfData.localRotAcceleration.z) * (180 / Math.PI));
                }
            }

            public float Yaw
            {
                get
                {
                    return (float)(rfData.localRot.y * (180 / Math.PI));
                }
            }

            public float YawVelocity
            {
                get
                {
                    return (float)((rfData.localRotAcceleration.y) * (180 / Math.PI));
                }
            }

            public float Heave
            {
                get
                {
                    return (float)(rfData.localAccel.y / 9.81);
                }
            }

            public float Surge
            {
                get
                {
                    return (float)(rfData.localAccel.z / -9.81);
                }
            }

            public float Sway
            {
                get
                {
                    return (float)(rfData.localAccel.x / -9.81);
                }
            }


        }
    }
}
