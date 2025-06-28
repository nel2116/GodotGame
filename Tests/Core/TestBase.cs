using NUnit.Framework;
using Core.Utilities;
using System;

namespace Tests.Core
{
    /// <summary>
    /// テストのベースクラス
    /// テスト環境の初期化とクリーンアップを統一管理
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// テスト開始前の初期化
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            // テスト環境を有効化
            GodotMock.SetTestEnvironment(true);
            TestLogger.SetEnabled(true);
            TestLogger.SetMinimumLevel(TestLogger.LogLevel.Debug);
            
            // ログとモック出力をクリア
            TestLogger.Clear();
            GodotMock.ClearMockOutput();
            
            // テスト環境の安定性を確認
            EnsureTestEnvironmentStability();
            
            OnSetUp();
        }

        /// <summary>
        /// テスト終了後のクリーンアップ
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            OnTearDown();
            
            // テスト環境を無効化
            GodotMock.SetTestEnvironment(false);
            TestLogger.SetEnabled(false);
        }

        /// <summary>
        /// テスト環境の安定性を確保
        /// </summary>
        private void EnsureTestEnvironmentStability()
        {
            // テスト環境が正しく初期化されているか確認
            if (!GodotMock.IsTestEnvironmentInitialized())
            {
                throw new InvalidOperationException("Test environment is not properly initialized");
            }

            // メモリリークのチェック
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// サブクラスでオーバーライド可能なSetUp処理
        /// </summary>
        protected virtual void OnSetUp()
        {
        }

        /// <summary>
        /// サブクラスでオーバーライド可能なTearDown処理
        /// </summary>
        protected virtual void OnTearDown()
        {
        }

        /// <summary>
        /// ログエントリを取得
        /// </summary>
        protected IReadOnlyList<TestLogger.LogEntry> GetLogEntries()
        {
            return TestLogger.GetLogEntries();
        }

        /// <summary>
        /// 指定レベルのログエントリを取得
        /// </summary>
        protected IReadOnlyList<TestLogger.LogEntry> GetLogEntries(TestLogger.LogLevel level)
        {
            return TestLogger.GetLogEntries(level);
        }

        /// <summary>
        /// モック出力を取得
        /// </summary>
        protected IReadOnlyList<string> GetMockOutput()
        {
            return GodotMock.GetMockOutput();
        }

        /// <summary>
        /// モック出力をクリア
        /// </summary>
        protected void ClearMockOutput()
        {
            GodotMock.ClearMockOutput();
        }

        /// <summary>
        /// エラーログが存在するかチェック
        /// </summary>
        protected bool HasErrors()
        {
            return TestLogger.GetStatistics().ErrorCount > 0 || 
                   TestLogger.GetStatistics().FatalCount > 0 ||
                   GodotMock.GetErrorCount() > 0;
        }

        /// <summary>
        /// 警告ログが存在するかチェック
        /// </summary>
        protected bool HasWarnings()
        {
            return TestLogger.GetStatistics().WarningCount > 0 ||
                   GodotMock.GetWarningCount() > 0;
        }

        /// <summary>
        /// ログ統計を取得
        /// </summary>
        protected TestLogger.LogStatistics GetLogStatistics()
        {
            return TestLogger.GetStatistics();
        }

        /// <summary>
        /// 指定されたメッセージがログに含まれているかチェック
        /// </summary>
        protected bool LogContains(string message)
        {
            var entries = TestLogger.GetLogEntries();
            return entries.Any(entry => entry.Message.Contains(message));
        }

        /// <summary>
        /// 指定されたメッセージがモック出力に含まれているかチェック
        /// </summary>
        protected bool MockOutputContains(string message)
        {
            return GodotMock.ContainsOutput(message);
        }

        /// <summary>
        /// 指定されたレベルのメッセージがモック出力に含まれているかチェック
        /// </summary>
        protected bool MockOutputContains(string message, string level)
        {
            return GodotMock.ContainsOutput(message, level);
        }

        /// <summary>
        /// エラーがないことをアサート
        /// </summary>
        protected void AssertNoErrors()
        {
            Assert.IsFalse(HasErrors(), "エラーが発生しています");
        }

        /// <summary>
        /// 警告がないことをアサート
        /// </summary>
        protected void AssertNoWarnings()
        {
            Assert.IsFalse(HasWarnings(), "警告が発生しています");
        }

        /// <summary>
        /// 指定されたメッセージがログに含まれていることをアサート
        /// </summary>
        protected void AssertLogContains(string message)
        {
            Assert.IsTrue(LogContains(message), $"ログにメッセージ '{message}' が含まれていません");
        }

        /// <summary>
        /// 指定されたメッセージがモック出力に含まれていることをアサート
        /// </summary>
        protected void AssertMockOutputContains(string message)
        {
            Assert.IsTrue(MockOutputContains(message), $"モック出力にメッセージ '{message}' が含まれていません");
        }

        /// <summary>
        /// 指定されたレベルのメッセージがモック出力に含まれていることをアサート
        /// </summary>
        protected void AssertMockOutputContains(string message, string level)
        {
            Assert.IsTrue(MockOutputContains(message, level), $"モック出力にレベル '{level}' のメッセージ '{message}' が含まれていません");
        }

        /// <summary>
        /// ログをファイルに出力（デバッグ用）
        /// </summary>
        protected void WriteLogToFile(string filePath)
        {
            TestLogger.WriteToFile(filePath);
        }

        /// <summary>
        /// 安全なテスト実行
        /// </summary>
        protected void SafeTestExecution(Action testAction, string testName = "")
        {
            try
            {
                testAction();
            }
            catch (Exception ex)
            {
                TestLogger.Error($"Test execution failed: {testName} - {ex.Message}", "TestBase");
                throw;
            }
        }

        /// <summary>
        /// パフォーマンステスト用のヘルパー
        /// </summary>
        protected void MeasurePerformance(Action action, string operationName, int maxMilliseconds = 1000)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            
            TestLogger.Info($"Performance: {operationName} took {stopwatch.ElapsedMilliseconds}ms", "TestBase");
            
            // 長時間実行テストの場合は基準を緩和
            if (maxMilliseconds > 10000)
            {
                // 60秒以内ならOK（長時間テスト用）
                Assert.LessOrEqual(stopwatch.ElapsedMilliseconds, maxMilliseconds, 
                    $"{operationName} took too long: {stopwatch.ElapsedMilliseconds}ms (max: {maxMilliseconds}ms)");
            }
            else
            {
                // 通常のテストは従来の基準
                Assert.LessOrEqual(stopwatch.ElapsedMilliseconds, maxMilliseconds, 
                    $"{operationName} took too long: {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// メモリ使用量のチェック
        /// </summary>
        protected void CheckMemoryUsage(string operationName)
        {
            var beforeMemory = GC.GetTotalMemory(false);
            GC.Collect();
            var afterMemory = GC.GetTotalMemory(true);
            
            TestLogger.Info($"Memory: {operationName} - Before: {beforeMemory}, After: {afterMemory}", "TestBase");
        }
    }
} 