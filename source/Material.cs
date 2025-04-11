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
        public static readonly MaterialFlags DefaultFlags = MaterialFlags.DepthTest | MaterialFlags.DepthWrite;
        public static readonly CompareOperation DefaultDepthCompareOperation = CompareOperation.Less;

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

        public readonly ref sbyte RenderOrder
        {
            get
            {
                ThrowIfNotLoaded();

                return ref GetComponent<IsMaterial>().renderOrder;
            }
        }

        public readonly DepthSettings DepthSettings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMaterial>().depthSettings;
            }
        }

        public readonly BlendSettings BlendSettings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetComponent<IsMaterial>().blendSettings;
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

        public readonly ReadOnlySpan<EntityComponentBinding> ComponentBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<EntityComponentBinding>();
            }
        }

        public readonly ReadOnlySpan<TextureBinding> TextureBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<TextureBinding>();
            }
        }

        public readonly ReadOnlySpan<InstanceDataBinding> InstanceBindings
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<InstanceDataBinding>();
            }
        }

        /// <summary>
        /// Creates a request to load a material entity from the given <paramref name="address"/>.
        /// </summary>
        public Material(World world, ASCIIText256 address, TimeSpan timeout = default)
        {
            this.world = world;
            value = world.CreateEntity(new IsMaterialRequest(address, timeout));
            CreateArray<InstanceDataBinding>();
            CreateArray<EntityComponentBinding>();
            CreateArray<TextureBinding>();
        }

        /// <summary>
        /// Creates a new material initialized with the given shaders.
        /// </summary>
        public Material(World world, Shader vertexShader, Shader fragmentShader)
        {
            MaterialFlags flags = DefaultFlags;
            CompareOperation depthCompareOperation = DefaultDepthCompareOperation;

            this.world = world;
            value = world.CreateEntity(new IsMaterial(0, 0, (rint)1, (rint)2, BlendSettings.Opaque, DepthSettings.Default));
            AddReference(vertexShader);
            AddReference(fragmentShader);
            CreateArray<InstanceDataBinding>();
            CreateArray<EntityComponentBinding>();
            CreateArray<TextureBinding>();
        }

        /// <summary>
        /// Creates a new material initialized with the given shaders.
        /// </summary>
        public Material(World world, Shader vertexShader, Shader fragmentShader, BlendSettings blendSettings, DepthSettings depthSettings)
        {
            this.world = world;
            value = world.CreateEntity(new IsMaterial(0, 0, (rint)1, (rint)2, blendSettings, depthSettings));
            AddReference(vertexShader);
            AddReference(fragmentShader);
            CreateArray<InstanceDataBinding>();
            CreateArray<EntityComponentBinding>();
            CreateArray<TextureBinding>();
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMaterial>();
            archetype.AddArrayType<InstanceDataBinding>();
            archetype.AddArrayType<EntityComponentBinding>();
            archetype.AddArrayType<TextureBinding>();
        }

        public readonly bool ContainsInstanceBinding(DataType componentType)
        {
            Span<InstanceDataBinding> array = GetArray<InstanceDataBinding>();
            for (int i = 0; i < array.Length; i++)
            {
                InstanceDataBinding binding = array[i];
                if (binding.componentType == componentType)
                {
                    return true;
                }
            }

            return false;
        }

        public readonly bool ContainsComponentBinding(DescriptorResourceKey key, ShaderType stage)
        {
            Span<EntityComponentBinding> array = GetArray<EntityComponentBinding>();
            for (int i = 0; i < array.Length; i++)
            {
                EntityComponentBinding binding = array[i];
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

        public readonly void AddInstanceBinding(DataType componentType, ShaderType stage)
        {
            ThrowIfPushBindingIsAlreadyPresent(componentType);

            int hash = default;
            int pushBindingType = world.Schema.GetArrayType<InstanceDataBinding>();
            Values<InstanceDataBinding> array = GetArray<InstanceDataBinding>(pushBindingType);
            uint start = 0;
            foreach (InstanceDataBinding existingBinding in array)
            {
                start += existingBinding.componentType.size;
                hash += existingBinding.GetHashCode();
            }

            InstanceDataBinding newBinding = new(start, componentType, stage);
            array.Add(newBinding);
            hash += newBinding.GetHashCode();

            ref IsMaterial component = ref TryGetComponent<IsMaterial>(out bool contains);
            if (!contains)
            {
                component = ref AddComponent<IsMaterial>();
            }

            //component = component.WithInstanceBindingsHash(hash);
        }

        public readonly void AddInstanceBinding<T>(ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            Schema schema = world.Schema;
            DataType componentType = schema.GetComponentDataType<T>();
            AddInstanceBinding(componentType, stage);
        }

        public readonly ref EntityComponentBinding AddComponentBinding(DescriptorResourceKey key, uint entity, DataType componentType, ShaderType stage)
        {
            ThrowIfComponentBindingIsAlreadyPresent(key, stage);

            int componentBindingType = world.Schema.GetArrayType<EntityComponentBinding>();
            Values<EntityComponentBinding> array = GetArray<EntityComponentBinding>(componentBindingType);
            ref EntityComponentBinding added = ref array.Add();
            added.key = key;
            added.entity = entity;
            added.componentType = componentType;
            added.stage = stage;
            return ref added;
        }

        public readonly ref EntityComponentBinding AddComponentBinding(DescriptorResourceKey key, Entity entity, DataType componentType, ShaderType stage)
        {
            return ref AddComponentBinding(key, entity.value, componentType, stage);
        }

        public readonly ref EntityComponentBinding AddComponentBinding<T>(DescriptorResourceKey key, uint entity, ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            Schema schema = world.Schema;
            DataType componentType = schema.GetComponentDataType<T>();
            return ref AddComponentBinding(key, entity, componentType, stage);
        }

        public readonly ref EntityComponentBinding AddComponentBinding<T>(DescriptorResourceKey key, Entity entity, ShaderType stage = ShaderType.Vertex) where T : unmanaged
        {
            return ref AddComponentBinding<T>(key, entity.value, stage);
        }

        public readonly ref EntityComponentBinding GetComponentBinding(DescriptorResourceKey key, ShaderType stage)
        {
            ThrowIfComponentBindingIsMissing(key, stage);

            Span<EntityComponentBinding> array = GetArray<EntityComponentBinding>();
            for (int i = 0; i < array.Length; i++)
            {
                ref EntityComponentBinding existingBinding = ref array[i];
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

            ref EntityComponentBinding binding = ref GetComponentBinding(key, stage);
            binding.entity = entity;
        }

        public readonly void SetComponentBinding<T>(DescriptorResourceKey key, Entity entity, ShaderType stage = ShaderType.Vertex)
        {
            SetComponentBinding<T>(key, entity.value, stage);
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, uint texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            ThrowIfTextureBindingIsAlreadyPresent(key);

            int textureBindingType = world.Schema.GetArrayType<TextureBinding>();
            Values<TextureBinding> array = GetArray<TextureBinding>(textureBindingType);
            ref TextureBinding added = ref array.Add();
            added = new(0, key, texture, region, filtering);
            return ref added;
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, uint texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture, new(0, 0, 1, 1), filtering);
        }

        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, Entity texture, Vector4 region, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture.value, region, filtering);
        }

        /// <summary>
        /// Binds the given <paramref name="texture"/> to <paramref name="key"/> with a default region of (0, 0, 1, 1).
        /// </summary>
        public readonly ref TextureBinding AddTextureBinding(DescriptorResourceKey key, Entity texture, TextureFiltering filtering = TextureFiltering.Linear)
        {
            return ref AddTextureBinding(key, texture.value, new(0, 0, 1, 1), filtering);
        }

        public readonly ref TextureBinding GetTextureBinding(DescriptorResourceKey key)
        {
            ThrowIfTextureBindingIsMissing(key);

            Span<TextureBinding> array = GetArray<TextureBinding>();
            for (int i = 0; i < array.Length; i++)
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
            Span<TextureBinding> array = GetArray<TextureBinding>();
            for (int i = 0; i < array.Length; i++)
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
            contains = TryIndexOfTextureBinding(key, out int index);
            if (contains)
            {
                Span<TextureBinding> array = GetArray<TextureBinding>();
                return ref array[index];
            }
            else
            {
                return ref *(TextureBinding*)default;
            }
        }

        public readonly bool TryIndexOfTextureBinding(DescriptorResourceKey key, out int index)
        {
            Span<TextureBinding> array = GetArray<TextureBinding>();
            for (int i = 0; i < array.Length; i++)
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

            Span<TextureBinding> array = GetArray<TextureBinding>();
            for (int i = 0; i < array.Length; i++)
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
            if (!ContainsInstanceBinding(componentType))
            {
                throw new InvalidOperationException($"Push binding `{componentType.ToString(world.Schema)}` is missing on `{value}`");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPushBindingIsAlreadyPresent(DataType componentType)
        {
            if (ContainsInstanceBinding(componentType))
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