﻿using MediaLib.Common;
using MediaLib.Model.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MediaLib.Data;

/// <summary>
/// Provides the functions for the interaction with the database
/// </summary>
/// <param name="disableTracking"><see langword="true"/> to disable the tracking, otherwise <see langword="false"/></param>
internal sealed class AppDbContext(bool disableTracking = false) : DbContext
{
    /// <summary>
    /// Gets or sets the distributor
    /// </summary>
    public DbSet<DistributorDbModel> Distributor => Set<DistributorDbModel>();

    /// <summary>
    /// Gets or sets the medium types
    /// <para />
    /// Example: <i>Amazon, Netflix, etc.</i>
    /// </summary>
    public DbSet<MediumTypeDbModel> MediumTypes => Set<MediumTypeDbModel>();

    /// <summary>
    /// Gets or sets the movies
    /// </summary>
    public DbSet<MovieDbModel> Movies => Set<MovieDbModel>();

    /// <summary>
    /// Gets or sets the comics
    /// </summary>
    public DbSet<ComicDbModel> Comics => Set<ComicDbModel>();

    /// <summary>
    /// Gets or sets the books
    /// </summary>
    public DbSet<BookDbModel> Books => Set<BookDbModel>();

    /// <summary>
    /// Gets or sets the music list
    /// </summary>
    public DbSet<MusicDbModel> Music => Set<MusicDbModel>();

    /// <summary>
    /// Gets or sets the keywords
    /// </summary>
    public DbSet<KeywordDbModel> Keywords => Set<KeywordDbModel>();

    /// <summary>
    /// Gets or sets the settings
    /// </summary>
    public DbSet<SettingsDbModel> Settings => Set<SettingsDbModel>();

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conString = new SqlConnectionStringBuilder
        {
            DataSource = Helper.Server,
            InitialCatalog = Helper.Database,
            TrustServerCertificate = true,
            IntegratedSecurity = true
        }.ConnectionString;

        optionsBuilder.UseSqlServer(conString);

        if (Helper.VerboseLog)
            optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging().LogTo(Log.Verbose);

        if (disableTracking)
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}