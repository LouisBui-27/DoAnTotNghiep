using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupScaler : MonoBehaviour
{
    public float animationDuration = 0.4f;

    private void OnEnable()
    {
        PlayScaleIn();
    }

    private void OnDisable()
    {
        // Đảm bảo không còn tween nào chạy khi đối tượng bị vô hiệu hóa
        DOTween.Kill(transform);
    }
    public void PlayScaleIn()
    {
        transform.localScale = Vector3.zero;

        // Scale from 0 → 1 with a bounce effect
        transform.DOScale(Vector3.one, animationDuration)
                 .SetEase(Ease.OutBack).SetUpdate(true); ; // You can try Ease.OutElastic too!
    }

    public void HidePopup(System.Action onHidden = null)
    {
        transform.DOScale(Vector3.zero, 0.25f)
                 .SetEase(Ease.InBack)
                 .SetUpdate(true) // Quan trọng để tween chạy khi Time.timeScale = 0
                 .OnComplete(() => // Sử dụng OnComplete để thực hiện hành động sau khi tween kết thúc
                 {
                     // Gọi callback sau khi hiệu ứng ẩn hoàn tất
                     onHidden?.Invoke();
                 });
    }
}
