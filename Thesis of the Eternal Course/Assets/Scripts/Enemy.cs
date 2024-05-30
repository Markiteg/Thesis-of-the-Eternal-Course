using System;
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
    public float TimeWalk, StartTimeWalk;
    public float visionRadius;
    public float visionAngle;
    public float jumpForce;
    private bool isPlayerInSight = false;
    private bool IsGround;
    private bool IsEnemy;
    private float TimeBTWAttack;
    public float StartTimeBTWAttack;

    private Transform player;
    private Rigidbody2D rb;

    public LayerMask obstacleLayer;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public GameObject GOisGround;
    public GameObject IsAttack;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, groundLayer);
        IsEnemy = Physics2D.OverlapCircle(IsAttack.transform.position, 0.5f, playerLayer);

        CheckVision();
        MoveEnemy();

        if (health <= 0)
            Destroy(gameObject);
        
        if (TimeBTWdmg > 0)
            TimeBTWdmg -= Time.deltaTime;
        if (TimeBTWAttack > 0)
            TimeBTWAttack -= Time.deltaTime;
    }

    private void Attack()
    {
        if (IsEnemy)
        {
            if (TimeBTWAttack <= 0)
            {
                Physics2D.OverlapCircle(IsAttack.transform.position, 0.5f, playerLayer).GetComponent<Player>().Damage(waeponDMG);
                TimeBTWAttack = StartTimeBTWAttack;
            }
        }
    }

    private void CheckVision()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Проверить, находится ли игрок в радиусе видимости
        if (Vector2.Distance(transform.position, player.position) < visionRadius)
        {
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

            if (angleToPlayer < visionAngle / 2)
            {
                // Выполнить Raycast, чтобы проверить наличие препятствий
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRadius, ~obstacleLayer);
                // Если Raycast попадает в объект игрока, то игрок в зоне видимости
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    isPlayerInSight = true;
                    return;
                }
            }
        }
        isPlayerInSight = false;
    }

    private void MoveEnemy()
    {
        if (isPlayerInSight)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Follow player
            rb.velocity = new Vector2(directionToPlayer.x * speed, rb.velocity.y);

            if (directionToPlayer.x > 0)
                transform.localScale = new Vector2(1, 1);
            else
                transform.localScale = new Vector2(-1, 1);

            // Check ground
            IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, groundLayer);
            // Jump Enemy
            if (player.position.y > transform.position.y && IsGround)
            {
                Jump();
            }
            Attack();
        }
        else
        {
            if (TimeWalk >= 0.5)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                TimeWalk -= Time.deltaTime;
                transform.localScale = new Vector2(-1, 1);
            }
            else if (TimeWalk >= 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                TimeWalk -= Time.deltaTime;
            }
            else if (TimeWalk >= -1)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                transform.localScale = new Vector2(1, 1);

                TimeWalk -= Time.deltaTime;
            }
            else if (TimeWalk >= -1.5)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                TimeWalk -= Time.deltaTime;
            }
            else
                TimeWalk = StartTimeWalk;
        }
    }

    private void Jump()
    {
        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, groundLayer);
        if (IsGround)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
