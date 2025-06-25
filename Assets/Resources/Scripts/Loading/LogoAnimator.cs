using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnimator : MonoBehaviour
{
    public Image logoImage; // Kéo GameObject "Logo" (có thành phần Image) vào đây trong Inspector

    [Header("Scale Animation Settings")]
    public float initialScale = 0.5f; // Bắt đầu thu nhỏ
    public float targetScale = 1.0f; // Phóng to đến kích thước gốc
    public float scaleDuration = 1.0f; // Thời gian cho hiệu ứng scale
    public Ease scaleEase = Ease.OutBack; // Kiểu chuyển động (OutBack tạo hiệu ứng nảy nhẹ)

    [Header("Shake Animation Settings")]
    public float shakeDuration = 0.5f; // Thời gian rung lắc
    public float shakeStrength = 10f; // Cường độ rung (tùy chỉnh để thấy rõ)
    public int shakeVibrato = 10; // Số lần rung
    public float shakeRandomness = 90f; // Độ ngẫu nhiên của rung lắc

    [Header("Sequence Settings")]
    public float delayBeforeAnimation = 0.5f; // Độ trễ trước khi bắt đầu hiệu ứng
    public float delayBetweenScaleAndShake = 0.2f; // Độ trễ giữa hiệu ứng scale và shake
    private Sequence logoSequence;

    void Start()
    {
        if (logoImage == null)
        {
            // Thử tự động lấy component Image nếu chưa được gán
            logoImage = GetComponent<Image>();
            if (logoImage == null)
            {
                Debug.LogError("LogoAnimator: Image component not found on this GameObject or not assigned in Inspector.");
                return; // Dừng nếu không tìm thấy Image
            }
        }

        // Đặt kích thước ban đầu của logo (thường là nhỏ hơn)
        logoImage.transform.localScale = Vector3.one * initialScale;

        // Bắt đầu chuỗi hiệu ứng
        StartLogoAnimationSequence();
    }

    void StartLogoAnimationSequence()
    {
        if (logoSequence != null && logoSequence.IsActive())
        {
            logoSequence.Kill();
        }
        // Tạo một chuỗi hiệu ứng
        Sequence sequence = DOTween.Sequence();

        // 1. Thêm độ trễ trước khi bắt đầu hiệu ứng
        sequence.AppendInterval(delayBeforeAnimation);

        // 2. Hiệu ứng Scale In (thu nhỏ rồi phóng to ra)
        // .From() ở đây không cần thiết vì chúng ta đã đặt scale ban đầu trong Start
        sequence.Append(logoImage.transform.DOScale(targetScale, scaleDuration)
                                  .SetEase(scaleEase));

        // 3. Thêm độ trễ giữa hiệu ứng scale và shake
        sequence.AppendInterval(delayBetweenScaleAndShake);

        // 4. Hiệu ứng Shake (rung lắc)
        // .PunchRotation tạo hiệu ứng rung lắc xoay nhẹ
        // .Append() nghĩa là hiệu ứng này sẽ chạy sau hiệu ứng trước
        sequence.Append(logoImage.transform.DOPunchRotation(
                            new Vector3(0, 0, shakeStrength), // Chỉ rung lắc trên trục Z (xoay)
                            shakeDuration,
                            shakeVibrato,
                            shakeRandomness
                        ));

        // Optional: Để hiệu ứng này lặp lại sau khi loading hoàn tất?
        // Ví dụ, lặp lại hiệu ứng shake vô hạn cho đến khi scene chuyển.
        sequence.Append(logoImage.transform.DOPunchRotation(
                            new Vector3(0, 0, shakeStrength),
                            shakeDuration,
                            shakeVibrato,
                            shakeRandomness
                        )); // Lặp lại vô hạn từ đầu

        logoSequence.SetLink(gameObject);
    }
    void OnDestroy()
    {
        // Không cần gọi logoSequence.Kill() nếu bạn đã dùng SetLink(gameObject)
        // Nhưng nếu bạn có các tween riêng lẻ (như cái tween shake lặp lại),
        // thì bạn có thể cần kill chúng bằng ID
        DOTween.Kill(this); // Sẽ kill tất cả các tween được SetId(this)

        // Bạn có thể giữ dòng này nếu bạn muốn chắc chắn hoặc nếu có các tween không thuộc sequence chính
        // if (logoSequence != null && logoSequence.IsActive())
        // {
        //     logoSequence.Kill();
        // }
    }

}
