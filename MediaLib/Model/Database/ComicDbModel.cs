using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a comic entry
/// </summary>
[Table("Comic", Schema = "dbo")]
public sealed class ComicDbModel : BaseDbModel;