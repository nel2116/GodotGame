using Godot;

#nullable enable

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Defending,
    Dead
}

[GlobalClass]
public partial class PlayerStateMachine : Node
{
    // 現在のプレイヤー状態
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    // 状態変更時にイベントバスへ通知するための参照
    public EventBus? EventBus { get; set; }

    // 状態を変更してイベント通知を行う
    public void ChangeState(PlayerState new_state)
    {
        if (CurrentState == new_state)
        {
            return;
        }

        var previous_state = CurrentState;
        CurrentState = new_state;
        EventBus?.EmitEvent("PlayerStateChanged", new Godot.Collections.Dictionary
        {
            {"from", (int)previous_state},
            {"to", (int)new_state}
        });
    }
}

