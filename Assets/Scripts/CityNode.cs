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
using UnityEngine.Events;

public class CityNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Material _outline;
    public List<LineRenderer> _paths;
    public List<TunnelScript> _tunnels;
    public List<TunnelScript> _invertedTunnels;

    public List<CityNode> _cities;

    public OreManager _oreManager; 

    [SerializeField] private WaterFill _waterFill;

    [SerializeField] private Light2D _light2D;
    [SerializeField] private DrillScript _drillPrefab;

    [SerializeField] private Collider2D _placeNodeCollider;

    [SerializeField] private Collider2D _cityDrillCollider;

    [SerializeField] private float _drillDistance;

    DrillScript _currentDrill;

    public bool _fillingWithWater = false;

    public bool _filledWithWater = false;

    public bool _aimingDrill = false;

    [HideInInspector] public float _constructingProgress;

    public CityNodesHolder _cityNodesHolder;

    DG.Tweening.Sequence _sequence;

    float _clickDelay;
    
    public UnityEvent onNodePlace = new UnityEvent();

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
            _filledWithWater = true;
            _cityNodesHolder.RemoveCity(this);
            _cityNodesHolder.DoneConstructing(this);
            _sequence = DOTween.Sequence();
            _sequence.Insert(0, DOTween.To(x => _light2D.intensity = x, 1f, 0f, 1f).OnComplete(()=>{_light2D.enabled = false;}));
            
            foreach (TunnelScript tunnel in _tunnels)
            {
                if(!tunnel._fillingWithWater){
                    tunnel.FillWithWater(_invertedTunnels.Contains(tunnel));
                }
            }
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_fillingWithWater || (_constructingProgress > 0 && _constructingProgress < 1) || _oreManager._OreAmount == 0){
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
            _currentDrill.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

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

    [SerializeField] private UnityEvent onDrillShoot = new UnityEvent();
    public void ShootDrill(Vector3 mousePosition)
    {
        if(_currentDrill._ableToShoot){
            var pos = Camera.main.ScreenToWorldPoint(mousePosition);
            pos.z = 0;

            _oreManager._OreAmount -= 1;

            //_currentDrill.transform.position = transform.position;
            _currentDrill.transform.eulerAngles = Vector3.zero;
            Vector3 relative = transform.InverseTransformPoint(pos);
            var angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            _currentDrill.transform.Rotate(0, 0, -angle);

            _currentDrill._tunnel._cities.Add(this);
            _tunnels.Add(_currentDrill._tunnel);

            List<Collider2D> ignoreColliders = new List<Collider2D>();
            Collider2D collider;
            foreach (var path in _paths)
            {
                collider = path.GetComponentInChildren<Collider2D>();
                if(collider != null){
                    ignoreColliders.Add(collider);
                }
            }

            _currentDrill.InitializeLaunch(ignoreColliders, _oreManager, 2.5f);
            
            _currentDrill = null;
            _cityDrillCollider.enabled = false;
            _aimingDrill = false;
            onDrillShoot.Invoke();
        }
    }
}
