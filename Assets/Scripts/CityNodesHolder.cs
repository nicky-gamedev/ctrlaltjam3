using System.Collections;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Xml.Serialization;

public class CityNodesHolder : MonoBehaviour
{
    [SerializeField] CityNode _initialNode;

    [SerializeField] private List<CityNode> _cityNodes = new List<CityNode>();

    [SerializeField] private List<CityNode> _constructingNodes = new List<CityNode>();

    [SerializeField] private PeopleManager _peopleManager;

    public Action<bool> _enablePlaceNodeCollider;

    public OreManager _oreManager;

    private bool _courotineCalled = false;

    public void ConstructedCity(){
        if(!_courotineCalled){
            _courotineCalled = true;
            _oreManager._allowEarnMoreOrePassive = true;
            StartCoroutine(CourotineStartWaterFill());
        }
    }

    IEnumerator CourotineStartWaterFill(){
        yield return new WaitForSeconds(45);
        _initialNode.FillWithWater();
    }

    public void AddCity(CityNode cityNode){
        if(!_cityNodes.Contains(cityNode)){
            _cityNodes.Add(cityNode);
        }
    }

    public void RemoveCity(CityNode cityNode){
        if(_cityNodes.Contains(cityNode)){
            _cityNodes.Remove(cityNode);

            if(_cityNodes.Count == 0){
                Debug.Log("Game Over");
            }
        }
    }

    public void ConstructingCity(CityNode cityNode){
        if(!_constructingNodes.Contains(cityNode)){
            _constructingNodes.Add(cityNode);
        }
    }

    public void DoneConstructing(CityNode cityNode){
        if(_constructingNodes.Contains(cityNode)){
            _constructingNodes.Remove(cityNode);
            cityNode.DoneConstructing();
        }
    }

    private void FixedUpdate(){
        List<CityNode> removeNodes = new List<CityNode>();

        foreach (CityNode city in _constructingNodes)
        {
            city._constructingProgress += _peopleManager._constructSpeedperPerson * Mathf.Floor(_peopleManager._PeopleAmount / _constructingNodes.Count);
            if(city._constructingProgress >= 1){
                removeNodes.Add(city);
            }
        }

        foreach (CityNode removeCity in removeNodes)
        {
            DoneConstructing(removeCity);
        }
    }
}
