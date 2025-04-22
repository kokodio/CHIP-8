using System.Diagnostics;
using CHIP_8;
using CHIP_8.Display;
using CHIP_8.Keyboard;

var display = new ConsoleDisplay();
var keyboard = new WindowsConsoleKeyboard();
var chip = new Chip8(display, keyboard);

chip.LoadRom(@".\tetris.ch8", false);

const int iterationsPerFrame = 8;
const int targetFps = 60;

var frameTime = TimeSpan.FromMilliseconds(1000.0 / targetFps);
var frameTimer = Stopwatch.StartNew();

while (true)
{
    frameTimer.Restart();
    for (var i = 0; i < iterationsPerFrame; i++)
    {
        chip.RunCycle();
    }
    
    chip.TickTimers();
    
    var sleep = frameTime - frameTimer.Elapsed;
    if (sleep > TimeSpan.Zero)
    {
        Thread.Sleep(sleep);
    }
}