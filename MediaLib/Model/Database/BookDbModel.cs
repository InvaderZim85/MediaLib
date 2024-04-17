using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLib.Model.Database;

/// <summary>
/// Represents a book
/// </summary>
[Table("Book", Schema = "dbo")]
public sealed class BookDbModel : BaseDbModel;