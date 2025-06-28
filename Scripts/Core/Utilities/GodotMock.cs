using System;
using System.Collections.Generic;
using Core.Utilities;
using Godot;

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
        private static bool _isInitialized = false;

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
                    InitializeTestEnvironment();
                }
                else
                {
                    CleanupTestEnvironment();
                }
            }
        }

        /// <summary>
        /// テスト環境かどうかを取得
        /// </summary>
        public static bool IsTestEnvironment()
        {
            return _isTestEnvironment;
        }

        /// <summary>
        /// テスト環境の初期化
        /// </summary>
        private static void InitializeTestEnvironment()
        {
            if (_isInitialized) return;
            
            try
            {
                // テスト環境での初期化処理
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                TestLogger.Error($"Failed to initialize test environment: {ex.Message}", "GodotMock");
            }
        }

        /// <summary>
        /// テスト環境のクリーンアップ
        /// </summary>
        private static void CleanupTestEnvironment()
        {
            if (!_isInitialized) return;
            
            try
            {
                ClearMockOutput();
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                TestLogger.Error($"Failed to cleanup test environment: {ex.Message}", "GodotMock");
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
                try
                {
                    // テスト環境では重要なログのみ出力
                    if (level == "ERROR" || level == "PUSH_ERROR" || level == "PUSH_WARNING")
                    {
                        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                        var output = $"[{timestamp}] [{level}] {message}";
                        _mockOutput.Add(output);
                        TestLogger.Info(message, "GodotMock");
                    }
                    // 通常のログは出力しない（パフォーマンス向上のため）
                }
                catch (Exception _)
                {
                    // ログ出力中のエラーを無視（無限ループ防止）
                }
            }
        }

        /// <summary>
        /// GD.Print のモック
        /// </summary>
        public static void Print(object? message)
        {
            if (_isTestEnvironment)
            {
                // テスト環境では重要なログのみ出力（デバッグログは無効化）
                var messageStr = message?.ToString() ?? "null";
                
                // 重要なメッセージのみ出力
                if (messageStr.Contains("ERROR") || 
                    messageStr.Contains("WARNING") || 
                    messageStr.Contains("Exception") ||
                    messageStr.Contains("Failed"))
                {
                    AddMockOutput(messageStr, "PRINT");
                }
                // 通常のデバッグログは出力しない
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
        /// Input.GetVector のモック
        /// </summary>
        public static Vector2 GetVector(string negativeX, string positiveX, string negativeY, string positiveY)
        {
            if (_isTestEnvironment)
            {
                // テスト環境では固定値を返す
                return new Vector2(0, 0);
            }
            else
            {
                // 実際のGodot環境ではInput.GetVectorを使用
                // ただし、テスト環境では使用しない
                try
                {
                    return Godot.Input.GetVector(negativeX, positiveX, negativeY, positiveY);
                }
                catch
                {
                    // GodotのネイティブAPIが利用できない場合はデフォルト値を返す
                    return new Vector2(0, 0);
                }
            }
        }

        /// <summary>
        /// Input.IsActionPressed のモック
        /// </summary>
        public static bool IsActionPressed(string action)
        {
            if (_isTestEnvironment)
            {
                // テスト環境ではfalseを返す
                return false;
            }
            else
            {
                // 実際のGodot環境ではInput.IsActionPressedを使用
                // ただし、テスト環境では使用しない
                try
                {
                    return Godot.Input.IsActionPressed(action);
                }
                catch
                {
                    // GodotのネイティブAPIが利用できない場合はデフォルト値を返す
                    return false;
                }
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

        /// <summary>
        /// テスト環境が初期化されているかチェック
        /// </summary>
        public static bool IsTestEnvironmentInitialized()
        {
            return _isTestEnvironment && _isInitialized;
        }

        /// <summary>
        /// 安全なGodot関数呼び出し
        /// </summary>
        public static T SafeGodotCall<T>(Func<T> godotFunction, T defaultValue = default!)
        {
            if (_isTestEnvironment)
            {
                return defaultValue;
            }
            
            try
            {
                return godotFunction();
            }
            catch (Exception ex)
            {
                PrintErr($"Godot function call failed: {ex.Message}");
                return defaultValue;
            }
        }
    }
} 