using System;
using System.Numerics;
using Silk.NET;
using Silk.NET.Maths;

using Voxelia.Engine.Rendering;

namespace Voxelia.Engine.Components
{
    public class Camera
    {
        public Transform Transform;

        public float Fov, ClipNear, ClipFar, Aspect;

        public Camera()
        {
            Transform = new Transform();

            Fov = 70.0f;

            ClipNear = 0.0f;
            ClipFar = 1.0f;
            Aspect = (float) 1280 / 720;
        }

        internal Camera(Transform transform, float fov, float clipNear, float clipFar) : this()
        {
            this.Transform = transform;
            this.Fov = fov;
            this.ClipNear = clipNear;
            this.ClipFar = clipFar;
        }

        public Matrix4X4<float> ViewMatrix
        {
            get
            {
                return Matrix4X4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
            }
        }

        public Matrix4X4<float> ProjectionMatrix
        {
            get
            {
                return Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(Fov), Aspect, ClipNear, ClipFar);
            }
        }
    }

}


