using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour 
{
    public UnityEvent<float> onTimerUpdate;
    public UnityEvent onTimerFinished;
    
    private IEnumerator C_StartTimer(float waitTime)
    {
        float timer = 0.0f;

        while(timer < waitTime)
        {
            timer += Time.deltaTime;
            onTimerUpdate.Invoke(Mathf.Clamp01(timer / waitTime));
            yield return null;
        }

        onTimerFinished.Invoke();
    }

    public void StartTimer(float waitTime)
    {
        StopAllCoroutines();
        StartCoroutine(C_StartTimer(waitTime));
    }
}
