using UnityEditor;
using SnowyOwl.GraphicsFramework;

namespace SnowyOwl.GraphicsFramework.Editor
{
    public static class MPLEditorUtils
    {
        public const string GeneratorDefaultName = "new_propertylut";
        public const string PresetDefaultName = "new_propertylut_preset";
        public const string CharacterStylizedTempletePath = "Packages/com.snowyowl.graphicsframework/Assets/MaterialPropertyLUT/MPLTemplete_CharacterStylized.asset";

        [MenuItem(CoreUtils.AssetMenuItemPrefix + "Material Property LUT/Generator/Character Stylized", priority = CoreUtils.EditorPriority.Default)]
        public static void CreateValueLUTGenerator_CharacterStylized()
        {
            var generator = MPLUtils.CreateGenerator(CharacterStylizedTempletePath);
            ProjectWindowUtil.CreateAsset(generator, $"{GeneratorDefaultName}.asset");
        }
        
        [MenuItem(CoreUtils.AssetMenuItemPrefix + "Material Property LUT/Preset/Character Stylized", priority = CoreUtils.EditorPriority.Default)]
        public static void CreateValueLUTPreset_CharacterStylized()
        {
            var preset = MPLUtils.CreatePreset(CharacterStylizedTempletePath);
            ProjectWindowUtil.CreateAsset(preset, $"{PresetDefaultName}.asset");
        }
    }
}
