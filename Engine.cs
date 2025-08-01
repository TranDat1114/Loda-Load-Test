using System;
using System.Collections.Generic;
using System.Threading;

namespace Loda;

// Delegate cho event update và render
public delegate void UpdateEventHandler(float deltaTime);
public delegate void RenderEventHandler();

// Event system cơ bản
public class EventManager
{
    private readonly Dictionary<string, Action<object>> _eventTable = new();

    public void Subscribe(string eventName, Action<object> callback)
    {
        if (_eventTable.ContainsKey(eventName))
            _eventTable[eventName] += callback;
        else
            _eventTable[eventName] = callback;
    }

    public void Unsubscribe(string eventName, Action<object> callback)
    {
        if (_eventTable.ContainsKey(eventName))
            _eventTable[eventName] -= callback;
    }

    public void Publish(string eventName, object param = null)
    {
        if (_eventTable.ContainsKey(eventName))
            _eventTable[eventName]?.Invoke(param);
    }
}

// UI Manager cơ bản
public class UIManager
{
    private Action _drawAction;

    public void SetDrawAction(Action drawAction)
    {
        _drawAction = drawAction;
    }

    public void DrawUI()
    {
        _drawAction?.Invoke();
    }
}

// Base Engine
public class Engine
{
    public event UpdateEventHandler OnUpdate;
    public event RenderEventHandler OnRender;
    public bool IsRunning { get; private set; }
    public EventManager EventManager { get; } = new();
    public UIManager UIManager { get; } = new();

    public void Run()
    {
        IsRunning = true;
        var lastTime = DateTime.Now;
        while (IsRunning)
        {
            var now = DateTime.Now;
            float deltaTime = (float)(now - lastTime).TotalSeconds;
            lastTime = now;

            // Update
            OnUpdate?.Invoke(deltaTime);

            // Render
            OnRender?.Invoke();
            // UIManager chỉ vẽ UI nếu có action
            UIManager.DrawUI();

            Thread.Sleep(16); // ~60 FPS
        }
    }

    public void Stop()
    {
        IsRunning = false;
    }
}

