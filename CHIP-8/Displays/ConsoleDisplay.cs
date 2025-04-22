namespace CHIP_8.Displays;

public class ConsoleDisplay : IDisplay
{
    private readonly byte[] displayBuffer;
    private readonly bool[] pixels;
    private readonly Stream stdout;
    
    public ConsoleDisplay()
    {
        Width = 64;
        Height = 32;
        
        displayBuffer = new byte[Width * Height + Height];
        pixels = new bool[Width * Height];
        stdout = Console.OpenStandardOutput(Width * Height + Height);
        Console.CursorVisible = false;
        for (var y = 0; y < 32; y++)
        {
            var bufferOffset = y * (64 + 1);
            displayBuffer[bufferOffset + 64] = (byte)'\n';
        }
    }

    public int Width { get; }
    public int Height { get; }

    public void Render()
    {
        Console.SetCursorPosition(0, 0);

        for (var y = 0; y < Height; y++)
        {
            var rowOffset = y * Width;
            var bufferOffset = y * (Width + 1);
            for (var x = 0; x < Width; x++)
            {
                displayBuffer[bufferOffset + x] = (byte)(pixels[rowOffset + x] ? '#' : ' ');
            }
        }

        stdout.Write(displayBuffer, 0, displayBuffer.Length);
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
        for (var j = 0; j < 32; j++)
        {
            for (var i = 0; i < 64; i++)
            {
                pixels[j*64+i] = false;
            }
        }
    }
}