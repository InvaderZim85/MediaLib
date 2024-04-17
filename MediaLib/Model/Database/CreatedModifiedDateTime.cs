namespace MediaLib.Model.Database;

/// <summary>
/// Provides the creation / modification date / time
/// </summary>
public class CreatedModifiedDateTime
{
    /// <summary>
    /// Gets or sets the creation date / time
    /// </summary>
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the modified date / time
    /// </summary>
    public DateTime ModifiedDateTime { get; set; } = DateTime.Now;
}