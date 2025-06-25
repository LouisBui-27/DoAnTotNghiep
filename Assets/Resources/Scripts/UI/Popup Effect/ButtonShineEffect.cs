using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonShineEffect : MonoBehaviour
{
    public RectTransform shineTransform;
    public float duration = 1.5f;
    public float delayBetweenLoops = 2f;
    private Vector2 startPos;
    private Vector2 endPos;
    void OnDestroy()
    {
        if (shineTransform != null)
        {
            DOTween.Kill(shineTransform);
        }
    }
    private void Start()
    {
        float parentHeight = ((RectTransform)shineTransform.parent).rect.height;
        float shineHeight = shineTransform.rect.height;

        startPos = new Vector2(0, parentHeight);
        endPos = new Vector2(0, -shineHeight);


        PlayShineLoop();
    }

    void PlayShineLoop()
    {
        shineTransform.anchoredPosition = startPos;

        shineTransform.DOAnchorPos(endPos, duration)
                      .SetEase(Ease.InOutSine)
                      .SetDelay(delayBetweenLoops).SetUpdate(true)
                      .OnComplete(PlayShineLoop);
    }
}
