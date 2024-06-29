using System.Windows;
using MahApps.Metro.Controls;
using MediaLib.Ui.ViewModel;

namespace MediaLib.Ui.View;

/// <summary>
/// Interaction logic for FtpSettingsWindow.xaml
/// </summary>
public partial class FtpSettingsWindow : MetroWindow
{
    /// <summary>
    /// Creates a new instance of the <see cref="FtpSettingsWindow"/>
    /// </summary>
    public FtpSettingsWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets the password of the <see cref="PwBox"/>
    /// </summary>
    /// <returns>The password</returns>
    private string GetPassword()
    {
        return PwBox.Password;
    }

    /// <summary>
    /// Sets the password of the <see cref="PwBox"/>
    /// </summary>
    /// <param name="value">The password</param>
    private void SetPassword(string value)
    {
        PwBox.Password = value;
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="FtpSettingsWindow"/></param>
    /// <param name="e">The event arguments</param>
    private async void FtpSettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is FtpSettingsWindowViewModel viewModel)
            await viewModel.InitViewModel(SetPassword, GetPassword);
    }

    /// <summary>
    /// Occurs when the user hits the close button (bottom right)
    /// </summary>
    /// <param name="sender">The close button</param>
    /// <param name="e">The event arguments</param>
    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}