using Materials.Components;
using Shaders;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Materials
{
    public readonly partial struct Material : IEntity
    {
        public readonly bool IsLoaded
        {
            get
            {
                if (TryGetComponent(out IsMaterialRequest request))
                {
                    return request.status == IsMaterialRequest.Status.Loaded;
                }

                return IsCompliant;
            }
        }

        public readonly Shader VertexShader
        {
            get
            {
                ThrowIfNotLoaded();

                rint reference = GetComponent<IsMaterial>().vertexShaderReference;
                return new Entity(world, GetReference(reference)).As<Shader>();
            }
        }

        public readonly Shader FragmentShader
        {
            get
            {
                ThrowIfNotLoaded();

                rint reference = GetComponent<IsMaterial>().fragmentShaderReference;
                return new Entity(world, GetReference(reference)).As<Shader>();
            }
        }

        public readonly USpan<ComponentBinding> ComponentBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<ComponentBinding>();
            }
        }

        public readonly USpan<TextureBinding> TextureBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<TextureBinding>();
            }
        }

        public readonly USpan<PushBinding> PushBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<PushBinding>();
            }
        }

        /// <summary>
        /// Creates a request to load a material entity from the given <paramref name="address"/>.
        /// </summary>
        public Material(World world, FixedString address, TimeSpan timeout = default)
        {
            this.world = world;
            value = world.CreateEntity(new IsMaterialRequest(address, timeout));
            CreateArray<PushBinding>();
            CreateArray<ComponentBinding>();
            CreateArray<TextureBinding>();
        }

        /// <summary>
        /// Creates a new material initialized with the given shaders.
        /// </summary>
        public Material(World world, Shader vertexShader, Shader fragmentShader)
        {
            this.world = world;
            value = world.CreateEntity(new IsMaterial(0, (rint)1, (rint)2));
            AddReference(vertexShader);
            AddReference(fragmentShader);
            CreateArray<PushBinding>();
            CreateArray<ComponentBinding>();
            CreateArray<TextureBinding>();
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMaterial>();
            archetype.AddArrayType<PushBinding>();
            archetype.AddArrayType<ComponentBinding>();
            archetype.AddArrayType<TextureBinding>();
        }

        public readonly bool ContainsPushBinding(DataType componentType)
        {
            USpan<PushBinding> array = GetArray<PushBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                PushBinding binding = array[i];
                if (binding.componentType == componentType)
                {
                    return true;
                }
            }

            return false;
        }

        public readonly bool ContainsComponentBinding(DescriptorResourceKey key, ShaderType stage)
        {
            USpan<ComponentBinding> array = GetArray<ComponentBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                ComponentBinding binding = array[i];
                if (binding.key == key && binding.stage == stage)
                {
                    return true;
                }
            }

            return false;
        }

        public readonly bool ContainsTextureBinding(DescriptorResourceKey key)
        {
            return TryIndexOfTextureBinding(key, out _);
        }

        public readonly ref PushBinding AddPushBinding(DataType componentType, ShaderType stage)
        {
            ThrowIfPushBindingIsAlreadyPresent(componentType);

            USpan<PushBinding> array = GetArray<PushBinding>();
            uint start = 0;
            foreach (PushBinding existingBinding in array)
            {
                start += existingBinding.componentType.size;
            }

            uint length = array.Length;
            array = ResizeArray<PushBinding>(length + 1);
            array[length] = new(start, componentType, stage);
            return ref array[length];
        }

        public readonly ref PushBinding AddPushBinding<T>(ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            Schema schema = world.Schema;
            DataType componentType = schema.GetComponentDataType<T>();
            return ref AddPushBinding(componentType, stage);
        }

        public readonly ref ComponentBinding AddComponentBinding(DescriptorResourceKey key, uint entity, DataType componentType, ShaderType stage)
        {
            ThrowIfComponentBindingIsAlreadyPresent(key, stage);

            USpan<ComponentBinding> array = GetArray<ComponentBinding>();
            uint length = array.Length;
            array = ResizeArray<ComponentBinding>(length + 1);
            array[length] = new(key, entity, componentType, stage);
            return ref array[length];
        }

        public readonly ref ComponentBinding AddComponentBinding(DescriptorResourceKey key, Entity entity, DataType componentType, ShaderType stage)
        {
            return ref AddComponentBinding(key, entity.value, componentType, stage);
        }

        public readonly ref ComponentBinding AddComponentBinding<T>(DescriptorResourceKey key, uint entity, ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            Schema schema = world.Schema;
            DataType componentType = schema.GetComponentDataType<T>();
            return ref AddComponentBinding(key, entity, componentType, stage);
        }

        public readonly ref ComponentBinding AddComponentBinding<T>(DescriptorResourceKey key, Entity entity, ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            return ref AddComponentBinding<T>(key, entity.value, stage);
        }

        public readonly ref ComponentBinding GetComponentBinding(DescriptorResourceKey key, ShaderType stage)
        {
            ThrowIfComponentBindingIsMissing(key, stage);

            USpan<ComponentBinding> array = GetArray<ComponentBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                ref ComponentBinding existingBinding = ref array[i];
                if (existingBinding.key == key && existingBinding.stage == stage)
                {
                    return ref existingBinding;
                }
            }

            throw new InvalidOperationException($"Component binding `{key}` is missing on `{value}`");
        }

        public readonly void SetComponentBinding<T>(DescriptorResourceKey key, uint entity, ShaderType stage = ShaderType.Vertex)
        {
            ThrowIfComponentBindingIsMissing(key, stage);

            ref ComponentBinding binding = ref GetComponentBinding(key, stage);
            binding.entity = entity;
        }

        public readonly void SetComponentBinding<T>(DescriptorResourceKey key, Entity entity, ShaderType stage = ShaderType.Vertex)
        {
            SetComponentBinding<T>(key, entity.value, stage);
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, uint texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            ThrowIfTextureBindingIsAlreadyPresent(key);

            USpan<TextureBinding> array = GetArray<TextureBinding>();
            uint length = array.Length;
            array = ResizeArray<TextureBinding>(length + 1);
            array[length] = new(0, key, texture, region, filtering);
            return ref array[length];
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, uint texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture, new(0, 0, 1, 1), filtering);
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, Entity texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture.value, region, filtering);
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, Entity texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture.value, new(0, 0, 1, 1), filtering);
        }

        public readonly ref TextureBinding GetTextureBinding(DescriptorResourceKey key)
        {
            ThrowIfTextureBindingIsMissing(key);

            USpan<TextureBinding> array = GetArray<TextureBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                ref TextureBinding existingBinding = ref array[i];
                if (existingBinding.key == key)
                {
                    return ref existingBinding;
                }
            }

            throw new InvalidOperationException($"Texture binding `{key}` is missing on `{value}`");
        }

        public readonly bool TryGetFirstTextureBinding(uint texture, out TextureBinding binding)
        {
            USpan<TextureBinding> array = GetArray<TextureBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                TextureBinding existingBinding = array[i];
                if (existingBinding.Entity == texture)
                {
                    binding = existingBinding;
                    return true;
                }
            }

            binding = default;
            return false;
        }

        public unsafe readonly ref TextureBinding TryGetTextureBinding(DescriptorResourceKey key, out bool contains)
        {
            contains = TryIndexOfTextureBinding(key, out uint index);
            if (contains)
            {
                USpan<TextureBinding> array = GetArray<TextureBinding>();
                return ref array[index];
            }
            else
            {
                return ref *(TextureBinding*)default;
            }
        }

        public readonly bool TryIndexOfTextureBinding(DescriptorResourceKey key, out uint index)
        {
            USpan<TextureBinding> array = GetArray<TextureBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                if (array[i].key == key)
                {
                    index = i;
                    return true;
                }
            }

            index = 0;
            return false;
        }

        public readonly void SetTextureBinding(DescriptorResourceKey key, uint texture, Vector4 region, TextureFiltering filtering)
        {
            ThrowIfTextureBindingIsMissing(key);

            USpan<TextureBinding> array = GetArray<TextureBinding>();
            for (uint i = 0; i < array.Length; i++)
            {
                ref TextureBinding existingBinding = ref array[i];
                if (existingBinding.key == key)
                {
                    existingBinding.SetTexture(texture);
                    existingBinding.SetRegion(region);
                    existingBinding.SetFiltering(filtering);
                }
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNotLoaded()
        {
            if (!IsLoaded)
            {
                throw new InvalidOperationException($"Material `{value}` is not loaded");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPushBindingIsMissing(DataType componentType)
        {
            if (!ContainsPushBinding(componentType))
            {
                throw new InvalidOperationException($"Push binding `{componentType.ToString(world.Schema)}` is missing on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPushBindingIsAlreadyPresent(DataType componentType)
        {
            if (ContainsPushBinding(componentType))
            {
                throw new InvalidOperationException($"Push binding `{componentType.ToString(world.Schema)}` already exists on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfComponentBindingIsMissing(DescriptorResourceKey key, ShaderType stage)
        {
            if (!ContainsComponentBinding(key, stage))
            {
                throw new InvalidOperationException($"Component binding `{key}` is missing on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfComponentBindingIsAlreadyPresent(DescriptorResourceKey key, ShaderType stage)
        {
            if (ContainsComponentBinding(key, stage))
            {
                throw new InvalidOperationException($"Component binding `{key}` already exists on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfTextureBindingIsMissing(DescriptorResourceKey key)
        {
            if (!ContainsTextureBinding(key))
            {
                throw new InvalidOperationException($"Texture binding `{key}` is missing on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfTextureBindingIsAlreadyPresent(DescriptorResourceKey key)
        {
            if (ContainsTextureBinding(key))
            {
                throw new InvalidOperationException($"Texture binding `{key}` already exists on `{value}`");
            }
        }
    }
}