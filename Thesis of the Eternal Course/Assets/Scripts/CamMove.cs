using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public GameObject player;

    public float lockX, lockY;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        if (transform.position.x < lockY)
        {

        }
        if (transform.position.x > lockX)
        {

        }
    }
}
