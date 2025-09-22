using System;

namespace Materials
{
    public readonly struct DescriptorResourceKey : IEquatable<DescriptorResourceKey>
    {
        /// <summary>
        /// Maximum allowed value for both the binding and set values.
        /// </summary>
        public const byte MaxSetOrBindingValue = 15;

        private readonly byte value;

        public readonly byte Set => (byte)(value >> 4);
        public readonly byte Binding => (byte)(value & 0x0F);

        public DescriptorResourceKey(byte binding, byte set)
        {
            if (set >= MaxSetOrBindingValue)
            {
                throw new ArgumentOutOfRangeException(nameof(set), $"Given set {set} is greater than the maximum allowed {MaxSetOrBindingValue}");
            }
            else if (binding >= MaxSetOrBindingValue)
            {
                throw new ArgumentOutOfRangeException(nameof(binding), $"Given binding {binding} is greater than the maximum allowed {MaxSetOrBindingValue}");
            }

            value = (byte)(set << 4 | binding);
        }

        public readonly override string ToString()
        {
            Span<char> buffer = stackalloc char[32];
            int length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly int ToString(Span<char> buffer)
        {
            int length = 0;
            length += Binding.ToString(buffer);
            buffer[length++] = ':';
            length += Set.ToString(buffer.Slice(length));
            return length;
        }

        public readonly (byte binding, byte set) Deconstruct()
        {
            return ((byte)(value & 0x0F), (byte)(value >> 4));
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DescriptorResourceKey key && Equals(key);
        }

        public readonly bool Equals(DescriptorResourceKey other)
        {
            return value == other.value;
        }

        public readonly bool Equals(byte binding, byte set)
        {
            return value == (byte)(set << 4 | binding);
        }

        public readonly override int GetHashCode()
        {
            return value;
        }

        /// <summary>
        /// Attempts to parse and retrieve the given text as a <see cref="DescriptorResourceKey"/>.
        /// </summary>
        public static bool TryParse(ReadOnlySpan<char> text, out DescriptorResourceKey key)
        {
            if (text.Length == 0)
            {
                key = default;
                return false;
            }
            else
            {
                if (text.TryIndexOf(':', out int colonIndex))
                {
                    ReadOnlySpan<char> bindingText = text.Slice(0, colonIndex);
                    ReadOnlySpan<char> setText = text.Slice(colonIndex + 1);
                    if (byte.TryParse(bindingText, out byte binding) && byte.TryParse(setText, out byte set))
                    {
                        key = new(binding, set);
                        return true;
                    }
                    else
                    {
                        key = default;
                        return false;
                    }
                }
                else
                {
                    key = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Parses the given text into a <see cref="DescriptorResourceKey"/>.
        /// <para>
        /// Text must be in the format `binding:set`.
        /// </para>
        /// <para>
        /// May throw an <see cref="Exception"/> if the text is not in the correct format.
        /// </para>
        public static DescriptorResourceKey Parse(ReadOnlySpan<char> text)
        {
            int colonIndex = text.IndexOf(':');
            ReadOnlySpan<char> bindingText = text.Slice(0, colonIndex);
            ReadOnlySpan<char> setText = text.Slice(colonIndex + 1);
            byte binding = byte.Parse(bindingText);
            byte set = byte.Parse(setText);
            return new(binding, set);
        }

        public static bool operator ==(DescriptorResourceKey left, DescriptorResourceKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DescriptorResourceKey left, DescriptorResourceKey right)
        {
            return !(left == right);
        }
    }
}