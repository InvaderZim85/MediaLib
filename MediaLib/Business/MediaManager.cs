﻿using MediaLib.Common;
using MediaLib.Common.Enums;
using MediaLib.Data;
using MediaLib.Model.Database;
using MediaLib.Model.Internal;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

namespace MediaLib.Business;

/// <summary>
/// Provides the functions for the interaction with the media
/// </summary>
public sealed class MediaManager
{
    /// <summary>
    /// The context for the media
    /// </summary>
    private readonly AppDbContext _context = new();

    #region Base data

    /// <summary>
    /// Gets the list with the medium types
    /// </summary>
    public List<MediumTypeDbModel> MediumTypes { get; private set; } = [];

    /// <summary>
    /// Gets the list with the distributors
    /// </summary>
    public List<DistributorDbModel> Distributors { get; private set; } = [];

    #endregion

    #region Media
    /// <summary>
    /// Gets the list with the movies
    /// </summary>
    public List<MovieDbModel> Movies { get; private set; } = [];

    /// <summary>
    /// Gets the list with the comics
    /// </summary>
    public List<ComicDbModel> Comics { get; private set; } = [];

    /// <summary>
    /// Gets the list with the books
    /// </summary>
    public List<BookDbModel> Books { get; private set; } = [];

    /// <summary>
    /// Gets the list with the music
    /// </summary>
    public List<MusicDbModel> Music { get; private set; } = [];
    #endregion

    /// <summary>
    /// Loads the base data and stores them into <see cref="Distributors"/> <see cref="MediumTypes"/>
    /// </summary>
    /// <returns>The awaitable task</returns>
    public async Task LoadBaseDataAsync()
    {
        Distributors = await _context.Distributor.AsNoTracking().ToListAsync();
        MediumTypes = await _context.MediumTypes.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Loads the available medias
    /// </summary>
    /// <returns>The awaitable task</returns>
    public async Task LoadMediaAsync()
    {
        // Movies
        Movies = await _context.Movies.ToListAsync();

        // Set the medium type / distributor
        foreach (var movie in Movies)
        {
            movie.MediumType = MediumTypes.FirstOrDefault(f => f.Id == movie.MediumTypeId)?.Name ?? string.Empty;
            movie.Distributor = Distributors.FirstOrDefault(f => f.Id == movie.DistributorId)?.Name ?? string.Empty;
        }

        // Comics
        Comics = await _context.Comics.ToListAsync();

        // Books
        Books = await _context.Books.ToListAsync();

        // Music
        Music = await _context.Music.ToListAsync();

        // Load the keywords
        var keywords = await _context.Keywords.AsNoTracking().ToListAsync();

        // Add the keywords to the entries
        var objectIds = keywords.Select(s => s.ObjectId).Distinct().ToList();

        foreach (var objectId in objectIds)
        {
            var types = keywords.Where(w => w.ObjectId == objectId).Select(s => s.KeywordTypeId).Distinct().ToList();

            foreach (var type in types)
            {
                var tmpKeywords = keywords.Where(w => w.KeywordTypeId == type && w.ObjectId == objectId).ToList();

                object? entry = (KeywordType)type switch
                {
                    KeywordType.Book => Books.FirstOrDefault(f => f.Id == objectId),
                    KeywordType.Comic => Comics.FirstOrDefault(f => f.Id == objectId),
                    KeywordType.Movie => Movies.FirstOrDefault(f => f.Id == objectId),
                    KeywordType.Music => Music.FirstOrDefault(f => f.Id == objectId),
                    _ => null
                };

                if (entry is not BaseDbModel model)
                    continue;

                model.Keywords = string.Join(", ", tmpKeywords.Select(s => s.Value));
            }
        }
    }

    #region Save / Delete

    /// <summary>
    /// Saves the desired entry
    /// </summary>
    /// <param name="type">The type of the entry</param>
    /// <param name="media">The entry</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is not supported</exception>
    public Task SaveMediaAsync(MediaType type, object media)
    {
        return type switch
        {
            MediaType.Comic when media is ComicDbModel comic => SaveEntryAsync(type, _context.Comics, comic,
                entry => entry.Id == 0, Comics),
            MediaType.Book when media is BookDbModel book => SaveEntryAsync(type, _context.Books, book,
                entry => entry.Id == 0, Books),
            MediaType.Movie when media is MovieDbModel movie => SaveEntryAsync(type, _context.Movies, movie,
                entry => entry.Id == 0, Movies),
            MediaType.Music when media is MusicDbModel music => SaveEntryAsync(type, _context.Music, music,
                entry => entry.Id == 0, Music),
            _ => throw new NotSupportedException($"The specified type '{type}' is not supported.")
        };
    }

    /// <summary>
    /// Deletes the desired entry
    /// </summary>
    /// <param name="type">The type of the entry</param>
    /// <param name="media">The entry</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is not supported</exception>
    public Task DeleteMediaAsync(MediaType type, object media)
    {
        return type switch
        {
            MediaType.Comic when media is ComicDbModel comic => DeleteEntryAsync(type, _context.Comics, comic,
                entry => entry.Id == 0, Comics),
            MediaType.Book when media is BookDbModel book => DeleteEntryAsync(type, _context.Books, book,
                entry => entry.Id == 0, Books),
            MediaType.Movie when media is MovieDbModel movie => DeleteEntryAsync(type, _context.Movies, movie,
                entry => entry.Id == 0, Movies),
            MediaType.Music when media is MusicDbModel music => DeleteEntryAsync(type, _context.Music, music,
                entry => entry.Id == 0, Music),
            _ => throw new NotSupportedException($"The specified type '{type}' is not supported.")
        };
    }

    /// <summary>
    /// Saves an entry
    /// </summary>
    /// <typeparam name="T">The type of the entry</typeparam>
    /// <param name="type">The media type</param>
    /// <param name="dbSet">The data set</param>
    /// <param name="entry">The entry which should be saved</param>
    /// <param name="checkIsNew">The function to check if the entry is new or not</param>
    /// <param name="dataList">The data list</param>
    /// <returns>The awaitable task</returns>
    private async Task SaveEntryAsync<T>(MediaType type, DbSet<T> dbSet, T entry, Func<T, bool> checkIsNew, List<T> dataList) where T : class
    {
        if (checkIsNew(entry))
        {
            await dbSet.AddAsync(entry);

            dataList.Add(entry);
        }
        else
        {
            if (_context.Entry(entry).State != EntityState.Unchanged && entry is CreatedModifiedDateTime tmpEntry)
                tmpEntry.ModifiedDateTime = DateTime.Now;
        }

        // Save the changes
        await _context.SaveChangesAsync();

        if (entry is BaseDbModel model)
            await SaveKeywordsAsync(type, model);
    }

    /// <summary>
    /// Saves the keywords
    /// </summary>
    /// <param name="mediaType">The desired media type</param>
    /// <param name="model">The model with the data</param>
    /// <returns>The awaitable task</returns>
    private async Task SaveKeywordsAsync(MediaType mediaType, BaseDbModel model)
    {
        var keywordType = mediaType.ToKeywordType();

        // Split the entries
        var keywordList = model.Keywords.Split(',', StringSplitOptions.TrimEntries);

        // Load all keywords for the specified entry
        var dbKeywords = _context.Keywords
            .Where(w => w.KeywordTypeId == (int)keywordType && w.ObjectId == model.Id)
            .ToList();

        if (keywordList.Length == 0 && dbKeywords.Count == 0)
            return;

        // Add the keywords
        foreach (var keyword in keywordList)
        {
            // Check if the keyword exists in the database
            if (dbKeywords.Select(s => s.Value).Contains(keyword))
                continue; // Value already exists

            // Create a new entry
            await _context.Keywords.AddAsync(new KeywordDbModel
            {
                KeywordTypeId = (int)keywordType,
                ObjectId = model.Id,
                Value = keyword
            });
        }

        // Now remove the db keywords which are not in the list
        foreach (var dbKeyword in dbKeywords.Where(dbKeyword => !keywordList.Contains(dbKeyword.Value)))
        {
            _context.Keywords.Remove(dbKeyword);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entry
    /// </summary>
    /// <typeparam name="T">The type of the entry</typeparam>
    /// <param name="mediaType">The media type</param>
    /// <param name="dbSet">The data set</param>
    /// <param name="entry">The entry which should be deleted</param>
    /// <param name="checkIsNew">The function to check if the entry is new or not</param>
    /// <param name="dataList">The data list</param>
    /// <returns>The awaitable task</returns>
    private async Task DeleteEntryAsync<T>(MediaType mediaType, DbSet<T> dbSet, T entry, Func<T, bool> checkIsNew, List<T> dataList) where T : class
    {
        if (!checkIsNew(entry))
        {
            dbSet.Remove(entry);

            await _context.SaveChangesAsync();
        }

        dataList.Remove(entry);

        // Delete the keywords
        var keywords = await _context.Keywords.Where(w => w.KeywordTypeId == (int)mediaType.ToKeywordType())
            .ToListAsync();

        foreach (var keyword in keywords)
        {
            _context.Keywords.Remove(keyword);
        }

        await _context.SaveChangesAsync();
    }
    #endregion

    #region Csv Import
    /// <summary>
    /// Checks if the desired media is unique
    /// </summary>
    /// <param name="id">The id of the selected entry, if nothing is selected, insert <c>0</c></param>
    /// <param name="title">The title of the media</param>
    /// <param name="mediumTypeId">The id of the medium type</param>
    /// <returns><see langword="true"/> when the entry is unique, otherwise <see langword="false"/></returns>
    public async Task<bool> IsMovieUniqueAsync(int id, string title, int mediumTypeId)
    {
        var exists = await _context.Movies.AnyAsync(a => (id == 0 || a.Id != id) &&
                                                         a.MediumTypeId == mediumTypeId &&
                                                         a.Title.Equals(title));

        return !exists;
    }

    /// <summary>
    /// Checks if the desired title is unique
    /// </summary>
    /// <param name="type">The desired type</param>
    /// <param name="entry">The entry which should be checked</param>
    /// <returns></returns>
    public async Task<bool> IsTitleUnique(MediaType type, object entry)
    {
        return type switch
        {
            MediaType.Comic when entry is ComicDbModel comic => !await EntryExists(_context.Comics, comic),
            MediaType.Book when entry is BookDbModel book => !await EntryExists(_context.Books, book),
            MediaType.Music when entry is MusicDbModel music => !await EntryExists(_context.Music, music),
            _ => false
        };

        Task<bool> EntryExists<T>(DbSet<T> dbSet, T mediaEntry) where T : BaseDbModel
        {
            return dbSet.AnyAsync(a => (mediaEntry.Id == 0 || a.Id != mediaEntry.Id) && a.Title.Equals(mediaEntry.Title));
        }
    }

    /// <summary>
    /// Imports the content of a CSV file
    /// </summary>
    /// <param name="filepath">The path of the file</param>
    /// <returns>The awaitable task</returns>
    public async Task ImportCsvFileAsync(string filepath)
    {
        if (string.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath))
            return;

        var content = await ExtractContentAsync(filepath);

        foreach (var mediaType in Enum.GetValues<MediaType>())
        {
            var tmpList = content.Where(w => w.MediaTypeId == (int)mediaType).ToList();

            foreach (var entry in tmpList)
            {
                if (mediaType == MediaType.Movie)
                {
                    if (!await IsMovieUniqueAsync(0, entry.Title, entry.MediumTypeId))
                        continue;
                }
                else
                {
                    if (!await IsTitleUnique(mediaType, entry.Title))
                        continue;
                }

                object value = mediaType switch
                {
                    MediaType.Comic => new ComicDbModel
                    {
                        Title = entry.Title
                    },
                    MediaType.Book => new BookDbModel
                    {
                        Title = entry.Title
                    },
                    MediaType.Movie => new MovieDbModel
                    {
                        Title = entry.Title,
                        MediumTypeId = entry.MediumTypeId,
                        DistributorId = entry.DistributorId
                    },
                    MediaType.Music => new MusicDbModel
                    {
                        Title = entry.Title
                    },
                    _ => throw new NotSupportedException($"The specified type '{mediaType}' is not supported.")
                };

                // Save the entry
                await SaveMediaAsync(mediaType, value);
            }
        }

        // Reload the media list
        await LoadMediaAsync();
    }

    /// <summary>
    /// Extracts the content of the file
    /// </summary>
    /// <param name="filepath">The path of the file</param>
    /// <returns>The list with the entries</returns>
    private static async Task<List<FileEntry>> ExtractContentAsync(string filepath)
    {
        var content = await File.ReadAllLinesAsync(filepath);

        var result = new List<FileEntry>();

        var count = 0;
        foreach (var line in content)
        {
            if (count == 0)
            {
                count++;
                continue;
            }

            var lineContent = line.Split(';');

            switch (lineContent.Length)
            {
                case < 4:
                    continue;
                case 4:
                    result.Add(new FileEntry(lineContent[0].Trim(), lineContent[1].ToInt(), lineContent[2].ToInt()));
                    break;
            }
        }

        return result;
    }
    #endregion

    #region Html Export
    /// <summary>
    /// Exports the current content as HTML
    /// </summary>
    /// <param name="folder">The target directory</param>
    /// <returns>The path of the created file</returns>
    public async Task<string> ExportHtmlAsync(string folder)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "index.html");
        var template = await File.ReadAllTextAsync(path);

        // Create the body
        foreach (var type in Enum.GetValues<MediaType>())
        {
            var typeName = type.ToString().ToUpper();
            var entry = $"[BODY{typeName}]";
            var countEntry = $"[COUNT{typeName}]";
            var infoEntry = $"[INFOCOUNT{typeName}]";
            template = template
                .Replace(entry, CreateBody(type))
                .Replace(infoEntry, CreateInfoCount(type))
                .Replace(countEntry, type switch
                {
                    MediaType.Comic => Comics.Count.ToString("N0"),
                    MediaType.Book => Books.Count.ToString("N0"),
                    MediaType.Movie => Movies.Count.ToString("N0"),
                    MediaType.Music => Music.Count.ToString("N0"),
                    _ => "0"
                });
            
        }

        template = template.Replace("[UPDATE]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        // Save the template
        var filepath = Path.Combine(folder, "index.html");
        await File.WriteAllTextAsync(filepath, template);

        // Copy the logo
        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
        if (!File.Exists(logoPath))
            return filepath;

        File.Copy(logoPath, Path.Combine(folder, "logo.png"), true);

        return filepath;

        string CreateInfoCount(MediaType type)
        {
            var count = type switch
            {
                MediaType.Comic => Comics.Count,
                MediaType.Book => Books.Count,
                MediaType.Movie => Movies.Count,
                MediaType.Music => Music.Count,
                _ => 0
            };

            return type != MediaType.Music
                ? count <= 1 
                    ? $"{count} {type}" 
                    : $"{count:N0} {type}s"
                : count <= 1
                    ? $"{count} CD"
                    : $"{count:N0} CDs";
        }
    }

    /// <summary>
    /// Creates the body for the HTML export
    /// </summary>
    /// <param name="type">The media type</param>
    /// <returns>The body</returns>
    private string CreateBody(MediaType type)
    {
        var sb = new StringBuilder();

        var count = 1;
        switch (type)
        {
            case MediaType.Comic:
                if (Comics.Count == 0)
                    AddEmptyLine(4);
                else
                    AddContent(Comics.Select(s => (BaseDbModel)s));
                break;
            case MediaType.Book:
                if (Books.Count == 0)
                    AddEmptyLine(4);
                else
                    AddContent(Books.Select(s => (BaseDbModel)s));
                break;
            case MediaType.Movie:
                if (Movies.Count == 0)
                    AddEmptyLine(6);
                else
                {
                    foreach (var entry in Movies.OrderBy(o => o.Title))
                    {
                        var tmpContent =
                            $"""
                             <tr>
                                 <td>{count++}</td>
                                 <td>{CreateTitle(entry)}</td>
                                 <td>{entry.Keywords}</td>
                                 <td>{entry.MediumType}</td>
                                 <td>{entry.Distributor}</td>
                                 <td>{entry.CreatedDateTime:yyyy-MM-dd HH:mm:ss}</td>
                                 <td>{entry.ModifiedDateTime:yyyy-MM-dd HH:mm:ss}</td>
                             </tr>
                             """;

                        sb.AppendLine(tmpContent);
                    }
                }
                break;
            case MediaType.Music:
                if (Music.Count == 0)
                    AddEmptyLine(4);
                else
                    AddContent(Music.Select(s => (BaseDbModel)s));
                break;
            default:
                return string.Empty;
        }

        return sb.ToString();

        void AddEmptyLine(int columnCount)
        {
            var tmpContent = 
                $"""
                 <tr>
                    <td colspan='{columnCount}'>No data available</td>
                 </tr>
                 """;

            sb.AppendLine(tmpContent);
        }

        void AddContent(IEnumerable<BaseDbModel> entries)
        {
            foreach (var entry in entries.OrderBy(o => o.Title))
            {
                var tmpContent =
                    $"""
                     <tr>
                         <td>{count++}</td>
                         <td>{CreateTitle(entry)}</td>
                         <td>{entry.Keywords}</td>
                         <td>{entry.CreatedDateTime:yyyy-MM-dd HH:mm:ss}</td>
                         <td>{entry.ModifiedDateTime:yyyy-MM-dd HH:mm:ss}</td>
                     </tr>
                     """;

                sb.AppendLine(tmpContent);
            }
        }

        string CreateTitle(BaseDbModel entry)
        {
            return string.IsNullOrWhiteSpace(entry.Link)
                ? entry.Title
                : $"<a href='{entry.Link}' alt='{entry.Title}' target='_blank'>{entry.Title}</a>";
        }
    }
    #endregion
}