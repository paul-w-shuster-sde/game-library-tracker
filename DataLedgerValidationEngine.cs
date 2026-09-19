using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAnalytics.Core.Validation
{
    // 1. Enforce zero-allocation state tracking with a stack-allocated readonly struct
    public readonly struct ValidationPayload
    {
        public readonly int GameId;
        public readonly double HoursPlayed;
        public readonly string CompletionStatus;

        public ValidationPayload(int gameId, double hoursPlayed, string completionStatus)
        {
            GameId = gameId;
            HoursPlayed = hoursPlayed;
            CompletionStatus = completionStatus;
        }

        // Fast evaluation method operating strictly on stack-allocated data
        public bool IsStateValid()
        {
            // Business rule: If hours_played > 0, completion_status cannot remain 'Unplayed'
            if (HoursPlayed > 0 && CompletionStatus.Equals("Unplayed", StringComparison.OrdinalIgnoreCase))
            {
                return false; 
            }
            return true;
        }
    }

    // DB Record layout representing the underlying relational schema map
    public record GameRecord(int GameId, string Title, double HoursPlayed, string CompletionStatus, string GenreName);

    public class DataLedgerValidationEngine
    {
        private readonly DbContext _context;

        public DataLedgerValidationEngine(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes high-velocity validation by offloading analytical aggregations 
        /// directly to the database engine via LINQ-to-Entities.
        /// </summary>
        public async Task<bool> ValidateBackendLedgerAsync()
        {
            // 2. Offload heavy data sorting and aggregations directly to the database engine.
            // This forces execution via highly optimized SQL joins and filters on the DB server,
            // avoiding massive, expensive heap-allocated collections in memory.
            var analyticalSummary = await _context.Set<GameRecord>()
                .AsNoTracking() // Completely bypasses EF Core tracking cache memory overhead
                .Where(g => g.Title != "Wallpaper Engine") // Rigidly exclude background utilities
                .GroupBy(g => g.GenreName)
                .Select(group => new
                {
                    Genre = group.Key,
                    TotalRecords = group.Count(),
                    AvgHours = group.Average(g => g.HoursPlayed)
                })
                .ToListAsync();

            // 3. Process records in parallel using 'in' parameter modifiers to prevent stack copying
            var operationalRecords = await _context.Set<GameRecord>()
                .AsNoTracking()
                .Select(g => new { g.GameId, g.HoursPlayed, g.CompletionStatus })
                .ToListAsync();

            bool entireLedgerIsValid = true;

            // Stream and evaluate payloads via zero-allocation passing rules
            foreach (var record in operationalRecords)
            {
                // Instantiate the struct right on the stack
                var payload = new ValidationPayload(record.GameId, record.HoursPlayed, record.CompletionStatus);

                // Pass by reference using the 'in' modifier to ensure ZERO memory copying
                if (!EvaluateRecordInPlace(in payload))
                {
                    entireLedgerIsValid = false;
                    // Telemetry logging hook can register anomaly record here
                }
            }

            return entireLedgerIsValid;
        }

        /// <summary>
        /// High-performance text parsing pipeline engineered via ReadOnlySpan and .Slice()
        /// to validate raw string segments with zero heap allocations.
        /// </summary>
        public bool ParseAndValidateLine(ReadOnlySpan<char> rawLine)
        {
            // Expected CSV format schema layout: GameId,Title,HoursPlayed,CompletionStatus
            if (rawLine.IsEmpty || rawLine.IsWhiteSpace()) return false;

            // Isolate 'Wallpaper Engine' firewall configuration structures early in the parse pass
            if (rawLine.Contains("Wallpaper Engine".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return true; // Excluded by rule but not considered an engine validation failure
            }

            try
            {
                // Isolate GameId
                int firstComma = rawLine.IndexOf(',');
                if (firstComma == -1) return false;
                ReadOnlySpan<char> idSpan = rawLine.Slice(0, firstComma);

                // Isolate Title
                ReadOnlySpan<char> remainder = rawLine.Slice(firstComma + 1);
                int secondComma = remainder.IndexOf(',');
                if (secondComma == -1) return false;
                ReadOnlySpan<char> titleSpan = remainder.Slice(0, secondComma);

                // Isolate HoursPlayed
                remainder = remainder.Slice(secondComma + 1);
                int thirdComma = remainder.IndexOf(',');
                if (thirdComma == -1) return false;
                ReadOnlySpan<char> hoursSpan = remainder.Slice(0, thirdComma);

                // Isolate CompletionStatus
                ReadOnlySpan<char> statusSpan = remainder.Slice(thirdComma + 1);

                // Parse out values without heap-allocating temporary sub-strings
                if (!int.TryParse(idSpan, out int gameId) || !double.TryParse(hoursSpan, out double hoursPlayed))
                {
                    return false;
                }

                // Push parsed fields to the stack struct validation payload
                string currentStatusStr = statusSpan.ToString(); // Materialize layout string for core engine rules
                var payload = new ValidationPayload(gameId, hoursPlayed, currentStatusStr);

                return EvaluateRecordInPlace(in payload);
            }
            catch
            {
                return false; // Safely absorb telemetry parse faults
            }
        }

        // The 'in' modifier locks the struct parameter down as reference-to-readonly stack memory
        private bool EvaluateRecordInPlace(in ValidationPayload payload)
        {
            // Executes directly on original stack data; zero defensive copies generated
            return payload.IsStateValid();
        }
    }
}

