using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInOutAnimator : MonoBehaviour
{
    public enum SlideDirection { FromLeft, FromRight }

    [Header("Cài đặt")]
    public SlideDirection slideDirection = SlideDirection.FromRight;
    public float distance = 1500f;          // Khoảng cách trượt vào
    public float duration = 0.4f;
    public Ease ease = Ease.OutCubic;
    public bool playOnEnable = true;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlaySlideIn();
        }
    }
    private void OnDestroy()
    {
        if (rectTransform != null)
        {
            DOTween.Kill(rectTransform);
        }
    }
    public void PlaySlideIn()
    {
        Vector2 startPos = slideDirection == SlideDirection.FromLeft
            ? new Vector2(-distance, originalPosition.y)
            : new Vector2(distance, originalPosition.y);

        rectTransform.anchoredPosition = startPos;
        rectTransform.DOAnchorPos(originalPosition, duration)
                     .SetEase(ease)
                     .SetUpdate(true); // để hoạt động cả khi Time.timeScale = 0
    }

    public void PlaySlideOut(System.Action onComplete = null)
    {
        Vector2 targetPos = slideDirection == SlideDirection.FromLeft
            ? new Vector2(distance, originalPosition.y)
            : new Vector2(-distance, originalPosition.y);

        rectTransform.DOAnchorPos(targetPos, duration)
                     .SetEase(Ease.InCubic)
                     .SetUpdate(true)
                     .OnComplete(() => onComplete?.Invoke());
    }
}
