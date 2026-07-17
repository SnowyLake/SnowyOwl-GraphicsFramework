using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    public static class GeometryUtils
    {
        public static readonly Bounds EmptyBounds = new()
        {
            center = Vector3.zero,
            extents = Vector3.zero,
        };
        
        public static Vector3 RotateBoundsExtents(float4x4 transform, Vector3 extents)
        {
            return math.abs(transform.c0.xyz * extents.x) + math.abs(transform.c1.xyz * extents.y) + math.abs(transform.c2.xyz * extents.z);
        }
        
        public static Bounds TransformBounds(Matrix4x4 transform, Bounds bounds)
        {
            var transformed = new Bounds
            {
                extents = RotateBoundsExtents(transform, bounds.extents),
                center = math.transform(transform, bounds.center)
            };
            return transformed;
        }
        
        
#if UNITY_EDITOR
        public static void DrawBounds(Vector3 center, Vector3 extents)
        {
            Vector3[] corners =
            {
                center + new Vector3(-extents.x, -extents.y, -extents.z),
                center + new Vector3(extents.x, -extents.y, -extents.z),
                center + new Vector3(extents.x, -extents.y, extents.z),
                center + new Vector3(-extents.x, -extents.y, extents.z),

                center + new Vector3(-extents.x, extents.y, -extents.z),
                center + new Vector3(extents.x, extents.y, -extents.z),
                center + new Vector3(extents.x, extents.y, extents.z),
                center + new Vector3(-extents.x, extents.y, extents.z),
            };

            // bottom square
            Handles.DrawLine(corners[0], corners[1]);
            Handles.DrawLine(corners[1], corners[2]);
            Handles.DrawLine(corners[2], corners[3]);
            Handles.DrawLine(corners[3], corners[0]);

            // top square
            Handles.DrawLine(corners[4], corners[5]);
            Handles.DrawLine(corners[5], corners[6]);
            Handles.DrawLine(corners[6], corners[7]);
            Handles.DrawLine(corners[7], corners[4]);

            // verticals
            Handles.DrawLine(corners[0], corners[4]);
            Handles.DrawLine(corners[1], corners[5]);
            Handles.DrawLine(corners[2], corners[6]);
            Handles.DrawLine(corners[3], corners[7]);
        }
#endif
    }
}
