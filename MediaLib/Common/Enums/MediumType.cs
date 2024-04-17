namespace MediaLib.Common.Enums;

/// <summary>
/// Provides the different medium types
/// </summary>
/// <remarks>
/// This is a 1:1 copy of the table <c>MediumType</c>
/// </remarks>
internal enum MediumType
{
    /// <summary>
    /// DVD
    /// </summary>
    Dvd = 1,

    /// <summary>
    /// Blu-ray
    /// </summary>
    BluRay = 2,

    /// <summary>
    /// VHS (old school)
    /// </summary>
    Vhs = 3,

    /// <summary>
    /// Other
    /// </summary>
    Other = 4,

    /// <summary>
    /// Digital media
    /// </summary>
    Digital = 5
}