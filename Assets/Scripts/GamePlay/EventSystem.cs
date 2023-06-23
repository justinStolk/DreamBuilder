using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public enum EventType { ON_SCORE_CHANGED }

    private static Dictionary<EventType, System.Action<int>> intActions = new();

    public static void Subscribe(EventType eventType, System.Action<int> actionToSubscribe)
    {
        if (!intActions.ContainsKey(eventType))
        {
            intActions.Add(eventType, null);
        }
        intActions[eventType] += actionToSubscribe;
    }

    public static void Unsubscribe(EventType eventType, System.Action<int> actionToUnsubscribe)
    {
        if (intActions.ContainsKey(eventType))
        {
            intActions[eventType] -= actionToUnsubscribe;
        }
    }

    public static void CallEvent(EventType eventType, int eventValue)
    {
        intActions[eventType]?.Invoke(eventValue);
    }


}
