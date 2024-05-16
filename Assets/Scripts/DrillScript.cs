using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class DrillScript : MonoBehaviour
{
    [SerializeField] private float _velocity;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private Color _defaultColor;

    [SerializeField] private Color _negativeColor;

    [SerializeField] private LineRenderer _tunnelRenderer;

    public TunnelScript _tunnel;

    [SerializeField] private BoxCollider2D _collider;

    [SerializeField] private BoxCollider2D _miniColider;

    [HideInInspector] public OreManager _oreManager;
    
    [SerializeField] private UnityEvent _onLaunch = new UnityEvent();

    public SpriteRenderer _spriteRenderer;

    public bool _enabled = false;

    [HideInInspector] public bool _ableToShoot = false;

    private List<GameObject> _insideColliders = new List<GameObject>();

    private List<Collider2D> _ignoreColliders = new List<Collider2D>();

    private Vector3 _initialPos;

    [SerializeField] private Light2D _light2D;

    [SerializeField] private AnimationCurve _animationCurve;

    DG.Tweening.Sequence _sequence;

    public void InitializeLaunch(List<Collider2D> ignoreColliders, OreManager oreManager, float duration){
        _enabled = true;

        _oreManager = oreManager;

        _ignoreColliders = ignoreColliders;

        _initialPos = transform.position;

        _tunnelRenderer.material = new Material(_tunnelRenderer.material);
        _tunnel._lineRenderer.material = new Material(_tunnel._lineRenderer.material);

        _tunnelRenderer.material.SetFloat("_Rotate", 90);
        _tunnelRenderer.material.SetFloat("_SineValue1", 35);
        _tunnel._lineRenderer.material.SetFloat("_Rotate", 90);
        _tunnel._lineRenderer.material.SetFloat("_SineValue1", 35);

        _sequence = DOTween.Sequence();
        _sequence.Append(DOTween.To(x => {_light2D.intensity = x;}, 0f, 1f, duration).SetEase(_animationCurve));

        _onLaunch.Invoke();

        Destroy(this.gameObject, duration);
    }

    public void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel"){
            if(!_insideColliders.Contains(col.gameObject)){
                _insideColliders.Add(col.gameObject);
            }
        }

        if(col.gameObject.name == "Thin Collider Tunnel Drill"){
            if(_enabled){
                TunnelScript tunnel = col.transform.parent.parent.GetComponent<TunnelScript>(); 
                if(tunnel != null){
                    tunnel._tunnels.Add(_tunnel);
                    tunnel._invertedTunnels.Add(_tunnel);
                    _tunnel._tunnels.Add(tunnel);

                    if(tunnel._filledWithWater){
                        _tunnel.FillWithWater(true);
                    }
                }
            }
        }

        if(col.gameObject.name == "Thin Collider Tunnel" || col.gameObject.name == "Drill Node Collider"){
            if(_enabled){
                _enabled = false;
                _rigidbody.velocity = Vector2.zero;

                TunnelScript tunnel = col.transform.parent.parent.GetComponent<TunnelScript>(); 
                if(tunnel != null){
                    tunnel._tunnels.Add(_tunnel);
                    tunnel._invertedTunnels.Add(_tunnel);
                    _tunnel._tunnels.Add(tunnel);

                    if(tunnel._filledWithWater){
                        _tunnel.FillWithWater(true);
                    }
                }
                else{
                    
                    if(col.gameObject.name == "Drill Node Collider"){

                        if(col.transform.parent.TryGetComponent<CityNode>(out CityNode cityNode))
                        {
                            _tunnel._cities.Add(cityNode);

                            if (cityNode._filledWithWater)
                            {
                                _tunnel.FillWithWater(true);
                            }
                        }
                        else if(col.transform.parent.name.Contains("Ore"))
                        {
                            _oreManager._OreAmount += UnityEngine.Random.Range(3, 4);
                            Destroy(col.transform.parent.gameObject);
                        }
                    }
                }

                Destroy(this.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel"){
            if(_insideColliders.Contains(col.gameObject)){
                _insideColliders.Remove(col.gameObject);
            }
        }
    }

    private void SetPath(Vector3 origin, Vector3 destination)
    {
        _tunnelRenderer.material.SetFloat("_Distance", Mathf.InverseLerp(0, 100, Vector3.Distance(origin, destination) * 4));
        _tunnel._lineRenderer.material.SetFloat("_Distance", Mathf.InverseLerp(0, 100, Vector3.Distance(origin, destination) * 4));
        _tunnelRenderer.material.SetVector("_Tilling", new Vector4(Mathf.InverseLerp(0, 100, Vector3.Distance(origin, destination) * 4), 1, 0, 0));
        
        Vector2 direction = origin - destination;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        _collider.transform.parent.rotation = rotation;
        _miniColider.transform.parent.rotation = rotation;
        _collider.size = new Vector2(_collider.size.x, Vector3.Distance(origin, destination) / _collider.transform.lossyScale.y);
        _miniColider.size = new Vector2(_miniColider.size.x, Vector3.Distance(origin, destination) / _miniColider.transform.lossyScale.y);
        _collider.transform.localPosition = new Vector3(0, _collider.size.y / 2, 0);
        _miniColider.transform.localPosition = new Vector3(0, _miniColider.size.y / 2, 0);

        _tunnelRenderer.SetPosition(0, origin);
        _tunnelRenderer.SetPosition(1, destination);
        _tunnel._lineRenderer.SetPosition(0, origin);
        _tunnel._lineRenderer.SetPosition(1, destination);
    }

    void Update()
    {
        _ableToShoot = _insideColliders.Count == 0;
        _spriteRenderer.color = _ableToShoot || _enabled ? _defaultColor : _negativeColor;

        if(_enabled){
            _rigidbody.velocity = transform.up * _velocity;
            SetPath(_initialPos, transform.position);
        }
    }

    public void OnDestroy(){
        _tunnelRenderer.transform.SetParent(null);
    }
}
