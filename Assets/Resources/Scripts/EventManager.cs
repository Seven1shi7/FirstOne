using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // �¼��ֵ� - ʹ�� Action<object> ����
    private static Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    // �����¼�
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] += listener;
        else
            eventTable[eventName] = listener;
    }

    // ȡ������
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] -= listener;
            if (eventTable[eventName] == null)
                eventTable.Remove(eventName);
        }
    }

    // �����¼�
    public static void Publish(string eventName, object param = null)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName]?.Invoke(param);
    }

    // ��������¼�����ѡ�����ڳ����л�ʱ����
    public static void ClearAllEvents()
    {
        eventTable.Clear();
    }
}