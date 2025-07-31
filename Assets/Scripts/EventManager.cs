using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity简单事件管理系统
/// 支持泛型事件和参数传递
/// </summary>
public static class EventManager
{
    // 事件字典 - 使用 Action<object> 类型
    private static Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    // 泛型事件字典
    private static Dictionary<string, Delegate> genericEventTable = new Dictionary<string, Delegate>();

    // 调试模式
    public static bool DebugMode = false;

    #region 基础事件系统

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">监听器</param>
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: 事件名称不能为空！");
            return;
        }

        if (listener == null)
        {
            Debug.LogError("EventManager: 监听器不能为空！");
            return;
        }

        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] += listener;
        else
            eventTable[eventName] = listener;

        if (DebugMode)
            Debug.Log($"EventManager: 订阅事件 '{eventName}'");
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">监听器</param>
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (string.IsNullOrEmpty(eventName))
            return;

        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] -= listener;
            if (eventTable[eventName] == null)
                eventTable.Remove(eventName);

            if (DebugMode)
                Debug.Log($"EventManager: 取消订阅事件 '{eventName}'");
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="param">事件参数</param>
    public static void Publish(string eventName, object param = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: 事件名称不能为空！");
            return;
        }

        if (eventTable.ContainsKey(eventName))
        {
            try
            {
                eventTable[eventName]?.Invoke(param);
                if (DebugMode)
                    Debug.Log($"EventManager: 触发事件 '{eventName}' 参数: {param}");
            }
            catch (Exception e)
            {
                Debug.LogError($"EventManager: 触发事件 '{eventName}' 时发生错误: {e.Message}");
            }
        }
        else if (DebugMode)
        {
            Debug.LogWarning($"EventManager: 事件 '{eventName}' 没有订阅者");
        }
    }

    #endregion

    #region 泛型事件系统

    /// <summary>
    /// 订阅泛型事件
    /// </summary>
    /// <typeparam name="T">事件数据类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">监听器</param>
    public static void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: 事件名称不能为空！");
            return;
        }

        if (listener == null)
        {
            Debug.LogError("EventManager: 监听器不能为空！");
            return;
        }

        if (genericEventTable.ContainsKey(eventName))
        {
            var existingDelegate = genericEventTable[eventName];
            genericEventTable[eventName] = Delegate.Combine(existingDelegate, listener);
        }
        else
        {
            genericEventTable[eventName] = listener;
        }

        if (DebugMode)
            Debug.Log($"EventManager: 订阅泛型事件 '{eventName}' 类型: {typeof(T).Name}");
    }

    /// <summary>
    /// 取消订阅泛型事件
    /// </summary>
    /// <typeparam name="T">事件数据类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="listener">监听器</param>
    public static void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (string.IsNullOrEmpty(eventName))
            return;

        if (genericEventTable.ContainsKey(eventName))
        {
            var existingDelegate = genericEventTable[eventName];
            genericEventTable[eventName] = Delegate.Remove(existingDelegate, listener);

            if (genericEventTable[eventName] == null)
                genericEventTable.Remove(eventName);

            if (DebugMode)
                Debug.Log($"EventManager: 取消订阅泛型事件 '{eventName}'");
        }
    }

    /// <summary>
    /// 触发泛型事件
    /// </summary>
    /// <typeparam name="T">事件数据类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="data">事件数据</param>
    public static void Publish<T>(string eventName, T data)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: 事件名称不能为空！");
            return;
        }

        if (genericEventTable.ContainsKey(eventName))
        {
            try
            {
                var action = genericEventTable[eventName] as Action<T>;
                action?.Invoke(data);

                if (DebugMode)
                    Debug.Log($"EventManager: 触发泛型事件 '{eventName}' 数据: {data}");
            }
            catch (Exception e)
            {
                Debug.LogError($"EventManager: 触发泛型事件 '{eventName}' 时发生错误: {e.Message}");
            }
        }
        else if (DebugMode)
        {
            Debug.LogWarning($"EventManager: 泛型事件 '{eventName}' 没有订阅者");
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 检查事件是否有订阅者
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>是否有订阅者</returns>
    public static bool HasSubscribers(string eventName)
    {
        return eventTable.ContainsKey(eventName) || genericEventTable.ContainsKey(eventName);
    }

    /// <summary>
    /// 获取事件订阅者数量
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>订阅者数量</returns>
    public static int GetSubscriberCount(string eventName)
    {
        int count = 0;

        if (eventTable.ContainsKey(eventName))
            count += eventTable[eventName]?.GetInvocationList().Length ?? 0;

        if (genericEventTable.ContainsKey(eventName))
            count += genericEventTable[eventName]?.GetInvocationList().Length ?? 0;

        return count;
    }

    /// <summary>
    /// 清除所有事件（用于场景切换时清理）
    /// </summary>
    public static void ClearAllEvents()
    {
        eventTable.Clear();
        genericEventTable.Clear();

        if (DebugMode)
            Debug.Log("EventManager: 清除所有事件");
    }

    /// <summary>
    /// 获取所有事件名称（用于调试）
    /// </summary>
    /// <returns>事件名称列表</returns>
    public static List<string> GetAllEventNames()
    {
        var eventNames = new List<string>();
        eventNames.AddRange(eventTable.Keys);
        eventNames.AddRange(genericEventTable.Keys);
        return eventNames;
    }

    #endregion
}