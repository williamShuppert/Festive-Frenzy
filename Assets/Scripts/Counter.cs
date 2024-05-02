using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    public static Counter instance;

    [SerializeField] TextMeshProUGUI presentCountTextMesh;
    int presentCount = 0;

    [SerializeField] TextMeshProUGUI timerTextMesh;
    float timer = -1;
    bool timerEnabled = false;

    public UnityEvent OnWin;

    private void Awake()
    {
        instance = this;
    }

    public void AddPresent()
    {
        presentCount++;
        presentCountTextMesh.text = presentCount + "/10";

        if (presentCount == 10)
        {
            StopTimer();
            OnWin.Invoke();
        }
    }

    public void StartTimer()
    {
        if (timer > 0) return;
        timerEnabled = true;
        StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        timerEnabled = false;
        StopAllCoroutines();
    }

    private IEnumerator Timer()
    {
        while (timerEnabled)
        {
            timer += Time.deltaTime;

            int mins = (int)(timer / 60f);
            int secs = (int)timer - 60 * mins;

            timerTextMesh.text = mins.ToString("00") + ":" + secs.ToString("00");
            yield return null;
        }
    }
}
