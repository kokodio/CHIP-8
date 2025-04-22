using CHIP_8.Display;
using CHIP_8.Keyboard;

namespace CHIP_8;

public class Chip8
{
    private const int MemorySize = 4096;
    private const ushort ProgramStart = 0x200;
    private const ushort FontStartAddress = 0x50;

    private readonly IKeyboard keyboard;
    private readonly IDisplay display;
    
    private readonly byte[] memory = new byte[MemorySize];
    private readonly byte[] v = new byte[16];
    private readonly ushort[] stack = new ushort[16];
    private readonly Random random = new();

    private ushort pc = ProgramStart;
    private ushort index;
    private byte delayTimer;
    private byte stackPointer;
    private ushort currentOpcode;
    private bool isLegacy;
    
    private static readonly byte[] Fontset =
    [
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    ];

    public Chip8(IDisplay display, IKeyboard keyboard)
    {
        this.display = display;
        this.keyboard = keyboard;
        InitializeFontset();
    }
    
    private void InitializeFontset()
    {
        Fontset.AsSpan().CopyTo(memory.AsSpan().Slice(FontStartAddress));
    }

    public void LoadRom(string path, bool isRomLegacy)
    {
        isLegacy = isRomLegacy;
        File.ReadAllBytes(path).AsSpan().CopyTo(memory.AsSpan().Slice(pc));
    }

    public void RunCycle()
    {
        keyboard.UpdateKeyboard();
        FetchOpcode();
        DecodeAndExecute();
    }

    private void FetchOpcode()
    {
        currentOpcode = (ushort)((memory[pc] << 8) | memory[pc + 1]);
    }

    private void DecodeAndExecute()
    {
        var x = (currentOpcode & 0x0F00) >> 8;
        var y = (currentOpcode & 0x00F0) >> 4;
        var n = (byte)(currentOpcode & 0x000F);
        var nn = (byte)(currentOpcode & 0x00FF);
        var nnn = (ushort)(currentOpcode & 0x0FFF);

        switch (currentOpcode & 0xF000)
        {
            case 0x0000: HandleSystemOpcodes(); break;
            case 0x1000: JumpToAddress(nnn); break;
            case 0x2000: CallSubroutine(nnn); break;
            case 0x3000: SkipIfEqualImmediate(x, nn); break;
            case 0x4000: SkipIfNotEqualImmediate(x, nn); break;
            case 0x5000: SkipIfEqualRegister(x, y); break;
            case 0x6000: SetRegister(x, nn); break;
            case 0x7000: AddImmediate(x, nn); break;
            case 0x8000: HandleArithmeticAndLogic(x, y, n); break;
            case 0x9000: SkipIfNotEqualRegister(x, y); break;
            case 0xA000: SetIndex(nnn); break;
            case 0xB000: JumpWithOffset(nnn, x); break;
            case 0xC000: RandomAnd(x, nn); break;
            case 0xD000: DrawSprite(x, y, n); break;
            case 0xE000: HandleKeyOperations(x, nn); break;
            case 0xF000: HandleTimerAndMemory(x, nn); break;
            default: throw new InvalidOperationException($"Unknown opcode: 0x{currentOpcode:X4}");
        }

        pc += 2;
    }

    private void HandleSystemOpcodes()
    {
        switch (currentOpcode)
        {
            case 0x00E0: display.Clear(); break;
            case 0x00EE: ReturnFromSubroutine(); break;
        }
    }

    private void JumpToAddress(ushort address)
    {
        pc = address;
        pc -= 2;
    }

    private void CallSubroutine(ushort address)
    {
        stack[stackPointer++] = pc;
        pc = address;
        pc -= 2;
    }

    private void ReturnFromSubroutine() => pc = stack[--stackPointer];

    private void SkipIfEqualImmediate(int x, byte value)
    {
        if (v[x] == value) pc += 2;
    }

    private void SkipIfNotEqualImmediate(int x, byte value)
    {
        if (v[x] != value) pc += 2;
    }

    private void SkipIfEqualRegister(int x, int y) => pc += (ushort)(v[x] == v[y] ? 2 : 0);

    private void SkipIfNotEqualRegister(int x, int y) => pc += (ushort)(v[x] != v[y] ? 2 : 0);

    private void SetRegister(int x, byte value) => v[x] = value;

    private void AddImmediate(int x, byte value) => v[x] += value;

    private void HandleArithmeticAndLogic(int x, int y, byte op)
    {
        switch (op)
        {
            case 0x0: Assign(x, y); break;
            case 0x1: Or(x, y); break;
            case 0x2: And(x, y); break;
            case 0x3: Xor(x, y); break;
            case 0x4: AddWithCarry(x, y); break;
            case 0x5: SubtractWithBorrow(x, y); break;
            case 0x6: ShiftRight(x, y); break;
            case 0x7: ReverseSubtract(x, y); break;
            case 0xE: ShiftLeft(x, y); break;
        }
    }

    private void Assign(int x, int y)
    {
        v[x] = v[y];
    }
    
    private void Or(int x, int y)
    {
        v[x] |= v[y];
        if (isLegacy) v[0xF] = 0;
    }
    
    private void And(int x, int y)
    {
        v[x] &= v[y];
        if (isLegacy) v[0xF] = 0;
    }
    
    private void Xor(int x, int y)
    {
        v[x] ^= v[y];
        if (isLegacy) v[0xF] = 0;
    }

    private void AddWithCarry(int x, int y)
    {
        var sum = (ushort)(v[x] + v[y]);
        v[x] = (byte)(sum & 0xFF);
        if (sum > 0xFF) v[0xF] = 1;
        else v[0xF] = 0;
    }

    private void SubtractWithBorrow(int x, int y)
    {
        var result = (ushort)(v[x] - v[y]);
        v[x] = (byte)result;
        v[0xF] = (byte)(result > 255 ? 0 : 1);
    }

    private void ReverseSubtract(int x, int y)
    {
        var result = (ushort)(v[y] - v[x]);
        v[x] = (byte)result;
        v[0xF] = (byte)(result > 255 ? 0 : 1);
    }

    private void ShiftRight(int x, int y)
    {
        var carry = (byte)(v[x] & 1);
        if (isLegacy) v[x] = (byte)(v[y] >> 1);
        else v[x] >>= 1;
        v[0xF] = carry;
    }

    private void ShiftLeft(int x, int y)
    {
        var carry = (byte)(v[x] >> 7);
        if (isLegacy) v[x] = (byte)(v[y] << 1);
        else v[x] <<= 1;
        v[0xF] = carry;
    }

    private void SetIndex(ushort address) => index = address;

    private void JumpWithOffset(ushort address, int x)
    {
        pc = address;
        if (isLegacy) pc += v[0];
        else pc += v[x];
        pc -= 2;
    }
    
    private void RandomAnd(int x, byte mask) => v[x] = (byte)(random.Next(0, 256) & mask);

    private void DrawSprite(int xReg, int yReg, byte height)
    {
        var xPos = v[xReg] & 63;
        var yPos = v[yReg] & 31;
        v[0xF] = 0;

        for (var row = 0; row < height; row++)
        {
            var drawY = yPos + row;
            if (drawY == display.Height) break;
            var spriteRow = memory[index + row];
            for (var col = 0; col < 8; col++)
            {
                var pixel = (spriteRow & (0x80 >> col)) != 0;
                var drawX = xPos + col;
                if (drawX == display.Width) break;
                var idx = drawY * display.Width + drawX;

                if (pixel && display.Get(idx)) v[0xF] = 1;
                display.Update(idx, pixel);
            }
        }
    }

    private void HandleKeyOperations(int x, byte subcode)
    {
        switch (subcode)
        {
            case 0x9E:
                if (keyboard.IsKeyPressed(v[x])) pc += 2;
                break;
            case 0xA1:
                if (!keyboard.IsKeyPressed(v[x])) pc += 2;
                break;
        }
    }

    private void HandleTimerAndMemory(int x, byte subcode)
    {
        switch (subcode)
        {
            case 0x07: v[x] = delayTimer; break;
            case 0x0A: v[x] = keyboard.WaitForInput(); break;
            case 0x15: delayTimer = v[x]; break;
            case 0x1E: index += v[x]; break;
            case 0x29: index = (ushort)(0x50 + 5 * v[x]); break;
            case 0x33: StoreBcd(x); break;
            case 0x55: DumpRegisters(x); break;
            case 0x65: LoadRegisters(x); break;
        }
    }

    private void StoreBcd(int x)
    {
        memory[index] = (byte)(v[x] / 100);
        memory[index + 1] = (byte)((v[x] / 10) % 10);
        memory[index + 2] = (byte)(v[x] % 10);
    }

    private void DumpRegisters(int x)
    {
        for (var i = 0; i <= x; i++)
            memory[index + i] = v[i];
        if (isLegacy) index += (ushort)(x + 1);
    }

    private void LoadRegisters(int x)
    {
        for (var i = 0; i <= x; i++)
            v[i] = memory[index + i];
        if (isLegacy) index += (ushort)(x + 1);
    }

    public void TickTimers()
    {
        if (delayTimer > 0) delayTimer--;
    }
}