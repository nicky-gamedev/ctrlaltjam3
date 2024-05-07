using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    [SerializeField] private int _peopleAmount = 10;
    public int _PeopleAmount {
        get{
            return _peopleAmount;
        }

        set{
            _peopleAmount = value;
        }
    }

    // This could be per node
    public float _constructSpeedperPerson = 0.1f;
}
