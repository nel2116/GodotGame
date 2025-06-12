# テストガイド

## 目次

1. [単体テスト](#単体テスト)
2. [統合テスト](#統合テスト)
3. [パフォーマンステスト](#パフォーマンステスト)
4. [UI テスト](#UIテスト)
5. [テスト自動化](#テスト自動化)
6. [テスト環境](#テスト環境)
7. [テストレポート](#テストレポート)

## 単体テスト

### テストの基本構造

**目的**: 個々のコンポーネントの動作を検証

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
    }

    [Test]
    public void Initialize_ShouldCreateSubsystems()
    {
        // Arrange
        var expectedSubsystems = new[]
        {
            typeof(PlayerInputSystem),
            typeof(PlayerStateSystem),
            typeof(PlayerMovementSystem),
            typeof(PlayerCombatSystem),
            typeof(PlayerAnimationSystem)
        };

        // Act
        _playerSystem.Initialize();

        // Assert
        foreach (var subsystemType in expectedSubsystems)
        {
            Assert.That(_playerSystem.GetSubsystem(subsystemType), Is.Not.Null);
        }
    }

    [Test]
    public void HandleError_ShouldPublishErrorEvent()
    {
        // Arrange
        var errorReceived = false;
        _eventBus.Subscribe<PlayerErrorEvent>(e => errorReceived = true);

        // Act
        _playerSystem.HandleError(new Exception("Test error"));

        // Assert
        Assert.That(errorReceived, Is.True);
    }
}
```

### モックとスタブ

**目的**: 依存関係を分離し、テストを独立させる

**実装例**:

```csharp
public class PlayerInputSystemTests
{
    private PlayerInputSystem _inputSystem;
    private Mock<IEventBus> _eventBusMock;
    private Mock<IPlayerInputModel> _inputModelMock;

    [SetUp]
    public void Setup()
    {
        _eventBusMock = new Mock<IEventBus>();
        _inputModelMock = new Mock<IPlayerInputModel>();
        _inputSystem = new PlayerInputSystem(_eventBusMock.Object, _inputModelMock.Object);
    }

    [Test]
    public void ProcessInput_ShouldPublishInputEvent()
    {
        // Arrange
        var inputEvent = new PlayerInputEvent { Action = "Move", Value = 1.0f };
        _inputModelMock.Setup(m => m.ProcessInput(It.IsAny<PlayerInputEvent>()))
            .Returns(true);

        // Act
        _inputSystem.ProcessInput(inputEvent);

        // Assert
        _eventBusMock.Verify(b => b.Publish(It.IsAny<PlayerInputEvent>()), Times.Once);
    }
}
```

### テストカバレッジ

**目的**: コードのテストカバレッジを確保

**実装例**:

```csharp
[TestFixture]
public class PlayerStateSystemTests
{
    private PlayerStateSystem _stateSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _stateSystem = new PlayerStateSystem(_eventBus);
    }

    [Test]
    [TestCase(PlayerState.Idle, PlayerState.Walking)]
    [TestCase(PlayerState.Walking, PlayerState.Running)]
    [TestCase(PlayerState.Running, PlayerState.Jumping)]
    public void ChangeState_ShouldTransitionCorrectly(PlayerState fromState, PlayerState toState)
    {
        // Arrange
        _stateSystem.SetState(fromState);

        // Act
        var result = _stateSystem.ChangeState(toState);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_stateSystem.CurrentState, Is.EqualTo(toState));
    }

    [Test]
    public void ChangeState_ShouldRejectInvalidTransition()
    {
        // Arrange
        _stateSystem.SetState(PlayerState.Idle);

        // Act
        var result = _stateSystem.ChangeState(PlayerState.Attacking);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_stateSystem.CurrentState, Is.EqualTo(PlayerState.Idle));
    }
}
```

## 統合テスト

### システム間の連携テスト

**目的**: 複数のシステムが正しく連携することを検証

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemsIntegrationTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
    }

    [Test]
    public void InputToMovement_ShouldWorkCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();
        var movementSystem = _playerSystem.GetSubsystem<PlayerMovementSystem>();
        var stateSystem = _playerSystem.GetSubsystem<PlayerStateSystem>();

        // Act
        inputSystem.ProcessInput(new PlayerInputEvent { Action = "Move", Value = 1.0f });

        // Assert
        Assert.That(stateSystem.CurrentState, Is.EqualTo(PlayerState.Walking));
        Assert.That(movementSystem.IsMoving, Is.True);
    }

    [Test]
    public void CombatToAnimation_ShouldWorkCorrectly()
    {
        // Arrange
        var combatSystem = _playerSystem.GetSubsystem<PlayerCombatSystem>();
        var animationSystem = _playerSystem.GetSubsystem<PlayerAnimationSystem>();

        // Act
        combatSystem.StartAttack();

        // Assert
        Assert.That(animationSystem.CurrentAnimation, Is.EqualTo("Attack"));
    }
}
```

### イベントフロー検証

**目的**: イベントの伝播と処理を検証

**実装例**:

```csharp
[TestFixture]
public class EventFlowTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private List<IPlayerEvent> _receivedEvents;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _receivedEvents = new List<IPlayerEvent>();

        _eventBus.Subscribe<IPlayerEvent>(e => _receivedEvents.Add(e));
    }

    [Test]
    public void InputEvent_ShouldTriggerCorrectEventFlow()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();

        // Act
        inputSystem.ProcessInput(new PlayerInputEvent { Action = "Jump" });

        // Assert
        Assert.That(_receivedEvents.Count, Is.GreaterThan(0));
        Assert.That(_receivedEvents.Any(e => e is PlayerStateChangeEvent), Is.True);
        Assert.That(_receivedEvents.Any(e => e is PlayerAnimationEvent), Is.True);
    }
}
```

### エラー処理検証

**目的**: エラー発生時のシステムの挙動を検証

**実装例**:

```csharp
[TestFixture]
public class ErrorHandlingTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private List<PlayerErrorEvent> _errorEvents;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _errorEvents = new List<PlayerErrorEvent>();

        _eventBus.Subscribe<PlayerErrorEvent>(e => _errorEvents.Add(e));
    }

    [Test]
    public void SubsystemError_ShouldBeHandledCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();

        // Act
        inputSystem.ProcessInput(null);

        // Assert
        Assert.That(_errorEvents.Count, Is.EqualTo(1));
        Assert.That(_errorEvents[0].Message, Does.Contain("Input processing failed"));
    }
}
```

## パフォーマンステスト

### 負荷テスト

**目的**: システムの負荷耐性を検証

**実装例**:

```csharp
[TestFixture]
public class PerformanceTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private PerformanceProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _profiler = new PerformanceProfiler();
    }

    [Test]
    public void HighFrequencyInput_ShouldProcessCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();
        var iterations = 1000;

        // Act
        _profiler.StartMeasurement("InputProcessing");
        for (int i = 0; i < iterations; i++)
        {
            inputSystem.ProcessInput(new PlayerInputEvent { Action = "Move", Value = 1.0f });
        }
        _profiler.StopMeasurement("InputProcessing");

        // Assert
        var results = _profiler.GetResults("InputProcessing");
        Assert.That(results.Average, Is.LessThan(1.0)); // 1ms未満
    }
}
```

### メモリ使用量テスト

**目的**: メモリ使用量を検証

**実装例**:

```csharp
[TestFixture]
public class MemoryUsageTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private MemoryProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _profiler = new MemoryProfiler();
    }

    [Test]
    public void SystemInitialization_ShouldNotLeakMemory()
    {
        // Arrange
        _profiler.TakeSnapshot("BeforeInit");

        // Act
        _playerSystem.Initialize();
        _profiler.TakeSnapshot("AfterInit");

        // Assert
        var memoryDiff = _profiler.GetMemoryDifference("BeforeInit", "AfterInit");
        Assert.That(memoryDiff, Is.LessThan(10 * 1024 * 1024)); // 10MB未満
    }
}
```

### CPU 使用率テスト

**目的**: CPU 使用率を検証

**実装例**:

```csharp
[TestFixture]
public class CPUUsageTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private CPUProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _profiler = new CPUProfiler();
    }

    [Test]
    public void UpdateLoop_ShouldNotExceedCPULimit()
    {
        // Arrange
        _profiler.StartProfiling("UpdateLoop");

        // Act
        for (int i = 0; i < 1000; i++)
        {
            _playerSystem.Update(0.016f); // 60FPS相当
            _profiler.RecordCPUUsage("UpdateLoop");
        }

        // Assert
        var results = _profiler.GetResults("UpdateLoop");
        Assert.That(results.Average, Is.LessThan(5.0)); // 5%未満
    }
}
```

## UI テスト

### 入力検証

**目的**: UI 入力の正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIInputTests
{
    private PlayerUI _playerUI;
    private Mock<IEventBus> _eventBusMock;

    [SetUp]
    public void Setup()
    {
        _eventBusMock = new Mock<IEventBus>();
        _playerUI = new PlayerUI(_eventBusMock.Object);
    }

    [Test]
    public void ButtonClick_ShouldTriggerCorrectAction()
    {
        // Arrange
        var button = _playerUI.GetButton("AttackButton");

        // Act
        button.Click();

        // Assert
        _eventBusMock.Verify(b => b.Publish(It.Is<PlayerInputEvent>(
            e => e.Action == "Attack")), Times.Once);
    }
}
```

### 表示検証

**目的**: UI 表示の正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIDisplayTests
{
    private PlayerUI _playerUI;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerUI = new PlayerUI(_eventBus);
    }

    [Test]
    public void HealthDisplay_ShouldUpdateCorrectly()
    {
        // Arrange
        var healthBar = _playerUI.GetHealthBar();

        // Act
        _eventBus.Publish(new PlayerHealthEvent { CurrentHealth = 75, MaxHealth = 100 });

        // Assert
        Assert.That(healthBar.Value, Is.EqualTo(75));
        Assert.That(healthBar.MaxValue, Is.EqualTo(100));
    }
}
```

### アニメーション検証

**目的**: UI アニメーションの正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIAnimationTests
{
    private PlayerUI _playerUI;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerUI = new PlayerUI(_eventBus);
    }

    [Test]
    public void DamageAnimation_ShouldPlayCorrectly()
    {
        // Arrange
        var damageEffect = _playerUI.GetDamageEffect();

        // Act
        _eventBus.Publish(new PlayerDamageEvent { Amount = 10 });

        // Assert
        Assert.That(damageEffect.IsPlaying, Is.True);
        Assert.That(damageEffect.Duration, Is.EqualTo(0.5f));
    }
}
```

## テスト自動化

### CI/CD 統合

**目的**: 継続的インテグレーション/デリバリーの自動化

**実装例**:

```yaml
# .github/workflows/test.yml
name: Test

on:
    push:
        branches: [main]
    pull_request:
        branches: [main]

jobs:
    test:
        runs-on: ubuntu-latest
        steps:
            - uses: actions/checkout@v2
            - name: Setup .NET
              uses: actions/setup-dotnet@v1
              with:
                  dotnet-version: "6.0.x"
            - name: Restore dependencies
              run: dotnet restore
            - name: Build
              run: dotnet build --no-restore
            - name: Test
              run: dotnet test --no-build --verbosity normal
```

### テストレポート生成

**目的**: テスト結果の自動レポート生成

**実装例**:

```csharp
public class TestReporter
{
    private readonly string _reportPath;
    private readonly List<TestResult> _results;

    public TestReporter(string reportPath)
    {
        _reportPath = reportPath;
        _results = new List<TestResult>();
    }

    public void AddResult(TestResult result)
    {
        _results.Add(result);
    }

    public void GenerateReport()
    {
        var report = new StringBuilder();
        report.AppendLine("# テストレポート");
        report.AppendLine($"生成日時: {DateTime.Now}");
        report.AppendLine();

        report.AppendLine("## サマリー");
        report.AppendLine($"総テスト数: {_results.Count}");
        report.AppendLine($"成功: {_results.Count(r => r.Passed)}");
        report.AppendLine($"失敗: {_results.Count(r => !r.Passed)}");
        report.AppendLine();

        report.AppendLine("## 詳細");
        foreach (var result in _results)
        {
            report.AppendLine($"### {result.Name}");
            report.AppendLine($"結果: {(result.Passed ? "成功" : "失敗")}");
            if (!result.Passed)
            {
                report.AppendLine($"エラー: {result.ErrorMessage}");
            }
            report.AppendLine();
        }

        File.WriteAllText(_reportPath, report.ToString());
    }
}
```

### テストスケジュール

**目的**: テストの自動実行スケジュール管理

**実装例**:

```csharp
public class TestScheduler
{
    private readonly List<TestSchedule> _schedules;
    private readonly TestRunner _testRunner;
    private readonly TestReporter _reporter;

    public TestScheduler(TestRunner testRunner, TestReporter reporter)
    {
        _schedules = new List<TestSchedule>();
        _testRunner = testRunner;
        _reporter = reporter;
    }

    public void AddSchedule(TestSchedule schedule)
    {
        _schedules.Add(schedule);
    }

    public async Task RunScheduledTests()
    {
        foreach (var schedule in _schedules)
        {
            if (schedule.ShouldRun())
            {
                var results = await _testRunner.RunTests(schedule.TestCategories);
                _reporter.AddResults(results);
            }
        }
    }
}
```

## テスト環境

### テストデータ

**目的**: テスト用データの管理

**実装例**:

```csharp
public class TestDataManager
{
    private readonly Dictionary<string, object> _testData;

    public TestDataManager()
    {
        _testData = new Dictionary<string, object>();
        LoadTestData();
    }

    private void LoadTestData()
    {
        // テストデータの読み込み
        _testData["PlayerStats"] = new PlayerStats
        {
            Health = 100,
            Speed = 5.0f,
            JumpForce = 10.0f
        };

        _testData["EnemyStats"] = new EnemyStats
        {
            Health = 50,
            Damage = 10,
            AttackRange = 2.0f
        };
    }

    public T GetTestData<T>(string key)
    {
        if (_testData.TryGetValue(key, out var data))
        {
            return (T)data;
        }
        throw new KeyNotFoundException($"Test data not found: {key}");
    }
}
```

### テスト設定

**目的**: テスト環境の設定管理

**実装例**:

```csharp
public class TestConfiguration
{
    public bool EnableLogging { get; set; }
    public string LogLevel { get; set; }
    public bool EnablePerformanceProfiling { get; set; }
    public int MaxTestIterations { get; set; }
    public float TestTimeout { get; set; }

    public static TestConfiguration Load()
    {
        var config = new TestConfiguration
        {
            EnableLogging = true,
            LogLevel = "Debug",
            EnablePerformanceProfiling = true,
            MaxTestIterations = 1000,
            TestTimeout = 30.0f
        };

        return config;
    }
}
```

### テストフィクスチャ

**目的**: テスト環境のセットアップとクリーンアップ

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemTestFixture
{
    protected PlayerSystem PlayerSystem { get; private set; }
    protected IEventBus EventBus { get; private set; }
    protected TestDataManager TestData { get; private set; }
    protected TestConfiguration Config { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Config = TestConfiguration.Load();
        TestData = new TestDataManager();
    }

    [SetUp]
    public void Setup()
    {
        EventBus = new EventBus();
        PlayerSystem = new PlayerSystem(EventBus);
        PlayerSystem.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerSystem.Dispose();
        EventBus.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestData.Dispose();
    }
}
```

## テストレポート

### レポート生成

**目的**: テスト結果のレポート生成

**実装例**:

```csharp
public class TestReportGenerator
{
    private readonly string _outputPath;
    private readonly List<TestResult> _results;

    public TestReportGenerator(string outputPath)
    {
        _outputPath = outputPath;
        _results = new List<TestResult>();
    }

    public void AddResults(IEnumerable<TestResult> results)
    {
        _results.AddRange(results);
    }

    public void GenerateReport()
    {
        var report = new StringBuilder();
        report.AppendLine("# テストレポート");
        report.AppendLine($"生成日時: {DateTime.Now}");
        report.AppendLine();

        // サマリー
        report.AppendLine("## サマリー");
        var summary = GenerateSummary();
        report.AppendLine(summary);

        // 詳細
        report.AppendLine("## 詳細");
        var details = GenerateDetails();
        report.AppendLine(details);

        // パフォーマンス
        report.AppendLine("## パフォーマンス");
        var performance = GeneratePerformanceReport();
        report.AppendLine(performance);

        File.WriteAllText(_outputPath, report.ToString());
    }

    private string GenerateSummary()
    {
        var total = _results.Count;
        var passed = _results.Count(r => r.Passed);
        var failed = total - passed;

        return $@"総テスト数: {total}
成功: {passed}
失敗: {failed}
成功率: {(float)passed / total * 100:F2}%";
    }

    private string GenerateDetails()
    {
        var details = new StringBuilder();
        foreach (var result in _results)
        {
            details.AppendLine($"### {result.Name}");
            details.AppendLine($"結果: {(result.Passed ? "成功" : "失敗")}");
            if (!result.Passed)
            {
                details.AppendLine($"エラー: {result.ErrorMessage}");
                details.AppendLine($"スタックトレース: {result.StackTrace}");
            }
            details.AppendLine();
        }
        return details.ToString();
    }

    private string GeneratePerformanceReport()
    {
        var performance = new StringBuilder();
        var performanceResults = _results
            .Where(r => r.PerformanceMetrics != null)
            .Select(r => r.PerformanceMetrics);

        if (performanceResults.Any())
        {
            performance.AppendLine("### 実行時間");
            performance.AppendLine($"平均: {performanceResults.Average(p => p.ExecutionTime):F2}ms");
            performance.AppendLine($"最小: {performanceResults.Min(p => p.ExecutionTime):F2}ms");
            performance.AppendLine($"最大: {performanceResults.Max(p => p.ExecutionTime):F2}ms");

            performance.AppendLine("\n### メモリ使用量");
            performance.AppendLine($"平均: {performanceResults.Average(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
            performance.AppendLine($"最小: {performanceResults.Min(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
            performance.AppendLine($"最大: {performanceResults.Max(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
        }

        return performance.ToString();
    }
}
```

### レポート分析

**目的**: テストレポートの分析と改善提案

**実装例**:

```csharp
public class TestReportAnalyzer
{
    private readonly List<TestResult> _results;
    private readonly TestConfiguration _config;

    public TestReportAnalyzer(IEnumerable<TestResult> results, TestConfiguration config)
    {
        _results = results.ToList();
        _config = config;
    }

    public AnalysisResult Analyze()
    {
        var result = new AnalysisResult();

        // テスト成功率の分析
        result.SuccessRate = (float)_results.Count(r => r.Passed) / _results.Count;
        result.IsSuccessRateAcceptable = result.SuccessRate >= 0.95f;

        // パフォーマンスの分析
        var performanceResults = _results
            .Where(r => r.PerformanceMetrics != null)
            .Select(r => r.PerformanceMetrics);

        if (performanceResults.Any())
        {
            result.AverageExecutionTime = performanceResults.Average(p => p.ExecutionTime);
            result.IsPerformanceAcceptable = result.AverageExecutionTime < 100.0f;

            result.AverageMemoryUsage = performanceResults.Average(p => p.MemoryUsage);
            result.IsMemoryUsageAcceptable = result.AverageMemoryUsage < 100 * 1024 * 1024;
        }

        // 改善提案の生成
        result.ImprovementSuggestions = GenerateImprovementSuggestions(result);

        return result;
    }

    private List<string> GenerateImprovementSuggestions(AnalysisResult result)
    {
        var suggestions = new List<string>();

        if (!result.IsSuccessRateAcceptable)
        {
            suggestions.Add("テストの成功率が95%未満です。失敗したテストの修正を優先してください。");
        }

        if (!result.IsPerformanceAcceptable)
        {
            suggestions.Add($"テストの実行時間が平均{result.AverageExecutionTime:F2}msと長いです。パフォーマンスの最適化を検討してください。");
        }

        if (!result.IsMemoryUsageAcceptable)
        {
            suggestions.Add($"メモリ使用量が平均{result.AverageMemoryUsage / 1024 / 1024:F2}MBと多いです。メモリリークの可能性を調査してください。");
        }

        return suggestions;
    }
}
```

### レポート通知

**目的**: テスト結果の通知

**実装例**:

```csharp
public class TestReportNotifier
{
    private readonly string _slackWebhookUrl;
    private readonly string _emailRecipients;

    public TestReportNotifier(string slackWebhookUrl, string emailRecipients)
    {
        _slackWebhookUrl = slackWebhookUrl;
        _emailRecipients = emailRecipients;
    }

    public async Task NotifyTestResults(TestReport report)
    {
        // Slack通知
        await NotifySlack(report);

        // メール通知
        await NotifyEmail(report);
    }

    private async Task NotifySlack(TestReport report)
    {
        var message = new
        {
            text = $"テスト結果: {report.Summary}",
            attachments = new[]
            {
                new
                {
                    color = report.IsSuccess ? "good" : "danger",
                    fields = new[]
                    {
                        new { title = "総テスト数", value = report.TotalTests.ToString(), @short = true },
                        new { title = "成功", value = report.PassedTests.ToString(), @short = true },
                        new { title = "失敗", value = report.FailedTests.ToString(), @short = true },
                        new { title = "成功率", value = $"{report.SuccessRate:F2}%", @short = true }
                    }
                }
            }
        };

        using (var client = new HttpClient())
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(message),
                Encoding.UTF8,
                "application/json");
            await client.PostAsync(_slackWebhookUrl, content);
        }
    }

    private async Task NotifyEmail(TestReport report)
    {
        var message = new MailMessage
        {
            Subject = $"テスト結果: {report.Summary}",
            Body = GenerateEmailBody(report)
        };

        foreach (var recipient in _emailRecipients.Split(','))
        {
            message.To.Add(recipient.Trim());
        }

        using (var client = new SmtpClient())
        {
            await client.SendMailAsync(message);
        }
    }

    private string GenerateEmailBody(TestReport report)
    {
        var body = new StringBuilder();
        body.AppendLine($"テスト実行日時: {report.ExecutionTime}");
        body.AppendLine();
        body.AppendLine("サマリー:");
        body.AppendLine($"総テスト数: {report.TotalTests}");
        body.AppendLine($"成功: {report.PassedTests}");
        body.AppendLine($"失敗: {report.FailedTests}");
        body.AppendLine($"成功率: {report.SuccessRate:F2}%");
        body.AppendLine();
        body.AppendLine("失敗したテスト:");
        foreach (var failure in report.Failures)
        {
            body.AppendLine($"- {failure.Name}: {failure.ErrorMessage}");
        }

        return body.ToString();
    }
}
```
