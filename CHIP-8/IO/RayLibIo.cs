using CHIP_8.Display;
using CHIP_8.Keyboard;

namespace CHIP_8.IO;

public record RayLibIo : IIO
{
    public IDisplay Output { get; } = new RaylibDisplay();
    public IInput Input { get; } = new RaylibInput();
}