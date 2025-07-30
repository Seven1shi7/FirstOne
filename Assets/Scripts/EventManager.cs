using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // 事件字典 - 使用 Action<object> 类型
    private static Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    // 订阅事件
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] += listener;
        else
            eventTable[eventName] = listener;
    }

    // 取消订阅
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] -= listener;
            if (eventTable[eventName] == null)
                eventTable.Remove(eventName);
        }
    }

    // 触发事件
    public static void Publish(string eventName, object param = null)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName]?.Invoke(param);
    }

    // 清除所有事件（可选，用于场景切换时清理）
    public static void ClearAllEvents()
    {
        eventTable.Clear();
    }
}