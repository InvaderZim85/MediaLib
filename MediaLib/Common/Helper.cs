using ControlzEx.Theming;
using MediaLib.Common.Enums;
using Serilog;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace MediaLib.Common;

/// <summary>
/// Provides several helper methods
/// </summary>
internal static class Helper
{
    #region Common values

    /// <summary>
    /// The name / path of the SQL server
    /// </summary>
    public const string Server = "(localdb)\\MsSqlLocalDb";

    /// <summary>
    /// The name of the database
    /// </summary>
    public const string Database = "MediaLib";

    /// <summary>
    /// The google search link
    /// </summary>
    public const string GoogleSearchLink = "https://www.google.de/search?q=";

    /// <summary>
    /// Gets or sets the value which indicates if a verbose log should be created
    /// </summary>
    public static bool VerboseLog { get; set; }

    #endregion
    /// <summary>
    /// Init the logger
    /// </summary>
    public static void InitLog()
    {
        // Template
        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        var logEventLevel = VerboseLog ? LogEventLevel.Verbose : LogEventLevel.Information;

        // Init the logger
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logEventLevel) // Set the desired min. log level
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "log", "log_.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: template) // Add the file sink
            .CreateLogger();
    }

    /// <summary>
    /// Sets the color theme (if any is available)
    /// </summary>
    public static void SetColorTheme()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "CustomColor.txt");
        if (!File.Exists(path))
            return;

        var lines = File.ReadAllLines(path);
        var colorCode = lines.FirstOrDefault(f => !f.StartsWith("//") && !string.IsNullOrWhiteSpace(f));
        if (string.IsNullOrWhiteSpace(colorCode))
            return;

        if (ColorConverter.ConvertFromString(colorCode) is not Color color)
            return;

        // Create the new custom theme
        var newTheme = new Theme("AppTheme", "AppTheme", "Dark", color.ToHex(), color, new SolidColorBrush(color), true,
            false);

        // Apply the theme
        ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
    }
    
    /// <summary>
    /// Opens the specified link
    /// </summary>
    /// <param name="url">The url of the link</param>
    public static void OpenLink(string url)
    {
        try
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        catch(Exception ex)
        {
            Log.Warning(ex, "An error has occured while opening the specified url. Url: {url}", url);
        }
    }

    /// <summary>
    /// Opens google and search for the desired title
    /// </summary>
    /// <param name="title">The title</param>
    public static void SearchGoogle(string title)
    {
        title = title.Replace(" ", "%20");
        var url = $"{GoogleSearchLink}{title}";

        OpenLink(url);
    }

    #region Extensions
    /// <summary>
    /// Converts the source into an observable collection
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="source">The source list</param>
    /// <param name="addDefaultEntry">Adds a "default" / "empty" entry to the first position of the list</param>
    /// <returns>The observable collection</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source, bool addDefaultEntry = false) where T : class, new()
    {
        var tmpList = new List<T>();
        if (addDefaultEntry)
            tmpList.Add(new T());

        tmpList.AddRange(source);

        return new ObservableCollection<T>(tmpList);
    }

    /// <summary>
    /// Converts the string value into an int value
    /// </summary>
    /// <param name="value">The string value</param>
    /// <param name="fallback">The fallback</param>
    /// <returns>The int value</returns>
    public static int ToInt(this string value, int fallback = 0)
    {
        return int.TryParse(value, out var result) ? result : fallback;
    }

    /// <summary>
    /// Creates a clone of the entry
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="obj">The object</param>
    /// <returns>The clone</returns>
    public static T? Clone<T>(this T? obj) where T : class
    {
        if (obj == null) 
            return null;

        using var memoryStream = new MemoryStream();
        JsonSerializer.Serialize(memoryStream, obj);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        var val = JsonSerializer.Deserialize<T>(memoryStream);
        return val;
    }

    /// <summary>
    /// Converts the first char to a lower char
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>The converted value</returns>
    public static string FirstCharToLower(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Length switch
        {
            0 => string.Empty,
            1 => value.ToLower(),
            _ => $"{value[0].ToString().ToLower()}{value[1..]}"
        };
    }

    /// <summary>
    /// Converts the color to a HEX value (for example <i>Green</i> > <c>#FF00FF00</c>)
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns>The HEX value of the color</returns>
    public static string ToHex(this Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>
    /// Converts the <see cref="MediaType"/> into a <see cref="KeywordType"/>
    /// </summary>
    /// <param name="mediaType">The desired media type</param>
    /// <returns>The keyword type</returns>
    public static KeywordType ToKeywordType(this MediaType mediaType)
    {
        return mediaType switch
        {
            MediaType.Comic => KeywordType.Comic,
            MediaType.Book => KeywordType.Book,
            MediaType.Movie => KeywordType.Movie,
            MediaType.Music => KeywordType.Music,
            _ => throw new NotSupportedException($"The specified type '{mediaType}' is not supported")
        };
    }
    #endregion
}