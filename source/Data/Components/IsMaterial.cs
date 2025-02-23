using System;
using Worlds;

namespace Materials.Components
{
    public readonly struct IsMaterial : IEquatable<IsMaterial>
    {
        public readonly uint version;
        public readonly rint vertexShaderReference;
        public readonly rint fragmentShaderReference;
        public readonly MaterialFlags flags;
        public readonly CompareOperation depthCompareOperation;

        public IsMaterial(uint version, rint vertexShaderReference, rint fragmentShaderReference, MaterialFlags flags, CompareOperation depthCompareOperation)
        {
            this.version = version;
            this.vertexShaderReference = vertexShaderReference;
            this.fragmentShaderReference = fragmentShaderReference;
            this.flags = flags;
            this.depthCompareOperation = depthCompareOperation;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsMaterial material && Equals(material);
        }

        public readonly bool Equals(IsMaterial other)
        {
            return version == other.version && vertexShaderReference.Equals(other.vertexShaderReference) && fragmentShaderReference.Equals(other.fragmentShaderReference) && flags == other.flags && depthCompareOperation == other.depthCompareOperation;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(version, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation);
        }

        public readonly IsMaterial IncrementVersion(rint vertexShaderReference, rint fragmentShaderReference)
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation);
        }

        public static bool operator ==(IsMaterial left, IsMaterial right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IsMaterial left, IsMaterial right)
        {
            return !(left == right);
        }
    }
}