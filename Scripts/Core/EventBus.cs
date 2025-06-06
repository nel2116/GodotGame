using Godot;
using System.Collections.Generic;

#nullable enable

[GlobalClass]
public partial class EventBus : Node
{
    // イベントハンドラーの登録辞書
    private readonly Dictionary<string, List<Callable>> event_handlers = new();

    // イベント履歴を保持する辞書
    private readonly Dictionary<string, List<Godot.Collections.Dictionary>> event_history = new();

    // イベントを発火して購読者へ通知する
    public void EmitEvent(string event_name, Godot.Collections.Dictionary data)
    {
        if (!event_history.ContainsKey(event_name))
        {
            event_history[event_name] = new List<Godot.Collections.Dictionary>();
        }
        event_history[event_name].Add(data);

        if (event_handlers.TryGetValue(event_name, out var handlers))
        {
            foreach (var callback in handlers)
            {
                callback.Call(data);
            }
        }
    }

    // イベント購読を登録する
    public void Subscribe(string event_name, Callable callback)
    {
        if (!event_handlers.ContainsKey(event_name))
        {
            event_handlers[event_name] = new List<Callable>();
        }
        if (!event_handlers[event_name].Contains(callback))
        {
            event_handlers[event_name].Add(callback);
        }
    }

    // イベント購読を解除する
    public void Unsubscribe(string event_name, Callable callback)
    {
        if (event_handlers.TryGetValue(event_name, out var handlers))
        {
            handlers.Remove(callback);
        }
    }

    // 指定したイベントの履歴を取得する
    public Godot.Collections.Array GetEventHistory(string event_name)
    {
        if (event_history.TryGetValue(event_name, out var history))
        {
            var result = new Godot.Collections.Array();
            foreach (var entry in history)
            {
                result.Add(entry);
            }
            return result;
        }
        return new Godot.Collections.Array();
    }
}

