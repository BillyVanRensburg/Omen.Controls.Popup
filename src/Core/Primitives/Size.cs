namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>
/// Represents a size in two‑dimensional space using double precision dimensions.
/// This struct is immutable and lightweight, suitable for layout and animation calculations.
/// </summary>
/// <param name="width">The width component.</param>
/// <param name="height">The height component.</param>
public readonly struct Size(double width, double height)
{
    /// <summary>
    /// Gets the width component of the size.
    /// </summary>
    public double Width => width;

    /// <summary>
    /// Gets the height component of the size.
    /// </summary>
    public double Height => height;

    /// <summary>
    /// Deconstructs the size into its width and height components.
    /// Enables tuple‑like deconstruction syntax: <c>(double w, double h) = size;</c>
    /// </summary>
    /// <param name="w">The width component.</param>
    /// <param name="h">The height component.</param>
    public void Deconstruct(out double w, out double h)
    {
        w = width;
        h = height;
    }
}