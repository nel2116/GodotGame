using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Utilities
{
    /// <summary>
    /// テスト専用のログシステム
    /// Godotネイティブ関数を使用せずにログ出力を行う
    /// </summary>
    public static class TestLogger
    {
        private static readonly object _lock = new object();
        private static readonly List<LogEntry> _logEntries = new List<LogEntry>();
        private static bool _isEnabled = true;
        private static LogLevel _minimumLevel = LogLevel.Info;

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Fatal = 4
        }

        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Message { get; set; }
            public string Category { get; set; }
            public Exception? Exception { get; set; }

            public LogEntry(LogLevel level, string message, string category = "", Exception? exception = null)
            {
                Timestamp = DateTime.Now;
                Level = level;
                Message = message;
                Category = category;
                Exception = exception;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"[{Timestamp:HH:mm:ss.fff}] ");
                sb.Append($"[{Level}] ");
                if (!string.IsNullOrEmpty(Category))
                {
                    sb.Append($"[{Category}] ");
                }
                sb.Append(Message);
                
                if (Exception != null)
                {
                    sb.Append($" | Exception: {Exception.Message}");
                }
                
                return sb.ToString();
            }
        }

        /// <summary>
        /// ログシステムを有効/無効にする
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            lock (_lock)
            {
                _isEnabled = enabled;
            }
        }

        /// <summary>
        /// 最小ログレベルを設定
        /// </summary>
        public static void SetMinimumLevel(LogLevel level)
        {
            lock (_lock)
            {
                _minimumLevel = level;
            }
        }

        /// <summary>
        /// ログエントリをクリア
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _logEntries.Clear();
            }
        }

        /// <summary>
        /// 全ログエントリを取得
        /// </summary>
        public static IReadOnlyList<LogEntry> GetLogEntries()
        {
            lock (_lock)
            {
                return _logEntries.ToArray();
            }
        }

        /// <summary>
        /// 指定レベルのログエントリを取得
        /// </summary>
        public static IReadOnlyList<LogEntry> GetLogEntries(LogLevel level)
        {
            lock (_lock)
            {
                return _logEntries.FindAll(entry => entry.Level >= level).ToArray();
            }
        }

        /// <summary>
        /// ログをファイルに出力
        /// </summary>
        public static void WriteToFile(string filePath)
        {
            lock (_lock)
            {
                try
                {
                    var lines = _logEntries.ConvertAll(entry => entry.ToString());
                    File.WriteAllLines(filePath, lines);
                }
                catch (Exception ex)
                {
                    // ファイル書き込みに失敗した場合はコンソールに出力
                    Console.WriteLine($"Failed to write log to file: {ex.Message}");
                }
            }
        }

        private static void Log(LogLevel level, string message, string category = "", Exception? exception = null)
        {
            if (!_isEnabled || level < _minimumLevel)
                return;

            lock (_lock)
            {
                var entry = new LogEntry(level, message, category, exception);
                _logEntries.Add(entry);

                // コンソールにも出力（テスト実行時の可視性のため）
                Console.WriteLine(entry.ToString());
            }
        }

        /// <summary>
        /// デバッグレベルのログ
        /// </summary>
        public static void Debug(string message, string category = "")
        {
            Log(LogLevel.Debug, message, category);
        }

        /// <summary>
        /// 情報レベルのログ
        /// </summary>
        public static void Info(string message, string category = "")
        {
            Log(LogLevel.Info, message, category);
        }

        /// <summary>
        /// 警告レベルのログ
        /// </summary>
        public static void Warning(string message, string category = "")
        {
            Log(LogLevel.Warning, message, category);
        }

        /// <summary>
        /// エラーレベルのログ
        /// </summary>
        public static void Error(string message, string category = "", Exception? exception = null)
        {
            Log(LogLevel.Error, message, category, exception);
        }

        /// <summary>
        /// 致命的エラーレベルのログ
        /// </summary>
        public static void Fatal(string message, string category = "", Exception? exception = null)
        {
            Log(LogLevel.Fatal, message, category, exception);
        }

        /// <summary>
        /// 統計情報を取得
        /// </summary>
        public static LogStatistics GetStatistics()
        {
            lock (_lock)
            {
                var stats = new LogStatistics();
                foreach (var entry in _logEntries)
                {
                    switch (entry.Level)
                    {
                        case LogLevel.Debug:
                            stats.DebugCount++;
                            break;
                        case LogLevel.Info:
                            stats.InfoCount++;
                            break;
                        case LogLevel.Warning:
                            stats.WarningCount++;
                            break;
                        case LogLevel.Error:
                            stats.ErrorCount++;
                            break;
                        case LogLevel.Fatal:
                            stats.FatalCount++;
                            break;
                    }
                }
                return stats;
            }
        }

        public class LogStatistics
        {
            public int DebugCount { get; set; }
            public int InfoCount { get; set; }
            public int WarningCount { get; set; }
            public int ErrorCount { get; set; }
            public int FatalCount { get; set; }

            public int TotalCount => DebugCount + InfoCount + WarningCount + ErrorCount + FatalCount;
        }
    }
} 