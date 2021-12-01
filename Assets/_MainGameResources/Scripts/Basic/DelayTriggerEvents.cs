using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayTriggerEvents : MonoBehaviour
{
    public UnityEvent _events;

    public void Trigger(float delay)
    {
        StartCoroutine(delayAndTrigger(delay));
    }

    IEnumerator delayAndTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_events != null)
            _events.Invoke();
    }
}
