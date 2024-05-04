using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityNode : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Material outline;
    [SerializeField] private List<LineRenderer> paths;
    [SerializeField] private CityNode prefab;

    [SerializeField] private bool placingMode;

    float timerIgnoreClick = 0f;

    public void SetPath(int index, Vector3 destination)
    {
        var line = paths[index];
        line.SetPosition(0, transform.position);
        line.SetPosition(1, destination);
    }

    public void ToggleMode()
    {
        placingMode = !placingMode;
    }

    private void Update()
    {
        if (placingMode)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            SetPath(paths.Count-1, pos);

            timerIgnoreClick += Time.deltaTime;
            if(Input.GetMouseButtonDown(0) && timerIgnoreClick > 0.1f)
            {
                placingMode = false;
                timerIgnoreClick = 0f;
                Instantiate(prefab.gameObject, pos, Quaternion.identity);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleMode();
        var line = gameObject.AddComponent<LineRenderer>();
        line.SetPosition(0, transform.position);
        paths.Add(line);
    }
}
