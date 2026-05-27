namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>Represents a rectangle with position and size.</summary>
public readonly struct Rectangle
{
    public double X { get; }
    public double Y { get; }
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double x, double y, double width, double height)
    {
        X = x; Y = y; Width = width; Height = height;
    }

    public double Left => X;
    public double Top => Y;
    public double Right => X + Width;
    public double Bottom => Y + Height;

    public void Deconstruct(out double x, out double y, out double width, out double height)
    {
        x = X; y = Y; width = Width; height = Height;
    }
}