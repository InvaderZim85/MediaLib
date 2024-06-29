using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MediaLib.Business;
using MediaLib.Common;
using MediaLib.Common.Enums;
using MediaLib.Model.Database;
using MediaLib.Ui.View;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using MediaLib.Model.Internal;

namespace MediaLib.Ui.ViewModel;

/// <summary>
/// Provides the functions for the interaction with the <see cref="View.MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Instance for the interaction with the media
    /// </summary>
    private readonly MediaManager _manager = new();

    #region Properties

    /// <summary>
    /// Gets or sets the selected tab index
    /// </summary>
    [ObservableProperty]
    private int _selectedTab;

    /// <summary>
    /// Gets or sets the info
    /// </summary>
    [ObservableProperty]
    private string _info = "Disconnected";

    /// <summary>
    /// Gets or sets the window header
    /// </summary>
    [ObservableProperty]
    private string _windowHeader = "MediaLib - Movie";

    #region Options

    /// <summary>
    /// Gets or sets the value which indicates whether the delete function is enabled
    /// </summary>
    [ObservableProperty]
    private bool _funcDeleteEnabled;

    /// <summary>
    /// Gets or sets the value which indicates whether the open link function is enabled
    /// </summary>
    [ObservableProperty]
    private bool _funcOpenLinkEnabled;

    /// <summary>
    /// Gets or sets the value which indicates whether the edit function is enabled
    /// </summary>
    [ObservableProperty]
    private bool _funcEditEnabled;

    #endregion

    #region Lists

    /// <summary>
    /// Gets or sets the movie list
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<MovieDbModel> _movies = [];

    /// <summary>
    /// Gets or sets the selected movie
    /// </summary>
    [ObservableProperty]
    private MovieDbModel? _selectedMovie;

    /// <summary>
    /// Gets or sets the comic list
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ComicDbModel> _comics = [];

    /// <summary>
    /// Gets or sets the selected comic
    /// </summary>
    [ObservableProperty]
    private ComicDbModel? _selectedComic;

    /// <summary>
    /// Gets or sets the book list
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<BookDbModel> _books = [];

    /// <summary>
    /// Gets or sets the selected book
    /// </summary>
    [ObservableProperty]
    private BookDbModel? _selectedBook;

    /// <summary>
    /// Gets or sets the music list
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<MusicDbModel> _music = [];

    /// <summary>
    /// Gets or sets the selected music
    /// </summary>
    [ObservableProperty]
    private MusicDbModel? _selectedMusic;
    #endregion

    #region Filter

    /// <summary>
    /// Gets or sets the movie filter
    /// </summary>
    [ObservableProperty]
    private string _filterMovie = string.Empty;

    /// <summary>
    /// Gets or sets the comic filter
    /// </summary>
    [ObservableProperty]
    private string _filterComic = string.Empty;

    /// <summary>
    /// Gets or sets the book filter
    /// </summary>
    [ObservableProperty]
    private string _filterBook = string.Empty;

    /// <summary>
    /// Gets or sets the music filter
    /// </summary>
    [ObservableProperty]
    private string _filterMusic = string.Empty;
    #endregion

    #region List info

    /// <summary>
    /// Gets or sets the info about the movie list
    /// </summary>
    [ObservableProperty]
    private string _infoMovie = "Movies";

    /// <summary>
    /// Gets or sets the info about the comic list
    /// </summary>
    [ObservableProperty]
    private string _infoComic = "Comics";

    /// <summary>
    /// Gets or sets the info about the book list
    /// </summary>
    [ObservableProperty]
    private string _infoBook = "Books";

    /// <summary>
    /// Gets or sets the info about the music list
    /// </summary>
    [ObservableProperty]
    private string _infoMusic = "Music";
    #endregion

    #endregion

    /// <summary>
    /// Gets the current tab type
    /// </summary>
    private TabType CurrentTabType => (TabType)SelectedTab;

    #region Change methods
    /// <summary>
    /// Occurs when the user selected another tab
    /// </summary>
    /// <param name="value">The index of the selected tab</param>
    partial void OnSelectedTabChanged(int value)
    {
        var tabType = (TabType)value;

        var selectedTab = tabType switch
        {
            TabType.Movie => "Movie",
            TabType.Comic => "Comic",
            TabType.Book => "Book",
            TabType.Music => "Music",
            _ => string.Empty
        };

        WindowHeader = $"MediaLib{(string.IsNullOrEmpty(selectedTab) ? "" : $" - {selectedTab}")}";
        
        SetFunctionState(tabType, tabType switch
        {
            TabType.Movie => SelectedMovie,
            TabType.Comic => SelectedComic,
            TabType.Book => SelectedBook,
            TabType.Music => SelectedMusic,
            _ => null
        });
    }

    /// <summary>
    /// Occurs when the user selects another movie
    /// </summary>
    /// <param name="value">The selected movie</param>
    partial void OnSelectedMovieChanged(MovieDbModel? value)
    {
        SetFunctionState(TabType.Movie, value);
    }

    /// <summary>
    /// Occurs when the user selects another comic
    /// </summary>
    /// <param name="value">The selected comic</param>
    partial void OnSelectedComicChanged(ComicDbModel? value)
    {
        SetFunctionState(TabType.Comic, value);
    }

    /// <summary>
    /// Occurs when the user selects another book
    /// </summary>
    /// <param name="value">The selected book</param>
    partial void OnSelectedBookChanged(BookDbModel? value)
    {
        SetFunctionState(TabType.Book, value);
    }

    /// <summary>
    /// Occurs when the user selects another music
    /// </summary>
    /// <param name="value">The selected music</param>
    partial void OnSelectedMusicChanged(MusicDbModel? value)
    {
        SetFunctionState(TabType.Music, value);
    }

    /// <summary>
    /// Occurs when the user changes the movie filter
    /// </summary>
    /// <param name="value">The filter</param>
    partial void OnFilterMovieChanged(string value)
    {
        Movies = FilterList(_manager.Movies, value);
        SetListInfo(MediaType.Movie);
    }

    /// <summary>
    /// Occurs when the user changes the comic filter
    /// </summary>
    /// <param name="value">The filter</param>
    partial void OnFilterComicChanged(string value)
    {
        Comics = FilterList(_manager.Comics, value);
        SetListInfo(MediaType.Comic);
    }

    /// <summary>
    /// Occurs when the user changes the book filter
    /// </summary>
    /// <param name="value">The filter</param>
    partial void OnFilterBookChanged(string value)
    {
        Books = FilterList(_manager.Books, value);
        SetListInfo(MediaType.Book);
    }

    /// <summary>
    /// Occurs when the user changes the music filter
    /// </summary>
    /// <param name="value">The filter</param>
    partial void OnFilterMusicChanged(string value)
    {
        Music = FilterList(_manager.Music, value);
        SetListInfo(MediaType.Music);
    }

    /// <summary>
    /// Filters the list
    /// </summary>
    /// <typeparam name="T">The type of the entry</typeparam>
    /// <param name="list">The list</param>
    /// <param name="filter">The filter</param>
    /// <returns>The observable collection</returns>
    private static ObservableCollection<T> FilterList<T>(IEnumerable<T> list, string filter) where T : BaseDbModel, new()
    {
        return list
            .Where(w => string.IsNullOrWhiteSpace(filter) ||
                        w.Title.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                        w.Keywords.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(o => o.Title)
            .ToObservableCollection();
    }

    /// <summary>
    /// Filters the movie list
    /// </summary>
    /// <param name="list">The movie list</param>
    /// <param name="filter">The desired filter</param>
    /// <returns>The observable collection</returns>
    private static ObservableCollection<MovieDbModel> FilterList(IEnumerable<MovieDbModel> list, string filter)
    {
        return list.Where(w => string.IsNullOrWhiteSpace(filter) ||
                               w.Title.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                               w.Keywords.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                               w.MediumType.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                               w.Distributor.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(o => o.Title)
            .ToObservableCollection();
    }

    /// <summary>
    /// Sets the function state (enabled / disable)
    /// </summary>
    /// <remarks>
    /// Set the value of <see cref="FuncEditEnabled"/>, <see cref="FuncDeleteEnabled"/> and <see cref="FuncOpenLinkEnabled"/>
    /// </remarks>
    /// <param name="type">The desired type</param>
    /// <param name="value">The selected value</param>
    private void SetFunctionState(TabType type, BaseDbModel? value)
    {
        FuncEditEnabled = SelectedTab == (int)type && value != null;
        FuncDeleteEnabled = SelectedTab == (int)type && value != null;
        FuncOpenLinkEnabled = SelectedTab == (int)type && value != null && !string.IsNullOrWhiteSpace(value.Link);
    }

    #endregion

    /// <summary>
    /// Init the view model
    /// </summary>
    public async void InitViewModel()
    {
        Info = $"Server: {Helper.Server} | Database: {Helper.Database} | Version: {Assembly.GetExecutingAssembly().GetName().Version}";

        var controller = await ShowProgressAsync("Please wait", "Please wait while loading the data...");

        try
        {
            // Load the base data
            await _manager.LoadBaseDataAsync();

            // Load the media
            await _manager.LoadMediaAsync();

            // Set the media
            SetMediaList();
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

    #region Add / Edit / Delete

    /// <summary>
    /// Adds a new entry
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private void AddNewEntry()
    {
        var tabType = CurrentTabType;

        if (tabType == TabType.Movie)
        {
            var movieDialog = new MovieEntryWindow(_manager)
            {
                Owner = GetOwnerWindow()
            };

            movieDialog.ShowDialog();

            SetMediaList();
        }
        else
        {
            var mediaType = tabType switch
            {
                TabType.Comic => MediaType.Comic,
                TabType.Book => MediaType.Book,
                TabType.Music => MediaType.Music,
                _ => throw new NotSupportedException($"The specified type '{tabType}' is not supported.")
            };

            var mediaDialog = new MediaEntryWindow(_manager, mediaType)
            {
                Owner = GetOwnerWindow()
            };

            mediaDialog.ShowDialog();

            SetMediaList();
        }
    }

    /// <summary>
    /// Edits the selected entry
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private void EditEntry()
    {
        var tabType = CurrentTabType;

        if (tabType == TabType.Movie)
        {
            var movieDialog = new MovieEntryWindow(_manager, SelectedMovie)
            {
                Owner = GetOwnerWindow()
            };

            movieDialog.ShowDialog();

            SetMediaList();
        }
        else
        {
            var mediaType = tabType switch
            {
                TabType.Comic => MediaType.Comic,
                TabType.Book => MediaType.Book,
                TabType.Music => MediaType.Music,
                _ => throw new NotSupportedException($"The specified type '{tabType}' is not supported.")
            };

            var mediaDialog = new MediaEntryWindow(_manager, mediaType, tabType switch
            {
                TabType.Comic => SelectedComic,
                TabType.Book => SelectedBook,
                TabType.Music => SelectedMusic,
                _ => throw new NotSupportedException($"The specified type '{tabType}' is not supported.")
            })
            {
                Owner = GetOwnerWindow()
            };

            mediaDialog.ShowDialog();

            SetMediaList();
        }
    }

    /// <summary>
    /// Deletes the selected entry
    /// </summary>
    /// <returns>The entry which should be deleted</returns>
    [RelayCommand]
    private async Task DeleteEntryAsync()
    {
        try
        {
            switch (CurrentTabType)
            {
                case TabType.Movie when SelectedMovie != null:
                    if (!await DeleteEntry(SelectedMovie.Title))
                        return;

                    await _manager.DeleteMediaAsync(MediaType.Movie, SelectedMovie);
                    SelectedMovie = null;

                    SetMediaList();
                    break;
                case TabType.Comic when SelectedComic != null:
                    if (!await DeleteEntry(SelectedComic.Title))
                        return;

                    await _manager.DeleteMediaAsync(MediaType.Comic, SelectedComic);
                    SelectedComic = null;

                    SetMediaList();
                    break;
                case TabType.Book when SelectedBook != null:
                    if (!await DeleteEntry(SelectedBook.Title))
                        return;

                    await _manager.DeleteMediaAsync(MediaType.Book, SelectedBook);
                    SelectedBook = null;

                    SetMediaList();
                    break;
                case TabType.Music when SelectedMusic != null:
                    if (!await DeleteEntry(SelectedMusic.Title))
                        return;

                    await _manager.DeleteMediaAsync(MediaType.Music, SelectedMusic);
                    SelectedMusic = null;

                    SetMediaList();
                    break;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Delete);
        }

        return;

        async Task<bool> DeleteEntry(string name)
        {
            var result = await ShowQuestionAsync("Delete", $"Do you really want to delete the entry '{name}'?");
            return result == MessageDialogResult.Affirmative;
        }
    }

    #endregion

    #region Import / Export

    /// <summary>
    /// Imports the content of a csv file
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task ImportCsvFileAsync()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "CSV file (*.csv)|*.csv"
        };

        if (dialog.ShowDialog() != true)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while importing the data...");

        try
        {
            await _manager.ImportCsvFileAsync(dialog.FileName);

            SetMediaList();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Exports the current content as HTML
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task ExportHtmlAsync()
    {
        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() != true) 
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while exporting the data...");

        try
        {
            await _manager.ExportHtmlAsync(dialog.FolderName);

            await ShowMessageAsync("HTML export", "Data successfully exported.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Exports the current content as HTML and uploads it to the desired FTP server
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task ExportHtmlFtpAsync()
    {
        
        var controller = await ShowProgressAsync("Please wait", "Please wait while exporting / uploading the data...");

        try
        {
            // Export the file (into a temp. folder)
            var tempDir = Path.GetTempPath();
            // Create the dir if it not exists
            Directory.CreateDirectory(tempDir);
            // Create the file
            var path = await _manager.ExportHtmlAsync(tempDir);
            // Upload the file
            var settings = new FtpSettings
            {
                Server = await SettingsManager.LoadValueAsync(SettingsKey.FtpServer, string.Empty),
                Username = await SettingsManager.LoadValueAsync(SettingsKey.FtpUser, string.Empty),
                Password = await SettingsManager.LoadValueAsync(SettingsKey.FtpPassword, string.Empty)
            };

            if (string.IsNullOrEmpty(settings.Server))
            {
                await ShowMessageAsync("FTP Upload",
                    "The ftp settings are missing! Insert the needed settings (File > FTP Settings) and try again!");
                return;
            }

            // Upload the file
            await FtpManager.UploadFileAsync(path, settings);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    #endregion

    #region Various

    /// <summary>
    /// Opens the underlying link (if any is available)
    /// </summary>
    [RelayCommand]
    private void OpenLink()
    {
        var link = CurrentTabType switch
        {
            TabType.Movie => SelectedMovie?.Link ?? string.Empty,
            TabType.Comic => SelectedComic?.Link ?? string.Empty,
            TabType.Book => SelectedBook?.Link ?? string.Empty,
            TabType.Music => SelectedMusic?.Link ?? string.Empty,
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(link))
            return;

        Helper.OpenLink(link);
    }

    /// <summary>
    /// Opens google and search for the title
    /// </summary>
    [RelayCommand]
    private void OpenGoogle()
    {
        var title = CurrentTabType switch
        {
            TabType.Movie => SelectedMovie?.Title ?? string.Empty,
            TabType.Comic => SelectedComic?.Title ?? string.Empty,
            TabType.Book => SelectedBook?.Title ?? string.Empty,
            TabType.Music => SelectedMusic?.Title ?? string.Empty,
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(title))
            return;

        Helper.SearchGoogle(title);
    }

    /// <summary>
    /// Shows the FTP settings dialog
    /// </summary>
    [RelayCommand]
    private void ShowFtpSettings()
    {
        var ftpDialog = new FtpSettingsWindow()
        {
            Owner = GetOwnerWindow()
        };

        ftpDialog.ShowDialog();
    }

    /// <summary>
    /// Sets the media list
    /// </summary>
    private void SetMediaList()
    {
        Movies = FilterList(_manager.Movies, FilterMovie);
        SetListInfo(MediaType.Movie);

        Comics = FilterList(_manager.Comics, FilterComic);
        SetListInfo(MediaType.Comic);

        Books = FilterList(_manager.Books, FilterBook);
        SetListInfo(MediaType.Book);

        Music = FilterList(_manager.Music, FilterMusic);
        SetListInfo(MediaType.Music);
    }

    /// <summary>
    /// Sets the list info of the desired type
    /// </summary>
    /// <param name="type">The type</param>
    private void SetListInfo(MediaType type)
    {
        var count = type switch
        {
            MediaType.Comic => Comics.Count,
            MediaType.Book => Books.Count,
            MediaType.Movie => Movies.Count,
            MediaType.Music => Music.Count,
            _ => 0
        };

        var value = count switch
        {
            0 => $"{type}s",
            1 => $"1 {type.ToString().FirstCharToLower()}",
            _ => $"{count} {type.ToString().FirstCharToLower()}s"
        };

        switch (type)
        {
            case MediaType.Comic:
                InfoComic = value; 
                break;
            case MediaType.Book:
                InfoBook = value;
                break;
            case MediaType.Movie:
                InfoMovie = value;
                break;
            case MediaType.Music:
                InfoMusic = value;
                break;
        }
    }

    #endregion
}