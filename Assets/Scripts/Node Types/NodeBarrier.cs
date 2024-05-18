using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CityNode))]
public class NodeBarrier : MonoBehaviour
{
    public float _timeBarrier = 10f;

    public void Awake(){
        CityNode cityNode = GetComponent<CityNode>();
        cityNode._barrierTime = _timeBarrier;
    }
}
