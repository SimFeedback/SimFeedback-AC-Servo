using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFeedback.telemetry.kartkraft
{
    internal class KartKraftData
    {
        public float Yaw;
        public float Pitch;
        public float Roll;
        public float AccelerationX;
        public float AccelerationY;
        public float AccelerationZ;
        public float Traction;
        public float RPM;
        private float _speed;

        public float Heave
        {
            get { return (AccelerationZ / 9.81f); }
        }

        public float Surge
        {
            get { return (AccelerationX / 9.81f); }
        }

        public float Sway
        {
            get { return (AccelerationY / 9.81f); }
        }

        public float Speed
        {
            set { _speed = value; }
            get { return _speed * 3.6f; }
        }

    }
}
