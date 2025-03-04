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
        public readonly int instanceBindingsHash;

        public IsMaterial(uint version, rint vertexShaderReference, rint fragmentShaderReference, MaterialFlags flags, CompareOperation depthCompareOperation, int instanceDataHash)
        {
            this.version = version;
            this.vertexShaderReference = vertexShaderReference;
            this.fragmentShaderReference = fragmentShaderReference;
            this.flags = flags;
            this.depthCompareOperation = depthCompareOperation;
            this.instanceBindingsHash = instanceDataHash;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsMaterial material && Equals(material);
        }

        public readonly bool Equals(IsMaterial other)
        {
            return version == other.version && vertexShaderReference.Equals(other.vertexShaderReference) && fragmentShaderReference.Equals(other.fragmentShaderReference) && flags == other.flags && depthCompareOperation == other.depthCompareOperation && instanceBindingsHash == other.instanceBindingsHash;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(version, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation, instanceBindingsHash);
        }

        public readonly IsMaterial WithFlags(MaterialFlags flags)
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation, instanceBindingsHash);
        }

        public readonly IsMaterial WithDepthCompareOperation(CompareOperation depthCompareOperation)
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation, instanceBindingsHash);
        }

        public readonly IsMaterial WithShaderReferences(rint vertexShaderReference, rint fragmentShaderReference)
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation, instanceBindingsHash);
        }

        public readonly IsMaterial WithInstanceBindingsHash(int instanceDataHash)
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference, flags, depthCompareOperation, instanceDataHash);
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