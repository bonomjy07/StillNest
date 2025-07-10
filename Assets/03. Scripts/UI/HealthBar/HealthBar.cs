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

    private void Start()
    {
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!_target)
        {
            return;
        }
        
        RectTransform thisRect = transform as RectTransform;
        if (!thisRect)
        {
            return;
        }
        
        RectTransform canvasRect = transform.parent as RectTransform;
        
        Vector2 screenPosition = Utils.GetWorldPosition(canvasRect, _target, _mainCamera);
        thisRect.anchoredPosition = screenPosition;
    }
    
    public void SetBar(float percentage, Transform followTarget)
    {
        _target = followTarget;
        
        SetFillAmount(percentage);
    }
    
    private void SetFillAmount(float percentage)
    {
        _fillImage.fillAmount = percentage;
        _fillImage.color = percentage <= _dangerAmount ? _dangerColor : Color.white;
    }
}
