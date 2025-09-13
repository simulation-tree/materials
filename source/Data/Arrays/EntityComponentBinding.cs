using Shaders;
using System;
using Worlds;

namespace Materials.Components
{
    /// <summary>
    /// Describes where data is found for the shader property at the <see cref="DescriptorResourceKey"/>.
    /// </summary>
    public struct EntityComponentBinding : IEquatable<EntityComponentBinding>
    {
        public DescriptorResourceKey key;
        public uint entity;
        public DataType componentType;
        public ShaderType stage;

        public EntityComponentBinding(DescriptorResourceKey key, uint entity, DataType componentType, ShaderType stage)
        {
            this.key = key;
            this.entity = entity;
            this.componentType = componentType;
            this.stage = stage;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is EntityComponentBinding property && Equals(property);
        }

        public readonly bool Equals(EntityComponentBinding other)
        {
            return componentType.Equals(other.componentType) && key == other.key && stage == other.stage;
        }

        public readonly override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + componentType.GetHashCode();
            hash = hash * 31 + (int)entity;
            hash = hash * 31 + key.GetHashCode();
            hash = hash * 31 + stage.GetHashCode();
            return hash;
        }

        public static EntityComponentBinding Create<T>(DescriptorResourceKey key, uint entity, ShaderType stage, Schema schema) where T : unmanaged
        {
            return new EntityComponentBinding(key, entity, schema.GetComponentDataType<T>(), stage);
        }

        public static bool operator ==(EntityComponentBinding left, EntityComponentBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityComponentBinding left, EntityComponentBinding right)
        {
            return !(left == right);
        }
    }
}