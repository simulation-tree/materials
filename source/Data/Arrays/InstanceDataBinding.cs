using Shaders;
using Worlds;

namespace Materials.Components
{
    /// <summary>
    /// Describes how a component is laid out for the rendering entity.
    /// </summary>
    public struct InstanceDataBinding
    {
        public uint start;
        public DataType componentType;
        public ShaderType stage;

        public InstanceDataBinding(uint start, DataType componentType, ShaderType stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }
    }
}