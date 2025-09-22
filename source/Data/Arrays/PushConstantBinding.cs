using Shaders;
using System;

namespace Materials.Arrays
{
    /// <summary>
    /// Describes how a component is laid out for the rendering entity.
    /// </summary>
    public struct PushConstantBinding : IEquatable<PushConstantBinding>
    {
        public int start;
        public int componentType;
        public int componentSize;
        public ShaderType stage;

        public PushConstantBinding(int start, int componentType, int componentSize, ShaderType stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.componentSize = componentSize;
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
            hash = hash * 31 + start;
            hash = hash * 31 + componentType;
            hash = hash * 31 + componentSize;
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