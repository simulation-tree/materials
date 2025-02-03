using Worlds;

namespace Materials.Components
{
    [Component]
    public readonly struct IsMaterial
    {
        public readonly uint version;
        public readonly rint vertexShaderReference;
        public readonly rint fragmentShaderReference;

        public IsMaterial(uint version, rint vertexShaderReference, rint fragmentShaderReference)
        {
            this.version = version;
            this.vertexShaderReference = vertexShaderReference;
            this.fragmentShaderReference = fragmentShaderReference;
        }

        public readonly IsMaterial IncrementVersion()
        {
            return new IsMaterial(version + 1, vertexShaderReference, fragmentShaderReference);
        }
    }
}