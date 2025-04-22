using Raylib_cs;

namespace CHIP_8.Displays;

public class RaylibDisplay : IDisplay
{
    private readonly bool[] pixels;
    private const int PixelSize = 10;

    public int Width { get; }
    public int Height { get; }

    public RaylibDisplay()
    {
        Width = 64;
        Height = 32;
        pixels = new bool[Width * Height];

        Raylib.InitWindow(Width * PixelSize, Height * PixelSize, "CHIP-8 Emulator");
        Raylib.SetTargetFPS(60);
    }

    public void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (pixels[y * Width + x])
                {
                    Raylib.DrawRectangle(x * PixelSize, y * PixelSize, PixelSize, PixelSize, Color.White);
                }
            }
        }

        Raylib.EndDrawing();
    }

    public void Set(int idx, bool bit)
    {
        pixels[idx] = bit;
    }

    public void Update(int idx, bool bit)
    {
        pixels[idx] ^= bit;
    }

    public bool Get(int idx)
    {
        return pixels[idx];
    }

    public void Clear()
    {
        Array.Clear(pixels, 0, pixels.Length);
    }
}
