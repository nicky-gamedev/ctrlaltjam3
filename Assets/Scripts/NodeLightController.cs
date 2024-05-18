using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NodeLightController : MonoBehaviour
{
    NodeLight[] nodeLights;
    [SerializeField] Light2D[] lights;
    void Awake()
    {   
        nodeLights = GetComponentsInChildren<NodeLight>();
        
        foreach (Light2D light in lights){
            light.intensity = .05f;
        }
    }
    public void TurnLightsOn(){
        foreach (NodeLight nodeLight in nodeLights){
            nodeLight.TurnLightOn();
        }
        foreach (Light2D light in lights){
            DOTween.To(() => light.intensity, x => light.intensity = x, 1, 2);
        }
    }
    public void Blink(){
        foreach (NodeLight nodeLight in nodeLights){
            nodeLight.Blink();
        }
        foreach (Light2D light in lights){
            DOTween.To(() => light.intensity, x => light.intensity = x, 0, 5);
        }
    }
}
