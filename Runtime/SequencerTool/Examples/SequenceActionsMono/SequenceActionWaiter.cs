using SBN.SequencerTool;
using System;
using System.Collections;
using UnityEngine;

public class SequenceActionWaiter : SequenceActionMono
{
    [SerializeField] private float waitTime = 1.0f;

    private Coroutine coroutine;

    protected override void OnBeginAction()
    {
        Debug.Log($"Waiting {waitTime} second(s)..");

        coroutine = StartCoroutine(c_Wait(waitTime, () => EndAction()));
    }

    protected override void OnResetAction()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    protected override void OnEndAction()
    {
        Debug.Log($"Timer of {waitTime} second(s) complete!");
    }

    protected override void OnSkipAction()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        EndAction();
    }

    private IEnumerator c_Wait(float timeInSeconds, Action callback)
    {
        yield return new WaitForSeconds(timeInSeconds);
        callback?.Invoke();
    }
}