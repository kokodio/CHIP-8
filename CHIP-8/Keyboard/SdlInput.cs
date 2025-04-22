using SDL;
using static SDL.SDL3;

namespace CHIP_8.Keyboard;

public unsafe class SdlInput : IInput
{
    private readonly bool[] keyboard = new bool[16];

    private readonly Dictionary<SDL_Keycode, byte> map = new()
    {
        { SDL_Keycode.SDLK_1, 1 },
        { SDL_Keycode.SDLK_2, 2 },
        { SDL_Keycode.SDLK_3, 3 },
        { SDL_Keycode.SDLK_4, 12 },

        { SDL_Keycode.SDLK_Q, 4 },
        { SDL_Keycode.SDLK_W, 5 },
        { SDL_Keycode.SDLK_E, 6 },
        { SDL_Keycode.SDLK_R, 13 },

        { SDL_Keycode.SDLK_A, 7 },
        { SDL_Keycode.SDLK_S, 8 },
        { SDL_Keycode.SDLK_D, 9 },
        { SDL_Keycode.SDLK_F, 14 },

        { SDL_Keycode.SDLK_Z, 10 },
        { SDL_Keycode.SDLK_X, 0 },
        { SDL_Keycode.SDLK_C, 11 },
        { SDL_Keycode.SDLK_V, 15 },
    };

    public bool IsKeyPressed(int hexKey)
    {
        return keyboard[hexKey];
    }

    public void UpdateKeyboard()
    {
        SDL_Event input;
        while (SDL_PollEvent(&input))
        {
            if (input.quit.type == SDL_EventType.SDL_EVENT_QUIT)
            {
                Environment.Exit(0);
            }
            if (input.key.type is SDL_EventType.SDL_EVENT_KEY_DOWN or SDL_EventType.SDL_EVENT_KEY_UP)
            {
                var keycode = input.key.key;
                var isDown = (input.key.type == SDL_EventType.SDL_EVENT_KEY_DOWN);

                if (map.TryGetValue(keycode, out var index))
                {
                    keyboard[index] = isDown;
                }
            }
        }
    }

    public byte WaitForInput()
    {
        while (true)
        {
            SDL_Event input;
            while (SDL_PollEvent(&input))
            {
                foreach (var pair in map)
                {
                    if (input.key.down && pair.Key == input.key.key)
                        return pair.Value;
                }
            }
        }
    }
}
