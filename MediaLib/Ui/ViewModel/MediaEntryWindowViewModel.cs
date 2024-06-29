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
    /// Gets or sets the keywords
    /// </summary>
    [ObservableProperty]
    private string _keywords = string.Empty;

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
        if (_mediaEntry is BaseDbModel baseModel)
        {
            Title = baseModel.Title;
            Link = baseModel.Link;
            Keywords = baseModel.Keywords;
            CreationDateTime = baseModel.CreatedDateTime;
            ModifiedDateTime = baseModel.ModifiedDateTime;
        }

        _mediaEntry = _mediaType switch
        {
            MediaType.Comic when _mediaEntry == null => new ComicDbModel(),
            MediaType.Book when _mediaEntry == null => new BookDbModel(),
            MediaType.Music when _mediaEntry == null => new MusicDbModel(),
            _ => _mediaEntry
        };
    }

    /// <summary>
    /// Gets the title
    /// </summary>
    private void GetTitle()
    {
        if (_mediaEntry is not BaseDbModel baseModel) 
            return;

        baseModel.Title = Title;
        baseModel.Link = Link;
        baseModel.Keywords = Keywords;
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