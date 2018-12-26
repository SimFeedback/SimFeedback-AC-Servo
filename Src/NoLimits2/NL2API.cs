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
using System.IO;
using System.Text;

namespace SimFeedback.telemetry.NoLimits2
{
    /// <summary>
    /// Most of this code is from https://github.com/zm1868179/No-Limits-2-Controller and written by zach.minton.
    /// No license info was shown on his repo so I asume it is free to use.
    /// 
    /// The original code comes with the game (steam) and is written in Java. See "NoLimits 2\telemetry\java demo client\NL2TelemetryClient.java".
    /// </summary>
    internal class NL2API
    {

        /**
         * Info from this githup repo
         * @see https://github.com/zm1868179/No-Limits-2-Controller
         * 
         * Message Enum
         * Type: Request   
         * Can be used by the client to request common telemetry data. The server will reply with MSG_TELEMETRY
         * DataSize = 0
         */
        public const int REQ_TELEMETRY = 5;


        /**
         * Info from this githup repo
         * @see https://github.com/zm1868179/No-Limits-2-Controller
         * 
         * Message Enum
         * Type: Reply   
         * Will be send by the server as an anwser to MSG_GET_TELEMETRY
         * DataSize = 76 
         *    int32 (state flags)
         *      bit0 -> in play mode
         *      bit1 -> braking
         *      bit2-31 -> reserved
         *    int32 (current rendered frame number) -> can be used to detect if telemetry data is new
         *    int32 (view mode)
         *    int32 (current coaster)
         *    int32 (coaster style id)
         *    int32 (current train)
         *    int32 (current car)
         *    int32 (current seat)
         *    float32 (speed)
         *    float32 (Position x)
         *    float32 (Position y)
         *    float32 (Position z)
         *    float32 (Rotation quaternion x)
         *    float32 (Rotation quaternion y)
         *    float32 (Rotation quaternion z)
         *    float32 (Rotation quaternion w)
         *    float32 (G-Force x)
         *    float32 (G-Force y)
         *    float32 (G-Force z)   
         */
        public const int RES_TELEMETRY = 6;

        public NL2API()
        {
        }

        public bool InPlay;     // Game Status
        public bool Onboard;
        public bool Paused;


        public int FrameNumber;
        int ViewMode;
        int CoasterIndex;
        int CoasterStyleId;
        int CurrentTrain;
        int CurrentCar;
        int CurrentSeat;

        public float Speed;     // speed in km/h

        // position
        public float PosX;      // Position in the world
        public float PosY;
        public float PosZ;

        // acceleration
        public float GForceX;   // G-Forces in G for left right
        public float GForceY;   // G-Forces in G for up down
        public float GForceZ;   // G-Forces in G for front back

        // orientation
        public double Pitch;    // Pitch in degrees
        public double Yaw;      // Yaw in degrees
        public double Roll;     // Roll in degrees

        #region Protocol 

        private const int c_nExtraSizeOffset = 9;

        enum REPONSE_CODES
        {
            N_MSG_OK = 1,
            N_MSG_ERROR = 2,
            N_MSG_TELEMETRY = 6,
        }

        /// <summary>
        /// Create a request for telemetry data
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public static byte[] CreateRequest(int RequestId)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {

                byte[] bytes = new byte[10];
                bytes[0] = (byte)'N';
                encodeUShort16(bytes, 1, REQ_TELEMETRY);
                encodeInt32(bytes, 3, RequestId);
                encodeUShort16(bytes, 7, 0);
                bytes[9] = (byte)'L';
                return bytes;
            }
        }

        public bool ParseResponse(byte[] data)
        {

            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream, Encoding.GetEncoding("iso-8859-1")))
            {
                if (reader.ReadByte() != 'N')
                {
                    throw new Exception("Invalid packet head.");
                }

                REPONSE_CODES msg = (REPONSE_CODES)decodeUShort16(data[1], data[2]);
                int requestId = decodeInt32(data, 3);
                int size = decodeUShort16(data[7], data[8]);

                switch (msg)
                {
                    case REPONSE_CODES.N_MSG_OK:
                        return false;
                    case REPONSE_CODES.N_MSG_TELEMETRY:
                        break;
                    case REPONSE_CODES.N_MSG_ERROR:
                        throw new Exception("Error: " + BitConverter.ToString(data, c_nExtraSizeOffset, size));
                    default:
                        return false;
                }


                if (size == 76)
                {
                    int state = decodeInt32(data, c_nExtraSizeOffset);

                    FrameNumber = decodeInt32(data, c_nExtraSizeOffset + 4);

                    InPlay = (state & 1) != 0;
                    Onboard = (state & 2) != 0;
                    Paused = (state & 4) != 0;

                    ViewMode = decodeInt32(data, c_nExtraSizeOffset + 8);
                    CoasterIndex = decodeInt32(data, c_nExtraSizeOffset + 12);
                    CoasterStyleId = decodeInt32(data, c_nExtraSizeOffset + 16);
                    CurrentTrain = decodeInt32(data, c_nExtraSizeOffset + 20);
                    CurrentCar = decodeInt32(data, c_nExtraSizeOffset + 24);
                    CurrentSeat = decodeInt32(data, c_nExtraSizeOffset + 28);

                    Speed = decodeFloat(data, c_nExtraSizeOffset + 32) * 3.6f;

                    Quaternion quat = new Quaternion();

                    PosX = decodeFloat(data, c_nExtraSizeOffset + 36);
                    PosY = decodeFloat(data, c_nExtraSizeOffset + 40);
                    PosZ = decodeFloat(data, c_nExtraSizeOffset + 44);

                    quat.x = decodeFloat(data, c_nExtraSizeOffset + 48);
                    quat.y = decodeFloat(data, c_nExtraSizeOffset + 52);
                    quat.z = decodeFloat(data, c_nExtraSizeOffset + 56);
                    quat.w = decodeFloat(data, c_nExtraSizeOffset + 60);

                    GForceX = decodeFloat(data, c_nExtraSizeOffset + 64);
                    GForceY = decodeFloat(data, c_nExtraSizeOffset + 68) - 1.0f;
                    GForceZ = decodeFloat(data, c_nExtraSizeOffset + 72);

                    Pitch = RadianToDegree(quat.toPitchFromYUp());
                    Yaw = RadianToDegree(quat.toYawFromYUp());
                    Roll = RadianToDegree(quat.toRollFromYUp());

                    if (data[c_nExtraSizeOffset + 76] != 'L')
                        throw new Exception("Invalid packet head.");

                    return true;
                } else
                {
                    return false;
                }
            }
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private static void encodeUShort16(byte[] bytes, int offset, int n)
        {
            bytes[offset] = (byte)((n >> 8) & 0xFF);
            bytes[offset + 1] = (byte)(n & 0xFF);
        }

        private static void encodeInt32(byte[] bytes, int offset, int n)
        {
            bytes[offset] = (byte)((n >> 24) & 0xFF);
            bytes[offset + 1] = (byte)((n >> 16) & 0xFF);
            bytes[offset + 2] = (byte)((n >> 8) & 0xFF);
            bytes[offset + 3] = (byte)(n & 0xFF);
        }

        private static void encodeBoolean(byte[] bytes, int offset, Boolean b)
        {
            bytes[offset] = (byte)(b ? 1 : 0);
        }

        public static int decodeUShort16(byte b1, byte b2)
        {
            int n1 = (((int)b1) & 0xFF) << 8;
            int n2 = ((int)b2) & 0xFF;
            return n1 | n2;
        }

        private static int decodeInt32(byte[] bytes, int offset)
        {
            int n1 = (((int)bytes[offset]) & 0xFF) << 24;
            int n2 = (((int)bytes[offset + 1]) & 0xFF) << 16;
            int n3 = (((int)bytes[offset + 2]) & 0xFF) << 8;
            int n4 = ((int)bytes[offset + 3]) & 0xFF;
            return n1 | n2 | n3 | n4;
        }

        private static Boolean decodeBoolean(byte[] bytes, int offset)
        {
            return bytes[offset] != 0;
        }

        private static float decodeFloat(byte[] msg, int offset)
        {
            byte[] data = { msg[offset], msg[offset + 1], msg[offset + 2], msg[offset + 3] };
            if (System.BitConverter.IsLittleEndian)
            {
                byte[] data2 = { msg[offset + 3], msg[offset + 2], msg[offset + 1], msg[offset] };
                data = data2;
            }
            return System.BitConverter.ToSingle(data, 0);
        }

        #endregion
    }
}