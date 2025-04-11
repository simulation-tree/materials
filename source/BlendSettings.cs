using System;

namespace Materials
{
    public struct BlendSettings : IEquatable<BlendSettings>
    {
        public static readonly BlendSettings Opaque = new(false, BlendFactor.One, BlendFactor.Zero, BlendOperation.Add, BlendFactor.One, BlendFactor.Zero, BlendOperation.Add);
        public static readonly BlendSettings AlphaBlend = new(true, BlendFactor.One, BlendFactor.OneMinusSourceAlpha, BlendOperation.Add, BlendFactor.One, BlendFactor.OneMinusSourceAlpha, BlendOperation.Add);
        public static readonly BlendSettings Additive = new(true, BlendFactor.SourceAlpha, BlendFactor.One, BlendOperation.Add, BlendFactor.SourceAlpha, BlendFactor.One, BlendOperation.Add);
        public static readonly BlendSettings NonPremultiplied = new(true, BlendFactor.SourceAlpha, BlendFactor.OneMinusSourceAlpha, BlendOperation.Add, BlendFactor.SourceAlpha, BlendFactor.OneMinusSourceAlpha, BlendOperation.Add);

        public bool blendEnable;
        public BlendFactor sourceColorBlend;
        public BlendFactor destinationColorBlend;
        public BlendOperation colorBlendOperation;
        public BlendFactor sourceAlphaBlend;
        public BlendFactor destinationAlphaBlend;
        public BlendOperation alphaBlendOperation;

        public BlendSettings(bool blendEnable, BlendFactor sourceColorBlend, BlendFactor destinationColorBlend, BlendOperation colorBlendOperation, BlendFactor sourceAlphaBlend, BlendFactor destinationAlphaBlend, BlendOperation alphaBlendOperation)
        {
            this.blendEnable = blendEnable;
            this.sourceColorBlend = sourceColorBlend;
            this.destinationColorBlend = destinationColorBlend;
            this.colorBlendOperation = colorBlendOperation;
            this.sourceAlphaBlend = sourceAlphaBlend;
            this.destinationAlphaBlend = destinationAlphaBlend;
            this.alphaBlendOperation = alphaBlendOperation;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is BlendSettings settings && Equals(settings);
        }

        public readonly bool Equals(BlendSettings other)
        {
            return blendEnable == other.blendEnable &&
                   sourceColorBlend == other.sourceColorBlend &&
                   destinationColorBlend == other.destinationColorBlend &&
                   colorBlendOperation == other.colorBlendOperation &&
                   sourceAlphaBlend == other.sourceAlphaBlend &&
                   destinationAlphaBlend == other.destinationAlphaBlend &&
                   alphaBlendOperation == other.alphaBlendOperation;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(blendEnable, sourceColorBlend, destinationColorBlend, colorBlendOperation, sourceAlphaBlend, destinationAlphaBlend, alphaBlendOperation);
        }

        public static bool operator ==(BlendSettings left, BlendSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlendSettings left, BlendSettings right)
        {
            return !(left == right);
        }
    }
}