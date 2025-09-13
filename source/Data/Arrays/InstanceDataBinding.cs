using Shaders;
using System;
using Worlds;

namespace Materials.Components
{
    /// <summary>
    /// Describes how a component is laid out for the rendering entity.
    /// </summary>
    public struct PushConstantBinding : IEquatable<PushConstantBinding>
    {
        public uint start;
        public DataType componentType;
        public ShaderType stage;

        public PushConstantBinding(uint start, DataType componentType, ShaderType stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }

        public readonly override string ToString()
        {
            return $"Start: {start}\nComponent Type: {componentType}\nStage: {stage}";
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is PushConstantBinding binding && Equals(binding);
        }

        public readonly bool Equals(PushConstantBinding other)
        {
            return start == other.start && componentType.Equals(other.componentType) && stage == other.stage;
        }

        public readonly override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)start;
            hash = hash * 31 + componentType.index;
            hash = hash * 31 + componentType.size;
            hash = hash * 31 + (int)stage;
            return hash;
        }

        public static bool operator ==(PushConstantBinding left, PushConstantBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PushConstantBinding left, PushConstantBinding right)
        {
            return !(left == right);
        }
    }
}