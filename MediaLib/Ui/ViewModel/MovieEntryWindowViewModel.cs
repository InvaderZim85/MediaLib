using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaLib.Business;
using MediaLib.Common;
using MediaLib.Model.Database;
using System.Collections.ObjectModel;
using MediaLib.Common.Enums;
using ZimLabs.Mapper;

namespace MediaLib.Ui.ViewModel;

internal partial class MovieEntryWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The instance for the interaction with the movie
    /// </summary>
    private MediaManager? _manager;

    /// <summary>
    /// The action to close the window
    /// </summary>
    private Action? _closeWindow;

    /// <summary>
    /// The value which indicates if changes available
    /// </summary>
    private bool _hasChanges;

    /// <summary>
    /// Contains the original movie (a copy)
    /// </summary>
    private MovieDbModel? _original;

    /// <summary>
    /// Gets or sets the movie entry
    /// </summary>
    [ObservableProperty]
    private MovieDbModel _movie = new();

    /// <summary>
    /// Gets or sets the list with the available medium types
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<MediumTypeDbModel> _mediumTypes = [];

    /// <summary>
    /// Gets or sets the selected medium type
    /// </summary>
    [ObservableProperty]
    private MediumTypeDbModel? _selectedMediumType;

    /// <summary>
    /// Gets or sets the list with the available distributors
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<DistributorDbModel> _distributors = [];

    /// <summary>
    /// Gets the selected distributor
    /// </summary>
    [ObservableProperty]
    private DistributorDbModel? _selectedDistributor;

    /// <summary>
    /// Gets or sets the value which indicates if a new entry should be created
    /// </summary>
    [ObservableProperty]
    private bool _createNewEntry;

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="manager">The media manager</param>
    /// <param name="closeWindow">The action to close the window</param>
    /// <param name="movie">The entry which should be edited</param>
    public void InitViewModel(MediaManager manager, Action closeWindow, MovieDbModel? movie)
    {
        _manager = manager;
        _closeWindow = closeWindow;
        Movie = movie ?? new MovieDbModel();
        _original = movie.Clone();

        MediumTypes = manager.MediumTypes.ToObservableCollection();
        SelectedMediumType = movie == null
            ? MediumTypes.FirstOrDefault()
            : MediumTypes.FirstOrDefault(f => f.Id == movie.MediumTypeId);

        Distributors = manager.Distributors.ToObservableCollection(true);
        SelectedDistributor = movie == null
            ? null
            : Distributors.FirstOrDefault(f => f.Id == movie.DistributorId);
    }

    /// <summary>
    /// Saves the current entry
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveEntry()
    {
        if (string.IsNullOrWhiteSpace(Movie.Title) || SelectedMediumType == null)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the entry...");

        Movie.MediumType = SelectedMediumType.Name;
        Movie.MediumTypeId = SelectedMediumType.Id;

        Movie.Distributor = SelectedDistributor?.Name ?? string.Empty;
        Movie.DistributorId = SelectedDistributor?.Id;
        if (Movie.DistributorId == 0)
            Movie.DistributorId = null; // Remove the distributor id!

        var closeWindow = false;
        try
        {
            // Check if the entered values are "unique"
            if (!await _manager!.IsMovieUniqueAsync(Movie.Id, Movie.Title, Movie.MediumTypeId))
            {
                await ShowMessageAsync("Error", "The inserted title already exists with the selected medium type!");
                return;
            }

            await _manager!.SaveMediaAsync(MediaType.Movie, Movie);

            if (CreateNewEntry)
            {
                // Create a new movie
                Movie = new MovieDbModel();

                // Clear the selection
                SelectedMediumType = MediumTypes.FirstOrDefault();
                SelectedDistributor = null;
                _hasChanges = true;
                return;
            }

            closeWindow = true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }

        if (closeWindow)
            _closeWindow?.Invoke();
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    [RelayCommand]
    private void CloseWindow()
    {
        if (!_hasChanges && _original != null)
        {
            Mapper.Map(_original, Movie);
        }

        _closeWindow?.Invoke();
    }
}