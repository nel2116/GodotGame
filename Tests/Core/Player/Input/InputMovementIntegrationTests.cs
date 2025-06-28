using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Events;
using Core.Events;
using Godot;
using System.Reflection;
using System;

namespace Tests.Core.Player.Input
{
    public class InputMovementIntegrationTests
    {
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field!.GetValue(obj)!;
        }

        [Test]
        public void InputModel_Move_UpdatesMovementModel()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            MovementInputEvent? movementEvent = null;
            bus.GetEventStream<MovementInputEvent>().Subscribe(e => movementEvent = e);
            
            var inputModel = new PlayerInputModel(bus);
            var movementModel = new PlayerMovementModel(bus);
            
            // 両方のモデルを初期化（イベント購読を先に行うため、movementModelを先に初期化）
            movementModel.Initialize();
            inputModel.Initialize();

            // イベント発行の可視化
            bus.GetEventStream<MovementInputEvent>().Subscribe(e => Console.WriteLine($"Event published: {e.Direction}"));

            // InputStateを取得して設定
            var state = GetPrivateField<InputState>(inputModel, "_currentState");
            if (state != null)
            {
                state.SetMovementInput(new Vector2(1, 0));
            }

            // デバッグ出力
            Console.WriteLine($"MovementInput before UpdateInput: {state?.MovementInput}");

            // 正しい入力更新フローを実行（UpdateInputを使用）
            inputModel.UpdateInput();

            // バッファリング遅延を考慮して待機
            System.Threading.Thread.Sleep(20);

            // 移動モデルの状態を更新
            movementModel.Update();

            // デバッグ出力
            Console.WriteLine($"Velocity after Update: {movementModel.Velocity}");

            // 移動モデルの速度を確認（期待値は5.0f * 0.9 = 4.5, 0）
            Assert.AreEqual(new Vector2(4.5f, 0), movementModel.Velocity);
        }
    }
}

