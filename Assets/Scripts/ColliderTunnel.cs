using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ColliderTunnel : MonoBehaviour
{
    public BoxCollider2D _collider;

    public LayerMask _layerMask;

    public Dictionary<Collider2D, Vector2> _collidedTunnels = new Dictionary<Collider2D, Vector2>();

    private void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.name == "Collider Tunnel" || col.gameObject.name == "Collider Mini Tunnel"){
            if(!_collidedTunnels.ContainsKey(col)){
                List<Vector2> points = new List<Vector2>();
                Vector3 initialPos = transform.parent.position;
                Vector3 finalPos = initialPos + col.bounds.center;
                Debug.DrawLine(initialPos, finalPos - initialPos, Color.white, 20f, true); 
                RaycastHit2D[] hits = Physics2D.RaycastAll(initialPos, finalPos - initialPos, Vector2.Distance(initialPos, finalPos));
                if(hits != null && hits.Length > 0){
                    foreach(RaycastHit2D hit in hits)
                    {
                        if(hit.collider == col){
                            Debug.Log("Aspargo 6: " + col.gameObject.name);
                            _collidedTunnels.Add(col, hit.point);

                            if(col.gameObject.name == "Collider Mini Tunnel"){
                                col.transform.GetComponent<ColliderTunnel>()?._collidedTunnels?.Add(_collider, hit.point);
                            }
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
