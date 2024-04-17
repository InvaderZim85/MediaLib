using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a medium type like a DVD, Blu-ray or something else
/// </summary>
[Table("MediumType", Schema = "dbo")]
public sealed class MediumTypeDbModel
{
    /// <summary>
    /// Gets or sets the id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}