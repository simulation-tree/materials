namespace Materials
{
    public enum StencilOperation : byte
    {
        Keep,
        Zero,
        Replace,
        IncrementThenClamp,
        DecrementThenClamp,
        Invert,
        IncrementThenWrap,
        DecrementThenWrap
    }
}