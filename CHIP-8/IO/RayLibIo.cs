using CHIP_8.Displays;
using CHIP_8.Keyboards;

namespace CHIP_8.IO;

public record RayLibIo : IIo
{
    public IDisplay Output { get; } = new RaylibDisplay();
    public InputHandler Input { get; } = new RaylibInput();
}