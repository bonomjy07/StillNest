using System;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class FloatingMoneyText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    [Header("[Tween]")]
    [SerializeField] private float _floatUpDistance = 10f;
    [SerializeField] private float _durationMoveY = 0.2f;
    [SerializeField] private float _durationFade = 0.3f;

    public UnityAction<FloatingMoneyText> _onTweenComplete;
    
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void SetData(Transform axis, int money)
    {
        // Set position
        RectTransform thisRect = transform as RectTransform;
        RectTransform parentRect = transform.parent as RectTransform;
        thisRect.anchoredPosition = Utils.GetWorldPosition(parentRect, axis, _camera);
        
        // Reset alhpa
        _canvasGroup.alpha = 1f;
        
        // Set text
        _text.text = $"+{money}";
        
        // Tween
        FloatUp();
    }

    private void FloatUp()
    {
        RectTransform rectTransform = transform as RectTransform;
        if (!rectTransform)
        {
            return;
        }
        
        rectTransform.DOAnchorPosY(_floatUpDistance, _durationMoveY, true)
                     .SetRelative(true)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(FadeOut);
    }
    
    private void FadeOut()
    {
        _canvasGroup.DOFade(0f, _durationFade)
                    .OnComplete(() => _onTweenComplete?.Invoke(this));
    }
}
