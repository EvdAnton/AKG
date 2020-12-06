using System;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1.ModelDrawing3D
{
    public enum CameraMovement {
        Forward,
        Backward,
        Left,
        Right
    };
    
    public class Camera
    {
        
        private const float YAW         = -90.0f;
        private const float PITCH       =  0.0f;
        private const float SPEED       =  0.05f;
        private const float SENSITIVITY =  0.1f;
        private const float ZOOM        =  45.0f;
        
        public Vector<float> Position { get; set; }
        public Vector<float> Front { get; set; }
        public Vector<float> Up { get; set; }
        public Vector<float> Right { get; set; }
        public Vector<float> WorldUp { get; set; }
        public float MovementSpeed { get; set; }

        public float MouseSensitivity { get; set; }
        public float Zoom { get; set; }

        private float _yaw;
        private float _pitch;
        
        public float Yaw
        {
            get => _yaw;
            set => _yaw = value / 180f * (float) Math.PI;
        }

        public float Pitch
        {
            get => _pitch;
            set => _pitch = value / 180f * (float) Math.PI;
        }

        public Camera()
        {
            Position = Vector<float>.Build.Dense(new[] {0f, 0f, 1f});
            Front = Vector<float>.Build.Dense(new[] {0f, 0f, -1f});
            WorldUp = Vector<float>.Build.Dense(new[] {0f, 1f, 0f});

            Yaw = YAW;
            Pitch = PITCH;
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
            
            UpdateCameraVectors();
        }

        public Matrix<float> GetViewMatrix()
        {
            return MathNetExtension.GetViewMatrix(Position, Position + Front, Up);
        }
        
        public void ProcessKeyboard(CameraMovement direction)
        {
            switch (direction)
            {
                case CameraMovement.Forward:
                    Position += Front * MovementSpeed;
                    break;
                case CameraMovement.Backward:
                    Position -= Front * MovementSpeed;
                    break;
                case CameraMovement.Left:
                    Position -= Right * MovementSpeed;
                    break;
                case CameraMovement.Right:
                    Position += Right * MovementSpeed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        
        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw   += xOffset;
            Pitch += yOffset;

            // make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            // update Front, Right and Up Vectors using the updated Euler angles
            UpdateCameraVectors();
        }
        
        public void ProcessMouseScroll(float yOffset)
        {
            Zoom -= yOffset;
            if (Zoom < 1.0f)
                Zoom = 1.0f;
            if (Zoom > 45.0f)
                Zoom = 45.0f; 
        }
        
        private void UpdateCameraVectors()
        {
            // calculate the new Front vector
            var front = Vector<float>.Build.Dense(3);
            
            front[0] = (float)Math.Cos(Yaw) * (float)Math.Cos(Pitch);
            front[1] = (float)Math.Sin(Pitch);
            front[2] = (float)Math.Sin(Yaw) * (float)Math.Cos(Pitch);
            Front = front.Normalize(1);
            // also re-calculate the Right and Up vector
            Right = Front.CrossProduct(WorldUp).Normalize(1);  // normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up    = Right.CrossProduct(Front).Normalize(1);
        }
        
    }
}