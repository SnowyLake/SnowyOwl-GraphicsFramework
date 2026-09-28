using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace SnowyOwl.GraphicsFramework.Editor
{
    [InitializeOnLoad]
    public sealed class SnowyOwlScriptingDefineInstaller : IActiveBuildTargetChanged
    {
        private const string k_Define = "SNOWYOWL_INCLUDE";

        public int callbackOrder => 0;

        /// <summary>
        /// Schedule installation after Editor startup.
        /// </summary>
        static SnowyOwlScriptingDefineInstaller()
        {
            if (Application.isBatchMode)
            {
                EnsureForActiveBuildTarget();
            }
            else
            {
                EditorApplication.delayCall += EnsureForActiveBuildTarget;
            }
        }

        /// <summary>
        /// Ensure the new build target includes the SnowyOwl symbol.
        /// </summary>
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            EnsureForActiveBuildTarget();
        }

        /// <summary>
        /// Add the SnowyOwl symbol to the active build target if it is missing.
        /// </summary>
        private static void EnsureForActiveBuildTarget()
        {
            if (!TryGetActiveBuildTarget(out var target))
            {
                return;
            }

            PlayerSettings.GetScriptingDefineSymbols(target, out string[] defines);
            var symbols = new List<string>(defines);
            if (symbols.Contains(k_Define))
            {
                return;
            }

            symbols.Add(k_Define);
            PlayerSettings.SetScriptingDefineSymbols(target, symbols.ToArray());
        }

        /// <summary>
        /// Resolve the active platform's scripting define group.
        /// </summary>
        private static bool TryGetActiveBuildTarget(out NamedBuildTarget target)
        {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            target = group == BuildTargetGroup.Standalone && EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server
                ? NamedBuildTarget.Server
                : NamedBuildTarget.FromBuildTargetGroup(group);
            return group != BuildTargetGroup.Unknown;
        }
    }
}
