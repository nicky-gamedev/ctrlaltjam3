using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PointLocatorColliderCircle : MonoBehaviour
{
    private CityNodeUI _cityNodeUI;

    public void Init(CityNodeUI cityNodeUI){
        _cityNodeUI = cityNodeUI;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.name == "City Area Collider"){
            CityNode cityNode =  col.transform.parent.GetComponent<CityNode>();
            if(!_cityNodeUI._collidedCityAreas.Contains(cityNode)){
                _cityNodeUI._collidedCityAreas.Add(cityNode);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.name == "City Area Collider"){
            CityNode cityNode =  col.transform.parent.GetComponent<CityNode>();
            if(_cityNodeUI._collidedCityAreas.Contains(cityNode)){
                _cityNodeUI._collidedCityAreas.Remove(cityNode);
            }
        }
    }
}
