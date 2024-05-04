using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CityNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Material _outline;
    [SerializeField] private List<LineRenderer> _paths;
    [SerializeField] private CityNode _prefab;

    [SerializeField] private bool _placingMode;

    [SerializeField] private Material _waterMaterial;

    float _timerIgnoreClick = 0f;

    Sequence _sequence;

    public void SetPath(int index, Vector3 destination)
    {
        var line = _paths[index];
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
            SetPath(_paths.Count-1, pos);

            _timerIgnoreClick += Time.deltaTime;
            if(Input.GetMouseButtonDown(0) && _timerIgnoreClick > 0.1f)
            {
                _placingMode = false;
                _timerIgnoreClick = 0f;
                Instantiate(_prefab.gameObject, pos, Quaternion.identity);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleMode();
        var gameObjectChild = new GameObject();
        gameObjectChild.name = "Line Renderer";
        gameObject.transform.SetParent(this.transform);
        var line = gameObjectChild.AddComponent<LineRenderer>();
        line.sortingOrder = -1;
        line.material = new Material(_waterMaterial);
        line.material.SetFloat("_FinalAlpha", WaterFill._waterAlpha);
        _sequence.Append(line.material.DOFloat(1.2f, "_Percentage", 2));
        _sequence.Join(line.material.DOFloat(-0.08f, "_Percentage_Add", 2));
        line.SetPosition(0, transform.position);
        _paths.Add(line);
    }
}
