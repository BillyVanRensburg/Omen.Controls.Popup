namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>
/// Represents a rectangle defined by its position (X, Y) and size (Width, Height).
/// This struct is immutable and lightweight, suitable for positioning and layout calculations.
/// </summary>
/// <param name="x">The X‑coordinate of the left edge.</param>
/// <param name="y">The Y‑coordinate of the top edge.</param>
/// <param name="width">The width of the rectangle.</param>
/// <param name="height">The height of the rectangle.</param>
public readonly struct Rectangle(double x, double y, double width, double height)
{
    /// <summary>
    /// Gets the X‑coordinate of the rectangle's left edge.
    /// </summary>
    public double X => x;

    /// <summary>
    /// Gets the Y‑coordinate of the rectangle's top edge.
    /// </summary>
    public double Y => y;

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public double Width => width;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public double Height => height;

    /// <summary>
    /// Gets the X‑coordinate of the left edge (same as <see cref="X"/>).
    /// </summary>
    public double Left => x;

    /// <summary>
    /// Gets the Y‑coordinate of the top edge (same as <see cref="Y"/>).
    /// </summary>
    public double Top => y;

    /// <summary>
    /// Gets the X‑coordinate of the right edge (X + Width).
    /// </summary>
    public double Right => x + width;

    /// <summary>
    /// Gets the Y‑coordinate of the bottom edge (Y + Height).
    /// </summary>
    public double Bottom => y + height;

    /// <summary>
    /// Deconstructs the rectangle into its X, Y, Width, and Height components.
    /// Enables tuple‑like deconstruction syntax: <c>(double xOut, double yOut, double w, double h) = rectangle;</c>
    /// </summary>
    /// <param name="xOut">The X‑coordinate (left edge).</param>
    /// <param name="yOut">The Y‑coordinate (top edge).</param>
    /// <param name="w">The width.</param>
    /// <param name="h">The height.</param>
    public void Deconstruct(out double xOut, out double yOut, out double w, out double h)
    {
        xOut = x;
        yOut = y;
        w = width;
        h = height;
    }
}