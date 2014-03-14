namespace LogitechWrapper
{
    /// <summary>
    ///     The type of LCD screen. Use the Int constructor and mix, to allow support for both keyboards.
    ///     <para>Keep in mind, this library only currently supports Mono.</para>
    /// </summary>
    public enum LcdScreenType
    {
        Mono = (0x00000001),
        Color = (0x00000002)
    }
}