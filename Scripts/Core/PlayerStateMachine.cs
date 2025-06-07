using Godot;
using System.Collections.Generic;

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

    // 前の状態
    private PlayerState previous_state = PlayerState.Idle;

    // 状態遷移表
    private readonly Dictionary<PlayerState, List<PlayerState>> transitions = new()
    {
        { PlayerState.Idle, new List<PlayerState> { PlayerState.Moving, PlayerState.Attacking, PlayerState.Defending } },
        { PlayerState.Moving, new List<PlayerState> { PlayerState.Idle, PlayerState.Attacking, PlayerState.Defending } },
        { PlayerState.Attacking, new List<PlayerState> { PlayerState.Idle, PlayerState.Defending } },
        { PlayerState.Defending, new List<PlayerState> { PlayerState.Idle, PlayerState.Attacking } },
        { PlayerState.Dead, new List<PlayerState>() }
    };

    // 各状態のタイムアウト秒
    // 各状態のタイムアウト秒 (キーは int で管理)
    public Godot.Collections.Dictionary Timeouts { get; } = new Godot.Collections.Dictionary
    {
        {(int)PlayerState.Attacking, 1.0f},
        {(int)PlayerState.Defending, 1.0f}
    };

    private double state_start_time;

    // 状態変更イベント通知
    public EventBus? EventBus { get; set; }

    // 状態管理連携
    public StateManager? StateManager { get; set; }

    public override void _Process(double delta)
    {
        if (Timeouts.ContainsKey((int)CurrentState))
        {
            var limit = (float)Timeouts[(int)CurrentState];
            var now = Time.GetTicksMsec() / 1000.0;
            if (now - state_start_time >= limit)
            {
                CancelState();
            }
        }
    }

    // 遷移可能か判定する
    public bool CanTransition(PlayerState to)
    {
        return transitions.TryGetValue(CurrentState, out var list) && list.Contains(to);
    }

    // 状態を変更して通知する
    public void ChangeState(PlayerState new_state)
    {
        if (CurrentState == new_state || !CanTransition(new_state))
        {
            return;
        }

        previous_state = CurrentState;
        CurrentState = new_state;
        state_start_time = Time.GetTicksMsec() / 1000.0;

        EventBus?.EmitEvent("PlayerStateChanged", new Godot.Collections.Dictionary
        {
            {"from", (int)previous_state},
            {"to", (int)new_state}
        });

        StateManager?.SetState("PlayerState", (int)new_state);
    }

    // 現在の状態をキャンセルし Idle に戻す
    public void CancelState()
    {
        ChangeState(PlayerState.Idle);
    }
}

