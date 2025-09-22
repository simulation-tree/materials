using System;

namespace Materials
{
    [Flags]
    public enum MaterialFlags : byte
    {
        None = 0,
        Instanced = 1,
    }
}