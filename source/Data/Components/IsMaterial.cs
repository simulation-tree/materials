using System;
using Worlds;

namespace Materials.Components
{
    public struct IsMaterial : IEquatable<IsMaterial>
    {
        public uint version;
        public sbyte renderOrder;
        public rint vertexShaderReference;
        public rint fragmentShaderReference;
        public BlendSettings blendSettings;
        public DepthSettings depthSettings;

        public IsMaterial(uint version, sbyte renderOrder, rint vertexShaderReference, rint fragmentShaderReference, BlendSettings blendSettings, DepthSettings depthSettings)
        {
            this.version = version;
            this.renderOrder = renderOrder;
            this.vertexShaderReference = vertexShaderReference;
            this.fragmentShaderReference = fragmentShaderReference;
            this.blendSettings = blendSettings;
            this.depthSettings = depthSettings;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsMaterial material && Equals(material);
        }

        public readonly bool Equals(IsMaterial other)
        {
            return version == other.version && renderOrder == other.renderOrder && vertexShaderReference.Equals(other.vertexShaderReference) && fragmentShaderReference.Equals(other.fragmentShaderReference) && blendSettings == other.blendSettings && depthSettings == other.depthSettings;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(version, renderOrder, vertexShaderReference, fragmentShaderReference, blendSettings, depthSettings);
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