using Worlds;

namespace Materials.Arrays
{
    public struct InstanceAttributeBinding
    {
        public DescriptorResourceKey key;
        public DataType dataType;

        public InstanceAttributeBinding(DescriptorResourceKey key, DataType dataType)
        {
            this.key = key;
            this.dataType = dataType;
        }
    }
}