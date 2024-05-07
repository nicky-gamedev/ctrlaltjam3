using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillScript : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private Rigidbody2D rigidbody;


    private void Start()
    {
        Destroy(gameObject, 5f);
    }
    
    void Update()
    {
        rigidbody.velocity = transform.up * velocity;
    }
}
