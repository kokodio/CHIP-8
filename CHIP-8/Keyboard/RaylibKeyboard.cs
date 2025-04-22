using Raylib_cs;

namespace CHIP_8.Keyboard;

public class RaylibKeyboard : IKeyboard
{
    private readonly bool[] keyboard = new bool[16];

    private readonly Dictionary<KeyboardKey, byte> map = new()
    {
        { KeyboardKey.One, 1 },
        { KeyboardKey.Two, 2 },
        { KeyboardKey.Three, 3 },
        { KeyboardKey.Four, 12 },

        { KeyboardKey.Q, 4 },
        { KeyboardKey.W, 5 },
        { KeyboardKey.E, 6 },
        { KeyboardKey.R, 13 },

        { KeyboardKey.A, 7 },
        { KeyboardKey.S, 8 },
        { KeyboardKey.D, 9 },
        { KeyboardKey.F, 14 },

        { KeyboardKey.Z, 10 },
        { KeyboardKey.X, 0 },
        { KeyboardKey.C, 11 },
        { KeyboardKey.V, 15 },
    };

    public bool IsKeyPressed(int hexKey)
    {
        return keyboard[hexKey];
    }

    public void UpdateKeyboard()
    {
        Raylib.PollInputEvents();
        foreach (var pair in map)
        {
            keyboard[pair.Value] = Raylib.IsKeyDown(pair.Key);
        }
    }

    public byte WaitForInput()
    {
        while (true)
        {
            Raylib.PollInputEvents();
            foreach (var pair in map)
            {
                if (Raylib.IsKeyPressed(pair.Key))
                    return pair.Value;
            }
        }
    }
}
