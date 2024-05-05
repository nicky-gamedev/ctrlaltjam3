using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class CityNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Material _outline;
    public List<LineRenderer> _paths;
    public List<LineRenderer> _pathsWater;
    public List<LineRenderer> _invertedPathsWater;
    public List<CityNode> _cities;
    [SerializeField] private GameObject _prefab;

    [SerializeField] private bool _placingMode;

    [SerializeField] private Gradient _tunnelColor;

    [SerializeField] private Material _waterMaterial;

    [SerializeField] private WaterFill _waterFill;

    [SerializeField] private Light2D _light2D;

    public bool _fillingWithWater = false;

    float _timerIgnoreClick = 0f;

    Sequence _sequence;
    
    public void FillWithWater(){
        _fillingWithWater = true;
        _waterFill.Fill(()=>{
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

    public void SetPath(List<LineRenderer> paths, int index, Vector3 destination)
    {
        var line = paths[index];
        line.material.SetFloat("_Distance", Mathf.InverseLerp(0, 30, Vector3.Distance(transform.position, destination) * 4));
        if(paths == _paths){
            line.material.SetVector("_Tilling", new Vector4(Mathf.InverseLerp(0, 30, Vector3.Distance(transform.position, destination) * 4), 1, 0, 0));
        }
        line.SetPosition(0, transform.position);
        line.SetPosition(1, destination);
    }

    public void ToggleMode()
    {
        _placingMode = !_placingMode;
    }

    private void Update()
    {
        if (_placingMode)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            SetPath(_paths, _paths.Count-1, pos);
            SetPath(_pathsWater, _pathsWater.Count-1, pos);

            _timerIgnoreClick += Time.deltaTime;
            if(Input.GetMouseButtonDown(0) && _timerIgnoreClick > 0.1f)
            {
                _placingMode = false;
                _timerIgnoreClick = 0f;
                CityNode cityNode = Instantiate(_prefab, pos, Quaternion.identity).GetComponent<CityNode>();
                cityNode._paths.Clear();
                cityNode._pathsWater.Clear();
                cityNode._cities.Clear();
                cityNode._paths.Add(_paths[_paths.Count-1]);
                cityNode._pathsWater.Add(_pathsWater[_pathsWater.Count-1]);
                cityNode._invertedPathsWater.Add(_pathsWater[_pathsWater.Count-1]);
                cityNode._cities.Add(this);
                Transform[] transforms = cityNode.GetComponentsInChildren<Transform>();

                foreach (Transform t in transforms)
                {
                    if (t.name == "Line Renderer")
                    {
                        Destroy(t.gameObject);
                    }
                }

                _cities.Add(cityNode);
            }
        }
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

        ToggleMode();

        var gameObjectChild = new GameObject();
        gameObjectChild.name = "Line Renderer";
        gameObjectChild.transform.SetParent(this.transform);
        gameObjectChild.transform.position = new Vector3(0,0,0);

        var gameObjectGrandchild = new GameObject();
        gameObjectGrandchild.name = "Line Renderer Water";
        gameObjectGrandchild.transform.SetParent(gameObjectChild.transform);

        var line = gameObjectChild.AddComponent<LineRenderer>();
        var lineWater = gameObjectGrandchild.AddComponent<LineRenderer>();

        line.sortingOrder = -2;
        line.material = _outline;
        line.colorGradient  = _tunnelColor;

        lineWater.sortingOrder = -1;
        lineWater.material = new Material(_waterMaterial);
        lineWater.material.SetFloat("_FinalAlpha", WaterFill._waterAlpha);

        line.SetPosition(0, transform.position);
        lineWater.SetPosition(0, transform.position);

        _paths.Add(line);
        _pathsWater.Add(lineWater);
    }
}
