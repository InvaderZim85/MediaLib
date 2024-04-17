using System.Windows;
using MahApps.Metro.Controls;
using MediaLib.Business;
using MediaLib.Common.Enums;
using MediaLib.Ui.ViewModel;

namespace MediaLib.Ui.View;

/// <summary>
/// Interaction logic for MediaEntryWindow.xaml
/// </summary>
public partial class MediaEntryWindow : MetroWindow
{
    /// <summary>
    /// The instance for the interaction with the media
    /// </summary>
    private readonly MediaManager _manager;

    /// <summary>
    /// The desired media type
    /// </summary>
    private readonly MediaType _mediaType;

    /// <summary>
    /// The entry which should be edited
    /// </summary>
    private readonly object? _mediaEntry;

    /// <summary>
    /// Creates a new instance of the <see cref="MediaEntryWindow"/>
    /// </summary>
    /// <param name="manager">The instance of the media manager</param>
    /// <param name="type">The media type</param>
    /// <param name="entry">The entry which should be edited</param>
    public MediaEntryWindow(MediaManager manager, MediaType type, object? entry = null)
    {
        InitializeComponent();

        _manager = manager;
        _mediaType = type;
        _mediaEntry = entry;
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    private void CloseWindow()
    {
        Close();
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="MediaEntryWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void MediaEntryWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MediaEntryWindowViewModel viewModel)
            viewModel.InitViewModel(_manager, CloseWindow, _mediaType, _mediaEntry);
    }
}