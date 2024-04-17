namespace MediaLib.Model.Internal;

/// <summary>
/// Represents a file entry
/// </summary>
/// <param name="title">The title</param>
/// <param name="mediaTypeId">The media type</param>
/// <param name="mediumTypeId">The medium type id</param>
/// <param name="distributorId">The distributor id</param>
internal class FileEntry(string title, int mediaTypeId, int mediumTypeId = 0, int distributorId = 0)
{
    /// <summary>
    /// Gets the title
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Gets or sets the media type
    /// </summary>
    /// <remarks>
    /// Matches <see cref="Common.Enums.MediaType"/>
    /// </remarks>
    public int MediaTypeId { get; } = mediaTypeId;

    /// <summary>
    /// Gets the medium type id
    /// </summary>
    /// <remarks>
    /// Matches <see cref="Common.Enums.MediumType"/>
    /// </remarks>
    public int MediumTypeId { get; } = mediumTypeId;

    /// <summary>
    /// Gets the distributor id
    /// </summary>
    /// <remarks>
    /// Matches <see cref="Common.Enums.Distributor"/>
    /// </remarks>
    public int? DistributorId { get; } = distributorId == 0 ? null : distributorId;
}