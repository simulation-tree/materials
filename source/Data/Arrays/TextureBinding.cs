using System;
using System.Numerics;
using Worlds;

namespace Materials.Components
{
    [ArrayElement]
    public struct TextureBinding : IEquatable<TextureBinding>
    {
        public DescriptorResourceKey key;

        private uint version;
        private Vector4 region;
        private uint entity;
        private TextureFiltering filtering;

        public readonly uint Version => version;
        public readonly Vector4 Region => region;
        public readonly uint Entity => entity;
        public readonly TextureFiltering Filter => filtering;

        public TextureBinding(uint version, DescriptorResourceKey key, uint texture, Vector4 region, TextureFiltering filtering)
        {
            this.version = version;
            this.key = key;
            entity = texture;
            this.region = region;
            this.filtering = filtering;
        }

        public TextureBinding(uint version, DescriptorResourceKey key, Entity texture, Vector4 region, TextureFiltering filtering)
        {
            this.version = version;
            this.key = key;
            entity = texture.value;
            this.region = region;
            this.filtering = filtering;
        }

        public void SetTexture(Entity texture)
        {
            uint entity = texture.value;
            if (this.entity != entity)
            {
                this.entity = entity;
                version++;
            }
        }

        public void SetTexture(uint texture)
        {
            if (entity != texture)
            {
                entity = texture;
                version++;
            }
        }

        public void SetRegion(Vector4 region)
        {
            if (this.region != region)
            {
                this.region = region;
                version++;
            }
        }

        public void SetRegion(float x, float y, float width, float height)
        {
            SetRegion(new Vector4(x, y, width, height));
        }

        public void SetFiltering(TextureFiltering filtering)
        {
            if (this.filtering != filtering)
            {
                this.filtering = filtering;
                version++;
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TextureBinding binding && Equals(binding);
        }

        public readonly bool Equals(TextureBinding other)
        {
            return key.Equals(other.key) && entity.Equals(other.entity) && region.Equals(other.region);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(key, entity, region);
        }

        public static bool operator ==(TextureBinding left, TextureBinding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextureBinding left, TextureBinding right)
        {
            return !(left == right);
        }
    }
}