using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TunnelScript : MonoBehaviour
{
    public LineRenderer _lineRenderer;
    public List<CityNode> _cities;
    public List<TunnelScript> _tunnels;
    public List<TunnelScript> _invertedTunnels;

    public bool _drillTunnel = false;

    public bool _fillingWithWater = false;

    Sequence _sequence;

    public void FillWithWater(bool inverted){
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
    
        _sequence.OnComplete(()=>{
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
