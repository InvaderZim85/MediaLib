using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a keyword type
/// </summary>
/// <remarks>
/// Table <c>KeywordType</c>
/// </remarks>
[Table("KeywordType", Schema = "dbo")]
internal sealed class KeywordTypeDbModel
{
    /// <summary>
    /// Gets or sets the internal id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
}