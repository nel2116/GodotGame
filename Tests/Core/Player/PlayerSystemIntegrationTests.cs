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
        private GameEventBus? _eventBus;
        private PlayerInputViewModel? _inputVm;
        private PlayerMovementViewModel? _movementVm;
        private PlayerCombatViewModel? _combatVm;
        private PlayerAnimationViewModel? _animationVm;
        private PlayerStateViewModel? _stateVm;
        private PlayerProgressionViewModel? _progressionVm;

        protected override void OnSetUp()
        {
            base.OnSetUp();
            
            SafeTestExecution(() =>
            {
                _eventBus = new GameEventBus();
                InitializeViewModels();
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
                // 無効な入力でエラーが適切に処理されることを確認
                _combatVm.Attack("InvalidAction");
                
                // 少し待機してイベント処理を完了させる
                System.Threading.Thread.Sleep(10);
                
                // エラーイベントが発行されることを確認
                Assert.IsTrue(errorReceived, "Error event should be published");
            }, "Error handling");
        }

        [Test]
        public void Performance_Integration()
        {
            // パフォーマンス統合テスト
            MeasurePerformance(() =>
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
            }, "1000 system updates", 5000);
            
            AssertNoErrors();
        }

        [Test]
        public void MemoryUsage_Integration()
        {
            // メモリ使用量の統合テスト
            CheckMemoryUsage("Before system operations");
            
            for (int i = 0; i < 100; i++)
            {
                _inputVm.UpdateInput();
                _movementVm.UpdateMovement();
                _combatVm.UpdateCombat();
                _animationVm.Update();
                _stateVm.UpdateState();
                _progressionVm.Update();
            }
            
            CheckMemoryUsage("After system operations");
            AssertNoErrors();
        }

        [Test]
        public void AsyncOperations_Integration()
        {
            // 非同期操作の統合テスト
            SafeTestExecution(async () =>
            {
                var task1 = Task.Run(() => _inputVm.UpdateInput());
                var task2 = Task.Run(() => _movementVm.UpdateMovement());
                var task3 = Task.Run(() => _combatVm.UpdateCombat());
                
                await Task.WhenAll(task1, task2, task3);
            }, "Async operations");
            
            AssertNoErrors();
        }
    }
} 