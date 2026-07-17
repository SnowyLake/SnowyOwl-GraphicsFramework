using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SnowyOwl.GraphicsFramework
{
    public class MPLItemPrototype : ScriptableObject
    {
        [VerticalGroup("EditorOnly"), TableColumnWidth(100), ShowInInspector, PropertyOrder(-1)]
        public ScriptableObject self => this;
        [VerticalGroup("EditorOnly"), TableColumnWidth(100)]
        public string displayName = "";
        
        [VerticalGroup("Base"), TableColumnWidth(100)]
        public MPLItemType itemType;
        [VerticalGroup("Base"), TableColumnWidth(100)]
        public string itemName = "";
        [VerticalGroup("Base"), TableColumnWidth(100)]
        public string conditional = "";
        
        [HideInInspector]
        public Vector4 defaultValue;
        public MPLItemValueType itemValueType => MPLUtils.GetItemValueType(itemType);
        
        [VerticalGroup("Value"), TableColumnWidth(100), LabelText("Default Value"), ShowIf("@itemValueType == MPLItemValueType.Scalar")]
        [ShowInInspector, EnableGUI, PropertyOrder(0)]
        public float defaultScalarValue
        {
            get => defaultValue.x;
            set => defaultValue.x = value;
        }
        [VerticalGroup("Value"), TableColumnWidth(100), LabelText("Default Value"), ShowIf("@itemValueType == MPLItemValueType.Vector3")]
        [ShowInInspector, EnableGUI, PropertyOrder(0)]
        public Vector3 defaultVector3Value
        {
            get => new(defaultValue.x, defaultValue.y, defaultValue.z);
            set
            {
                defaultValue.x = value.x;
                defaultValue.y = value.y;
                defaultValue.z = value.z;
            }
        }
        [VerticalGroup("Value"), TableColumnWidth(100), LabelText("Default Value"), ShowIf("@itemValueType == MPLItemValueType.Vector4")]
        [ShowInInspector, EnableGUI, PropertyOrder(0)]
        public Vector4 defaultVector4Value
        {
            get => defaultValue;
            set => defaultValue = value;
        }
        
        [VerticalGroup("Value"), TableColumnWidth(100), ShowIf("@MPLUtils.RequireRange(itemType)"), PropertyOrder(1)]
        public float valueMin = 0.0f;
        [VerticalGroup("Value"), TableColumnWidth(100), ShowIf("@MPLUtils.RequireRange(itemType)"), PropertyOrder(1)]
        [MinValue("valueMin")]
        public float valueMax = 1.0f;
    }
}