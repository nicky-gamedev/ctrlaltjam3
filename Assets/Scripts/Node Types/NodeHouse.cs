using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHouse : MonoBehaviour
{
    public int _amountOFPeople = 10;
    PeopleManager _peopleManager;
    public void Awake(){
        _peopleManager = FindObjectOfType<PeopleManager>();
    }

    public void OnConstruct(){
        _peopleManager._PeopleAmount += _amountOFPeople;
    }

    public void OnDestroy(){
        _peopleManager._PeopleAmount -= _amountOFPeople;
    }
}
