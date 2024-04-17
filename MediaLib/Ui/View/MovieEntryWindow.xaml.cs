using MahApps.Metro.Controls;
using MediaLib.Business;
using MediaLib.Model.Database;
using MediaLib.Ui.ViewModel;
using System.Windows;

namespace MediaLib.Ui.View;

/// <summary>
/// Interaction logic for MovieEntryWindow.xaml
/// </summary>
public partial class MovieEntryWindow : MetroWindow
{
    /// <summary>
    /// The instance for the interaction with the media
    /// </summary>
    private readonly MediaManager _manager;

    /// <summary>
    /// The selected media entry
    /// </summary>
    private readonly MovieDbModel? _movie;

    /// <summary>
    /// Creates a new instance of the <see cref="MovieEntryWindow"/>
    /// </summary>
    /// <param name="manager">The instance for the interaction with the media</param>
    /// <param name="movie">The movie which should be edited</param>
    public MovieEntryWindow(MediaManager manager, MovieDbModel? movie = null)
    {
        InitializeComponent();

        _manager = manager;
        _movie = movie;
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
    /// <param name="sender">The <see cref="MovieEntryWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void MovieEntryWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MovieEntryWindowViewModel viewModel)
            viewModel.InitViewModel(_manager, CloseWindow, _movie);
    }
}