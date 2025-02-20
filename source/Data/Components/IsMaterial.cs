using Worlds;

namespace Materials.Components
{
    public readonly struct IsMaterial
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
    }
}