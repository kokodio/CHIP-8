using CHIP_8.Display;
using CHIP_8.Keyboard;

namespace CHIP_8.IO;

public record ConsoleWindowsIo : IIO
{
    public IDisplay Output { get; } = new ConsoleDisplay();
    public IInput Input { get; } = new WindowsConsoleInput();
}