using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class OreManager : MonoBehaviour
{
    [SerializeField] private int _oreAmount = 3;

    [SerializeField] private TMP_Text _oreAmountText;

    [SerializeField] private float _timerPassiveOre = 10f;

    public bool _allowEarnMoreOrePassive = false;

    private float _timer = 0f;

    private void Start(){
        UpdateOreAmountText();
    }

    private void UpdateOreAmountText(){
        _oreAmountText.text = _oreAmount.ToString();
    }

    public int _OreAmount {
        get{
            return _oreAmount;
        }
        set{
            _oreAmount = value;
            UpdateOreAmountText();
        }
    }

    private void Update(){
        if(_allowEarnMoreOrePassive){
            _timer += Time.deltaTime;

            if(_timer >= _timerPassiveOre){
                _timer = 0f;
                _OreAmount += 1;
            }
        }
    }
}
