using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GameLibraryAnalytics
{
    public class GameMetric
    {
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public double HoursLogged { get; set; }
    }

    public static class DataDashboardExporter
    {
        public static void GenerateStaticPage(
            List<GameMetric> softwareMetrics, 
            List<GameMetric> gameMetrics, 
            GameMetric recommendation)
        {
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
            var sb = new StringBuilder();

            // 1. Build Document Header and include Tailwind CSS via CDN for styling
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en' class='bg-slate-900 text-slate-100'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>Game Library Analytics Dashboard</title>");
            sb.AppendLine("    <script src='https://tailwindcss.com'></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body class='font-sans antialiased min-h-screen p-6 md:p-12'>");

            // 2. Dashboard Container & Hero Section
            sb.AppendLine("    <div class='max-w-6xl mx-auto space-y-8'>");
            sb.AppendLine("        <header class='border-b border-slate-800 pb-6'>");
            sb.AppendLine("            <h1 class='text-4xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-cyan-400 to-indigo-500'>🛰️ Game Library Analytics Matrix</h1>");
            sb.AppendLine("            <p class='text-slate-400 mt-2 font-mono text-sm'>Ingestion Target: PostgreSQL 16 @10.0.0.205 | Pipeline Status: Operational</p>");
            sb.AppendLine("        </header>");

            // 3. Highlight Card: Randomized Unplayed Backlog Selection
            sb.AppendLine("        <section class='bg-gradient-to-br from-indigo-900/50 to-slate-800/50 border border-indigo-500/30 rounded-xl p-6 shadow-xl backdrop-blur-sm'>");
            sb.AppendLine("            <span class='text-xs font-bold text-indigo-400 uppercase tracking-widest bg-indigo-500/10 px-3 py-1 rounded-full'>🎲 Next System Directive</span>");
            sb.AppendLine($"            <h2 class='text-2xl font-black text-white mt-3'>{recommendation.Title} ({recommendation.ReleaseYear})</h2>");
            sb.AppendLine("            <p class='text-slate-400 mt-1 text-sm font-mono'>Architecture Directive: Fire up your client station and log your first metrics!</p>");
            sb.AppendLine("        </section>");

            // 4. Multi-Column Layout for Data Reports
            sb.AppendLine("        <div class='grid grid-cols-1 lg:grid-cols-2 gap-8'>");

            // Left Column: Top 25 True Entertainment Games
            sb.AppendLine("            <section class='bg-slate-800/40 border border-slate-800 rounded-xl p-6 space-y-4'>");
            sb.AppendLine("                <h3 class='text-xl font-bold text-cyan-400 flex items-center gap-2'>🎮 Top Entertainment Game Matrix</h3>");
            sb.AppendLine("                <div class='overflow-x-auto'>");
            sb.AppendLine("                    <table class='w-full text-left text-sm border-collapse'>");
            sb.AppendLine("                        <thead>");
            sb.AppendLine("                            <tr class='border-b border-slate-700 text-slate-400 text-xs uppercase font-mono'>");
            sb.AppendLine("                                <th class='pb-3 w-12'>Rank</th>");
            sb.AppendLine("                                <th class='pb-3'>Title</th>");
            sb.AppendLine("                                <th class='pb-3 text-right'>Hours Logged</th>");
            sb.AppendLine("                            </tr>");
            sb.AppendLine("                        </thead>");
            sb.AppendLine("                        <tbody class='divide-y divide-slate-800/60'>");
            
            for (int i = 0; i < gameMetrics.Count; i++)
            {
                string rowBg = i == 0 ? "bg-cyan-500/5 text-cyan-200" : "";
                sb.AppendLine($"                            <tr class='hover:bg-slate-700/30 transition {rowBg}'>");
                sb.AppendLine($"                                <td class='py-3 font-mono text-slate-500'>{i + 1}</td>");
                sb.AppendLine($"                                <td class='py-3 font-medium text-slate-200'>{gameMetrics[i].Title} <span class='text-xs text-slate-500 font-mono'>({gameMetrics[i].ReleaseYear})</span></td>");
                sb.AppendLine($"                                <td class='py-3 text-right font-mono font-semibold text-slate-300'>{gameMetrics[i].HoursLogged:N1} hrs</td>");
                sb.AppendLine("                            </tr>");
            }
            
            sb.AppendLine("                        </tbody>");
            sb.AppendLine("                    </table>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </section>");

            // Right Column: Top Software & Utilities
            sb.AppendLine("            <section class='bg-slate-800/40 border border-slate-800 rounded-xl p-6 space-y-4'>");
            sb.AppendLine("                <h3 class='text-xl font-bold text-indigo-400 flex items-center gap-2'>🧰 Software & Utilities Metrics</h3>");
            sb.AppendLine("                <div class='overflow-x-auto'>");
            sb.AppendLine("                    <table class='w-full text-left text-sm border-collapse'>");
            sb.AppendLine("                        <thead>");
            sb.AppendLine("                            <tr class='border-b border-slate-700 text-slate-400 text-xs uppercase font-mono'>");
            sb.AppendLine("                                <th class='pb-3 w-12'>Rank</th>");
            sb.AppendLine("                                <th class='pb-3'>Utility Tool</th>");
            sb.AppendLine("                                <th class='pb-3 text-right'>Hours Logged</th>");
            sb.AppendLine("                            </tr>");
            sb.AppendLine("                        </thead>");
            sb.AppendLine("                        <tbody class='divide-y divide-slate-800/60'>");

            for (int i = 0; i < softwareMetrics.Count; i++)
            {
                sb.AppendLine("                            <tr class='hover:bg-slate-700/30 transition'>");
                sb.AppendLine($"                                <td class='py-3 font-mono text-slate-500'>{i + 1}</td>");
                sb.AppendLine($"                                <td class='py-3 font-medium text-slate-200'>{softwareMetrics[i].Title} <span class='text-xs text-slate-500 font-mono'>({softwareMetrics[i].ReleaseYear})</span></td>");
                sb.AppendLine($"                                <td class='py-3 text-right font-mono font-semibold text-slate-300'>{softwareMetrics[i].HoursLogged:N1} hrs</td>");
                sb.AppendLine("                            </tr>");
            }

            sb.AppendLine("                        </tbody>");
            sb.AppendLine("                    </table>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </section>");

            sb.AppendLine("        </div>"); // End grid
            sb.AppendLine("    </div>"); // End container
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            // 5. Stream the constructed HTML markup straight to disk
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"\n[+] Static Dashboard Report Compiled Successfully -> {outputPath}");
        }
    }
}
