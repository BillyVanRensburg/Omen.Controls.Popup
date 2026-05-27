namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>Represents width and height.</summary>
public readonly struct Size
{
    public double Width { get; }
    public double Height { get; }

    public Size(double width, double height) { Width = width; Height = height; }

    public void Deconstruct(out double width, out double height) { width = Width; height = Height; }
}