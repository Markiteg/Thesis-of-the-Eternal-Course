using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using System;
using MySql.Data.MySqlClient;
using System.Data;

public class CamMove : MonoBehaviour
{
    public GameObject player;

    public GameObject mountains, ruins;

    public float lockX, lockY;
    public float offset;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + offset, -10f);
        
    }

    void Start()
    {

    }
}
