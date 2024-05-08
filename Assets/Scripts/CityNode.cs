using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using System;

public class CityNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Material _outline;
    public List<LineRenderer> _paths;
    public List<LineRenderer> _pathsWater;
    public List<LineRenderer> _invertedPathsWater;

    public List<CityNode> _cities;

    [SerializeField] private WaterFill _waterFill;

    [SerializeField] private Light2D _light2D;
    [SerializeField] private DrillScript _drillPrefab;

    [SerializeField] private Collider2D _placeNodeCollider;

    [SerializeField] private Collider2D _cityDrillCollider;

    [SerializeField] private float _drillDistance;

    DrillScript _currentDrill;

    public bool _fillingWithWater = false;
    public bool _aimingDrill = false;

    [HideInInspector] public float _constructingProgress;

    public CityNodesHolder _cityNodesHolder;

    DG.Tweening.Sequence _sequence;

    float _clickDelay;

    public void DoneConstructing(){
        if(!_fillingWithWater){
            _light2D.intensity = 1f;
        }
    }

    public void EnablePlaceNodeCollider(bool value){
        _placeNodeCollider.enabled = value;
    }

    private void Start(){
        _cityNodesHolder._enablePlaceNodeCollider += EnablePlaceNodeCollider;
        EnablePlaceNodeCollider(false);
    }
    
    public void FillWithWater(){
        _fillingWithWater = true;
        _waterFill.Fill(()=>{
            _cityNodesHolder.RemoveCity(this);
            _cityNodesHolder.DoneConstructing(this);
            _sequence = DOTween.Sequence();
            _sequence.Insert(0, DOTween.To(x => _light2D.intensity = x, 1f, 0f, 1f).OnComplete(()=>{_light2D.enabled = false;}));
            
            List<bool> fillingCities = new List<bool>();

            foreach (CityNode city in _cities){
                fillingCities.Add(!city._fillingWithWater);
            }

            int index = 0;
            foreach (LineRenderer pathWater in _pathsWater)
            {
                if(!fillingCities[index]){
                    index += 1;
                    continue;
                }

                if(_invertedPathsWater.Contains(pathWater)){
                    pathWater.material.SetInt("_Up", 0);
                }
                pathWater.material.SetFloat("_Percentage", 0);
                pathWater.material.SetFloat("_Percentage_Add", 0);
                _sequence.Insert(0, pathWater.material.DOFloat(1.2f, "_Percentage", 2 * _pathsWater.Count).SetEase(Ease.Linear));
                _sequence.Insert(0, pathWater.material.DOFloat(-0.08f, "_Percentage_Add", 2 * _pathsWater.Count).SetEase(Ease.Linear));
                index += 1;
            }
            _sequence.OnComplete(()=>{
                foreach (CityNode city in _cities){
                    if(!city._fillingWithWater){
                        city.FillWithWater();
                    }
                }
            });
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_fillingWithWater){
            return;
        }

        if(eventData.button == PointerEventData.InputButton.Right){
            FillWithWater();
            return;
        }

        if(eventData.button == PointerEventData.InputButton.Left && !_aimingDrill)
        {
            _aimingDrill = true;
            _cityDrillCollider.enabled = false;
            _currentDrill = Instantiate(_drillPrefab.gameObject, transform.position, Quaternion.identity).GetComponent<DrillScript>();
        }
    }

    private void Update()
    {
        if (_aimingDrill)
        {
            _clickDelay += Time.deltaTime;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = mousePos - transform.position;
            direction = direction.normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _currentDrill.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            float distance = Vector2.Distance(transform.position, mousePos);
            Vector3 newPosition = transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(_drillDistance, 0, 0);

            _currentDrill.transform.position = newPosition;

            if(_clickDelay > 0.25f && Input.GetMouseButtonDown(0))
            {
                ShootDrill(Input.mousePosition);
                _clickDelay = 0f;
            }
        }
    }

    public void ShootDrill(Vector3 mousePosition)
    {
        if(_currentDrill._ableToShoot){
            var pos = Camera.main.ScreenToWorldPoint(mousePosition);
            pos.z = 0;

            //_currentDrill.transform.position = transform.position;
            _currentDrill.transform.eulerAngles = Vector3.zero;
            Vector3 relative = transform.InverseTransformPoint(pos);
            var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            _currentDrill.transform.Rotate(0, 0, -angle);


            List<Collider2D> ignoreColliders = new List<Collider2D>();
            Collider2D collider;
            foreach (var path in _paths)
            {
                collider = path.GetComponentInChildren<Collider2D>();
                if(collider != null){
                    ignoreColliders.Add(collider);
                }
            }

            _currentDrill.InitializeLaunch(ignoreColliders);
            Destroy(_currentDrill.gameObject, 5);
            _currentDrill = null;
            _cityDrillCollider.enabled = false;
            _aimingDrill = false;
        }
    }
}
