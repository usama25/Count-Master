using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    public List<string> _tags;

    public UnityEvent enterEvents, exitEvents;

    public bool tunrSelfOff, turnColliderOff;

    private void OnTriggerEnter(Collider other)
    {
        if (_tags.Contains(other.tag))
        {
            if(turnColliderOff)
                other.gameObject.SetActive(false);
            if(tunrSelfOff)
                gameObject.SetActive(false);
            if (enterEvents != null)
                enterEvents.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_tags.Contains(other.tag))
            if (exitEvents != null)
                exitEvents.Invoke();
    }
}
