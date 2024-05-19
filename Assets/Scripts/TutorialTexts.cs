using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class TutorialTexts : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _cityText1;

    [SerializeField] private TMPro.TMP_Text _cityText2;

    [SerializeField] private TMPro.TMP_Text _cityText3;

    [SerializeField] private TMPro.TMP_Text _drillText;

    [SerializeField] private Color _color;

    DG.Tweening.Sequence _textSequence;

    DG.Tweening.Sequence _sequence;

    private void Start(){
        _textSequence = DOTween.Sequence();

        _textSequence.Append(_cityText1.DOColor(_color, 2f));
        _textSequence.Append(_cityText2.DOColor(_color, 2f));
        _textSequence.Append(_cityText3.DOColor(_color, 2f));
    }

    public void ConstructedCity(){
        _sequence = DOTween.Sequence();

        _sequence.Append(_drillText.DOColor(_color, 2f));
    }
}
