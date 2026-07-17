using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public abstract class WorldGraphicsComponent
    {
        public bool enable = true;
        public bool IsEnabled { get; set; }
        public bool IsStarted { get; set; }

        public virtual void OnEnable(SwyoWorldGraphicsSettings owner) { }
        public virtual void Start(SwyoWorldGraphicsSettings owner) { }
        public virtual void Update(SwyoWorldGraphicsSettings owner) { }
        public virtual void LateUpdate(SwyoWorldGraphicsSettings owner) { }
        public virtual void OnDisable(SwyoWorldGraphicsSettings owner) { }
    }
    
    [ExecuteAlways]
    public class SwyoWorldGraphicsSettings : MonoBehaviour
    {
        public static SwyoWorldGraphicsSettings Instance { get; private set; }
        
        [SerializeReference]
        [ListDrawerSettings(DefaultExpandedState = true, CustomRemoveIndexFunction = nameof(ComponentsRemoveIndexFunction))]
        [TypeSelectorSettings(ShowNoneItem = false, FilterTypesFunction = nameof(ComponentsFilterTypesFunction))]
        [PolymorphicDrawerSettings(ReadOnlyIfNotNullReference = true)]
        public List<WorldGraphicsComponent> components = new();
        
        public static void SetInstance(SwyoWorldGraphicsSettings instance) => Instance = instance;
        public static bool IsInstance(SwyoWorldGraphicsSettings instance) => Instance == instance;

        public bool TryGet<T>(out T component) where T : WorldGraphicsComponent
        {
            component = components.Find(component => component is T) as T;
            return component != null;
        }

        public bool TryAdd<T>() where T : WorldGraphicsComponent, new()
        {
            if (!components.Any(component => component is T))
            {
                components.Add(new T());
                return true;
            }
            return false;
        }

        public bool TryRemove<T>() where T : WorldGraphicsComponent
        {
            var component = components.Find(component => component is T);
            if (component != null)
            {
                TryExecuteOnDisable(this, component, true);
                components.Remove(component);
                return true;
            }
            return false;
        }

        public static bool InstanceTryGet<T>(out T component) where T : WorldGraphicsComponent
        {
            component = null;
            return Instance && Instance.TryGet(out component);
        }

        public static bool InstanceTryAdd<T>() where T : WorldGraphicsComponent, new()
        {
            return Instance && Instance.TryAdd<T>();
        }
        
        public static bool InstanceTryRemove<T>() where T : WorldGraphicsComponent
        {
            return Instance && Instance.TryRemove<T>();
        }

        private void OnEnable()
        {
            if (!Instance)
            {
                SetInstance(this);
            }
            else
            {
                Debug.LogWarning("The current scene contains multiple instances of SwyoSceneRenderingManager!");
            }

            if (IsInstance(this))
            {
                OnEnableComponents(Instance);
            }
        }
        
        private void Start()
        {
            if (IsInstance(this))
            {
                StartComponents(Instance);
            }
        }

        private void Update()
        {
            if (IsInstance(this))
            {
                UpdateComponents(Instance, false);
            }
        }

        private void LateUpdate()
        {
            if (IsInstance(this))
            {
                UpdateComponents(Instance, true);
            }
        }
        
        private void OnDisable()
        {
            if (IsInstance(this))
            {
                OnDisableComponents(Instance);
                SetInstance(null);
            }
        }
        
        private static void OnEnableComponents(SwyoWorldGraphicsSettings settings)
        {
            foreach (var component in settings.components.Where(component => component != null))
            {
                TryExecuteOnEnable(settings, component);
            }
        }
        
        private static void StartComponents(SwyoWorldGraphicsSettings settings)
        {
            foreach (var component in settings.components.Where(component => component != null))
            {
                TryExecuteOnStart(settings, component);
            }
        }
                
        private static void UpdateComponents(SwyoWorldGraphicsSettings settings, bool isInLateUpdate)
        {
            foreach (var component in settings.components.Where(component => component != null))
            {
                TryExecuteOnEnable(settings, component);
                TryExecuteOnStart(settings, component);
                TryExecuteUpdate(settings, component, isInLateUpdate);
                TryExecuteOnDisable(settings, component, false);
            }
        }

        private static void OnDisableComponents(SwyoWorldGraphicsSettings settings)
        {
            foreach (var component in settings.components.Where(component => component != null))
            {
                TryExecuteOnDisable(settings, component, true);
            }
        }

        private static void TryExecuteOnEnable(SwyoWorldGraphicsSettings settings, WorldGraphicsComponent component)
        {
            if (component.enable && !component.IsEnabled)
            {
                component.OnEnable(settings);
                component.IsEnabled = true;
            }
        }
        
        private static void TryExecuteOnStart(SwyoWorldGraphicsSettings settings, WorldGraphicsComponent component)
        {
            if (component.enable && component.IsEnabled && !component.IsStarted)
            {
                component.Start(settings);
                component.IsStarted = true;
            }
        }
        
        private static void TryExecuteUpdate(SwyoWorldGraphicsSettings settings, WorldGraphicsComponent component, bool isInLateUpdate)
        {
            if (component.enable)
            {
                if (isInLateUpdate)
                {
                    component.LateUpdate(settings);
                }
                else
                {
                    component.Update(settings);
                }
            }
        }
        
        private static void TryExecuteOnDisable(SwyoWorldGraphicsSettings settings, WorldGraphicsComponent component, bool isInOnDisable)
        {
            if (component.enable == isInOnDisable && component.IsEnabled)
            {
                component.OnDisable(settings);
                component.IsEnabled = false;
            }
        }
        
        
        // -------------------------------------
        // Odin 
        private bool ComponentsFilterTypesFunction(Type type)
        {
            return components.Where(component => component != null).All(component => component?.GetType() != type);
        }
        
        private void ComponentsRemoveIndexFunction(List<WorldGraphicsComponent> list, int index)
        {
            if (list[index] != null)
            {
                TryExecuteOnDisable(this, list[index], true);
            }
            list.RemoveAt(index);
        }
    }
}