using Godot;
using System.Collections.Generic;

#nullable enable

[GlobalClass]
public partial class EventBus : Node
{
    // 履歴の上限
    private const int HISTORY_LIMIT = 100;

    // ハンドラーとフィルターの辞書
    private readonly Dictionary<string, List<(Callable callback, Callable? filter)>> event_handlers = new();

    // イベント履歴
    private readonly Dictionary<string, List<Godot.Collections.Dictionary>> event_history = new();

    // 非同期イベントキュー
    private readonly PriorityQueue<EventEntry, int> event_queue = new();

    private record struct EventEntry(string Name, Godot.Collections.Dictionary Data, int Priority);

    // イベントをキューへ追加する（優先度指定）
    public void EmitEvent(string event_name, Godot.Collections.Dictionary data, int priority)
    {
        event_queue.Enqueue(new EventEntry(event_name, data, priority), priority);
    }

    // イベントをキューへ追加する（優先度なし）
    public void EmitEvent(string event_name, Godot.Collections.Dictionary data)
    {
        EmitEvent(event_name, data, 0);
    }

    public override void _Process(double delta)
    {
        while (event_queue.Count > 0)
        {
            var entry = event_queue.Dequeue();
            AddHistory(entry.Name, entry.Data);
            if (event_handlers.TryGetValue(entry.Name, out var handlers))
            {
                foreach (var (callback, filter) in handlers.ToArray())
                {
                    if (filter != null)
                    {
                        try
                        {
                            var result = filter.Value.Call(entry.Data);
                            if (result.VariantType == Variant.Type.Bool && !result.AsBool())
                            {
                                continue;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            GD.PrintErr($"Error in filter callback for event '{entry.Name}': {ex.Message}");
                            continue;
                        }
                    }
                    callback.Call(entry.Data);
                }
            }
        }
    }

    private void AddHistory(string event_name, Godot.Collections.Dictionary data)
    {
        if (!event_history.ContainsKey(event_name))
        {
            event_history[event_name] = new List<Godot.Collections.Dictionary>();
        }
        var history = event_history[event_name];
        history.Add(data);
        if (history.Count > HISTORY_LIMIT)
        {
            history.RemoveAt(0);
        }
    }

    // イベント購読を登録する
    public void Subscribe(string event_name, Callable callback, Callable filter)
    {
        AddHandler(event_name, callback, filter);
    }

    // フィルターなし購読
    public void Subscribe(string event_name, Callable callback)
    {
        AddHandler(event_name, callback, null);
    }

    private void AddHandler(string event_name, Callable callback, Callable? filter)
    {
        if (!event_handlers.ContainsKey(event_name))
        {
            event_handlers[event_name] = new List<(Callable, Callable?)>();
        }
        var handlers = event_handlers[event_name];
        if (!handlers.Exists(x => x.callback.Equals(callback)))
        {
            handlers.Add((callback, filter));
        }
    }

    // イベント購読を解除する
    public void Unsubscribe(string event_name, Callable callback)
    {
        if (event_handlers.TryGetValue(event_name, out var handlers))
        {
            handlers.RemoveAll(x => x.callback.Equals(callback));
        }
    }

    // 指定したイベントの履歴を取得する
    public Godot.Collections.Array GetEventHistory(string event_name)
    {
        var result = new Godot.Collections.Array();
        if (event_history.TryGetValue(event_name, out var history))
        {
            foreach (var entry in history)
            {
                result.Add(entry);
            }
        }
        return result;
    }
}

