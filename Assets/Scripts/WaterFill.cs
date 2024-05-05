using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaterFill : MonoBehaviour
{
    public Material _material;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    public static float _waterAlpha =0.7f;

    Sequence _sequence;

    private void Start(){
        _spriteRenderer.material = new Material(_material);
        _spriteRenderer.material.SetFloat("_FinalAlpha", _waterAlpha);
        _spriteRenderer.material.SetFloat("_Percentage", 0);
        _spriteRenderer.material.SetFloat("_Percentage_Add", 0);
    }

    public void Fill(Action endOfFillACtion){
        _sequence = DOTween.Sequence();
        _sequence.Append(_spriteRenderer.material.DOFloat(1.2f, "_Percentage", 2).SetEase(Ease.InSine));
        _sequence.Join(_spriteRenderer.material.DOFloat(-0.08f, "_Percentage_Add", 2).SetEase(Ease.InSine));
        _sequence.OnComplete(()=>{endOfFillACtion.Invoke();});
    }
}
