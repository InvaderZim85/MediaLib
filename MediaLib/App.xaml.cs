using System.Windows;
using MediaLib.Common;

namespace MediaLib;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Occurs when the program starts
    /// </summary>
    /// <param name="sender">The <see cref="App"/></param>
    /// <param name="e">The event arguments</param>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        if (e.Args.Length > 0 && e.Args.Contains("verbose", StringComparer.OrdinalIgnoreCase))
            Helper.VerboseLog = true;

        // Init the logger
        Helper.InitLog();

        // Set the color theme
        Helper.SetColorTheme();
    }
}