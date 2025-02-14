using Shaders;
using Worlds;

namespace Materials.Components
{
    public struct PushBinding
    {
        public uint start;
        public DataType componentType;
        public ShaderType stage;

        public PushBinding(uint start, DataType componentType, ShaderType stage)
        {
            this.start = start;
            this.componentType = componentType;
            this.stage = stage;
        }
    }
}