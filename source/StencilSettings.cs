using System;

namespace Materials
{
    public struct StencilSettings : IEquatable<StencilSettings>
    {
        public static readonly StencilSettings Default = new(StencilOperation.Keep, StencilOperation.Keep, StencilOperation.Keep, CompareOperation.Always);

        public StencilOperation failOperation;
        public StencilOperation passOperation;
        public StencilOperation depthFailOperation;
        public CompareOperation compareOperation;
        public uint compareMask;
        public uint writeMask;
        public uint referenceMask;

        public StencilSettings(StencilOperation failOperation, StencilOperation passOperation, StencilOperation depthFailOperation, CompareOperation compareOperation)
        {
            this.failOperation = failOperation;
            this.passOperation = passOperation;
            this.depthFailOperation = depthFailOperation;
            this.compareOperation = compareOperation;
            compareMask = uint.MaxValue;
            writeMask = uint.MaxValue;
            referenceMask = default;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is StencilSettings settings && Equals(settings);
        }

        public readonly bool Equals(StencilSettings other)
        {
            return failOperation == other.failOperation &&
                   passOperation == other.passOperation &&
                   depthFailOperation == other.depthFailOperation &&
                   compareOperation == other.compareOperation &&
                   compareMask == other.compareMask &&
                   writeMask == other.writeMask &&
                   referenceMask == other.referenceMask;
        }

        public readonly override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + failOperation.GetHashCode();
            hash = hash * 31 + passOperation.GetHashCode();
            hash = hash * 31 + depthFailOperation.GetHashCode();
            hash = hash * 31 + compareOperation.GetHashCode();
            hash = hash * 31 + compareMask.GetHashCode();
            hash = hash * 31 + writeMask.GetHashCode();
            hash = hash * 31 + referenceMask.GetHashCode();
            return hash;
        }

        public static bool operator ==(StencilSettings left, StencilSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StencilSettings left, StencilSettings right)
        {
            return !(left == right);
        }
    }
}