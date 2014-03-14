namespace LogitechWrapper
{
    public interface IImage
    {
        int Width { get; }
        int Height { get; }

        byte GetPixel(int x, int y);
        void Resize(int width, int height);
    }
}