using Systems.Player.Events;
using Core.Events;
using Godot;

namespace Systems.Player.Input
{
    /// <summary>
    /// 拡張された入力処理システム
    /// </summary>
    public class EnhancedInputProcessor
    {
        private readonly InputState _state = new();
        private readonly InputBuffer _buffer;
        private readonly IGameEventBus _event_bus;

        public EnhancedInputProcessor(IGameEventBus bus)
        {
            _event_bus = bus;
            _buffer = new InputBuffer(bus);
        }

        /// <summary>
        /// 入力を更新して処理する
        /// </summary>
        public void Update()
        {
            _state.Update();
            if (_state.ButtonStates.TryGetValue("Dash", out var dash) && dash)
            {
                _buffer.BufferAction("Dash");
            }
            if (_state.ButtonStates.TryGetValue("Jump", out var jump) && jump)
            {
                _buffer.BufferAction("Jump");
            }
            if (_state.ButtonStates.TryGetValue("Attack", out var atk) && atk)
            {
                _buffer.BufferAction("Attack");
            }
            if (_state.MovementInput != Vector2.Zero)
            {
                _buffer.BufferMovement(_state.MovementInput);
            }

            var action = _buffer.PopAction();
            if (action != null)
            {
                switch (action)
                {
                    case "Dash":
                        _event_bus.Publish(new DashInputEvent());
                        break;
                    case "Jump":
                        _event_bus.Publish(new JumpInputEvent());
                        break;
                    case "Attack":
                        _event_bus.Publish(new AttackInputEvent());
                        break;
                }
            }

            var move = _buffer.GetMovement();
            if (move != Vector2.Zero)
            {
                _event_bus.Publish(new MovementInputEvent(move.Normalized()));
            }

            _event_bus.Publish(new InputStateChangedEvent(_state));
        }
    }
}
