using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Thiết lập hiệu ứng")]
    [Tooltip("Tỷ lệ phóng to khi nút được nhấn.")]
    public float pressedScale = 1.1f; // Ví dụ: phóng to 10% so với kích thước ban đầu

    [Tooltip("Thời gian (giây) để hiệu ứng phóng to/thu nhỏ hoàn tất.")]
    public float duration = 0.1f; // Thời gian hiệu ứng ngắn để cảm giác nhanh nhạy

    private Vector3 originalScale; // Lưu trữ kích thước ban đầu của nút
    private Tween currentTween;    // Biến để lưu trữ tween hiện tại, giúp dừng nó nếu cần

    void Start()
    {
        // Lưu kích thước ban đầu của nút khi game bắt đầu
        originalScale = transform.localScale;
    }

    // Được gọi khi con trỏ chuột được nhấn xuống trên đối tượng này
    public void OnPointerDown(PointerEventData eventData)
    {
        // Dừng tween hiện tại nếu có để tránh xung đột
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(true); // Kill(true) để hoàn tất ngay tween cũ trước khi bắt đầu tween mới
        }

        // Tạo tween để phóng to nút
        currentTween = transform.DOScale(originalScale * pressedScale, duration)
                                .SetEase(Ease.OutQuad); // Ease.OutQuad tạo hiệu ứng phóng to mượt mà, giảm tốc độ ở cuối
    }

    // Được gọi khi con trỏ chuột được nhả ra sau khi nhấn trên đối tượng này
    public void OnPointerUp(PointerEventData eventData)
    {
        // Dừng tween hiện tại nếu có để tránh xung đột
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(true);
        }

        // Tạo tween để đưa nút về kích thước ban đầu
        currentTween = transform.DOScale(originalScale, duration)
                                .SetEase(Ease.OutQuad); // Ease.OutQuad giúp thu nhỏ mượt mà
    }

    // Đảm bảo nút trở về kích thước ban đầu khi GameObject bị tắt hoặc phá hủy
    void OnDisable()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        transform.localScale = originalScale; // Đặt lại kích thước ban đầu
    }

    void OnDestroy()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        transform.localScale = originalScale; // Đặt lại kích thước ban đầu
    }
}
