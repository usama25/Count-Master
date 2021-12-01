using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableDisableEvents : MonoBehaviour
{
    public UnityEvent OnEnableEvents, OnDisableEvents;

    private void OnEnable()
    {
        if (OnEnableEvents != null)
            OnEnableEvents.Invoke();
    }

    private void OnDisable()
    {
        if (OnDisableEvents != null)
            OnDisableEvents.Invoke();
    }
}
