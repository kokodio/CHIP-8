namespace CHIP_8.Keyboard;

public interface IKeyboard
{
    public bool IsKeyPressed(int hexKey);
    public void UpdateKeyboard();
    public byte WaitForInput();
}