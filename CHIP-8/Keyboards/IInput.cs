namespace CHIP_8.Keyboards;

public abstract class InputHandler
{
    public abstract bool IsKeyPressed(int hexKey);
    public abstract void UpdateKeyboard();
    public abstract byte WaitForInput();
}