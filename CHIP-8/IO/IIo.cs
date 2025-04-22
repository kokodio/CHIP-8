using CHIP_8.Displays;
using CHIP_8.Keyboards;

namespace CHIP_8.IO;

public interface IIo
{
    public IDisplay Output { get; }
    public InputHandler Input { get; }
}