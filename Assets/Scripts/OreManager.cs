using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class OreManager : MonoBehaviour
{
    [SerializeField] private int _oreAmount = 3;

    [SerializeField] private TMP_Text _oreAmountText;

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
}
