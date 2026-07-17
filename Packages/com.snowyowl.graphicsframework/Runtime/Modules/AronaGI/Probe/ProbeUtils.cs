using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnowyOwl.GraphicsFramework
{
    public static class ProbeUtils
    {
        // public struct Tetrahedron
        // {
        //     public int[] indices;
        // }
        //
        // public class TetrahedronProbeGroup
        // {
        //     public Vector3[] probePositions;
        //     public Tetrahedron[] tetrahedrons;
        // }
        //
        // public static TetrahedronProbeGroup Tetrahedralize(Vector3[] probePositions)
        // {
        //     Lightmapping.Tetrahedralize(probePositions, out var indices, out var outProbePositions);
        //     
        //     var tetrahedrons = new Tetrahedron[indices.Length / 4];
        //     for (var i = 0; i < indices.Length / 4; i++)
        //     {
        //         var tetrahedron = new Tetrahedron();
        //         {
        //             tetrahedron.indices = new int[4]
        //             {
        //                 indices[i],
        //                 indices[i + 1],
        //                 indices[i + 2],
        //                 indices[i + 3]
        //             };
        //         }
        //         tetrahedrons[i] = tetrahedron;
        //     }
        //     
        //     var tetrahedronProbeGroup = new TetrahedronProbeGroup
        //     {
        //         probePositions = outProbePositions,
        //         tetrahedrons = tetrahedrons
        //     };
        //
        //     return tetrahedronProbeGroup;
        // }

        // public static void GetTetrahedronInterpolationWeights(TetrahedronProbeGroup probeGroup, Vector3 position, out int tetraIndex, out Vector4 weights)
        // {
        //     
        // }
    }
}
