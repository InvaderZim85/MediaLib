using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZimLabs.Mapper;

namespace MediaLib.Model.Database;

/// <summary>
/// Provides the basic information which are provided by each media table
/// </summary>
public class BaseDbModel : CreatedModifiedDateTime
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the link (optional)
    /// </summary>
    [MaxLength(1000)]
    public string Link { get; set; } = string.Empty;

    /// <summary>
    /// Gets the value which indicates whether the entry has a link
    /// </summary>
    [NotMapped]
    [IgnoreProperty]
    public bool HasLink => !string.IsNullOrWhiteSpace(Link);

    /// <summary>
    /// Gets or sets the keywords
    /// </summary>
    [NotMapped]
    public string Keywords { get; set; } = string.Empty;
}