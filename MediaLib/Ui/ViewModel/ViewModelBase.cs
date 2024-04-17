using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System.Runtime.CompilerServices;
using System.Windows;
using Serilog;

namespace MediaLib.Ui.ViewModel;

/// <summary>
/// Provides the base functions of a view model
/// </summary>
internal class ViewModelBase : ObservableObject
{
    /// <summary>
    /// The different error types
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// General error message
        /// </summary>
        General,

        /// <summary>
        /// Error while loading
        /// </summary>
        Load,

        /// <summary>
        /// Error while saving
        /// </summary>
        Save,

        /// <summary>
        /// Error while deleting
        /// </summary>
        Delete
    }

    /// <summary>
    /// The instance of the mah apps dialog coordinator
    /// </summary>
    private readonly IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Creates a new instance of the <see cref="ViewModelBase"/>
    /// </summary>
    protected ViewModelBase()
    {
        _dialogCoordinator = DialogCoordinator.Instance;
    }

    /// <summary>
    /// Shows a message dialog
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <returns>The awaitable task</returns>
    protected async Task ShowMessageAsync(string title, string message)
    {
        await _dialogCoordinator.ShowMessageAsync(this, title, message);
    }

    /// <summary>
    /// Shows a question dialog with two buttons
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <param name="okButtonText">The text of the ok button (optional)</param>
    /// <param name="cancelButtonText">The text of the cancel button (optional)</param>
    /// <returns>The dialog result</returns>
    protected async Task<MessageDialogResult> ShowQuestionAsync(string title, string message, string okButtonText = "OK",
        string cancelButtonText = "Cancel")
    {
        var result = await _dialogCoordinator.ShowMessageAsync(this, title, message,
            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AffirmativeButtonText = okButtonText,
                NegativeButtonText = cancelButtonText
            });

        return result;
    }

    /// <summary>
    /// Shows an error message and logs the exception
    /// </summary>
    /// <param name="ex">The exception which was thrown</param>
    /// <param name="errorType">The desired message type</param>
    /// <param name="caller">The name of the method, which calls this method. Value will be filled automatically</param>
    /// <returns>The awaitable task</returns>
    protected async Task ShowErrorAsync(Exception ex, ErrorType errorType = ErrorType.General, [CallerMemberName] string caller = "")
    {
        LogError(ex, caller);

        var message = errorType switch
        {
            ErrorType.Load => "An error has occurred while loading the data",
            ErrorType.Save => "An error has occurred while saving the data",
            ErrorType.Delete => "An error has occurred while deleting the data",
            _ => "An error has occurred"
        };

        await _dialogCoordinator.ShowMessageAsync(this, "Error", message);
    }

    /// <summary>
    /// Logs an error
    /// </summary>
    /// <param name="ex">The exception which was thrown</param>
    /// <param name="caller">The name of the method, which calls this method. Value will be filled automatically</param>
    protected static void LogError(Exception ex, [CallerMemberName] string caller = "")
    {
        Log.Error(ex, "An error has occurred. Caller: {caller}", caller);
    }

    /// <summary>
    /// Shows a progress dialog
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <param name="ctSource">The cancellation token source (optional)</param>
    /// <returns>The dialog controller</returns>
    protected async Task<ProgressDialogController> ShowProgressAsync(string title, string message, CancellationTokenSource? ctSource = default)
    {
        var controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, ctSource != null);
        controller.SetIndeterminate();

        if (ctSource != null)
        {
            controller.Canceled += (_, _) => ctSource.Cancel();
        }

        return controller;
    }

    /// <summary>
    /// Copies the content to the clipboard
    /// </summary>
    /// <param name="content">The content which should be copied</param>
    protected static void CopyToClipboard(string content)
    {
        Clipboard.SetText(content);
    }

    /// <summary>
    /// Returns the owner window
    /// </summary>
    /// <returns>The owner window</returns>
    protected static Window? GetOwnerWindow()
    {
        return Application.Current.MainWindow;
    }
}