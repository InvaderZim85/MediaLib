namespace MediaLib.Model.Internal;

/// <summary>
/// Represents a simple id / name entry
/// </summary>
public sealed class IdNameEntry
{
    /// <summary>
    /// Gets the id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the name
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Creates a new, empty entry
    /// </summary>
    public IdNameEntry() { }

    /// <summary>
    /// Creates a new entry
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="name">The name</param>
    public IdNameEntry(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}