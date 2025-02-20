using System;

namespace Materials
{
    [Flags]
    public enum MaterialFlags : byte
    {
        None = 0,
        DepthTest = 1,
        DepthWrite = 2
    }
}