using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TunnelScript : MonoBehaviour
{
    public LineRenderer _lineRenderer;

    public ColliderTunnel _colliderTunnel;

    public Material _waterMultidirectional;

    public bool _multidirectional;

    public List<CityNode> _cities;
    public List<TunnelScript> _tunnels;
    public List<TunnelScript> _invertedTunnels;

    public bool _drillTunnel = false;

    public bool _fillingWithWater = false;

    public bool _filledWithWater = false;

    Sequence _sequence;

    public void FillWithWater(bool inverted){
        if(_fillingWithWater){
            return;
        }

        _fillingWithWater = true;

        inverted = _drillTunnel ? !inverted : inverted;

        if(inverted){
            _lineRenderer.material.SetInt("_Up", 0);
        }

        _sequence = DOTween.Sequence();
        _lineRenderer.material.SetFloat("_Percentage", 0);
        _lineRenderer.material.SetFloat("_Percentage_Add", 0);
        _sequence.Insert(0, _lineRenderer.material.DOFloat(1.2f, "_Percentage", 2 * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));
        _sequence.Insert(0, _lineRenderer.material.DOFloat(-0.08f, "_Percentage_Add", 2 * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));
        
        Debug.Log("Aspargo legal 100: " + gameObject.name);

        _sequence.OnComplete(()=>{
            Debug.Log("Aspargo legal 90000: " + gameObject.name);
            _filledWithWater = true;

            foreach (CityNode city in _cities){
                if(!city._fillingWithWater){
                    city.FillWithWater();
                }
            }

            foreach (TunnelScript tunnel in _tunnels)
            {
                Debug.Log("Aspargo legal 000: " + gameObject.name);
                if(!tunnel._fillingWithWater){
                    foreach (KeyValuePair<Collider2D, Vector2> collidedTunnel in _colliderTunnel._collidedTunnels)
                    {
                        Debug.Log("Aspargo legal 0: " + gameObject.name);
                        if(collidedTunnel.Key.transform.parent.parent.GetComponent<TunnelScript>() == tunnel){
                            Debug.Log("Aspargo legal 1: " + gameObject.name);
                            tunnel._multidirectional = true;
                            tunnel._lineRenderer.material = new Material(_waterMultidirectional);
                            tunnel.FillWithWaterMultidirectional(_invertedTunnels.Contains(tunnel), collidedTunnel.Value);
                            continue;
                        }   
                    }

                    tunnel.FillWithWater(_invertedTunnels.Contains(tunnel));
                }
            }
        });
    }

    private float InverseLerpVector2(Vector2 a, Vector2 b, Vector2 value)
    {
        Vector2 AB = b - a;
        Vector2 AV = value - a;
        return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
    }

    private float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }

    public void FillWithWaterMultidirectional(bool inverted, Vector2 intersectionPosition){
        if(_fillingWithWater){
            return;
        }
        
        _fillingWithWater = true;

        inverted = _drillTunnel ? !inverted : inverted;

        if(inverted){
            _lineRenderer.material.SetInt("_Up", 0);
        }

        float middlePos = InverseLerpVector2(_lineRenderer.GetPosition(0), _lineRenderer.GetPosition(1), intersectionPosition);

        _lineRenderer.material.SetFloat("_Rotate", 90);
        _lineRenderer.material.SetFloat("_FinalAlpha", WaterFill._waterAlpha);
        _lineRenderer.material.SetFloat("_Percentage1Start", inverted ? middlePos : 1 - middlePos);
        _lineRenderer.material.SetFloat("_Percentage2Start", inverted ? 1 - middlePos : middlePos);

        _sequence = DOTween.Sequence();
        _lineRenderer.material.SetFloat("_Percentage", Remap(0, 1 - middlePos, 0, 0.75f, 1));
        _lineRenderer.material.SetFloat("_Percentage2", Remap(0, middlePos, 0, 0.75f, 1));
        _lineRenderer.material.SetFloat("_Percentage_Add", 0);
        _lineRenderer.material.SetFloat("_Percentage_Add_2", 0);
        _sequence.Insert(0, _lineRenderer.material.DOFloat(0f, "_Percentage", 2 * (middlePos) * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));
        _sequence.Insert(0, _lineRenderer.material.DOFloat(0f, "_Percentage2", 2 * (1 - middlePos) * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));
        _sequence.Insert(0, _lineRenderer.material.DOFloat(-0.08f, "_Percentage_Add", 2 * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));
        _sequence.Insert(0, _lineRenderer.material.DOFloat(-0.15f, "_Percentage_Add_2", 2 * (_tunnels.Count + _cities.Count)).SetEase(Ease.Linear));

        _sequence.OnComplete(()=>{
            _filledWithWater = true;

            foreach (CityNode city in _cities){
                if(!city._fillingWithWater){
                    city.FillWithWater();
                }
            }

            foreach (TunnelScript tunnel in _tunnels)
            {
                if(!tunnel._fillingWithWater){
                    tunnel.FillWithWater(_invertedTunnels.Contains(tunnel));
                }
            }
        });
    }
}
