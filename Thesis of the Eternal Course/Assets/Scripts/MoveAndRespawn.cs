using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveAndRespawn : MonoBehaviour
{
    public float speed;
    public Transform positionSpawn;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed, 0);
    }

    private void Update()
    {
        if (transform.position.x < -24)
            transform.position = new Vector2(positionSpawn.position.x, positionSpawn.position.y);
    }
}
