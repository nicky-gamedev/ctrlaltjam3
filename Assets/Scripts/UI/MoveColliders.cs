using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveColliders : MonoBehaviour
{

    [HideInInspector] public bool _enabled = true;

    [SerializeField] float _delta = 10; // Pixels. The width border at the edge in which the movement work

    [SerializeField] float _speed = 3.0f; // Scale. Speed of the movement

    float _timerUp = 0;

    float _timerDown = 0;

    [SerializeField] float _timerSpeed = 0.1f;

    [SerializeField] OreGeneration _oreGeneration;
    
    void FixedUpdate ()
    {
        if(_enabled){
            float screenPercentage = Screen.height / _delta;
            if (Input.mousePosition.y >= Screen.height - screenPercentage)
            {
                _timerUp = Mathf.Clamp(_timerUp + Time.fixedDeltaTime * _timerSpeed, 0, 1);
                _timerDown = Mathf.Clamp(_timerDown - Time.fixedDeltaTime * _timerSpeed, 0, 1);
                // Move the camera up
                Camera.main.transform.position += Vector3.up * Time.deltaTime * Mathf.Lerp(0, _speed, _timerUp);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y >= 0 ? 0 : Camera.main.transform.position.y, Camera.main.transform.position.z);
            }
            else{
                if(Input.mousePosition.y <=  screenPercentage){
                    // Move the camera down
                    _timerUp = Mathf.Clamp(_timerUp - Time.fixedDeltaTime * _timerSpeed, 0, 1);
                    _timerDown = Mathf.Clamp(_timerDown + Time.fixedDeltaTime * _timerSpeed, 0, 1);
                    Camera.main.transform.position += Vector3.down * Time.deltaTime * Mathf.Lerp(0, _speed, _timerDown);

                    if(Camera.main.transform.position.y <= _oreGeneration.NextChunk)
                    {
                        _oreGeneration.GenerateChunk();
                    }
                }
                else{
                    _timerUp = Mathf.Clamp(_timerUp - Time.fixedDeltaTime * _timerSpeed, 0, 1);
                    _timerDown = Mathf.Clamp(_timerDown - Time.fixedDeltaTime * _timerSpeed, 0, 1);
                }
            }
        }
    }
}
