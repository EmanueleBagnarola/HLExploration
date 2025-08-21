using UnityEngine;
using UnityEngine.Events;

public static class EventsHandler
{
    public static UnityEvent OnGenerateButton = new UnityEvent();
    public static UnityEvent OnStartEditGridButton = new UnityEvent();
    public static UnityEvent OnStopEditGridButton = new UnityEvent();
}
