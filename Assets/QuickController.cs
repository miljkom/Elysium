using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickController : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject boundary;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(Vector2.up * force);
    }
}
