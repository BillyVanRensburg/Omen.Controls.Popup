namespace Omen.Controls.Popup.Core.Primitives;

/// <summary>
/// Represents a point in two‑dimensional space using double precision coordinates.
/// This struct is immutable and lightweight, suitable for use in positioning calculations.
/// </summary>
/// <param name="x">The X‑coordinate.</param>
/// <param name="y">The Y‑coordinate.</param>
public readonly struct Point(double x, double y)
{
    /// <summary>
    /// Gets the X‑coordinate of the point.
    /// </summary>
    public double X => x;

    /// <summary>
    /// Gets the Y‑coordinate of the point.
    /// </summary>
    public double Y => y;

    /// <summary>
    /// Deconstructs the point into its X and Y components.
    /// Enables tuple‑like deconstruction syntax: <c>(double x, double y) = point;</c>
    /// </summary>
    /// <param name="xOut">The X‑coordinate.</param>
    /// <param name="yOut">The Y‑coordinate.</param>
    public void Deconstruct(out double xOut, out double yOut)
    {
        xOut = x;
        yOut = y;
    }
}