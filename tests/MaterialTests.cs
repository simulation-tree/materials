using Shaders;
using Types;
using Worlds;
using Worlds.Tests;

namespace Materials.Tests
{
    public abstract class MaterialTests : WorldTests
    {
        static MaterialTests()
        {
            MetadataRegistry.Load<MaterialsMetadataBank>();
            MetadataRegistry.Load<ShadersMetadataBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<MaterialsSchemaBank>();
            schema.Load<ShadersSchemaBank>();
            return schema;
        }
    }
}
