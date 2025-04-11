namespace Materials
{
    public enum BlendFactor : byte
    {
        Zero,
        One,
        SourceColor,
        OneMinusSourceColor,
        DestinationColor,
        OneMinusDestinationColor,
        SourceAlpha,
        OneMinusSourceAlpha,
        DestinationAlpha,
        OneMinusDestinationAlpha,
        ConstantColor,
        OneMinusConstantColor,
        ConstantAlpha,
        OneMinusConstantAlpha,
        SourceAlphaSaturate,
        SourceOneColor,
        OneMinusSourceOneColor,
        SourceOneAlpha,
        OneMinusSourceOneAlpha
    }
}