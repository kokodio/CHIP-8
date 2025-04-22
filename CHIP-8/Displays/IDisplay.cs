namespace CHIP_8.Displays;

public interface IDisplay
{
    public int Width { get; }
    public int Height { get; }
    public void Render();
    public void Set(int idx, bool bit);
    public void Update(int idx, bool bit);
    public bool Get(int idx);
    
    public void Clear();
}