using CHIP_8.Display;
using CHIP_8.Keyboard;

namespace CHIP_8.IO;

public interface IIO
{
    public IDisplay Output { get; }
    public IInput Input { get; }
}