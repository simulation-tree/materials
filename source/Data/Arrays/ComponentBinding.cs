using Shaders;
using System;
using Worlds;

namespace Materials.Components
{
    public struct ComponentBinding : IEquatable<ComponentBinding>
    {
        public DescriptorResourceKey key;
        public uint entity;
        public DataType componentType;
        public ShaderType stage;

        public ComponentBinding(DescriptorResourceKey key, uint entity, DataType componentType, ShaderType stage)
        {
            this.key = key;
            this.entity = entity;
            this.componentType = componentType;
            this.stage = stage;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ComponentBinding property && Equals(property);
        }

        public readonly bool Equals(ComponentBinding other)
        {
            return componentType.Equals(other.componentType) && key == other.key && stage == other.stage;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(componentType, key, stage);
        }

        public static ComponentBinding Create<T>(DescriptorResourceKey key, uint entity, ShaderType stage, Schema schema) where T : unmanaged
        {
            return new ComponentBinding(key, entity, schema.GetComponentDataType<T>(), stage);
        }

        public static bool operator ==(ComponentBinding left, ComponentBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComponentBinding left, ComponentBinding right)
        {
            return !(left == right);
        }
    }
}