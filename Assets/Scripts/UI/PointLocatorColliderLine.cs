using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class PointLocatorColliderLine : MonoBehaviour
{
    private EdgeCollider2D _collider;

    public CityNodeUI _cityNodeUI;

    private List<Vector2> points = new List<Vector2>();

    public void Init(CityNodeUI cityNodeUI){
        _cityNodeUI = cityNodeUI;
    }

    public void SetColliderPositions(Vector2 origin, Vector2 destination){
        points.Clear();
        points.Add(origin);
        points.Add(destination);
        _collider.SetPoints(points);
    }

    void Start()
    {
        _collider = GetComponent<EdgeCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel"){
            if(!_cityNodeUI._collidedLines.Contains(this)){
                _cityNodeUI._collidedLines.Add(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel"){
            if(_cityNodeUI._collidedLines.Contains(this)){
                _cityNodeUI._collidedLines.Remove(this);
            }
        }
    }
}
