using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SnowyOwl.GraphicsFramework
{
    internal static class SwyoGlobalGraphicsSettingsProvider
    {
        private static UnityEditor.Editor s_SettingsEditor;

        /// <summary>
        /// Create the SnowyOwl Graphics Project Settings provider.
        /// </summary>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/SnowyOwl Graphics", SettingsScope.Project, new[] { "SnowyOwl", "Graphics" })
            {
                guiHandler = _ => DrawSettings(),
                deactivateHandler = DisposeEditor,
            };
        }

        /// <summary>
        /// Draw the settings asset selector and inspector.
        /// </summary>
        private static void DrawSettings()
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;
            try
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        var settings = GetPreloadedSettings();

                        EditorGUI.BeginChangeCheck();
                        settings = (SwyoGlobalGraphicsSettings)EditorGUILayout.ObjectField("Settings", settings, typeof(SwyoGlobalGraphicsSettings), false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            SetPreloadedSettings(settings);
                        }

                        if (!settings)
                        {
                            EditorGUILayout.HelpBox("Select or create a settings asset to preload it in Player builds.", MessageType.Info);
                            if (GUILayout.Button("Create Settings Asset"))
                            {
                                CreateSettingsAsset();
                            }

                            return;
                        }

                        EditorGUILayout.Space();
                        UnityEditor.Editor.CreateCachedEditor(settings, null, ref s_SettingsEditor);
                        s_SettingsEditor.OnInspectorGUI();
                    }
                }
            }
            finally
            {
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        /// <summary>
        /// Get the first SnowyOwl settings asset registered for preloading.
        /// </summary>
        private static SwyoGlobalGraphicsSettings GetPreloadedSettings()
        {
            return PlayerSettings.GetPreloadedAssets().OfType<SwyoGlobalGraphicsSettings>().FirstOrDefault();
        }

        /// <summary>
        /// Replace all preloaded SnowyOwl settings assets while preserving other preloaded assets.
        /// </summary>
        private static void SetPreloadedSettings(SwyoGlobalGraphicsSettings settings)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().Where(asset => !(asset is SwyoGlobalGraphicsSettings)).ToList();
            if (settings)
            {
                preloadedAssets.Add(settings);
            }

            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Create and preload a SnowyOwl settings asset selected by the user.
        /// </summary>
        private static void CreateSettingsAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create SnowyOwl Graphics Settings", "SwyoGlobalGraphicsSettings", "asset", "Choose a location for the global graphics settings asset.");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var settings = ScriptableObject.CreateInstance<SwyoGlobalGraphicsSettings>();
            AssetDatabase.CreateAsset(settings, path);
            SetPreloadedSettings(settings);
            Selection.activeObject = settings;
        }

        /// <summary>
        /// Dispose the cached settings inspector.
        /// </summary>
        private static void DisposeEditor()
        {
            if (s_SettingsEditor)
            {
                Object.DestroyImmediate(s_SettingsEditor);
                s_SettingsEditor = null;
            }
        }
    }
}
