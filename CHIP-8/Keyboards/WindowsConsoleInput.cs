using System.Runtime.InteropServices;

namespace CHIP_8.Keyboards;

public unsafe partial class WindowsConsoleInput : InputHandler
{
    private readonly bool[] keyboard = new bool[16];
    
    private readonly Dictionary<ConsoleKey, byte> map = new()
    {
        { ConsoleKey.D1, 1 },
        { ConsoleKey.D2, 2 },
        { ConsoleKey.D3, 3 },
        { ConsoleKey.D4, 12 },
        
        { ConsoleKey.Q, 4 },
        { ConsoleKey.W, 5 },
        { ConsoleKey.E, 6 },
        { ConsoleKey.R, 13 },
        
        { ConsoleKey.A, 7 },
        { ConsoleKey.S, 8 },
        { ConsoleKey.D, 9 },
        { ConsoleKey.F, 14 },
        
        { ConsoleKey.Z, 10 },
        { ConsoleKey.X, 0 },
        { ConsoleKey.C, 11 },
        { ConsoleKey.V, 15 },
    };
    public override bool IsKeyPressed(int hexKey)
    {
        return keyboard[hexKey];
    }
    
    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(int vKey);
    
    public override void UpdateKeyboard()
    {
        foreach (var pair in map)
        {
            var vKey = (int)pair.Key;
            keyboard[pair.Value] = (GetAsyncKeyState(vKey) & 0x8000) != 0;
        }
    }

    public override byte WaitForInput()
    {
        byte keyIndex;
        while (!map.TryGetValue(Console.ReadKey(true).Key, out keyIndex)) { }
        return keyIndex;
    }
}