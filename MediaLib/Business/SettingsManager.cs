using MediaLib.Common.Enums;
using MediaLib.Data;
using MediaLib.Model.Database;
using Serilog;

namespace MediaLib.Business;

/// <summary>
/// Provides the functions for the interaction with the settings
/// </summary>
internal static class SettingsManager
{
    /// <summary>
    /// Loads the value of the desired key
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The desired key</param>
    /// <param name="fallback">The fallback value (will be returned when the value is null, empty or the conversion failed)</param>
    /// <returns>The value</returns>
    public static async Task<T> LoadValueAsync<T>(SettingsKey key, T fallback)
    {
        await using var context = CreateContext(true);

        var value = context.Settings.FirstOrDefault(f => f.Key == (int)key);
        if (value == null)
            return fallback;

        if (string.IsNullOrEmpty(value.Value))
            return fallback;

        try
        {
            return (T)Convert.ChangeType(value.Value, typeof(T));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error has occurred while changing the type of the value '{value}' into '{type}'.", value.Value, nameof(T));
            return fallback;
        }
    }

    /// <summary>
    /// Saves a value
    /// </summary>
    /// <param name="key">The key of the value</param>
    /// <param name="value">The value which should be saved</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsync(SettingsKey key, object value)
    {
        var tmpValue = value.ToString() ?? string.Empty;
        await using var context = CreateContext(false);

        var entry = context.Settings.FirstOrDefault(f => f.Key == (int)key);
        if (entry == null)
        {
            await context.Settings.AddAsync(new SettingsDbModel
            {
                Key = (int)key,
                Value = tmpValue,
                Description = $"Value of key '{key}'"
            });

            await context.SaveChangesAsync();
        }
        else if (!entry.Value.Equals(tmpValue))
        {
            entry.Value = tmpValue;
            entry.ModifiedDateTime = DateTime.Now;

            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Saves a list of values
    /// </summary>
    /// <param name="values">The list with the values</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValuesAsync(SortedList<SettingsKey, object> values)
    {
        await using var context = CreateContext(false);

        var keyIds = values.Keys.Select(s => (int)s).ToList();
        var dbValues = context.Settings.Where(w => keyIds.Contains(w.Key)).ToList();

        var saveChanges = false;
        foreach (var value in values)
        {
            var tmpValue = value.Value.ToString() ?? string.Empty;
            var dbValue = dbValues.FirstOrDefault(f => f.Key == (int)value.Key);
            if (dbValue == null)
            {
                await context.Settings.AddAsync(new SettingsDbModel
                {
                    Key = (int)value.Key,
                    Value = tmpValue,
                    Description = $"Value of key '{value.Key}'"
                });

                saveChanges = true;
            }
            else if (!dbValue.Value.Equals(tmpValue))
            {
                dbValue.Value = tmpValue;
                dbValue.ModifiedDateTime = DateTime.Now;

                saveChanges = true;
            }
        }

        if (saveChanges)
            await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="AppDbContext"/>
    /// </summary>
    /// <param name="disableTracking"><see langword="true"/> to disable the tracking, otherwise <see langword="false"/></param>
    /// <returns>The instance of the context</returns>
    private static AppDbContext CreateContext(bool disableTracking)
    {
        return new AppDbContext(disableTracking);
    }
}