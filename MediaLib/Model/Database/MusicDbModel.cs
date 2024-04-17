using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a music entry
/// </summary>
[Table("Music", Schema = "dbo")]
public sealed class MusicDbModel : BaseDbModel;