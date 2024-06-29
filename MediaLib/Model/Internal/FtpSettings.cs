namespace MediaLib.Model.Internal;

/// <summary>
/// Provides the FTP settings
/// </summary>
internal sealed class FtpSettings
{
    /// <summary>
    /// Gets or sets the name / path of the FTP server
    /// </summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}