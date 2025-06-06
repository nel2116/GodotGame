using Godot;
using System.Collections.Generic;

#nullable enable

public enum GameState
{
    Title,
    Menu,
    Gameplay,
    Pause,
    GameOver
}

[GlobalClass]
public partial class StateManager : Node
{
    // 現在の状態を保持する辞書
    private readonly Dictionary<string, Variant> current_states = new();

    // 各状態の履歴を保持する辞書
    private readonly Dictionary<string, List<Variant>> state_history = new();

    // 状態を設定して履歴に追加する
    public void SetState(string state_name, Variant value)
    {
        current_states[state_name] = value;
        if (!state_history.ContainsKey(state_name))
        {
            state_history[state_name] = new List<Variant>();
        }
        state_history[state_name].Add(value);
    }

    // 状態を取得する。存在しない場合は空の Variant
    public Variant GetState(string state_name)
    {
        return current_states.TryGetValue(state_name, out var value) ? value : new Variant();
    }

    // 状態の存在有無を返す
    public bool HasState(string state_name)
    {
        return current_states.ContainsKey(state_name);
    }

    // 状態を削除する
    public void RemoveState(string state_name)
    {
        current_states.Remove(state_name);
    }

    // 履歴を取得する
    public IList<Variant> GetStateHistory(string state_name)
    {
        return state_history.TryGetValue(state_name, out var history) ? history : new List<Variant>();
    }
}

