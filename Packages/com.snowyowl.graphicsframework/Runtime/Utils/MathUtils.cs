using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnowyOwl.GraphicsFramework
{
    public static class MathUtils
    {
        public static bool Equal(float value1, float value2)
        {
            return Mathf.Approximately(value1, value2);
        }
        
        // Remap (min, max) to (0, 1)
        public static float Normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
        public static Vector2 Normalize(Vector2 value, float min, float max)
        {
            return new Vector2(Normalize(value.x, min, max), Normalize(value.y, min, max));
        }
        public static Vector3 Normalize(Vector3 value, float min, float max)
        {
            return new Vector3(Normalize(value.x, min, max), Normalize(value.y, min, max), Normalize(value.z, min, max));
        }
        public static Vector4 Normalize(Vector4 value, float min, float max)
        {
            return new Vector4(Normalize(value.x, min, max), Normalize(value.y, min, max), Normalize(value.z, min, max), Normalize(value.w, min, max));
        }
        
        // Remap (0, 1) to (min, max)
        public static float Denormalize(float value, float min, float max)
        {
            return value * (max - min) + min;
        }
        public static Vector2 Denormalize(Vector2 value, float min, float max)
        {
            return new Vector2(Normalize(value.x, min, max), Normalize(value.y, min, max));
        }
        public static Vector3 Denormalize(Vector3 value, float min, float max)
        {
            return new Vector3(Denormalize(value.x, min, max), Denormalize(value.y, min, max), Denormalize(value.z, min, max));
        }
        public static Vector4 Denormalize(Vector4 value, float min, float max)
        {
            return new Vector4(Denormalize(value.x, min, max), Denormalize(value.y, min, max), Denormalize(value.z, min, max), Denormalize(value.w, min, max));
        }
    }
}
