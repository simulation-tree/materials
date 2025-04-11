using System;

namespace Materials
{
    public struct DepthSettings : IEquatable<DepthSettings>
    {
        public static readonly DepthSettings None = new(Flags.None, CompareOperation.LessOrEqual);
        public static readonly DepthSettings Default = new(Flags.DepthTest | Flags.DepthWrite, CompareOperation.LessOrEqual);
        public static readonly DepthSettings Read = new(Flags.DepthTest, CompareOperation.LessOrEqual);
        public static readonly DepthSettings ReverseZ = new(Flags.DepthTest | Flags.DepthWrite, CompareOperation.GreaterOrEqual);
        public static readonly DepthSettings ReadReverseZ = new(Flags.DepthTest, CompareOperation.GreaterOrEqual);

        public Flags flags;
        public CompareOperation compareOperation;
        public float minDepth;
        public float maxDepth;
        public StencilSettings front;
        public StencilSettings back;

        public bool DepthTest
        {
            readonly get => (flags & Flags.DepthTest) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.DepthTest;
                }
                else
                {
                    flags &= ~Flags.DepthTest;
                }
            }
        }

        public bool DepthWrite
        {
            readonly get => (flags & Flags.DepthWrite) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.DepthWrite;
                }
                else
                {
                    flags &= ~Flags.DepthWrite;
                }
            }
        }

        public bool DepthBoundsTest
        {
            readonly get => (flags & Flags.DepthBoundsTest) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.DepthBoundsTest;
                }
                else
                {
                    flags &= ~Flags.DepthBoundsTest;
                }
            }
        }

        public bool StencilTest
        {
            readonly get => (flags & Flags.StencilTest) != 0;
            set
            {
                if (value)
                {
                    flags |= Flags.StencilTest;
                }
                else
                {
                    flags &= ~Flags.StencilTest;
                }
            }
        }

        public DepthSettings(Flags flags, CompareOperation compareOperation, float minDepth, float maxDepth, StencilSettings front, StencilSettings back)
        {
            this.flags = flags;
            this.compareOperation = compareOperation;
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
            this.front = front;
            this.back = back;
        }

        public DepthSettings(Flags flags, CompareOperation compareOperation)
        {
            this.flags = flags;
            this.compareOperation = compareOperation;
            minDepth = 0f;
            maxDepth = 0f;
            front = StencilSettings.Default;
            back = StencilSettings.Default;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DepthSettings settings && Equals(settings);
        }

        public readonly bool Equals(DepthSettings other)
        {
            return flags == other.flags &&
                   compareOperation == other.compareOperation &&
                   minDepth == other.minDepth &&
                   maxDepth == other.maxDepth &&
                   front == other.front &&
                   back == other.back;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(flags, compareOperation, minDepth, maxDepth, front, back);
        }

        public static bool operator ==(DepthSettings left, DepthSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DepthSettings left, DepthSettings right)
        {
            return !(left == right);
        }

        [Flags]
        public enum Flags : byte
        {
            None = 0,
            DepthTest = 1,
            DepthWrite = 2,
            DepthBoundsTest = 4,
            StencilTest = 8
        }
    }
}