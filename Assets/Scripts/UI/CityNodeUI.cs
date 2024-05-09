using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System;

public class CityNodeUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 _uiPosition;

    [HideInInspector] public List<PointLocatorColliderLine> _collidedLines = new List<PointLocatorColliderLine>();

    [HideInInspector] public List<CityNode> _collidedCityAreas = new List<CityNode>();

    [SerializeField] private GameObject _cityPrefab;

    [SerializeField] private GameObject _pointLocatorPrefab;

    [SerializeField] private GameObject _linePrefab;

    [SerializeField] private CityNodesHolder _citiesNodeHolder;

    private PointLocatorColliderCircle _pointLocatorCircle;

    private PointLocatorColliderLine _pointLocatorLine;

    private LineRenderer _line;

    private TunnelScript _tunnel;

    private EdgeCollider2D _lineEdgeCollider;

    [SerializeField] private Image _image;

    [SerializeField] private Color _defaultColor;

    [SerializeField] private Color _holdingColor;

    [SerializeField] private Color _negativeColor;

    private bool _followingMouse = false;

    private bool _onMouseUp = false;

    private CityNode _currentCityNode;

    private void Start(){
        _uiPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData){
        _followingMouse = true;
        _citiesNodeHolder._enablePlaceNodeCollider?.Invoke(true);
        _line = Instantiate(_linePrefab, Vector2.zero, Quaternion.identity).GetComponent<LineRenderer>();
        _tunnel = _line.GetComponent<TunnelScript>();
        _lineEdgeCollider = _line.transform.Find("Collider Tunnel").GetComponent<EdgeCollider2D>();

        _line.material = new Material(_line.material);
        _tunnel._lineRenderer.material = new Material(_tunnel._lineRenderer.material);
        
        _line.material.SetFloat("_Rotate", 90);
        _tunnel._lineRenderer.material.SetFloat("_Rotate", 90);

        _pointLocatorCircle = Instantiate(_pointLocatorPrefab, Vector2.zero, Quaternion.identity).GetComponent<PointLocatorColliderCircle>();
        _pointLocatorLine = _pointLocatorCircle.GetComponentInChildren<PointLocatorColliderLine>();

        _pointLocatorCircle.Init(this);
        _pointLocatorLine.Init(this);

        var nodePos = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, Camera.main.nearClipPlane));
            nodePos.z = 0;
        _pointLocatorCircle.transform.position = nodePos;
    }

    private void Update(){
        if(_followingMouse){
            _onMouseUp = false;
            transform.position = Input.mousePosition;
            var nodePos = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, Camera.main.nearClipPlane));
            nodePos.z = 0;
            if(_currentCityNode != null){
                SetPath(nodePos, _currentCityNode.transform.position);
                _pointLocatorLine.enabled = true;
                _pointLocatorLine.SetColliderPositions(Vector2.zero, _currentCityNode.transform.position - nodePos);
            }
            else{
                SetPath(Vector2.zero, Vector2.zero);
                _pointLocatorLine.enabled = false;
            }

            _pointLocatorCircle.transform.position = nodePos;

            _currentCityNode = null;
            StartCoroutine(EndOfFrame());
        }
    }

    private IEnumerator EndOfFrame(){
        yield return new WaitForEndOfFrame();

        if(_collidedLines.Count == 0 && _collidedCityAreas.Count > 0){
            float minDistance = Mathf.Infinity;
            _currentCityNode = null;

            foreach (CityNode city in _collidedCityAreas)
            {
                if(city._fillingWithWater){
                    continue;
                }

                float distance = Mathf.Abs(Vector3.Distance(city.transform.position, _pointLocatorCircle.transform.position));
                if(distance <= minDistance){
                    minDistance = distance;
                    _currentCityNode = city;
                }
            }

            _image.color = _currentCityNode == null ? _negativeColor : _holdingColor;
        }
        else{
            _image.color = _negativeColor;
        }
    }

    private void SetPath(Vector3 origin, Vector3 destination)
    {
        _line.material.SetFloat("_Distance", Mathf.InverseLerp(0, 30, Vector3.Distance(transform.position, destination) * 4));
        _tunnel._lineRenderer.material.SetFloat("_Distance", Mathf.InverseLerp(0, 30, Vector3.Distance(transform.position, destination) * 4));
        _line.material.SetVector("_Tilling", new Vector4(Mathf.InverseLerp(0, 30, Vector3.Distance(transform.position, destination) * 4), 1, 0, 0));
        List<Vector2> colliderPoints = new List<Vector2>();
        colliderPoints.Add(Vector2.zero);
        colliderPoints.Add(new Vector2(origin.x - destination.x, origin.y - destination.y));
        _lineEdgeCollider.SetPoints(colliderPoints);
        _line.SetPosition(0, origin);
        _line.SetPosition(1, destination);
        _tunnel._lineRenderer.SetPosition(0, origin);
        _tunnel._lineRenderer.SetPosition(1, destination);
    }

    public void OnPointerUp(PointerEventData eventData){
        _onMouseUp = true;

        if(_currentCityNode){
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, Camera.main.nearClipPlane));
            point.z = 0;
            CityNode cityNode = Instantiate(_cityPrefab, point, Quaternion.identity).GetComponent<CityNode>();
            cityNode._cities.Add(_currentCityNode);
            _currentCityNode._cities.Add(cityNode);

            EdgeCollider2D childCollider = _line.transform.Find("Thin Collider Tunnel").GetComponent<EdgeCollider2D>();
            List<Vector2> points = new List<Vector2>();
            _lineEdgeCollider.GetPoints(points);
            childCollider.SetPoints(points);

            cityNode._paths.Add(_line);
            _currentCityNode._paths.Add(_line);
            _line.transform.SetParent(null);
            _line.transform.position = new Vector3(_currentCityNode.transform.position.x,_currentCityNode.transform.position.y, 1);

            cityNode._tunnels.Add(_tunnel);
            _currentCityNode._tunnels.Add(_tunnel);
            
            _tunnel._cities.Add(_currentCityNode);
            _tunnel._cities.Add(cityNode);

            cityNode._invertedTunnels.Add(_tunnel);

            Light2D light2d = cityNode.GetComponentInChildren<Light2D>();
            light2d.intensity = 0f;

            cityNode._cityNodesHolder = _citiesNodeHolder;

            cityNode._cityNodesHolder.ConstructingCity(cityNode);
            cityNode.onNodePlace?.Invoke();

            Destroy(_pointLocatorCircle.gameObject);

            _followingMouse = false;
            transform.position = _uiPosition;
            _image.color = _defaultColor;

            _currentCityNode = null;
            _citiesNodeHolder._enablePlaceNodeCollider?.Invoke(false);
        }
        else{
                _followingMouse = false;
                transform.position = _uiPosition;
                _image.color = _defaultColor;

                Destroy(_pointLocatorCircle.gameObject);
                Destroy(_line.gameObject);
                _citiesNodeHolder._enablePlaceNodeCollider?.Invoke(false);
            }
    }
}
