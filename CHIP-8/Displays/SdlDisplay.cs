using SDL;
using static SDL.SDL3;

namespace CHIP_8.Displays;

public unsafe class SdlDisplay : IDisplay
{
    public int Width { get; }
    public int Height { get; }
    
    private const int PixelSize = 10;
    private readonly bool[] pixels;

    private const string Name = "CHIP-8";
    private readonly string device;
    private readonly SDL_Window* window;
    private readonly SDL_Renderer* renderer;
    
    private readonly SDL_Color colorBlack = new() { r = 0, g = 0, b = 0, a = 255 };
    private readonly SDL_Color colorWhite = new() { r = 255, g = 255, b = 255, a = 255 };

    public SdlDisplay()
    {
        Width = 64;
        Height = 32;
        pixels = new bool[Width * Height];

        window = SDL_CreateWindow((Utf8String)(Name),Width * PixelSize, Height * PixelSize, 0);
        if (window == (void*)nint.Zero)
        {
            throw new InvalidOperationException($"SDL Window creation failed: {SDL_GetError()}");
        }
        
        device = SDL_GetRenderDriver(0);
        if (device == null)
        {
            throw new InvalidOperationException($"SDL Driver creation failed: {SDL_GetError()}");
        }
        
        renderer = SDL_CreateRenderer(window, (Utf8String)(device));
        if (renderer == (void*)nint.Zero)
        {
            SDL_DestroyWindow(window);
            throw new InvalidOperationException($"SDL Renderer creation failed: {SDL_GetError()}");
        }
    }
    
    public void Render()
    {
        SDL_SetRenderDrawColor(renderer, colorBlack.r, colorBlack.g, colorBlack.b, colorBlack.a);
        SDL_RenderClear(renderer);
        SDL_SetRenderDrawColor(renderer, colorWhite.r, colorWhite.g, colorWhite.b, colorWhite.a);

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (pixels[y * Width + x])
                {
                    var rect = new SDL_FRect
                    {
                        x = x * PixelSize,
                        y = y * PixelSize,
                        w = PixelSize,
                        h = PixelSize
                    };
                    
                    SDL_RenderFillRect(renderer, &rect);
                }
            }
        }

        SDL_RenderPresent(renderer);
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