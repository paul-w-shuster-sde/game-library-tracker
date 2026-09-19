using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GameLibraryAnalytics
{
    public class GameMetricEntry
    {
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public double HoursPlayed { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
namespace GameLibraryAnalytics
{
    public partial class AnalyticsReportGenerator
    {
        private static void AppendDocumentHeader(StringBuilder sb)
        {
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en' class='h-full bg-slate-950'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>Game Library Analytics Pipeline Dashboard</title>");
            sb.AppendLine("    <script src='https://cdn.tailwindcss.com'></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body class='h-full text-slate-100 font-sans'>");
            sb.AppendLine("    <div class='min-h-full'>");
        }

        private static void AppendNavigationGrid(StringBuilder sb)
        {
            sb.AppendLine("        <nav class='border-b border-slate-800 bg-slate-900'>");
            sb.AppendLine("            <div class='mx-auto max-w-7xl px-4 sm:px-6 lg:px-8'>");
            sb.AppendLine("                <div class='flex h-16 items-center justify-between'>");
            sb.AppendLine("                    <div class='flex items-center'>");
            sb.AppendLine("                        <span class='text-xl font-bold bg-gradient-to-r from-violet-400 to-indigo-400 bg-clip-text text-transparent'>🎮 GameLibrary.Analytics</span>");
            sb.AppendLine("                    </div>");
            sb.AppendLine("                    <div class='text-sm text-slate-400 font-mono'>Pipeline Status: <span class='text-emerald-400 font-semibold'>● ONLINE</span></div>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </nav>");
        }
    }
}
namespace GameLibraryAnalytics
{
    public partial class AnalyticsReportGenerator
    {
        public static void GenerateStaticDashboard(
            List<GameMetricEntry> topHoursLeaderboard, 
            List<GameMetricEntry> lowUsageQueue, 
            GameMetricEntry? randomBacklogGame, 
            string outputFilePath = "index.html")
        {
            var htmlBuilder = new StringBuilder();
            AppendDocumentHeader(htmlBuilder);
            AppendNavigationGrid(htmlBuilder);

            htmlBuilder.AppendLine("        <header class='py-8 bg-slate-900/50'>");
            htmlBuilder.AppendLine("            <div class='mx-auto max-w-7xl px-4 sm:px-6 lg:px-8'>");
            htmlBuilder.AppendLine("                <h1 class='text-3xl font-bold tracking-tight text-white'>Workstation Execution Blueprint</h1>");
            htmlBuilder.AppendLine($"               <p class='mt-2 text-sm text-slate-400 font-mono'>Data Refresh Interval Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss} EST</p>");
            htmlBuilder.AppendLine("            </div>");
            htmlBuilder.AppendLine("        </header>");

            htmlBuilder.AppendLine("        <main class='py-10'>");
            htmlBuilder.AppendLine("            <div class='mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 space-y-10'>");

            if (randomBacklogGame != null)
            {
                htmlBuilder.AppendLine("                <div class='relative overflow-hidden rounded-2xl border border-indigo-500/30 bg-gradient-to-r from-indigo-950/40 to-slate-900 p-8 shadow-2xl shadow-indigo-500/5'>");
                htmlBuilder.AppendLine("                    <div class='relative z-10 space-y-2'>");
                htmlBuilder.AppendLine("                        <span class='inline-flex items-center rounded-md bg-indigo-500/10 px-2.5 py-0.5 text-xs font-medium text-indigo-400 ring-1 ring-inset ring-indigo-500/20 font-mono uppercase tracking-wider'>🎲 Report 3: Optimization Directive</span>");
                htmlBuilder.AppendLine("                        <h2 class='text-2xl font-bold tracking-tight text-white'>Your Next Game to Try</h2>");
                htmlBuilder.AppendLine("                        <p class='text-slate-400 max-w-xl text-sm'>The dynamic backlog selection algorithm evaluated unplayed items and isolated a high-priority discovery target:</p>");
                htmlBuilder.AppendLine("                        <div class='pt-4 flex flex-wrap items-baseline gap-x-4 gap-y-2'>");
                htmlBuilder.AppendLine($"                           <span class='text-xl font-bold text-indigo-300'>{randomBacklogGame.Title}</span>");
                htmlBuilder.AppendLine($"                           <span class='text-xs font-mono text-slate-500'>| Location: <span class='text-slate-400 font-semibold'>{randomBacklogGame.Platform}</span></span>");
                htmlBuilder.AppendLine($"                           <span class='text-xs font-mono text-slate-500'>| Progress Metric: <span class='text-amber-400 font-semibold'>{randomBacklogGame.HoursPlayed:F1}h logged</span></span>");
                htmlBuilder.AppendLine("                        </div>");
                htmlBuilder.AppendLine("                    </div>");
                htmlBuilder.AppendLine("                    <div class='absolute -right-10 -top-10 h-40 w-40 rounded-full bg-indigo-500/10 blur-3xl pointer-events-none'></div>");
                htmlBuilder.AppendLine("                </div>");
            }

            htmlBuilder.AppendLine("                <div class='grid grid-cols-1 gap-5 sm:grid-cols-3'>");
            htmlBuilder.AppendLine("                    <div class='overflow-hidden rounded-xl bg-slate-900 border border-slate-800 p-6'>");
            htmlBuilder.AppendLine("                        <dt class='truncate text-sm font-medium text-slate-400'>Tracked Inventory Count</dt>");
            htmlBuilder.AppendLine($"                       <dd class='mt-1 text-3xl font-semibold tracking-tight text-indigo-400'>{topHoursLeaderboard.Count + lowUsageQueue.Count} Titles</dd>");
            htmlBuilder.AppendLine("                    </div>");
            htmlBuilder.AppendLine("                    <div class='overflow-hidden rounded-xl bg-slate-900 border border-slate-800 p-6'>");
            htmlBuilder.AppendLine("                        <dt class='truncate text-sm font-medium text-slate-400'>Data Ingestion Model</dt>");
            htmlBuilder.AppendLine("                        <dd class='mt-1 text-3xl font-semibold tracking-tight text-violet-400'>PostgreSQL 3NF</dd>");
            htmlBuilder.AppendLine("                    </div>");
            htmlBuilder.AppendLine("                    <div class='overflow-hidden rounded-xl bg-slate-900 border border-slate-800 p-6'>");
            htmlBuilder.AppendLine("                        <dt class='truncate text-sm font-medium text-slate-400'>Validation Layer</dt>");
            htmlBuilder.AppendLine("                        <dd class='mt-1 text-3xl font-semibold tracking-tight text-emerald-400'>Zero-Alloc</dd>");
            htmlBuilder.AppendLine("                    </div>");
            htmlBuilder.AppendLine("                </div>");

            htmlBuilder.AppendLine("                <div class='grid grid-cols-1 gap-8 lg:grid-cols-2'>");
            htmlBuilder.AppendLine("                    <div class='rounded-xl border border-slate-800 bg-slate-900 p-6'>");
            htmlBuilder.AppendLine("                        <h2 class='text-lg font-bold text-white mb-4 flex items-center gap-2'>📊 Report 1: Top Core Entertainment Games</h2>");
            BuildMetricsTable(htmlBuilder, topHoursLeaderboard);
            htmlBuilder.AppendLine("                    </div>");

            htmlBuilder.AppendLine("                    <div class='rounded-xl border border-slate-800 bg-slate-900 p-6'>");
            htmlBuilder.AppendLine("                        <h2 class='text-lg font-bold text-white mb-4 flex items-center gap-2'>🔍 Report 2: Low-Usage Discovery Targets</h2>");
            BuildMetricsTable(htmlBuilder, lowUsageQueue);
            htmlBuilder.AppendLine("                    </div>");
            htmlBuilder.AppendLine("                </div>");

            htmlBuilder.AppendLine("            </div>");
            htmlBuilder.AppendLine("        </main>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            File.WriteAllText(outputFilePath, htmlBuilder.ToString(), Encoding.UTF8);
            Console.WriteLine($"[+] Dashboard generated at: {Path.GetFullPath(outputFilePath)}");
        }

        private static void BuildMetricsTable(StringBuilder sb, List<GameMetricEntry> dataset)
        {
            sb.AppendLine("        <div class='overflow-x-auto mt-2'>");
            sb.AppendLine("            <table class='min-w-full divide-y divide-slate-800 font-mono text-xs text-left'>");
            sb.AppendLine("                <thead>");
            sb.AppendLine("                    <tr class='text-slate-400 uppercase tracking-wider'>");
            sb.AppendLine("                        <th class='py-3 px-4 font-semibold'>Game Title</th>");
            sb.AppendLine("                        <th class='py-3 px-4 font-semibold'>Platform</th>");
            sb.AppendLine("                        <th class='py-3 px-4 font-semibold text-right'>Hours</th>");
            sb.AppendLine("                    </tr>");
            sb.AppendLine("                </thead>");
            sb.AppendLine("<tbody class='divide-y divide-slate-800 text-slate-300'>");

            foreach (var game in dataset)
            {
                sb.AppendLine("                    <tr class='hover:bg-slate-800/40 transition-colors'>");
                sb.AppendLine($"                       <td class='py-3 px-4 font-medium text-slate-100 truncate max-w-[180px]'>{game.Title}</td>");
                sb.AppendLine($"                       <td class='py-3 px-4'><span class='px-2 py-0.5 rounded text-[10px] font-bold bg-slate-800 text-slate-400 border border-slate-700'>{game.Platform}</span></td>");
                sb.AppendLine($"                       <td class='py-3 px-4 text-right font-semibold text-indigo-400'>{game.HoursPlayed:F1}h</td>");
                sb.AppendLine("                    </tr>");
            }
            sb.AppendLine("                </tbody>");
            sb.AppendLine("            </table>");
            sb.AppendLine("        </div>");
        }
    }
}
