using System;

using Silk.NET;
using Silk.NET.Maths;

namespace Voxelia.Engine.Components
{
    class Transform
    {
        public Vector3D<float> Position;
        public Vector3D<float> Rotation;
        public Vector3D<float> Scale;

        public Transform()
        {
            Position = new();
            Rotation = new();
            Scale = Vector3D<float>.One;
        }

        public Transform(Vector3D<float> position, Vector3D<float> rotation, Vector3D<float> scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Quaternion<float> RotationQuaternion
        {
            get
            {
                return Quaternion<float>.CreateFromYawPitchRoll(float.DegreesToRadians(Rotation.Y), float.DegreesToRadians(Rotation.X), float.DegreesToRadians(Rotation.Z));
            }
        }

        public Vector3D<float> Forward => Vector3D.Normalize(Vector3D.Transform(-Vector3D<float>.UnitZ, RotationQuaternion));
        public Vector3D<float> Right => Vector3D.Normalize(Vector3D.Transform(Vector3D<float>.UnitX, RotationQuaternion));
        public Vector3D<float> Up => Vector3D.Normalize(Vector3D.Transform(Vector3D<float>.UnitY, RotationQuaternion));
    }   
}