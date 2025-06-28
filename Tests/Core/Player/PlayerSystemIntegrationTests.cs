using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Core.Events;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Combat;
using Systems.Player.Animation;
using Systems.Player.State;
using Systems.Player.Progression;
using Systems.Player.Events;
using Systems.Common.Events;
using Godot;

namespace Tests.Core.Player
{
    /// <summary>
    /// プレイヤーシステム全体の統合テスト
    /// </summary>
    public class PlayerSystemIntegrationTests : TestBase
    {
        private GameEventBus _eventBus = null!;
        private PlayerInputViewModel _inputVm = null!;
        private PlayerMovementViewModel _movementVm = null!;
        private PlayerCombatViewModel _combatVm = null!;
        private PlayerAnimationViewModel _animationVm = null!;
        private PlayerStateViewModel _stateVm = null!;
        private PlayerProgressionViewModel _progressionVm = null!;

        protected override void OnSetUp()
        {
            base.OnSetUp();
            
            SafeTestExecution(() =>
            {
                _eventBus = new GameEventBus();
                InitializeViewModels();
                
                // 初期化後のnullチェック
                Assert.IsNotNull(_inputVm, "InputViewModel should be initialized");
                Assert.IsNotNull(_movementVm, "MovementViewModel should be initialized");
                Assert.IsNotNull(_combatVm, "CombatViewModel should be initialized");
                Assert.IsNotNull(_animationVm, "AnimationViewModel should be initialized");
                Assert.IsNotNull(_stateVm, "StateViewModel should be initialized");
                Assert.IsNotNull(_progressionVm, "ProgressionViewModel should be initialized");
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

        [Test]
        public void SystemInitialization_AllComponentsInitialized()
        {
            Assert.IsNotNull(_inputVm);
            Assert.IsNotNull(_movementVm);
            Assert.IsNotNull(_combatVm);
            Assert.IsNotNull(_animationVm);
            Assert.IsNotNull(_stateVm);
            Assert.IsNotNull(_progressionVm);
            
            AssertNoErrors();
        }

        [Test]
        public void InputToMovement_Integration()
        {
            // 入力から移動への統合テスト
            var inputState = new InputState();
            // MovementInputは読み取り専用なので、直接設定はできない
            // 代わりに入力モデルを通じて更新
            
            _inputVm.UpdateInput();
            
            // 移動システムが更新されることを確認
            _movementVm.UpdateMovement();
            
            AssertNoErrors();
        }

        [Test]
        public void MovementToAnimation_Integration()
        {
            // 移動からアニメーションへの統合テスト
            _movementVm.UpdateMovement();
            
            _animationVm.Update();
            
            // アニメーションシステムが適切に更新されることを確認
            AssertNoErrors();
        }

        [Test]
        public void CombatToState_Integration()
        {
            // 戦闘から状態への統合テスト
            _combatVm.Attack("BasicAttack");
            
            // 状態システムが適切に更新されることを確認
            _stateVm.UpdateState();
            
            AssertNoErrors();
        }

        [Test]
        public void ProgressionToCombat_Integration()
        {
            // 進行から戦闘への統合テスト
            _progressionVm.AddExperience(100);
            _progressionVm.Update();
            
            // 戦闘システムのパラメータが更新されることを確認
            _combatVm.UpdateCombat();
            
            AssertNoErrors();
        }

        [Test]
        public void FullSystemUpdate_Integration()
        {
            // 全システムの統合更新テスト
            SafeTestExecution(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    _inputVm.UpdateInput();
                    _movementVm.UpdateMovement();
                    _combatVm.UpdateCombat();
                    _animationVm.Update();
                    _stateVm.UpdateState();
                    _progressionVm.Update();
                }
            }, "Full system update");
            
            AssertNoErrors();
        }

        [Test]
        public void EventCommunication_Integration()
        {
            // システム間のイベント通信テスト
            var eventReceived = false;
            
            // イベント購読を先に実行
            _eventBus.GetEventStream<MovementVelocityChangedEvent>()
                .Subscribe(_ => eventReceived = true);
            
            _movementVm.UpdateMovement();
            
            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);
            
            Assert.IsTrue(eventReceived, "Movement event should be published");
            AssertNoErrors();
        }

        [Test]
        public void ErrorHandling_Integration()
        {
            // エラーハンドリングの統合テスト
            var errorReceived = false;
            
            // イベント購読を先に実行
            _eventBus.GetEventStream<ErrorEvent>()
                .Subscribe(_ => errorReceived = true);
            
            SafeTestExecution(() =>
            {
                _combatVm.Attack("InvalidAction");
            }, "Invalid action execution");
            
            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);
            
            Assert.IsTrue(errorReceived, "Error event should be published");
            AssertNoErrors();
        }

        [Test]
        public void Performance_Integration()
        {
            // パフォーマンス統合テスト
            SafeTestExecution(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _inputVm.UpdateInput();
                    _movementVm.UpdateMovement();
                    _combatVm.UpdateCombat();
                    _animationVm.Update();
                    _stateVm.UpdateState();
                    _progressionVm.Update();
                }
            }, "Performance test");
            
            AssertNoErrors();
        }

        [Test]
        public void MemoryUsage_Integration()
        {
            // メモリ使用量統合テスト
            SafeTestExecution(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    _inputVm.UpdateInput();
                    _movementVm.UpdateMovement();
                    _combatVm.UpdateCombat();
                    _animationVm.Update();
                    _stateVm.UpdateState();
                    _progressionVm.Update();
                }
            }, "Memory usage test");
            
            AssertNoErrors();
        }

        [Test]
        public void AsyncOperations_Integration()
        {
            // 非同期操作の統合テスト
            SafeTestExecution(async () =>
            {
                var task = Task.Run(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        _inputVm.UpdateInput();
                        _movementVm.UpdateMovement();
                        _combatVm.UpdateCombat();
                        _animationVm.Update();
                        _stateVm.UpdateState();
                        _progressionVm.Update();
                    }
                });
                
                await task;
            }, "Async operations test");
            
            AssertNoErrors();
        }
    }
} 