using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CityNodesHolder : MonoBehaviour
{
    private List<CityNode> _cityNodes = new List<CityNode>();

    private List<CityNode> _constructingNodes = new List<CityNode>();

    [SerializeField] private PeopleManager _peopleManager;

    public void AddCity(CityNode cityNode){
        if(!_cityNodes.Contains(cityNode)){
            _cityNodes.Add(cityNode);
        }
    }

    public void RemoveCity(CityNode cityNode){
        if(_cityNodes.Contains(cityNode)){
            _cityNodes.Remove(cityNode);
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
