using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a settings entry
/// </summary>
/// <remarks>
/// Table <c>Settings</c>
/// </remarks>
[Table("Settings", Schema = "dbo")]
public sealed class SettingsDbModel : CreatedModifiedDateTime
{
    /// <summary>
    /// Gets or sets the internal id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the settings key
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

}