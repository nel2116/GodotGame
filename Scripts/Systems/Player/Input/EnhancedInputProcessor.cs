using Core.Events;
using Godot;

namespace Systems.Player.Input
{
    /// <summary>
    /// 入力バッファリングを備えた入力モデル。
    /// </summary>
    public class EnhancedInputProcessor : PlayerInputModel
    {
        private readonly InputBuffer _buffer = new();
        private Vector2 _pendingBufferedMovement = Vector2.Zero;

        public EnhancedInputProcessor(IGameEventBus bus) : base(bus)
        {
        }

        protected override void ProcessInput()
        {
            _buffer.CollectInputState(CurrentState);
            _pendingBufferedMovement = _buffer.GetMovement();

            var bufferedAction = _buffer.PopAction();
            if (bufferedAction != null)
            {
                ExecuteAction(bufferedAction);
            }
            else
            {
                ExecuteDefaultActions();
            }
        }

        protected override void HandleMoveAction()
        {
            if (_pendingBufferedMovement != Vector2.Zero)
            {
                PublishMovementEvent(_pendingBufferedMovement);
                _pendingBufferedMovement = Vector2.Zero;
                return;
            }

            base.HandleMoveAction();
        }
    }
}
