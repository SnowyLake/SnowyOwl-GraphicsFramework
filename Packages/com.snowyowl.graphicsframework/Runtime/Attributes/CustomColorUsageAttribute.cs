using System;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    public class CustomColorUsageAttribute : Attribute
    {
        public bool showAlpha { get; private set; }
        public bool hdr { get; private set; }
        
        public CustomColorUsageAttribute(bool showAlpha = true)
        {
            this.showAlpha = showAlpha;
        }

        public CustomColorUsageAttribute(bool showAlpha = true, bool hdr = false)
        {
            this.showAlpha = showAlpha;
            this.hdr = hdr;
        }
    }
    
#if UNITY_EDITOR
    public class CustomColorUsageDrawer : OdinAttributeDrawer<CustomColorUsageAttribute, Color>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            label ??= GUIContent.none;
            
            var color = ValueEntry.SmartValue;
            
            EditorGUI.BeginChangeCheck();
            var newColor = EditorGUILayout.ColorField(label, color, true, Attribute.showAlpha, Attribute.hdr);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = newColor;
            }
        }
    }
#endif
}
