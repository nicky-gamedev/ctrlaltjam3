using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrillScript : MonoBehaviour
{
    [SerializeField] private float _velocity;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private Color _defaultColor;

    [SerializeField] private Color _negativeColor;

    [SerializeField] private LineRenderer _tunnelRenderer;

    [SerializeField] private LineRenderer _waterRenderer;

    [SerializeField] private EdgeCollider2D _collider;

    public SpriteRenderer _spriteRenderer;

    public bool _enabled = false;

    [HideInInspector] public bool _ableToShoot = false;

    private List<GameObject> _insideColliders = new List<GameObject>();

    private List<Collider2D> _ignoreColliders = new List<Collider2D>();

    private Vector3 _initialPos; 

    public void InitializeLaunch(List<Collider2D> ignoreColliders){
        _enabled = true;

        _ignoreColliders = ignoreColliders;

        _initialPos = transform.position;
        _tunnelRenderer.material = new Material(_tunnelRenderer.material);
        _waterRenderer.material = new Material(_waterRenderer.material);

        _tunnelRenderer.material.SetFloat("_Rotate", 90);
        _tunnelRenderer.material.SetFloat("_SineValue1", 35);
        _waterRenderer.material.SetFloat("_Rotate", 90);
        _waterRenderer.material.SetFloat("_SineValue1", 35);
    }

    public void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel"){
            if(!_insideColliders.Contains(col.gameObject)){
                _insideColliders.Add(col.gameObject);
            }
        }

        if(col.gameObject.name == "Thin Collider Tunnel" || col.gameObject.name == "Drill Node Collider"){
            if(_enabled){
                _enabled = false;
                _rigidbody.velocity = Vector2.zero;
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
        _waterRenderer.material.SetFloat("_Distance", Mathf.InverseLerp(0, 100, Vector3.Distance(origin, destination) * 4));
        _tunnelRenderer.material.SetVector("_Tilling", new Vector4(Mathf.InverseLerp(0, 100, Vector3.Distance(origin, destination) * 4), 1, 0, 0));
        List<Vector2> colliderPoints = new List<Vector2>();
        colliderPoints.Add(Vector2.zero);
        colliderPoints.Add(new Vector2(origin.x - destination.x, origin.y - destination.y));
        _collider.SetPoints(colliderPoints);
        _tunnelRenderer.SetPosition(0, origin);
        _tunnelRenderer.SetPosition(1, destination);
        _waterRenderer.SetPosition(0, origin);
        _waterRenderer.SetPosition(1, destination);
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
