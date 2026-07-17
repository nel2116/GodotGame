using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Events;
using Core.Reactive;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Combat;
using Systems.Player.Animation;
using Systems.Player.State;
using Systems.Player.Progression;
using Systems.Common.Movement;
using Systems.Common.State;
using Systems.Common.Resource;
using Godot;

namespace Tests.Core.Performance
{
    /// <summary>
    /// 長時間実行時の安定性テスト
    /// 実行時間が長く(数十秒～)、テストホストのメモリ状況次第でクラッシュすることも確認されているため、
    /// 通常のdotnet test実行からは除外する（dotnet test --filter "TestCategory=LongRunning" で個別実行）
    /// </summary>
    [Category("LongRunning")]
    public class LongRunningTests : TestBase
    {
        private GameEventBus? _eventBus;
        private PlayerInputViewModel? _inputVm;
        private PlayerMovementViewModel? _movementVm;
        private PlayerCombatViewModel? _combatVm;
        private PlayerAnimationViewModel? _animationVm;
        private PlayerStateViewModel? _stateVm;
        private PlayerProgressionViewModel? _progressionVm;
        private CommonMovementModel? _movementModel;
        private CommonStateModel? _stateModel;
        private CommonResourceModel? _resourceModel;

        protected override void OnSetUp()
        {
            base.OnSetUp();
            
            SafeTestExecution(() =>
            {
                _eventBus = new GameEventBus();
                InitializeViewModels();
                _movementModel = new CommonMovementModel();
                _stateModel = new CommonStateModel();
                _resourceModel = new CommonResourceModel();
                
                _movementModel.Initialize();
                _stateModel.Initialize();
                _resourceModel.Initialize();
            }, "System initialization");
        }

        protected override void OnTearDown()
        {
            SafeTestExecution(() =>
            {
                _inputVm?.Dispose();
                _movementVm?.Dispose();
                _combatVm?.Dispose();
                _animationVm?.Dispose();
                _stateVm?.Dispose();
                _progressionVm?.Dispose();
                _eventBus?.Dispose();
                _movementModel?.Dispose();
                _stateModel?.Dispose();
                _resourceModel?.Dispose();
            }, "System cleanup");
            
            base.OnTearDown();
        }

        private void InitializeViewModels()
        {
            var inputModel = new PlayerInputModel(_eventBus);
            _inputVm = new PlayerInputViewModel(inputModel, _eventBus);
            _inputVm.Initialize();

            var movementModel = new PlayerMovementModel(_eventBus);
            _movementVm = new PlayerMovementViewModel(movementModel, _eventBus);
            _movementVm.Initialize();

            var combatModel = new PlayerCombatModel(_eventBus);
            _combatVm = new PlayerCombatViewModel(combatModel, _eventBus);
            _combatVm.Initialize();

            var animationModel = new PlayerAnimationModel(_eventBus);
            _animationVm = new PlayerAnimationViewModel(animationModel, _eventBus);
            _animationVm.Initialize();

            var stateModel = new PlayerStateModel(_eventBus);
            _stateVm = new PlayerStateViewModel(stateModel, _eventBus);
            _stateVm.Initialize();

            var progressionModel = new PlayerProgressionModel();
            _progressionVm = new PlayerProgressionViewModel(progressionModel, _eventBus);
            _progressionVm.Initialize();
        }

        [Test, MaxTime(30000)] // 30秒制限
        public void ContinuousSystemUpdate_Stability()
        {
            var updateCount = 0;
            var startTime = DateTime.Now;
            var maxUpdates = 10000;

            SafeTestExecution(() =>
            {
                while (updateCount < maxUpdates && (DateTime.Now - startTime).TotalSeconds < 25)
                {
                    _inputVm.UpdateInput();
                    _movementVm.UpdateMovement();
                    _combatVm.UpdateCombat();
                    _animationVm.Update();
                    _stateVm.UpdateState();
                    _progressionVm.Update();

                    updateCount++;

                    // メモリ使用量の監視
                    if (updateCount % 1000 == 0)
                    {
                        CheckMemoryUsage($"Update {updateCount}");
                    }
                }
            }, "Continuous system update");

            Assert.Greater(updateCount, 1000, "Should complete at least 1000 updates");
            AssertNoErrors();
        }

        [Test, MaxTime(60000)] // 60秒制限
        public void EventBusStress_Stability()
        {
            var eventCount = 0;
            var subscribers = new List<IDisposable>();
            var startTime = DateTime.Now;

            SafeTestExecution(() =>
            {
                // 多数のサブスクライバーを作成
                for (int i = 0; i < 100; i++)
                {
                    var subscription = _eventBus.GetEventStream<TestEvent>()
                        .Subscribe(_ => eventCount++);
                    subscribers.Add(subscription);
                }

                // 長時間イベント発行
                while ((DateTime.Now - startTime).TotalSeconds < 50)
                {
                    _eventBus.Publish(new TestEvent());
                    
                    if (eventCount % 10000 == 0)
                    {
                        CheckMemoryUsage($"Event {eventCount}");
                    }
                }
            }, "Event bus stress test");

            // クリーンアップ
            foreach (var subscriber in subscribers)
            {
                subscriber.Dispose();
            }

            Assert.Greater(eventCount, 10000, "Should process at least 10000 events");
            AssertNoErrors();
        }

        [Test, MaxTime(45000)] // 45秒制限
        public void ReactivePropertyStress_Stability()
        {
            var properties = new List<ReactiveProperty<int>>();
            var updateCount = 0;
            var startTime = DateTime.Now;

            SafeTestExecution(() =>
            {
                // 多数のReactivePropertyを作成
                for (int i = 0; i < 1000; i++)
                {
                    var property = new ReactiveProperty<int>(i);
                    property.Subscribe(_ => updateCount++);
                    properties.Add(property);
                }

                // 長時間値更新
                while ((DateTime.Now - startTime).TotalSeconds < 40)
                {
                    for (int i = 0; i < properties.Count; i += 10)
                    {
                        properties[i].Value = updateCount;
                    }

                    if (updateCount % 10000 == 0)
                    {
                        CheckMemoryUsage($"Property update {updateCount}");
                    }
                }
            }, "Reactive property stress test");

            // クリーンアップ
            foreach (var property in properties)
            {
                property.Dispose();
            }

            Assert.Greater(updateCount, 10000, "Should process at least 10000 updates");
            AssertNoErrors();
        }

        [Test, MaxTime(40000)] // 40秒制限
        public void MemoryLeak_Stability()
        {
            var initialMemory = GC.GetTotalMemory(true);
            var memorySnapshots = new List<long>();
            var startTime = DateTime.Now;

            SafeTestExecution(() =>
            {
                while ((DateTime.Now - startTime).TotalSeconds < 35)
                {
                    // オブジェクトの作成と破棄を繰り返し
                    for (int i = 0; i < 100; i++)
                    {
                        var bus = new GameEventBus();
                        var property = new ReactiveProperty<int>(i);
                        
                        using (bus.GetEventStream<TestEvent>().Subscribe(_ => { }))
                        {
                            bus.Publish(new TestEvent());
                        }
                        
                        property.Dispose();
                        bus.Dispose();
                    }

                    // メモリスナップショット
                    GC.Collect();
                    memorySnapshots.Add(GC.GetTotalMemory(true));
                }
            }, "Memory leak test");

            var finalMemory = GC.GetTotalMemory(true);
            var memoryIncrease = finalMemory - initialMemory;

            // メモリ増加が許容範囲内であることを確認
            Assert.LessOrEqual(memoryIncrease, 5 * 1024 * 1024, "Memory leak detected");
            AssertNoErrors();
        }

        [Test, MaxTime(50000)] // 50秒制限
        public void ConcurrentOperations_Stability()
        {
            var operationCount = 0;
            var startTime = DateTime.Now;
            var tasks = new List<Task>();

            SafeTestExecution(() =>
            {
                while ((DateTime.Now - startTime).TotalSeconds < 45)
                {
                    // 並行タスクの作成
                    for (int i = 0; i < 10; i++)
                    {
                        var task = Task.Run(() =>
                        {
                            _inputVm.UpdateInput();
                            _movementVm.UpdateMovement();
                            _combatVm.UpdateCombat();
                            Interlocked.Increment(ref operationCount);
                        });
                        tasks.Add(task);
                    }

                    // 完了したタスクのクリーンアップ
                    tasks.RemoveAll(t => t.IsCompleted);

                    if (operationCount % 1000 == 0)
                    {
                        CheckMemoryUsage($"Concurrent operation {operationCount}");
                    }
                }

                // 残りのタスクの完了を待機
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(5));
            }, "Concurrent operations test");

            Assert.Greater(operationCount, 1000, "Should complete at least 1000 operations");
            AssertNoErrors();
        }

        [Test, MaxTime(35000)] // 35秒制限
        public void SystemRecovery_Stability()
        {
            var recoveryCount = 0;
            var startTime = DateTime.Now;

            SafeTestExecution(() =>
            {
                while ((DateTime.Now - startTime).TotalSeconds < 30)
                {
                    // システムの再初期化を繰り返し
                    _inputVm.Dispose();
                    _movementVm.Dispose();
                    _combatVm.Dispose();
                    _animationVm.Dispose();
                    _stateVm.Dispose();
                    _progressionVm.Dispose();

                    InitializeViewModels();
                    recoveryCount++;

                    // システムの動作確認
                    _inputVm.UpdateInput();
                    _movementVm.UpdateMovement();
                    _combatVm.UpdateCombat();
                    _animationVm.Update();
                    _stateVm.UpdateState();
                    _progressionVm.Update();

                    if (recoveryCount % 10 == 0)
                    {
                        CheckMemoryUsage($"Recovery {recoveryCount}");
                    }
                }
            }, "System recovery test");

            Assert.Greater(recoveryCount, 10, "Should complete at least 10 recoveries");
            AssertNoErrors();
        }

        [Test, MaxTime(55000)] // 55秒制限
        public void MixedWorkload_Stability()
        {
            var workloadCount = 0;
            var startTime = DateTime.Now;
            var random = new Random();

            SafeTestExecution(() =>
            {
                while ((DateTime.Now - startTime).TotalSeconds < 50)
                {
                    // ランダムなワークロード
                    switch (random.Next(5))
                    {
                        case 0:
                            _inputVm.UpdateInput();
                            break;
                        case 1:
                            _movementVm.UpdateMovement();
                            break;
                        case 2:
                            _combatVm.UpdateCombat();
                            break;
                        case 3:
                            _animationVm.Update();
                            break;
                        case 4:
                            _stateVm.UpdateState();
                            _progressionVm.Update();
                            break;
                    }

                    workloadCount++;

                    if (workloadCount % 1000 == 0)
                    {
                        CheckMemoryUsage($"Mixed workload {workloadCount}");
                    }
                }
            }, "Mixed workload test");

            Assert.Greater(workloadCount, 1000, "Should complete at least 1000 workloads");
            AssertNoErrors();
        }

        [Test, MaxTime(25000)] // 25秒制限
        public void PerformanceRegression_Stability()
        {
            var baselineTime = TimeSpan.Zero;
            var regressionCount = 0;
            var startTime = DateTime.Now;

            SafeTestExecution(() =>
            {
                while ((DateTime.Now - startTime).TotalSeconds < 20)
                {
                    var operationStart = DateTime.Now;

                    // 標準的な操作セット
                    for (int i = 0; i < 100; i++)
                    {
                        _inputVm.UpdateInput();
                        _movementVm.UpdateMovement();
                        _combatVm.UpdateCombat();
                        _animationVm.Update();
                        _stateVm.UpdateState();
                        _progressionVm.Update();
                    }

                    var operationTime = DateTime.Now - operationStart;

                    if (baselineTime == TimeSpan.Zero)
                    {
                        baselineTime = operationTime;
                    }
                    else if (operationTime > baselineTime * 2)
                    {
                        regressionCount++;
                    }

                    if (regressionCount > 5)
                    {
                        Assert.Fail("Performance regression detected");
                    }
                }
            }, "Performance regression test");

            AssertNoErrors();
        }

        [Test]
        public void LongRunningMovement_Test()
        {
            // 長時間の移動処理テスト
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _movementModel.Move(new Vector2(1, 0));
                    _movementModel.Update();
                    
                    if (i % 100 == 0)
                    {
                        CheckMemoryUsage($"Movement iteration {i}");
                    }
                }
            }, "Long running movement", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningStateTransitions_Test()
        {
            // 長時間の状態遷移テスト
            MeasurePerformance(() =>
            {
                var states = new[] { "Idle", "Moving", "Attacking", "Damaged" };
                
                for (int i = 0; i < 1000; i++)
                {
                    var state = states[i % states.Length];
                    _stateModel.ChangeState(state);
                    _stateModel.Update();
                    
                    if (i % 100 == 0)
                    {
                        CheckMemoryUsage($"State transition iteration {i}");
                    }
                }
            }, "Long running state transitions", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningResourceOperations_Test()
        {
            // 長時間のリソース操作テスト
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _resourceModel.UnloadResource("Stamina");
                    _resourceModel.Update();
                    
                    if (i % 100 == 0)
                    {
                        CheckMemoryUsage($"Resource operation iteration {i}");
                    }
                }
            }, "Long running resource operations", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningEventPublishing_Test()
        {
            // 長時間のイベント発行テスト
            var eventCount = 0;
            
            using (_eventBus.GetEventStream<TestEvent>()
                .Subscribe(_ => eventCount++))
            {
                MeasurePerformance(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _eventBus.Publish(new TestEvent());
                        
                        if (i % 100 == 0)
                        {
                            CheckMemoryUsage($"Event publishing iteration {i}");
                        }
                    }
                }, "Long running event publishing", 60000);
            }
            
            Assert.AreEqual(1000, eventCount, "All events should be received");
            AssertNoErrors();
        }

        [Test]
        public void LongRunningConcurrentOperations_Test()
        {
            // 長時間の並行操作テスト
            MeasurePerformance(() =>
            {
                var tasks = new Task[5];
                
                for (int taskIndex = 0; taskIndex < 5; taskIndex++)
                {
                    tasks[taskIndex] = Task.Run(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            _movementModel.Move(new Vector2(1, 0));
                            _stateModel.ChangeState("Moving");
                            _resourceModel.UnloadResource("Stamina");
                        }
                    });
                }
                
                Task.WaitAll(tasks);
            }, "Long running concurrent operations", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningMemoryStress_Test()
        {
            // 長時間のメモリストレステスト
            CheckMemoryUsage("Before memory stress test");
            
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    var tempModels = new CommonMovementModel[10];
                    
                    for (int j = 0; j < 10; j++)
                    {
                        tempModels[j] = new CommonMovementModel();
                        tempModels[j].Initialize();
                        tempModels[j].Move(new Vector2(1, 0));
                    }
                    
                    for (int j = 0; j < 10; j++)
                    {
                        tempModels[j].Dispose();
                    }
                    
                    if (i % 10 == 0)
                    {
                        CheckMemoryUsage($"Memory stress iteration {i}");
                    }
                }
            }, "Long running memory stress", 60000);
            
            CheckMemoryUsage("After memory stress test");
            AssertNoErrors();
        }

        [Test]
        public void LongRunningErrorRecovery_Test()
        {
            // 長時間のエラー回復テスト
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    try
                    {
                        // 時々エラーを発生させる
                        if (i % 100 == 0)
                        {
                            _movementModel.Move(new Vector2(float.NaN, 0));
                        }
                        else
                        {
                            _movementModel.Move(new Vector2(1, 0));
                        }
                        
                        _movementModel.Update();
                    }
                    catch
                    {
                        // エラーを無視して継続
                    }
                    
                    if (i % 100 == 0)
                    {
                        CheckMemoryUsage($"Error recovery iteration {i}");
                    }
                }
            }, "Long running error recovery", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningSystemIntegration_Test()
        {
            // 長時間のシステム統合テスト
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    // 全システムの統合更新
                    _movementModel.Move(new Vector2(1, 0));
                    _movementModel.Update();
                    
                    _stateModel.ChangeState("Moving");
                    _stateModel.Update();
                    
                    _resourceModel.UnloadResource("Stamina");
                    _resourceModel.Update();
                    
                    if (i % 100 == 0)
                    {
                        CheckMemoryUsage($"System integration iteration {i}");
                    }
                }
            }, "Long running system integration", 60000);
            
            AssertNoErrors();
        }

        [Test]
        public void LongRunningAsyncOperations_Test()
        {
            // 長時間の非同期操作テスト
            SafeTestExecution(async () =>
            {
                MeasurePerformance(async () =>
                {
                    var tasks = new Task[10];
                    
                    for (int i = 0; i < 10; i++)
                    {
                        tasks[i] = Task.Run(async () =>
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                _movementModel.Move(new Vector2(1, 0));
                                _stateModel.ChangeState("Moving");
                                _resourceModel.UnloadResource("Stamina");
                                
                                await Task.Delay(1); // 短い遅延
                            }
                        });
                    }
                    
                    await Task.WhenAll(tasks);
                }, "Long running async operations", 60000);
            }, "Async operations");
            
            AssertNoErrors();
        }

        private class TestEvent : GameEvent { }
    }
} 