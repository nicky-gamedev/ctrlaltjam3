using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaterFill : MonoBehaviour
{
    public Material _material;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    public static float _waterAlpha =0.85f;

    Sequence _sequence;

    public void Fill(Action endOfFillACtion){
        _spriteRenderer.material = new Material(_material);
        _spriteRenderer.material.SetFloat("_FinalAlpha", _waterAlpha);
        _sequence = DOTween.Sequence();
        _sequence.Append(_spriteRenderer.material.DOFloat(1.2f, "_Percentage", 2).SetEase(Ease.Linear));
        _sequence.Join(_spriteRenderer.material.DOFloat(-0.08f, "_Percentage_Add", 2).SetEase(Ease.Linear));
        _sequence.OnComplete(()=>{endOfFillACtion.Invoke();});
    }
}
