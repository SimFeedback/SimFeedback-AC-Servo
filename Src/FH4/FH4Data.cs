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

namespace FH4
{
    class FH4Data
    {
        #region Telemetry Property

        
        public int IsRaceOn { get; set; } // = 1 when race is on. = 0 when in menus/race stopped …


        
        public uint TimestampMS { get; set; } //Can overflow to 0 eventually


        
        public float EngineMaxRpm { get; set; }

        
        public float EngineIdleRpm { get; set; }

        
        public float CurrentEngineRpm { get; set; }


        private static float G = 9.81f;
        private static float Rad2Deg(float v) { return (float)(v * 180 / Math.PI); }


        private float _accX, _accY, _accZ;
        public float AccelerationX { get { return _accX / G; } set { _accX = value; } } //In the car's local space; X = right, Y = up, Z = forward

        
        public float AccelerationY { get { return _accY / G; } set { _accY = value; } }


        public float AccelerationZ { get { return _accZ / G; } set { _accZ = value; } }


        public float VelocityX { get; set; } //In the car's local space; X = right, Y = up, Z = forward

        
        public float VelocityY { get; set; }

        
        public float VelocityZ { get; set; }


        
        public float AngularVelocityX { get; set; } //In the car's local space; X = pitch, Y = yaw, Z = roll

        
        public float AngularVelocityY { get; set; }

        
        public float AngularVelocityZ { get; set; }


        

        public float Yaw { get; set; }

        private float _pitch;
        public float Pitch { get { return Rad2Deg(_pitch);} set { _pitch = value;  } }

        private float _roll;
        public float Roll { get { return Rad2Deg(_roll); } set { _roll = value; } }



        public float NormalizedSuspensionTravelFrontLeft { get; set; } // Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression

        
        public float NormalizedSuspensionTravelFrontRight { get; set; }

        
        public float NormalizedSuspensionTravelRearLeft { get; set; }

        
        public float NormalizedSuspensionTravelRearRight { get; set; }


        
        public float TireSlipRatioFrontLeft { get; set; } // Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.

        
        public float TireSlipRatioFrontRight { get; set; }

        
        public float TireSlipRatioRearLeft { get; set; }

        
        public float TireSlipRatioRearRight { get; set; }


        
        public float WheelRotationSpeedFrontLeft { get; set; } // Wheel rotation speed radians/sec. 

        
        public float WheelRotationSpeedFrontRight { get; set; }

        
        public float WheelRotationSpeedRearLeft { get; set; }

        
        public float WheelRotationSpeedRearRight { get; set; }


        
        public int WheelOnRumbleStripFrontLeft { get; set; } // = 1 when wheel is on rumble strip, = 0 when off.

        
        public int WheelOnRumbleStripFrontRight { get; set; }

        
        public int WheelOnRumbleStripRearLeft { get; set; }

        
        public int WheelOnRumbleStripRearRight { get; set; }


        
        public float WheelInPuddleDepthFrontLeft { get; set; } // = from 0 to 1, where 1 is the deepest puddle

        
        public float WheelInPuddleDepthFrontRight { get; set; }

        
        public float WheelInPuddleDepthRearLeft { get; set; }

        
        public float WheelInPuddleDepthRearRight { get; set; }


        
        public float SurfaceRumbleFrontLeft { get; set; } // Non-dimensional surface rumble values passed to controller force feedback

        
        public float SurfaceRumbleFrontRight { get; set; }

        
        public float SurfaceRumbleRearLeft { get; set; }

        
        public float SurfaceRumbleRearRight { get; set; }


        
        public float TireSlipAngleFrontLeft { get; set; } // Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.

        
        public float TireSlipAngleFrontRight { get; set; }

        
        public float TireSlipAngleRearLeft { get; set; }

        
        public float TireSlipAngleRearRight { get; set; }


        
        public float TireCombinedSlipFrontLeft { get; set; } // Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.

        
        public float TireCombinedSlipFrontRight { get; set; }

        
        public float TireCombinedSlipRearLeft { get; set; }

        
        public float TireCombinedSlipRearRight { get; set; }


        
        public float SuspensionTravelMetersFrontLeft { get; set; } // Actual suspension travel in meters

        
        public float SuspensionTravelMetersFrontRight { get; set; }

        
        public float SuspensionTravelMetersRearLeft { get; set; }

        
        public float SuspensionTravelMetersRearRight { get; set; }


        
        public int CarOrdinal { get; set; } //Unique ID of the car make/model

        
        public int CarClass { get; set; } //Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive 

        
        public int CarPerformanceIndex { get; set; } //Between 100 (slowest car) and 999 (fastest car) inclusive

        
        public int DrivetrainType { get; set; } //Corresponds to EDrivetrainType; 0 = FWD, 1 = RWD, 2 = AWD

        
        public int NumCylinders { get; set; } //Number of cylinders in the engine

        #endregion

        public FH4Data(byte[] rawTelemetryData)
        {
            this.IsRaceOn = BitConverter.ToInt32(rawTelemetryData, 0);
            this.TimestampMS = BitConverter.ToUInt32(rawTelemetryData, 4);
            this.EngineMaxRpm = BitConverter.ToSingle(rawTelemetryData, 8);
            this.EngineIdleRpm = BitConverter.ToSingle(rawTelemetryData, 12);
            this.CurrentEngineRpm = BitConverter.ToSingle(rawTelemetryData, 16);
            this.AccelerationX = BitConverter.ToSingle(rawTelemetryData, 20);
            this.AccelerationY = BitConverter.ToSingle(rawTelemetryData, 24);
            this.AccelerationZ = BitConverter.ToSingle(rawTelemetryData, 28);
            this.VelocityX = BitConverter.ToSingle(rawTelemetryData, 32);
            this.VelocityY = BitConverter.ToSingle(rawTelemetryData, 36);
            this.VelocityZ = BitConverter.ToSingle(rawTelemetryData, 40);
            this.AngularVelocityX = BitConverter.ToSingle(rawTelemetryData, 44);
            this.AngularVelocityY = BitConverter.ToSingle(rawTelemetryData, 48);
            this.AngularVelocityZ = BitConverter.ToSingle(rawTelemetryData, 52);
            this.Yaw = BitConverter.ToSingle(rawTelemetryData, 56);
            this.Pitch = BitConverter.ToSingle(rawTelemetryData, 60);
            this.Roll = BitConverter.ToSingle(rawTelemetryData, 64);
            this.NormalizedSuspensionTravelFrontLeft = BitConverter.ToSingle(rawTelemetryData, 68);
            this.NormalizedSuspensionTravelFrontRight = BitConverter.ToSingle(rawTelemetryData, 72);
            this.NormalizedSuspensionTravelRearLeft = BitConverter.ToSingle(rawTelemetryData, 76);
            this.NormalizedSuspensionTravelRearRight = BitConverter.ToSingle(rawTelemetryData, 80);
            this.TireSlipRatioFrontLeft = BitConverter.ToSingle(rawTelemetryData, 84);
            this.TireSlipRatioFrontRight = BitConverter.ToSingle(rawTelemetryData, 88);
            this.TireSlipRatioRearLeft = BitConverter.ToSingle(rawTelemetryData, 92);
            this.TireSlipRatioRearRight = BitConverter.ToSingle(rawTelemetryData, 96);
            this.WheelRotationSpeedFrontLeft = BitConverter.ToSingle(rawTelemetryData, 100);
            this.WheelRotationSpeedFrontRight = BitConverter.ToSingle(rawTelemetryData, 104);
            this.WheelRotationSpeedRearLeft = BitConverter.ToSingle(rawTelemetryData, 108);
            this.WheelRotationSpeedRearRight = BitConverter.ToSingle(rawTelemetryData, 112);
            this.WheelOnRumbleStripFrontLeft = BitConverter.ToInt32(rawTelemetryData, 116);
            this.WheelOnRumbleStripFrontRight = BitConverter.ToInt32(rawTelemetryData, 120);
            this.WheelOnRumbleStripRearLeft = BitConverter.ToInt32(rawTelemetryData, 124);
            this.WheelOnRumbleStripRearRight = BitConverter.ToInt32(rawTelemetryData, 128);
            this.WheelInPuddleDepthFrontLeft = BitConverter.ToSingle(rawTelemetryData, 132);
            this.WheelInPuddleDepthFrontRight = BitConverter.ToSingle(rawTelemetryData, 136);
            this.WheelInPuddleDepthRearLeft = BitConverter.ToSingle(rawTelemetryData, 140);
            this.WheelInPuddleDepthRearRight = BitConverter.ToSingle(rawTelemetryData, 144);
            this.SurfaceRumbleFrontLeft = BitConverter.ToSingle(rawTelemetryData, 148);
            this.SurfaceRumbleFrontRight = BitConverter.ToSingle(rawTelemetryData, 152);
            this.SurfaceRumbleRearLeft = BitConverter.ToSingle(rawTelemetryData, 156);
            this.SurfaceRumbleRearRight = BitConverter.ToSingle(rawTelemetryData, 160);
            this.TireSlipAngleFrontLeft = BitConverter.ToSingle(rawTelemetryData, 164);
            this.TireSlipAngleFrontRight = BitConverter.ToSingle(rawTelemetryData, 168);
            this.TireSlipAngleRearLeft = BitConverter.ToSingle(rawTelemetryData, 172);
            this.TireSlipAngleRearRight = BitConverter.ToSingle(rawTelemetryData, 176);
            this.TireCombinedSlipFrontLeft = BitConverter.ToSingle(rawTelemetryData, 180);
            this.TireCombinedSlipFrontRight = BitConverter.ToSingle(rawTelemetryData, 184);
            this.TireCombinedSlipRearLeft = BitConverter.ToSingle(rawTelemetryData, 188);
            this.TireCombinedSlipRearRight = BitConverter.ToSingle(rawTelemetryData, 192);
            this.SuspensionTravelMetersFrontLeft = BitConverter.ToSingle(rawTelemetryData, 196);
            this.SuspensionTravelMetersFrontRight = BitConverter.ToSingle(rawTelemetryData, 200);
            this.SuspensionTravelMetersRearLeft = BitConverter.ToSingle(rawTelemetryData, 204);
            this.SuspensionTravelMetersRearRight = BitConverter.ToSingle(rawTelemetryData, 208);
            this.CarOrdinal = BitConverter.ToInt32(rawTelemetryData, 212);
            this.CarClass = BitConverter.ToInt32(rawTelemetryData, 216);
            this.CarPerformanceIndex = BitConverter.ToInt32(rawTelemetryData, 220);
            this.DrivetrainType = BitConverter.ToInt32(rawTelemetryData, 224);
            this.NumCylinders = BitConverter.ToInt32(rawTelemetryData, 228);
        }


    }
}
