using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Components.Interfaces;

namespace GameWork.Core.Components
{
    public class ComponentContainer
    {
        private readonly Dictionary<Type, IComponent> Components = new Dictionary<Type, IComponent>();

        public bool HasComponent<TComponent>() where TComponent : IComponent
        {
            var componentType = typeof(TComponent);

            return Components.ContainsKey(componentType)
                || Components.Keys.Any(k => k.IsAssignableFrom(componentType));
        }

        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            var didGetComponent = false;
            component = default(TComponent);
            IComponent baseTypeComponent;

            if (Components.TryGetValue(typeof(TComponent), out baseTypeComponent))
            {
                didGetComponent = true;
                component = (TComponent) baseTypeComponent;
            }
            else
            {
                Type assignableType;
                if (TryGetAssignableType<TComponent>(out assignableType))
                {
                    didGetComponent = true;
                    component = (TComponent)Components[assignableType];
                }
            }

            return didGetComponent;
        }

        public bool TryAddComponent(IComponent component)
        {
            var didAddComponent = false;
            var type = component.GetType();

            if (!Components.ContainsKey(type))
            {
                Components[type] = component;
                didAddComponent = true;
            }

            return didAddComponent;
        }

        public bool TryRemoveComponent(IComponent component)
        {
            return TryRemoveComponent(component.GetType());
        }

        public bool TryRemoveComponent<TComponent>() where TComponent : IComponent
        {
            return TryRemoveComponent(typeof(TComponent));
        }

        private bool TryRemoveComponent(Type findType)
        {
            var didRemove = false;

            if (Components.Remove(findType))
            {
                didRemove = true;
            }
            else
            {
                Type assignableType;
                if(TryGetAssignableType(findType, out assignableType))
                {
                    didRemove = Components.Remove(assignableType);
                }
            }

            return didRemove;
        }

        private bool TryGetAssignableType<TComponent>(out Type assignableType) where TComponent : IComponent
        {
            return TryGetAssignableType(typeof(TComponent), out assignableType);
        }
        private bool TryGetAssignableType(Type findType, out Type assignableType)
        {
            assignableType = Components.Keys.FirstOrDefault(k => k.IsAssignableFrom(findType));

            return assignableType != null;
        }
    }
}
