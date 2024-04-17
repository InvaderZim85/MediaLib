using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaLib.Business;
using MediaLib.Common;
using MediaLib.Common.Enums;
using MediaLib.Model.Database;
using ZimLabs.Mapper;

namespace MediaLib.Ui.ViewModel;

internal partial class MediaEntryWindowViewModel : ViewModelBase
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
    /// The media type
    /// </summary>
    private MediaType _mediaType = MediaType.Comic;

    /// <summary>
    /// The value which indicates if changes available
    /// </summary>
    private bool _hasChanges;

    /// <summary>
    /// Contains the original movie (a copy)
    /// </summary>
    private object? _original;

    /// <summary>
    /// The media entry
    /// </summary>
    private object? _mediaEntry;

    /// <summary>
    /// Gets or sets the window header
    /// </summary>
    [ObservableProperty]
    private string _windowHeader = string.Empty;

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>
    /// Gets or sets the link
    /// </summary>
    [ObservableProperty]
    private string _link = string.Empty;

    /// <summary>
    /// Gets or sets the creation date time
    /// </summary>
    [ObservableProperty]
    private DateTime _creationDateTime = DateTime.Now;

    /// <summary>
    /// Gets or sets the modification date time
    /// </summary>
    [ObservableProperty]
    private DateTime _modifiedDateTime = DateTime.Now;

    /// <summary>
    /// Gets or sets the value which indicates if a new entry should be created
    /// </summary>
    [ObservableProperty]
    private bool _optionCreateNewEntry;

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="manager">The media manager</param>
    /// <param name="closeWindow">The action to close the window</param>
    /// <param name="type">The media type</param>
    /// <param name="entry">The entry which should be edited</param>
    public void InitViewModel(MediaManager manager, Action closeWindow, MediaType type, object? entry)
    {
        _manager = manager;
        _closeWindow = closeWindow;
        _mediaType = type;
        _mediaEntry = entry;
        _original = entry.Clone();

        // Set the data
        SetData();

        WindowHeader = type switch
        {
            MediaType.Comic => "Comic",
            MediaType.Book => "Book",
            MediaType.Music => "Music",
            _ => WindowHeader
        };
    }

    /// <summary>
    /// Sets the data (title, creation / modification date time)
    /// </summary>
    private void SetData()
    {
        switch (_mediaType)
        {
            case MediaType.Comic when _mediaEntry is ComicDbModel comic:
                Title = comic.Title;
                Link = comic.Link;
                CreationDateTime = comic.CreatedDateTime;
                ModifiedDateTime = comic.ModifiedDateTime;
                break;
            case MediaType.Comic when _mediaEntry == null:
                _mediaEntry = new ComicDbModel();
                break;
            case MediaType.Book when _mediaEntry is BookDbModel book:
                Title = book.Title;
                Link = book.Link;
                CreationDateTime = book.CreatedDateTime;
                ModifiedDateTime = book.ModifiedDateTime;
                break;
            case MediaType.Book when _mediaEntry == null:
                _mediaEntry = new BookDbModel();
                break;
            case MediaType.Music when _mediaEntry is MusicDbModel music:
                Title = music.Title;
                Link = music.Link;
                CreationDateTime = music.CreatedDateTime;
                ModifiedDateTime = music.ModifiedDateTime;
                break;
            case MediaType.Music when _mediaEntry == null:
                _mediaEntry = new MusicDbModel();
                break;
        }
    }

    /// <summary>
    /// Gets the title
    /// </summary>
    private void GetTitle()
    {
        switch (_mediaType)
        {
            case MediaType.Comic when _mediaEntry is ComicDbModel comic:
                comic.Title = Title;
                comic.Link = Link;
                break;
            case MediaType.Book when _mediaEntry is BookDbModel book:
                book.Title = Title;
                book.Link = Link;
                break;
            case MediaType.Music when _mediaEntry is MusicDbModel music:
                music.Title = Title;
                music.Link = Link;
                break;
        }
    }

    /// <summary>
    /// Creates a new entry
    /// </summary>
    private void CreateNewEntry()
    {
        _mediaEntry = _mediaType switch
        {
            MediaType.Comic => new ComicDbModel(),
            MediaType.Book => new BookDbModel(),
            MediaType.Music => new MusicDbModel(),
            _ => _mediaEntry
        };
    }

    /// <summary>
    /// Saves the current entry
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveEntry()
    {
        if (_mediaEntry == null || string.IsNullOrEmpty(Title))
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the entry...");
        
        var closeWindow = false;
        try
        {
            GetTitle();

            // Check if the entry is unique
            if (!await _manager!.IsTitleUnique(_mediaType, _mediaEntry))
            {
                await ShowMessageAsync("Error", "The inserted title already exists!");
                return;
            }

            await _manager!.SaveMediaAsync(_mediaType, _mediaEntry);

            if (OptionCreateNewEntry)
            {
                CreateNewEntry();

                SetData();

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
        if (!_hasChanges && _original != null && _mediaEntry != null)
        {
            Mapper.Map(_original, _mediaEntry);
        }

        _closeWindow?.Invoke();
    }
}