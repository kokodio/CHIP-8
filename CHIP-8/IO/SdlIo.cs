using CHIP_8.Displays;
using CHIP_8.Keyboards;

namespace CHIP_8.IO;

public record SdlIo : IIo
{
    public IDisplay Output { get; } = new SdlDisplay();
    public InputHandler Input { get; } = new SdlInput();
}