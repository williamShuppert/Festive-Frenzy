using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SantaLetter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] float animationTime = 1;

    [SerializeField] AnimationCurve posAnimation;
    [SerializeField] AnimationCurve rotAnimation;

    [SerializeField][TextArea] string endLetter;

    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector3 startRot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.localPosition;
        startRot = rectTransform.localEulerAngles;
    }

    public void Start()
    {
        PlayLetterAnimation(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StopAllCoroutines();
            StartCoroutine(AnimateLetter(false));
            Counter.instance.StartTimer();
        }
            
    }

    public void SetWinText()
    {
        textMesh.text = endLetter;
    }

    public void PlayLetterAnimation(bool animateIn)
    {
        StartCoroutine(AnimateLetter(animateIn));
    }

    IEnumerator AnimateLetter(bool animateIn)
    {
        Vector2 fromPos = animateIn ? startPos : Vector2.zero;
        Vector2 toPos = animateIn ? Vector2.zero : startPos;

        Vector3 fromRot = animateIn ? startRot : Vector3.zero;
        Vector3 toRot = animateIn ? Vector3.zero : startRot;

        var time = 0f;

        if (animateIn) yield return new WaitForSeconds(.5f);

        while (time < animationTime)
        {
            time += Time.deltaTime;

            rectTransform.localPosition = Vector2.Lerp(fromPos, toPos, posAnimation.Evaluate(time / animationTime));
            rectTransform.localEulerAngles = Vector3.Lerp(fromRot, toRot, rotAnimation.Evaluate(time / animationTime));

            yield return null;
        }

        if (!animateIn)
            enabled = false;
    }
}
