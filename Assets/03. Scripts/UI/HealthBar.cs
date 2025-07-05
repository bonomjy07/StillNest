using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("[Fill]")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private float _yOffset;
    
    [Header("[Danger]")]
    [SerializeField] private float _dangerAmount = 0.3f; // 30% 이하일 때 위험 표시
    [SerializeField] private Color _dangerColor = Color.red;
    
    private Transform _target;
    private Action<HealthBar> _onRelease;
    
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        RectTransform thisRect = transform as RectTransform;
        if (thisRect)
        {
            thisRect.anchoredPosition = GetWorldPosition();
        }
    }
    
    // 나중에 Util.cs로 뺴기
    private Vector2 GetWorldPosition()
    {
        if (!_target || !_mainCamera)
        {
            return Vector2.zero;
        }

        RectTransform canvasRect = transform.parent as RectTransform;
        if (!canvasRect)
        {
            return Vector2.zero;
        }
        
        Vector2 canvasSizeDelta = canvasRect.sizeDelta;

        Vector2 viewportPosition = _mainCamera.WorldToViewportPoint(_target.position);
        Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasSizeDelta.x) - (canvasSizeDelta.x * 0.5f)), ((viewportPosition.y * canvasSizeDelta.y) - (canvasSizeDelta.y * 0.5f)));
        return screenPosition;
    }

    public void SetBar(float percentage, Transform followTarget)
    {
        _target = followTarget;
        
        SetFillAmount(percentage);

        //StartFloat();
    }
    
    private void SetFillAmount(float percentage)
    {
        _fillImage.fillAmount = percentage;
        _fillImage.color = percentage <= _dangerAmount ? _dangerColor : Color.white;
    }

    private void StartFloat()
    {
        transform.DOMoveY(_yOffset, _duration)
                 .SetEase(Ease.OutQuad)
                 .SetRelative(true)
                 .OnComplete(Hide);
    }

    public void Hide()
    {
        _target = null;
        _onRelease?.Invoke(this); // it will hide
    }
    
    public void SetReleaseCallback(Action<HealthBar> onRelease)
    {
        _onRelease = onRelease;
    }
}
