using System;
using Worlds;

namespace Materials.Components
{
    public struct IsMaterial : IEquatable<IsMaterial>
    {
        public ushort version;
        public sbyte renderGroup;
        public rint vertexShaderReference;
        public rint fragmentShaderReference;
        public BlendSettings blendSettings;
        public DepthSettings depthSettings;
        public MaterialFlags flags;

        public IsMaterial(ushort version, sbyte renderGroup, rint vertexShaderReference, rint fragmentShaderReference, BlendSettings blendSettings, DepthSettings depthSettings, MaterialFlags flags)
        {
            this.version = version;
            this.renderGroup = renderGroup;
            this.vertexShaderReference = vertexShaderReference;
            this.fragmentShaderReference = fragmentShaderReference;
            this.blendSettings = blendSettings;
            this.depthSettings = depthSettings;
            this.flags = flags;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsMaterial material && Equals(material);
        }

        public readonly bool Equals(IsMaterial other)
        {
            return version == other.version && renderGroup == other.renderGroup && vertexShaderReference.Equals(other.vertexShaderReference) && fragmentShaderReference.Equals(other.fragmentShaderReference) && blendSettings == other.blendSettings && depthSettings == other.depthSettings && flags == other.flags;
        }

        public readonly override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + version;
            hash = hash * 31 + renderGroup;
            hash = hash * 31 + vertexShaderReference.GetHashCode();
            hash = hash * 31 + fragmentShaderReference.GetHashCode();
            hash = hash * 31 + blendSettings.GetHashCode();
            hash = hash * 31 + depthSettings.GetHashCode();
            hash = hash * 31 + flags.GetHashCode();
            return hash;
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