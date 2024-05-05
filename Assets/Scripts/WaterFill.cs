using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class WaterFill : MonoBehaviour
{
    public Material _material;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private AnimationCurve _lightsOutCurve;

    [SerializeField] private Light2D _light2D;

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
        _sequence.Join(DOTween.To(x => {_light2D.intensity = x;}, 0f, 1f, 2f).SetEase(_lightsOutCurve));
        _sequence.OnComplete(()=>{endOfFillACtion.Invoke();});
    }
}
