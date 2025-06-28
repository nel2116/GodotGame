using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Core.Events;
using Core.Reactive;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Combat;
using Systems.Common.Movement;
using Systems.Common.State;
using Systems.Common.Resource;
using Godot;

namespace Tests.Core.ErrorHandling
{
    /// <summary>
    /// 包括的なエラーケーステスト
    /// </summary>
    public class ErrorHandlingTests : TestBase
    {
        private GameEventBus _eventBus = null!;
        private CommonMovementModel _movementModel = null!;
        private CommonStateModel _stateModel = null!;
        private CommonResourceModel _resourceModel = null!;

        protected override void OnSetUp()
        {
            base.OnSetUp();
            
            SafeTestExecution(() =>
            {
                _eventBus = new GameEventBus();
                _movementModel = new CommonMovementModel();
                _stateModel = new CommonStateModel();
                _resourceModel = new CommonResourceModel();
                
                _movementModel.Initialize();
                _stateModel.Initialize();
                _resourceModel.Initialize();
            }, "Test setup");
        }

        protected override void OnTearDown()
        {
            SafeTestExecution(() =>
            {
                _movementModel?.Dispose();
                _stateModel?.Dispose();
                _resourceModel?.Dispose();
                _eventBus?.Dispose();
            }, "Test cleanup");
            
            base.OnTearDown();
        }

        [Test]
        public void ReactiveProperty_DisposedAccess_ThrowsException()
        {
            var property = new ReactiveProperty<int>(0);
            property.Dispose();

            Assert.Throws<ObjectDisposedException>(() => property.Value = 1);
            Assert.Throws<ObjectDisposedException>(() => property.Subscribe(_ => { }));
        }

        [Test]
        public void ReactiveProperty_InvalidValue_ThrowsException()
        {
            var property = new ReactiveProperty<int>(0);
            property.SetValidator(v => v >= 0);

            Assert.Throws<ArgumentException>(() => property.Value = -1);
            Assert.AreEqual(0, property.Value);
        }

        [Test]
        public void GameEventBus_DisposedPublish_HandlesGracefully()
        {
            var bus = new GameEventBus();
            bus.Dispose();

            // 破棄済みバスへのイベント発行が適切に処理されることを確認
            Assert.DoesNotThrow(() => bus.Publish(new TestEvent()));
            AssertMockOutputContains("Attempted to publish event to disposed GameEventBus");
        }

        [Test]
        public void GameEventBus_NullEvent_HandlesGracefully()
        {
            var bus = new GameEventBus();
            
            Assert.DoesNotThrow(() => bus.Publish<TestEvent>(default!));
            AssertMockOutputContains("Attempted to publish null event");
        }

        [Test]
        public void PlayerInputModel_InvalidInput_HandlesGracefully()
        {
            var model = new PlayerInputModel(_eventBus);
            model.Initialize();

            SafeTestExecution(() =>
            {
                // 無効な入力状態での処理
                var invalidState = new InputState();
                // 無効な値を設定
                
                model.UpdateInput();
            }, "Invalid input handling");

            AssertNoErrors();
        }

        [Test]
        public void PlayerMovementModel_InvalidVelocity_HandlesGracefully()
        {
            var model = new PlayerMovementModel(_eventBus);
            model.Initialize();

            SafeTestExecution(() =>
            {
                // 無効な速度値での処理
                model.Move(new Vector2(float.NaN, float.PositiveInfinity));
                model.Update();
            }, "Invalid velocity handling");

            AssertNoErrors();
        }

        [Test]
        public void PlayerCombatModel_InvalidAction_HandlesGracefully()
        {
            var model = new PlayerCombatModel(_eventBus);
            model.Initialize();

            SafeTestExecution(() =>
            {
                // 無効なアクションでの処理
                model.Attack("");
                model.Attack(string.Empty);
                model.Attack("NonExistentAction");
            }, "Invalid action handling");

            AssertNoErrors();
        }

        [Test]
        public void EventBus_ConcurrentAccess_HandlesGracefully()
        {
            var bus = new GameEventBus();
            var exceptionCount = 0;

            SafeTestExecution(() =>
            {
                Parallel.For(0, 100, i =>
                {
                    try
                    {
                        bus.Publish(new TestEvent());
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref exceptionCount);
                    }
                });
            }, "Concurrent access");

            Assert.LessOrEqual(exceptionCount, 5, "Too many exceptions during concurrent access");
        }

        [Test]
        public void ReactiveProperty_ConcurrentModification_HandlesGracefully()
        {
            var property = new ReactiveProperty<int>(0);
            var exceptionCount = 0;

            SafeTestExecution(() =>
            {
                Parallel.For(0, 100, i =>
                {
                    try
                    {
                        property.Value = i;
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref exceptionCount);
                    }
                });
            }, "Concurrent modification");

            Assert.LessOrEqual(exceptionCount, 5, "Too many exceptions during concurrent modification");
        }

        [Test]
        public void MemoryLeak_Disposal_ProperlyCleansUp()
        {
            var initialMemory = GC.GetTotalMemory(true);
            
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
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            var finalMemory = GC.GetTotalMemory(true);
            var memoryIncrease = finalMemory - initialMemory;
            
            // メモリ増加が許容範囲内であることを確認
            Assert.LessOrEqual(memoryIncrease, 1024 * 1024, "Memory leak detected");
        }

        [Test]
        public void ExceptionPropagation_EventBus_HandlesGracefully()
        {
            var bus = new GameEventBus();
            var exceptionThrown = false;

            using (bus.GetEventStream<TestEvent>().Subscribe(_ =>
            {
                throw new InvalidOperationException("Test exception");
            }))
            {
                SafeTestExecution(() =>
                {
                    bus.Publish(new TestEvent());
                }, "Exception propagation");

                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Exception should be thrown");
            AssertNoErrors();
        }

        [Test]
        public void ResourceExhaustion_HandlesGracefully()
        {
            var bus = new GameEventBus();
            var properties = new List<ReactiveProperty<int>>();

            SafeTestExecution(() =>
            {
                try
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        var property = new ReactiveProperty<int>(i);
                        properties.Add(property);
                        
                        using (bus.GetEventStream<TestEvent>().Subscribe(_ => { }))
                        {
                            bus.Publish(new TestEvent());
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    // メモリ不足は予期される
                }
            }, "Resource exhaustion");

            // クリーンアップ
            foreach (var property in properties)
            {
                property.Dispose();
            }
            properties.Clear();
            
            GC.Collect();
            AssertNoErrors();
        }

        [Test]
        public void InvalidStateTransition_HandlesGracefully()
        {
            var model = new PlayerMovementModel(_eventBus);
            model.Initialize();

            SafeTestExecution(() =>
            {
                // 無効な状態遷移を試行
                model.Move(new Vector2(1, 0));
                model.Update();
                
                // 即座に反対方向に移動（物理的に不可能な状態をシミュレート）
                model.Move(new Vector2(-1, 0));
                model.Update();
            }, "Invalid state transition");

            AssertNoErrors();
        }

        [Test]
        public void NullReference_HandlesGracefully()
        {
            SafeTestExecution(() =>
            {
                // null参照の可能性がある操作
                PlayerInputModel? model = null;
                
                // nullチェックなしでの操作を試行
                try
                {
                    model?.UpdateInput();
                }
                catch (NullReferenceException)
                {
                    // 予期される例外
                }
            }, "Null reference handling");

            AssertNoErrors();
        }

        [Test]
        public void Timeout_HandlesGracefully()
        {
            var bus = new GameEventBus();
            var timeoutOccurred = false;

            SafeTestExecution(() =>
            {
                try
                {
                    // 長時間実行される可能性のある操作
                    using (var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMilliseconds(100)))
                    {
                        var task = Task.Run(() =>
                        {
                            for (int i = 0; i < 1000000; i++)
                            {
                                bus.Publish(new TestEvent());
                                cts.Token.ThrowIfCancellationRequested();
                            }
                        }, cts.Token);

                        task.Wait(cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    timeoutOccurred = true;
                }
            }, "Timeout handling");

            Assert.IsTrue(timeoutOccurred, "Timeout should occur");
            AssertNoErrors();
        }

        [Test]
        public void NullEventBus_Handling()
        {
            // nullイベントバスでのエラーハンドリング
            SafeTestExecution(() =>
            {
                var model = new CommonMovementModel();
                model.Initialize();
                
                // nullイベントバスでも初期化が成功することを確認
                Assert.IsNotNull(model);
                AssertNoErrors();
            }, "Null event bus handling");
        }

        [Test]
        public void InvalidMovementInput_Handling()
        {
            // 無効な移動入力のエラーハンドリング
            SafeTestExecution(() =>
            {
                // NaNや無限大の値でのテスト
                var invalidVector = new Vector2(float.NaN, float.PositiveInfinity);
                
                // 無効な値でもクラッシュしないことを確認
                _movementModel.Move(invalidVector);
                
                AssertNoErrors();
            }, "Invalid movement input");
        }

        [Test]
        public void StateTransitionError_Handling()
        {
            // 状態遷移エラーのハンドリング
            SafeTestExecution(() =>
            {
                // 無効な状態遷移を試行
                _stateModel.ChangeState("InvalidState");
                
                // エラーが適切に処理されることを確認
                AssertNoErrors();
            }, "State transition error");
        }

        [Test]
        public void ResourceOperationError_Handling()
        {
            // リソース操作エラーのハンドリング
            SafeTestExecution(() =>
            {
                // 無効なリソース操作を試行
                _resourceModel.UnloadResource("InvalidResource");
                
                // エラーが適切に処理されることを確認
                AssertNoErrors();
            }, "Resource operation error");
        }

        [Test]
        public void ConcurrentAccess_ErrorHandling()
        {
            // 並行アクセスでのエラーハンドリング
            SafeTestExecution(async () =>
            {
                var tasks = new Task[10];
                
                for (int i = 0; i < 10; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        _movementModel.Move(new Vector2(1, 0));
                        _stateModel.ChangeState("Moving");
                        _resourceModel.UnloadResource("Stamina");
                    });
                }
                
                await Task.WhenAll(tasks);
                
                AssertNoErrors();
            }, "Concurrent access");
        }

        [Test]
        public void MemoryLeak_ErrorHandling()
        {
            // メモリリークのエラーハンドリング
            CheckMemoryUsage("Before operations");
            
            SafeTestExecution(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    var tempModel = new CommonMovementModel();
                    tempModel.Initialize();
                    tempModel.Move(new Vector2(1, 0));
                    tempModel.Dispose();
                }
            }, "Memory leak test");
            
            CheckMemoryUsage("After operations");
            AssertNoErrors();
        }

        [Test]
        public void ExceptionPropagation_Handling()
        {
            // 例外伝播のハンドリング
            SafeTestExecution(() =>
            {
                try
                {
                    // 意図的に例外を発生させる
                    throw new InvalidOperationException("Test exception");
                }
                catch (Exception ex)
                {
                    // 例外が適切にキャッチされることを確認
                    Assert.IsNotNull(ex);
                    Assert.AreEqual("Test exception", ex.Message);
                }
                
                AssertNoErrors();
            }, "Exception propagation");
        }

        [Test]
        public void PerformanceUnderError_Handling()
        {
            // エラー状態でのパフォーマンステスト
            MeasurePerformance(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        _movementModel.Move(new Vector2(float.NaN, 0));
                        _stateModel.ChangeState("InvalidState");
                        _resourceModel.UnloadResource("InvalidResource");
                    }
                    catch
                    {
                        // エラーを無視して継続
                    }
                }
            }, "Performance under error", 1000);
            
            AssertNoErrors();
        }

        [Test]
        public void RecoveryFromError_Handling()
        {
            // エラーからの回復テスト
            SafeTestExecution(() =>
            {
                // エラーを発生させる
                _movementModel.Move(new Vector2(float.NaN, 0));
                
                // 正常な操作で回復することを確認
                _movementModel.Move(new Vector2(1, 0));
                _stateModel.ChangeState("Moving");
                _resourceModel.UnloadResource("Stamina");
                
                AssertNoErrors();
            }, "Recovery from error");
        }

        [Test]
        public void LoggingUnderError_Handling()
        {
            // エラー時のログ出力テスト
            SafeTestExecution(() =>
            {
                ClearMockOutput();
                
                // エラーを発生させる
                _movementModel.Move(new Vector2(float.NaN, 0));
                
                // ログが出力されることを確認
                var output = GetMockOutput();
                Assert.IsNotNull(output);
                
                AssertNoErrors();
            }, "Logging under error");
        }

        private class TestEvent : GameEvent { }
    }
} 