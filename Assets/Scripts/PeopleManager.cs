using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PeopleManager : MonoBehaviour
{
    [SerializeField] private int _peopleAmount = 10;

    [SerializeField] private TMP_Text _peopleAmountText;

    void Start(){
        UpdatePeopleAmountText();
    }

    private void UpdatePeopleAmountText(){
        _peopleAmountText.text = _peopleAmount.ToString();
    }

    public int _PeopleAmount {
        get{
            return _peopleAmount;
        }

        set{
            _peopleAmount = value;
            UpdatePeopleAmountText();
        }
    }

    // This could be per node
    public float _constructSpeedperPerson = 0.05f;
}
