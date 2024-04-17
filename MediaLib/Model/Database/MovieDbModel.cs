using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a movie
/// </summary>
[Table("Movie", Schema = "dbo")]
public sealed class MovieDbModel : BaseDbModel
{
    /// <summary>
    /// Gets or sets the medium type id
    /// </summary>
    /// <remarks>
    /// Matches the <see cref="MediumTypeDbModel.Id"/>
    /// </remarks>
    public int MediumTypeId { get; set; }

    /// <summary>
    /// Gets or sets the medium type
    /// </summary>
    [NotMapped]
    public string MediumType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distributor id
    /// </summary>
    /// <remarks>
    /// Matches the <see cref="DistributorDbModel.Id"/>
    /// </remarks>
    public int? DistributorId { get; set; }

    /// <summary>
    /// Gets or sets the distributor
    /// </summary>
    [NotMapped]
    public string Distributor { get; set; } = string.Empty;

}