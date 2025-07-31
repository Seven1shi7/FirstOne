using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity���¼�����ϵͳ
/// ֧�ַ����¼��Ͳ�������
/// </summary>
public static class EventManager
{
    // �¼��ֵ� - ʹ�� Action<object> ����
    private static Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    // �����¼��ֵ�
    private static Dictionary<string, Delegate> genericEventTable = new Dictionary<string, Delegate>();

    // ����ģʽ
    public static bool DebugMode = false;

    #region �����¼�ϵͳ

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="listener">������</param>
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: �¼����Ʋ���Ϊ�գ�");
            return;
        }

        if (listener == null)
        {
            Debug.LogError("EventManager: ����������Ϊ�գ�");
            return;
        }

        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] += listener;
        else
            eventTable[eventName] = listener;

        if (DebugMode)
            Debug.Log($"EventManager: �����¼� '{eventName}'");
    }

    /// <summary>
    /// ȡ������
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="listener">������</param>
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
                Debug.Log($"EventManager: ȡ�������¼� '{eventName}'");
        }
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="param">�¼�����</param>
    public static void Publish(string eventName, object param = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: �¼����Ʋ���Ϊ�գ�");
            return;
        }

        if (eventTable.ContainsKey(eventName))
        {
            try
            {
                eventTable[eventName]?.Invoke(param);
                if (DebugMode)
                    Debug.Log($"EventManager: �����¼� '{eventName}' ����: {param}");
            }
            catch (Exception e)
            {
                Debug.LogError($"EventManager: �����¼� '{eventName}' ʱ��������: {e.Message}");
            }
        }
        else if (DebugMode)
        {
            Debug.LogWarning($"EventManager: �¼� '{eventName}' û�ж�����");
        }
    }

    #endregion

    #region �����¼�ϵͳ

    /// <summary>
    /// ���ķ����¼�
    /// </summary>
    /// <typeparam name="T">�¼���������</typeparam>
    /// <param name="eventName">�¼�����</param>
    /// <param name="listener">������</param>
    public static void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: �¼����Ʋ���Ϊ�գ�");
            return;
        }

        if (listener == null)
        {
            Debug.LogError("EventManager: ����������Ϊ�գ�");
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
            Debug.Log($"EventManager: ���ķ����¼� '{eventName}' ����: {typeof(T).Name}");
    }

    /// <summary>
    /// ȡ�����ķ����¼�
    /// </summary>
    /// <typeparam name="T">�¼���������</typeparam>
    /// <param name="eventName">�¼�����</param>
    /// <param name="listener">������</param>
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
                Debug.Log($"EventManager: ȡ�����ķ����¼� '{eventName}'");
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <typeparam name="T">�¼���������</typeparam>
    /// <param name="eventName">�¼�����</param>
    /// <param name="data">�¼�����</param>
    public static void Publish<T>(string eventName, T data)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogError("EventManager: �¼����Ʋ���Ϊ�գ�");
            return;
        }

        if (genericEventTable.ContainsKey(eventName))
        {
            try
            {
                var action = genericEventTable[eventName] as Action<T>;
                action?.Invoke(data);

                if (DebugMode)
                    Debug.Log($"EventManager: ���������¼� '{eventName}' ����: {data}");
            }
            catch (Exception e)
            {
                Debug.LogError($"EventManager: ���������¼� '{eventName}' ʱ��������: {e.Message}");
            }
        }
        else if (DebugMode)
        {
            Debug.LogWarning($"EventManager: �����¼� '{eventName}' û�ж�����");
        }
    }

    #endregion

    #region ���߷���

    /// <summary>
    /// ����¼��Ƿ��ж�����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <returns>�Ƿ��ж�����</returns>
    public static bool HasSubscribers(string eventName)
    {
        return eventTable.ContainsKey(eventName) || genericEventTable.ContainsKey(eventName);
    }

    /// <summary>
    /// ��ȡ�¼�����������
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <returns>����������</returns>
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
    /// ��������¼������ڳ����л�ʱ����
    /// </summary>
    public static void ClearAllEvents()
    {
        eventTable.Clear();
        genericEventTable.Clear();

        if (DebugMode)
            Debug.Log("EventManager: ��������¼�");
    }

    /// <summary>
    /// ��ȡ�����¼����ƣ����ڵ��ԣ�
    /// </summary>
    /// <returns>�¼������б�</returns>
    public static List<string> GetAllEventNames()
    {
        var eventNames = new List<string>();
        eventNames.AddRange(eventTable.Keys);
        eventNames.AddRange(genericEventTable.Keys);
        return eventNames;
    }

    #endregion
}