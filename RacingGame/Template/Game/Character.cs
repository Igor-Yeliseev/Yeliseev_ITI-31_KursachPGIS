using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Template
{
    /// <summary>
    /// Character object.
    /// </summary>
    class Character : Game3DObject
    {
        /// <summary>Speed of character movements.</summary>
        private float _speed;
        /// <summary>Speed of character movements.</summary>
        /// <value>Speed of character movements.</value>
        public float Speed { get => _speed; set => _speed = value; }


        /// <summary>
        /// Constructor sets initial position, rotation angles and speed.
        /// </summary>
        /// <param name="initialPosition">Initial position.</param>
        /// <param name="yaw">Initial angle of rotation around 0Y axis (x - to left, y - to up, z - to back), rad.</param>
        /// <param name="pitch">Initial angle of rotation around 0X axis (x - to left, y - to up, z - to back), rad.</param>
        /// <param name="roll">Initial rotation around 0Z axis (x - to left, y - to up, z - to back), rad.</param>
        public Character(Vector4 initialPosition, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f, float speed = 10.0f) :
            base(initialPosition, yaw, pitch, roll)
        {
            _speed = speed;
        }

        /// <summary>Move forward or backward (depends of moveBy sign: positive - forward). Yaw = 0 - look to -Z.</summary>
        /// <param name="moveBy">Amount of movement.</param>
        public void MoveForwardBy(float moveBy)
        {
            float dx = moveBy * (float)Math.Sin(_yaw);
            float dz = moveBy * (float)Math.Cos(_yaw);

            _position.X -= dx;
            _position.Z -= dz;
            
        }

        /// <summary>Move to right or to left (depends of moveBy sign: positive - to right).</summary>
        /// <param name="moveBy">Amount of movement.</param>
        public void MoveRightBy(float moveBy)
        {
            float dx = moveBy * (float)Math.Cos(_yaw);
            float dz = moveBy * (float)Math.Sin(_yaw);

            _position.X -= dx;
            _position.Z += dz;
            
        }

        /// <summary>Rotate around 0X axis by deltaPitch (x - to left, y - to up, z - to back), rad. In virtual world, where X0Z is
        /// ground plane and 0Y axis to up pitch angle saturated by -Pi/2 and Pi/2. Получается все по другому: roll - крен,
        /// pitch - рыскание, yaw - тангаж.</summary>
        /// <param name="deltaPitch">Angle, rad.</param>
        public override void PitchBy(float deltaPitch)
        {
            _pitch += deltaPitch;

            if (_PI2 < _pitch)
            {
                _pitch = _PI2;
            }
            else if (-_PI2 > _pitch)
            {
                _pitch = -_PI2;
            }
        }

        /// <summary>
        /// Distance from car to character's camera
        /// </summary>
        public Vector3 distance = new Vector3(-5.0f, 5.0f, -10.0f);
        public void FollowCar(Car car)
        {
            _position = car.Position + distance;

            distance = Vector3.Transform(distance, Matrix3x3.RotationY(car.Angle));
            YawBy(car.Angle);
        }
        
        /// <summary>Rotate around 0Z axis by deltaRoll (x - to left, y - to up, z - to back), rad.</summary>
        /// <param name="deltaRoll">Angle, rad.</param>
        public override void RollBy(float deltaRoll)
        {
        }
    }
}
