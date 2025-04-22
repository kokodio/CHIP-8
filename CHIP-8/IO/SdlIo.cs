using CHIP_8.Display;
using CHIP_8.Keyboard;

namespace CHIP_8.IO;

public record SdlIo : IIO
{
    public IDisplay Output { get; } = new SdlDisplay();
    public IInput Input { get; } = new SdlInput();
}