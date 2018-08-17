using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    /* Vector3 class for coordinates and other uses */
    public class Vector3
    {
        /// <summary>
        /// x, y, z coordinates
        /// </summary>
        public float x, y, z;

        /// <summary>
        /// Constructor ( initialise all to 0.0f )
        /// </summary>
        public Vector3() { x = y = z = 0.0f; }
        
        /// <summary>
        /// Overloaded Contructor ( initialise to values )
        /// </summary>
        public Vector3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        /// <summary>
        /// Magnitude of the Vector3 ( Squared )
        /// </summary>
        public float Length()
        {
            return (float)(Math.Sqrt(x * x + y * y + z * z));
        }
        /// <summary>
        /// Magnitude of the Vector3 ( Not Squared )
        /// </summary>
        public float LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Normalising to make Vector to Unit Vector
        /// Returns ref to this Vector using it
        /// </summary>
        public Vector3 Normalise()
        {
            try
            {
                x /= Length();
                y /= Length();
                z /= Length();
            }
            catch (DivideByZeroException e)
            {
                return null;
            }
            return this;
        }

        /// <summary>
        /// Normalising to make Vector to Unit Vector
        /// Creates a copy and returns that
        /// </summary>
        public Vector3 Normalised()
        {
            try
            {
                x /= Length();
                y /= Length();
                z /= Length();
            }
            catch (DivideByZeroException e)
            {
                return null;
            }
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Operation Overloading
        /// </summary>
        public Vector3 Zero()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
            return this;
        }

        /// <summary>
        /// Set the Vector3 to certain values
        /// </summary>
        public Vector3 Set(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
            return this;
        }

        /// <summary>
        /// Operation Overloading
        /// </summary>
        public static Vector3 operator-(Vector3 _pointA, Vector3 _pointB)
        {
            return new Vector3(_pointA.x - _pointB.x, _pointA.y - _pointB.y, _pointA.z - _pointB.z);
        }
        public static Vector3 operator+(Vector3 _pointA, Vector3 _pointB)
        {
            return new Vector3(_pointA.x + _pointB.x, _pointA.y + _pointB.y, _pointA.z + _pointB.z);
        }
    }
}
