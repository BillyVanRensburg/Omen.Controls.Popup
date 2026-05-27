namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>Represents an X/Y coordinate.</summary>
public readonly struct Point
{
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y) { X = x; Y = y; }

    public void Deconstruct(out double x, out double y) { x = X; y = Y; }
}