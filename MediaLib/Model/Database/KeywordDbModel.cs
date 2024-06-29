using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a keyword entry
/// </summary>
/// <remarks>
/// Table <c>Keyword</c>
/// </remarks>
[Table("Keyword", Schema = "dbo")]
internal sealed class KeywordDbModel
{
    /// <summary>
    /// Gets or sets the internal id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the id of the keyword type
    /// </summary>
    public int KeywordTypeId { get; set; }

    /// <summary>
    /// Gets or sets the id of the attached object
    /// </summary>
    public int ObjectId { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    [MaxLength(2000)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date / time
    /// </summary>
    public DateTime CreationDateTime { get; set; } = DateTime.Now;

}