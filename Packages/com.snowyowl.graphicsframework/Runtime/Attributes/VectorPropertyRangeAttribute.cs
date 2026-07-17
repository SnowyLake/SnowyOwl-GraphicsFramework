using System;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VectorPropertyRangeAttribute : Attribute
    {
        public string Min { get; }
        public string Max { get; }
        public int Decimals { get; }

        public VectorPropertyRangeAttribute(string min, string max, int decimals = 3)
        {
            Min = min;
            Max = max;
            Decimals = decimals;
        }
    }
    
#if UNITY_EDITOR
    public class Vector2PropertyRangeDrawer : OdinAttributeDrawer<VectorPropertyRangeAttribute, Vector2>
    {
        private ValueResolver<float> minResolver;
        private ValueResolver<float> maxResolver;

        protected override void Initialize()
        {
            minResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Min);
            maxResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Max);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = this.ValueEntry.SmartValue;
            float min = minResolver.GetValue();
            float max = maxResolver.GetValue();

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, label);

            float spacing = 2f;
            float width = (rect.width - spacing) / 2f;

            Rect rX = new Rect(rect.x, rect.y, width, rect.height);
            Rect rY = new Rect(rect.x + width + spacing, rect.y, width, rect.height);

            value.x = DrawLabeledSlider(rX, "X", value.x, min, max);
            value.y = DrawLabeledSlider(rY, "Y", value.y, min, max);

            this.ValueEntry.SmartValue = value;
        }

        private float DrawLabeledSlider(Rect rect, string label, float val, float min, float max)
        {
            float labelWidth = 12f;
            float fieldWidth = 45f;
            float sliderWidth = rect.width - labelWidth - fieldWidth - 2;

            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect sliderRect = new Rect(rect.x + labelWidth, rect.y, sliderWidth, rect.height);
            Rect fieldRect = new Rect(sliderRect.xMax + 2, rect.y, fieldWidth, rect.height);

            EditorGUI.LabelField(labelRect, label);
            val = GUI.HorizontalSlider(sliderRect, val, min, max);

            float rounded = (float)System.Math.Round(val, this.Attribute.Decimals);
            val = EditorGUI.FloatField(fieldRect, rounded);

            return Mathf.Clamp(val, min, max);
        }
    }
    public class Vector3PropertyRangeDrawer : OdinAttributeDrawer<VectorPropertyRangeAttribute, Vector3>
    {
        private ValueResolver<float> minResolver;
        private ValueResolver<float> maxResolver;

        protected override void Initialize()
        {
            minResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Min);
            maxResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Max);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = this.ValueEntry.SmartValue;
            float min = minResolver.GetValue();
            float max = maxResolver.GetValue();

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, label);

            float spacing = 2f;
            float width = (rect.width - spacing * 2) / 3f;

            Rect rX = new Rect(rect.x, rect.y, width, rect.height);
            Rect rY = new Rect(rect.x + width + spacing, rect.y, width, rect.height);
            Rect rZ = new Rect(rect.x + (width + spacing) * 2, rect.y, width, rect.height);

            value.x = DrawLabeledSlider(rX, "X", value.x, min, max);
            value.y = DrawLabeledSlider(rY, "Y", value.y, min, max);
            value.z = DrawLabeledSlider(rZ, "Z", value.z, min, max);

            this.ValueEntry.SmartValue = value;
        }

        private float DrawLabeledSlider(Rect rect, string label, float val, float min, float max)
        {
            float labelWidth = 12f;
            float fieldWidth = 45f; // 输入框宽度
            float sliderWidth = rect.width - labelWidth - fieldWidth - 2;

            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect sliderRect = new Rect(rect.x + labelWidth, rect.y, sliderWidth, rect.height);
            Rect fieldRect = new Rect(sliderRect.xMax + 2, rect.y, fieldWidth, rect.height);

            EditorGUI.LabelField(labelRect, label);
            val = GUI.HorizontalSlider(sliderRect, val, min, max);

            // ✅ 保留 N 位小数
            int decimals = this.Attribute.Decimals;
            float rounded = (float)System.Math.Round(val, decimals);
            val = EditorGUI.FloatField(fieldRect, rounded);

            return Mathf.Clamp(val, min, max);
        }
    }
    public class Vector4PropertyRangeDrawer : OdinAttributeDrawer<VectorPropertyRangeAttribute, Vector4>
    {
        private ValueResolver<float> minResolver;
        private ValueResolver<float> maxResolver;

        protected override void Initialize()
        {
            minResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Min);
            maxResolver = ValueResolver.Get<float>(this.Property, this.Attribute.Max);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = this.ValueEntry.SmartValue;
            float min = minResolver.GetValue();
            float max = maxResolver.GetValue();

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, label);

            float spacing = 2f;
            float width = (rect.width - spacing * 3) / 4f;

            Rect rX = new Rect(rect.x, rect.y, width, rect.height);
            Rect rY = new Rect(rect.x + width + spacing, rect.y, width, rect.height);
            Rect rZ = new Rect(rect.x + (width + spacing) * 2, rect.y, width, rect.height);
            Rect rW = new Rect(rect.x + (width + spacing) * 3, rect.y, width, rect.height);

            value.x = DrawLabeledSlider(rX, "X", value.x, min, max);
            value.y = DrawLabeledSlider(rY, "Y", value.y, min, max);
            value.z = DrawLabeledSlider(rZ, "Z", value.z, min, max);
            value.w = DrawLabeledSlider(rW, "W", value.w, min, max);

            this.ValueEntry.SmartValue = value;
        }

        private float DrawLabeledSlider(Rect rect, string label, float val, float min, float max)
        {
            float labelWidth = 12f;
            float fieldWidth = 45f;
            float sliderWidth = rect.width - labelWidth - fieldWidth - 2;

            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect sliderRect = new Rect(rect.x + labelWidth, rect.y, sliderWidth, rect.height);
            Rect fieldRect = new Rect(sliderRect.xMax + 2, rect.y, fieldWidth, rect.height);

            EditorGUI.LabelField(labelRect, label);
            val = GUI.HorizontalSlider(sliderRect, val, min, max);

            float rounded = (float)System.Math.Round(val, this.Attribute.Decimals);
            val = EditorGUI.FloatField(fieldRect, rounded);

            return Mathf.Clamp(val, min, max);
        }
    }
#endif
}
