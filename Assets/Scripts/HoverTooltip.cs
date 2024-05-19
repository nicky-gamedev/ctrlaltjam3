using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class HoverTooltip : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    Sequence _sequence;
    [SerializeField] CanvasGroup _canvasGroup;

    [SerializeField] TMP_Text[] _texts;

    [SerializeField] UnityEvent _eventOnClick;

    public void OnClick()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(0f, 0.1f));
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(1f, 1f));
    }

     public void OnPointerExit(PointerEventData pointerEventData)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(0f, 1f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _eventOnClick?.Invoke();
    }
}

