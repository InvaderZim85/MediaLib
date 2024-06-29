using FluentFTP;
using MediaLib.Model.Internal;
using Serilog;

namespace MediaLib.Business;

internal static class FtpManager
{
    /// <summary>
    /// Uploads a file
    /// </summary>
    /// <param name="filepath">The file which should be uploaded</param>
    /// <param name="settings">The ftp settings</param>
    /// <returns>The awaitable task</returns>
    public static async Task UploadFileAsync(string filepath, FtpSettings settings)
    {
        var token = new CancellationToken();
        await using var ftp = new AsyncFtpClient(settings.Server, settings.Username, settings.Password);
        ftp.Config.EncryptionMode = FtpEncryptionMode.Explicit;
        ftp.Config.ValidateAnyCertificate = true;

        Log.Debug("Connect to ftp server '{server}' with user '{user}'...", settings.Server, settings.Username);

        await ftp.Connect(token);

        Log.Debug("Connection established.");

        // Upload the file
        Log.Debug("Upload file '{file}'...", filepath);

        var result = await ftp.UploadFile(filepath, "/index.html", FtpRemoteExists.Overwrite, true, FtpVerify.Retry, token: token);

        Log.Debug("Upload result: {resul}", result);
    }
}