using CHIP_8.Displays;
using CHIP_8.Keyboards;

namespace CHIP_8.IO;

public record ConsoleWindowsIo : IIo
{
    public IDisplay Output { get; } = new ConsoleDisplay();
    public InputHandler Input { get; } = new WindowsConsoleInput();
}