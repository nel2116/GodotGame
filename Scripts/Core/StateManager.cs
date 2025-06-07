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

    // 履歴を保持する辞書
    private readonly Dictionary<string, List<Variant>> state_history = new();

    // 遷移テーブル
    private readonly Dictionary<string, Dictionary<Variant, List<Variant>>> transitions = new();

    // 監視コールバック
    private readonly Dictionary<string, List<Callable>> observers = new();

    // 状態を設定して履歴に追加する
    public void SetState(string state_name, Variant value)
    {
        Variant? prev = null;
        if (current_states.TryGetValue(state_name, out var existingState))
        {
            prev = existingState;
            if (!CanTransition(state_name, existingState, value))
            {
                return;
            }
        }

        current_states[state_name] = value;
        if (!state_history.ContainsKey(state_name))
        {
            state_history[state_name] = new List<Variant>();
        }
        state_history[state_name].Add(value);

        if (observers.TryGetValue(state_name, out var list))
        {
            var data = new Godot.Collections.Dictionary
            {
                {"to", value}
            };
            if (prev.HasValue)
            {
                data["from"] = prev.Value;
            }
            foreach (var cb in list.ToArray())
            {
                cb.Call(data);
            }
        }
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
    public Godot.Collections.Array GetStateHistory(string state_name)
    {
        var result = new Godot.Collections.Array();
        if (state_history.TryGetValue(state_name, out var history))
        {
            foreach (var value in history)
            {
                result.Add(value);
            }
        }
        return result;
    }

    // 遷移登録
    public void RegisterTransition(string state_name, Variant from, Variant to)
    {
        if (!transitions.ContainsKey(state_name))
        {
            transitions[state_name] = new Dictionary<Variant, List<Variant>>();
        }
        var map = transitions[state_name];
        if (!map.ContainsKey(from))
        {
            map[from] = new List<Variant>();
        }
        if (!map[from].Contains(to))
        {
            map[from].Add(to);
        }
    }

    private bool CanTransition(string state_name, Variant from, Variant to)
    {
        if (!transitions.ContainsKey(state_name))
        {
            return true;
        }
        var map = transitions[state_name];
        if (!map.ContainsKey(from))
        {
            return false;
        }
        return map[from].Contains(to);
    }

    // オブザーバー登録
    public void Observe(string state_name, Callable callback)
    {
        if (!observers.ContainsKey(state_name))
        {
            observers[state_name] = new List<Callable>();
        }
        if (!observers[state_name].Contains(callback))
        {
            observers[state_name].Add(callback);
        }
    }

    public void Unobserve(string state_name, Callable callback)
    {
        if (observers.TryGetValue(state_name, out var list))
        {
            list.Remove(callback);
        }
    }

    // 永続化
    public void SaveAll(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            return;
        }
        var dict = new Godot.Collections.Dictionary();
        foreach (var kv in current_states)
        {
            dict[kv.Key] = kv.Value;
        }
        var json = Json.Stringify(dict);
        file.StoreString(json);
    }

    public void LoadAll(string path)
    {
        if (!FileAccess.FileExists(path))
        {
            return;
        }
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            return;
        }
        var json = file.GetAsText();
        var variant = Json.ParseString(json);
        if (variant.VariantType != Variant.Type.Dictionary)
        {
            return;
        }
        var parsed = variant.AsGodotDictionary();
        foreach (string key in parsed.Keys)
        {
            SetState(key, parsed[key]);
        }
    }
}

