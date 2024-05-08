using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float speed;
    public float waeponDMG;
    public float TimeBTWdmg;
    public float StartTimeBTWdmg;

    void Start()
    {

    }

    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);
    }

    public void Damage(float dmg)
    {
        if (health > 0)
        {
            if (TimeBTWdmg <= 0)
            {
                //anim.SetBool("DMG", true);
                health -= dmg;
                TimeBTWdmg = StartTimeBTWdmg;
            }
        }
    }
}
