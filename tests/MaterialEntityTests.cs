using Shaders;
using Worlds;

namespace Materials.Tests
{
    public class MaterialEntityTests : MaterialTests
    {
        [Test]
        public void VerifyMaterial()
        {
            using World world = CreateWorld();
            Shader vertex = new(world, ShaderType.Vertex);
            Shader fragment = new(world, ShaderType.Fragment);
            Material material = new(world, vertex, fragment);

            Assert.That(material.IsCompliant, Is.True);
            Assert.That(material.VertexShader, Is.EqualTo(vertex));
            Assert.That(material.FragmentShader, Is.EqualTo(fragment));
        }
    }
}
