using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaLib.Business;
using MediaLib.Common.Enums;
using MediaLib.Model.Internal;

namespace MediaLib.Ui.ViewModel;

/// <summary>
/// Provides the functions for <see cref="View.FtpSettingsWindow"/>
/// </summary>
internal partial class FtpSettingsWindowViewModel : ViewModelBase
{
    /// <summary>
    /// THe action to set the password
    /// </summary>
    private Action<string>? _setPassword;

    /// <summary>
    /// The function to get the currently set password
    /// </summary>
    private Func<string>? _getPassword;

    /// <summary>
    /// Gets or sets the FTP settings
    /// </summary>
    [ObservableProperty]
    private FtpSettings _settings = new();

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="setPassword">The action to set the password</param>
    /// <param name="getPassword">The function to get the password</param>
    /// <returns>The awaitable task</returns>
    public async Task InitViewModel(Action<string> setPassword, Func<string> getPassword)
    {
        _setPassword = setPassword;
        _getPassword = getPassword;

        await LoadFtpSettingsAsync();
    }

    /// <summary>
    /// Loads the FTP settings
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task LoadFtpSettingsAsync()
    {
        var controller = await ShowProgressAsync("Please wait", "Please wait while loading the settings...");

        try
        {
            Settings = new FtpSettings
            {
                Server = await SettingsManager.LoadValueAsync(SettingsKey.FtpServer, string.Empty),
                Username = await SettingsManager.LoadValueAsync(SettingsKey.FtpUser, string.Empty),
                Password = await SettingsManager.LoadValueAsync(SettingsKey.FtpPassword, string.Empty)
            };

            // Set the password
            _setPassword?.Invoke(Settings.Password);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Load);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Saves the FTP settings
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveFtpSettingsAsync()
    {
        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the settings...");

        try
        {
            // Get the password
            Settings.Password = _getPassword?.Invoke() ?? string.Empty;

            // Convert the values into a list
            var valueList = new SortedList<SettingsKey, object>
            {
                { SettingsKey.FtpServer, Settings.Server },
                { SettingsKey.FtpUser, Settings.Username },
                { SettingsKey.FtpPassword, Settings.Password }
            };

            // Save the values
            await SettingsManager.SaveValuesAsync(valueList);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Load);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }
}