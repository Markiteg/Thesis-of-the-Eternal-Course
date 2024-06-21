using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using System.Data.SqlClient;
using System.Data;

public class CamMove : MonoBehaviour
{
    public GameObject player;

    public float lockX, lockY;
    public float offset;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + offset, -10f);
        
    }

    
}
