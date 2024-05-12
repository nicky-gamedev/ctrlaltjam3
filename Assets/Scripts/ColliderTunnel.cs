using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ColliderTunnel : MonoBehaviour
{
    public EdgeCollider2D _collider;

    public LayerMask _layerMask;

    public Dictionary<Collider2D, Vector2> _collidedTunnels = new Dictionary<Collider2D, Vector2>();

    private void OnTriggerEnter2D(Collider2D col){
        Debug.Log("Aspargo: " + col.gameObject.name);
        if(col.gameObject.name == "Collider Tunnel" || col.gameObject.name == "Collider Mini Tunnel"){
            Debug.Log("Aspargo 2");
            if(!_collidedTunnels.ContainsKey(col)){
                Debug.Log("Aspargo 3");
                List<Vector2> points = new List<Vector2>();
                _collider.GetPoints(points);
                Vector3 _globalPos = transform.position;
                Vector3 initialPos = _globalPos + new Vector3(points[0].x, points[0].y, 0);
                Vector3 finalPos = _globalPos + new Vector3(points[1].x, points[1].y, 0);
                RaycastHit2D[] hits = Physics2D.RaycastAll(initialPos, finalPos - initialPos, Vector2.Distance(initialPos, finalPos));
                if(hits != null && hits.Length > 0){
                    Debug.Log("Aspargo 4");
                    foreach(RaycastHit2D hit in hits)
                    {
                        Debug.Log("Aspargo 5");
                        if(hit.collider == col){
                            Debug.Log("Aspargo 6");
                            _collidedTunnels.Add(col, hit.point);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel" || col.gameObject.name == "Collider Mini Tunnel"){
            if(_collidedTunnels.ContainsKey(col)){
                _collidedTunnels.Remove(col);
            }
        }
    }
}
