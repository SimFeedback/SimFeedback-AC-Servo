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

/// <summary>
/// This code is copied most from the Java Demo Client of no limits 2 that comes with the game
/// </summary>
namespace SimFeedback.telemetry.NoLimits2
{
    /// <summary>
    /// Quaternion implemention
    /// </summary>
    internal class Quaternion
    {
        public double x, y, z, w;

        public Quaternion() { }

        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public double toPitchFromYUp()
        {
            double vx = 2 * (x * y + w * y);
            double vy = 2 * (w * x - y * z);
            double vz = 1.0 - 2 * (x * x + y * y);

            return Math.Atan2(vy, Math.Sqrt(vx * vx + vz * vz));
        }

        public double toYawFromYUp()
        {
            return Math.Atan2(2 * (x * y + w * y), 1.0 - 2 * (x * x + y * y));
        }

        public double toRollFromYUp()
        {
            return Math.Atan2(2 * (x * y + w * z), 1.0 - 2 * (x * x + z * z));
        }

        public void ToEulerAngle(out double roll, out double pitch, out double yaw)
        {
	        // roll (x-axis rotation)
	        double sinr_cosp = +2.0 * (w * x + y * z);
            double cosr_cosp = +1.0 - 2.0 * (x * x + y * y);
            roll = Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = +2.0 * (w * y - z * x);
    	    if (Math.Abs(sinp) >= 1)
	    	    pitch = Math.PI / 2 * Math.Sign(sinp); // use 90 degrees if out of range
	        else
		        pitch = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = +2.0 * (w * z + x * y);
            double cosy_cosp = +1.0 - 2.0 * (y * y + z * z);
            yaw = Math.Atan2(siny_cosp, cosy_cosp);
        }
    }
}