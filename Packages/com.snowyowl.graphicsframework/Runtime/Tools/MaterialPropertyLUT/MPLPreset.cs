using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public class MPLPresetData
    {
        [TableColumnWidth(100)]
        public string name = "";

        [HorizontalGroup("Item Datas"), TableColumnWidth(500)]
        [ListDrawerSettings(IsReadOnly = true, DefaultExpandedState = false, ShowPaging = false)]
        public List<MPLItemData> value = new();
    }

    public class MPLPreset : ScriptableObject
    {
        [ReadOnly]
        public MPLTemplete templete;
        
        [OnValueChanged("UpdatePresetData")]
        [TableList(ShowIndexLabels = true, ShowPaging = false)]
        public List<MPLPresetData> datas = new();
        
        public void Init(MPLTemplete inTemplete)
        {
            templete = inTemplete;
        }

        [OnInspectorInit]
        public void Reinitialize()
        {
            UpdatePresetData();
        }
        
        private void UpdatePresetData()
        {
            foreach (var data in datas)
            {
                var oldItemDatas = data.value;
                var newItemDatas = new List<MPLItemData>();
                foreach (var prototype in templete.itemPrototypes)
                {
                    var oldItemDataIndex = oldItemDatas.FindIndex(itemData => itemData.prototype == prototype);
                    if (oldItemDataIndex != -1)
                    {
                        newItemDatas.Add(oldItemDatas[oldItemDataIndex]);
                    }
                    else
                    {
                        var itemData = new MPLItemData
                                       {
                                           prototype = prototype,
                                           value = prototype.defaultValue
                                       };
                        newItemDatas.Add(itemData);
                    }
                }
                data.value = newItemDatas;
                if (data.name == string.Empty)
                {
                    data.name = "default";
                }
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}