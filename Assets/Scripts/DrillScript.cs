using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillScript : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private Rigidbody2D rigidbody;

    void Update()
    {
        rigidbody.velocity = transform.up * velocity;
    }
}
