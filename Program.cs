
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameLibraryAnalytics
{
    // ==========================================
    // 1. DATA ENTITY SCHEMAS (POCO Models)
    // ==========================================
  
    [Table("games")]
    public class Game
    {
        [Key]
        [Column("game_id")]
        public int GameId { get; set; }

        [Column("title")]
        public string Title { get; set; } = null!;

        [Column("hours_played")]
        public decimal HoursPlayed { get; set; }

        [Column("release_date")]
        public string? ReleaseDate { get; set; }

        [Column("completion_status")]
        public string? CompletionStatus { get; set; }

        public List<Genre> Genres { get; set; } = new();
    }

    [Table("genres")]
    public class Genre
    {
        [Key]
        [Column("genre_id")]
        public int GenreId { get; set; }

        [Column("genre_name")]
        public string GenreName { get; set; } = null!; // Verified PascalCase Property

        public List<Game> Games { get; set; } = new();
    }

    // ==========================================
    // 2. ORM DATABASE CONTEXT BROKER
    // ==========================================
    public class GameLibraryContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=10.0.0.205;Port=5432;Database=game_library;User Id=postgres;Password=dredd4345;";
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Genres)
                .WithMany(g => g.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "games_genres",
                    j => j.HasOne<Genre>().WithMany().HasForeignKey("genre_id"),
                    j => j.HasOne<Game>().WithMany().HasForeignKey("game_id")
                );
        }
    }

    // ==========================================
    // 3. MULTI-SEGREGATED PROCESSING PIPELINE
    // ==========================================
    class Program
    {
        private const string RawConnectionString = "Host=10.0.0.205;Port=5432;Username=postgres;Password=dredd4345;Database=game_library";

        static void Main(string[] args)
        {
            Console.WriteLine("🛰️ Initializing Segregated Library Analytics Loops...");

            // Ingress Slicing Engine Execution
            //string telemetryBatch = "Binary Domain-0.1-2-Unplayed|7 Days to Die-0.4-2-Unplayed|8 Eyes-0.0-2-Unplayed";
            // Updated schema tokens: Title-HoursPlayed-ReleaseYear-CompletionStatus
            string telemetryBatch = "Binary Domain-0.1-2012-Unplayed|7 Days to Die-0.4-2013-Unplayed|8 Eyes-0.0-1988-Unplayed";



            Console.WriteLine("\n⚡ RUNNING: Ingress Slicing Engine on Incoming Telemetry Stream...");
            ExecuteTelemetryPipeline(telemetryBatch);

            // Analytics Report Iterations
            using (var db = new GameLibraryContext())
            {
                try
                {
                    // REPORT 1: Absolute Top Utilities / Software Applications
                    Console.WriteLine("\n🧰 REPORT 1: Top Software & Utilities by Operational Hours");
                    var topSoftware = db.Games
                                        .Include(g => g.Genres)
                                        .Where(g => g.Title != "Wallpaper Engine" && 
                                                    g.Genres.Any(genre => genre.GenreName == "utilities" || genre.GenreName == "software"))
                                        .OrderByDescending(g => g.HoursPlayed)
                                        .Take(25)
                                        .ToList();

                    if (!topSoftware.Any())
                    {
                        Console.WriteLine("⚠️ No matching software applications found in the dataset.");
                    }
                    int softRank = 1;
                    foreach (var app in topSoftware)
                    {
                        string year = !string.IsNullOrEmpty(app.ReleaseDate) && app.ReleaseDate.Length >= 4 
                            ? app.ReleaseDate.Substring(0, 4) 
                            : "N/A";

                        Console.WriteLine($"{softRank}. ⚙️ {app.Title} ({year}) — {app.HoursPlayed} hours logged.");
                        softRank++;
                    }

                    // REPORT 2: Absolute Top True Core Entertainment Games
                    Console.WriteLine("\n🎮 REPORT 2: Top 25 True Entertainment Games (Excluding Software)");
                    var topTrueGames = db.Games
                                         .Include(g => g.Genres)
                                         .Where(g => !g.Genres.Any(genre => genre.GenreName == "utilities" || genre.GenreName == "software"))
                                         .OrderByDescending(g => g.HoursPlayed)
                                         .Take(25)
                                         .ToList();

                    int rank = 1;
                    foreach (var game in topTrueGames)
                    {
                        string year = !string.IsNullOrEmpty(game.ReleaseDate) && game.ReleaseDate.Length >= 4 
                            ? game.ReleaseDate.Substring(0, 4) 
                            : "N/A";

                        Console.WriteLine($"{rank}. 👉 {game.Title} ({year}) — {game.HoursPlayed} hours logged.");
                        rank++;
                    }

                    // REPORT 3: Dynamic Backlog Randomizer
                    Console.WriteLine("\n🎲 REPORT 3: Your Next Game to Try (Randomized Unplayed Backlog Selection)");
                    
                    var unplayedBacklog = db.Games
                                            .Include(g => g.Genres)
                                            .Where(g => g.HoursPlayed <= 2 && !g.Genres.Any(genre => genre.GenreName == "utilities" || genre.GenreName == "software"))
                                            .ToList();

                    if (!unplayedBacklog.Any())
                    {
                        Console.WriteLine("⚠️ Absolute backlog cleared! No unplayed entertainment games remaining.");
                    }
                    else
                    {
                        var rand = new Random();
                        int targetIndex = rand.Next(0, unplayedBacklog.Count);
                        var recommendedGame = unplayedBacklog[targetIndex];

                        string recYear = !string.IsNullOrEmpty(recommendedGame.ReleaseDate) && recommendedGame.ReleaseDate.Length >= 4 
                            ? recommendedGame.ReleaseDate.Substring(0, 4) 
                            : "N/A";

                        Console.WriteLine("==========================================================");
                        Console.WriteLine($"✨ SELECTED RECOMMENDATION: {recommendedGame.Title} ({recYear})");
                        Console.WriteLine("==========================================================");
                        Console.WriteLine("💡 Architecture Directive: Fire up your client station and log your first metrics!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Runtime Connection Exception: {ex.Message}");
                }
            }
            Console.WriteLine("\n🏁 Analysis segments complete. Systems resting.");
            Console.ReadLine();
        }

       /* private static void ExecuteTelemetryPipeline(string batchPayload)
        {
            if (string.IsNullOrEmpty(batchPayload)) return;

            ReadOnlySpan<char> payloadSpan = batchPayload.AsSpan();
            int startPosition = 0;

            try
            {
                using var connection = new NpgsqlConnection(RawConnectionString);
                connection.Open();

                while (true)
                {
                    int nextDelimiter = payloadSpan.Slice(startPosition).IndexOf('|');
                    
                    if (nextDelimiter == -1)
                    {
                        ParseAndPersistSegment(payloadSpan.Slice(startPosition), connection);
                        break;
                    }

                    ReadOnlySpan<char> segment = payloadSpan.Slice(startPosition, nextDelimiter);
                    ParseAndPersistSegment(segment, connection);

                    startPosition += nextDelimiter + 1;
                }
                Console.WriteLine("✓ Ingestion Sweep: Clean telemetry data synchronized.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Slicing Engine Pipeline Stoppage: {ex.Message}");
            }
        }*/

        private static void ExecuteTelemetryPipeline(string batchPayload)
        {
            if (string.IsNullOrEmpty(batchPayload)) return;

            ReadOnlySpan<char> payloadSpan = batchPayload.AsSpan();
            int startPosition = 0;

            try
            {
                using var connection = new NpgsqlConnection(RawConnectionString);
                connection.Open();

                // Efficiently slice the payload by segments using ReadOnlySpan delimiters
                for (int i = 0; i <= payloadSpan.Length; i++)
                {
                    if (i == payloadSpan.Length || payloadSpan[i] == '|')
                    {
                        ReadOnlySpan<char> recordSpan = payloadSpan.Slice(startPosition, i - startPosition);
                        startPosition = i + 1;

                        if (recordSpan.IsEmpty) continue;

                        // Parse string positions out with zero allocation heap overhead
                        int firstDash = recordSpan.IndexOf('-');
                        if (firstDash == -1) continue;
                        ReadOnlySpan<char> titleSpan = recordSpan.Slice(0, firstDash);

                        ReadOnlySpan<char> remainder = recordSpan.Slice(firstDash + 1);
                        int secondDash = remainder.IndexOf('-');
                        if (secondDash == -1) continue;
                        ReadOnlySpan<char> hoursSpan = remainder.Slice(0, secondDash);

                        ReadOnlySpan<char> statusSpan = remainder.Slice(secondDash + 1);

                        // Extract data metrics
                        string title = titleSpan.ToString();
                        decimal.TryParse(hoursSpan, out decimal hoursPlayed);
                        string completionStatus = statusSpan.ToString();

                        // BUSINESS VALIDATION LAYER: If hours_played > 0, completion_status is dynamically overridden
                        if (hoursPlayed > 0 && completionStatus.Equals("Unplayed", StringComparison.OrdinalIgnoreCase))
                        {
                            completionStatus = "Playing";
                        }

                        // Execute high-speed parameterized upsert directly over the raw ADO.NET pipe
                        string upsertSql = @"
                            INSERT INTO games (title, hours_played, completion_status) 
                            VALUES (@title, @hours, @status)
                            ON CONFLICT (title) 
                            DO UPDATE SET hours_played = EXCLUDED.hours_played, completion_status = EXCLUDED.completion_status;";

                        using var command = new NpgsqlCommand(upsertSql, connection);
                        command.Parameters.AddWithValue("title", title);
                        command.Parameters.AddWithValue("hours", hoursPlayed);
                        command.Parameters.AddWithValue("status", completionStatus);
                        command.ExecuteNonQuery();

                        Console.WriteLine($"   ✔️ Parsed & Synced: {title} | Hours: {hoursPlayed} | Status: {completionStatus}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ingress Stream Validation Failure: {ex.Message}");
            }
        }
    }
}


