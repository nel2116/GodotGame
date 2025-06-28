using System;
using System.Collections.Generic;
using Core.Utilities;

namespace Core.Utilities
{
    /// <summary>
    /// Godotネイティブ関数のモック
    /// テスト環境で安全に使用できる
    /// </summary>
    public static class GodotMock
    {
        private static bool _isTestEnvironment = false;
        private static readonly List<string> _mockOutput = new List<string>();
        private static readonly object _lock = new object();

        /// <summary>
        /// テスト環境かどうかを設定
        /// </summary>
        public static void SetTestEnvironment(bool isTest)
        {
            lock (_lock)
            {
                _isTestEnvironment = isTest;
                if (isTest)
                {
                    ClearMockOutput();
                }
            }
        }

        /// <summary>
        /// モック出力をクリア
        /// </summary>
        public static void ClearMockOutput()
        {
            lock (_lock)
            {
                _mockOutput.Clear();
            }
        }

        /// <summary>
        /// モック出力を取得
        /// </summary>
        public static IReadOnlyList<string> GetMockOutput()
        {
            lock (_lock)
            {
                return _mockOutput.ToArray();
            }
        }

        /// <summary>
        /// モック出力に追加
        /// </summary>
        private static void AddMockOutput(string message, string level = "INFO")
        {
            if (!_isTestEnvironment)
                return;

            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var output = $"[{timestamp}] [{level}] {message}";
                _mockOutput.Add(output);
                TestLogger.Info(message, "GodotMock");
            }
        }

        /// <summary>
        /// GD.Print のモック
        /// </summary>
        public static void Print(object? message)
        {
            if (_isTestEnvironment)
            {
                AddMockOutput(message?.ToString() ?? "null", "PRINT");
            }
            else
            {
                // 実際のGodot環境ではGD.Printを使用
                // ただし、テスト環境では使用しない
            }
        }

        /// <summary>
        /// GD.PrintErr のモック
        /// </summary>
        public static void PrintErr(object? message)
        {
            if (_isTestEnvironment)
            {
                AddMockOutput(message?.ToString() ?? "null", "ERROR");
                TestLogger.Error(message?.ToString() ?? "null", "GodotMock");
            }
            else
            {
                // 実際のGodot環境ではGD.PrintErrを使用
                // ただし、テスト環境では使用しない
            }
        }

        /// <summary>
        /// GD.PrintRaw のモック
        /// </summary>
        public static void PrintRaw(object? message)
        {
            if (_isTestEnvironment)
            {
                AddMockOutput(message?.ToString() ?? "null", "RAW");
            }
            else
            {
                // 実際のGodot環境ではGD.PrintRawを使用
            }
        }

        /// <summary>
        /// GD.PushError のモック
        /// </summary>
        public static void PushError(string message)
        {
            if (_isTestEnvironment)
            {
                AddMockOutput(message, "PUSH_ERROR");
                TestLogger.Error(message, "GodotMock");
            }
            else
            {
                // 実際のGodot環境ではGD.PushErrorを使用
            }
        }

        /// <summary>
        /// GD.PushWarning のモック
        /// </summary>
        public static void PushWarning(string message)
        {
            if (_isTestEnvironment)
            {
                AddMockOutput(message, "PUSH_WARNING");
                TestLogger.Warning(message, "GodotMock");
            }
            else
            {
                // 実際のGodot環境ではGD.PushWarningを使用
            }
        }

        /// <summary>
        /// 指定されたメッセージがモック出力に含まれているかチェック
        /// </summary>
        public static bool ContainsOutput(string message)
        {
            lock (_lock)
            {
                return _mockOutput.Exists(output => output.Contains(message));
            }
        }

        /// <summary>
        /// 指定されたレベルのメッセージがモック出力に含まれているかチェック
        /// </summary>
        public static bool ContainsOutput(string message, string level)
        {
            lock (_lock)
            {
                return _mockOutput.Exists(output => 
                    output.Contains(message) && output.Contains($"[{level}]"));
            }
        }

        /// <summary>
        /// エラーメッセージの数を取得
        /// </summary>
        public static int GetErrorCount()
        {
            lock (_lock)
            {
                return _mockOutput.Count(output => output.Contains("[ERROR]") || output.Contains("[PUSH_ERROR]"));
            }
        }

        /// <summary>
        /// 警告メッセージの数を取得
        /// </summary>
        public static int GetWarningCount()
        {
            lock (_lock)
            {
                return _mockOutput.Count(output => output.Contains("[PUSH_WARNING]"));
            }
        }

        /// <summary>
        /// 全メッセージの数を取得
        /// </summary>
        public static int GetTotalCount()
        {
            lock (_lock)
            {
                return _mockOutput.Count;
            }
        }
    }
} 