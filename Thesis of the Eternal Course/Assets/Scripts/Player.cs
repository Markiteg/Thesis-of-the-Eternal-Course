using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float TimeJump;
    public float StartTimeJump;
    public float StartJumpCount;
    public float TimeBTWDash;
    public float StartTimeBTWDash;
    public float TimeAFTDash;
    public float StartTimeAFTDash;
    public float DashSpeed;
    public float TimeWallJump;
    public float StartTimeWallJump;
    public float upDownSpeed;
    public float SlideSpeed;
    public float JumpWallTime;
    public Vector2 jumpAngle = new Vector2(3.5f, 10);
    public float FixYSpeed;

    public float timeBTWattack; //Attack
    public float startTimeBTWAttack;
    public Transform atkPos;
    public float atkRange;
    public LayerMask enemy;
    public float weaponDamage;
    public float health;
    public float TimeBTWdmg;
    public float StartTimeBTWdmg;
    public float TimeAFTAttack;
    public float StartAFTAttack;
    public int AttackNum = 0;

    private bool IsGround = false;
    private float JumpCount = 1;
    private bool JumpControl;
    private bool IsSide;
    private float DashCount = 1;
    private float StartDashCount = 1;
    private bool IsSite;
    private bool IsCanStay;
    private bool IsWallRight;
    private bool IsWall;
    private float GravityDev;
    private bool BlockMove = false;
    private float TimerJumpWall;
    private bool IsWallJumping = false;
    private bool IsDash = false;
    private bool IsAttack = false;
    private bool IsEnemy = false;
    public static bool IsDialog = false;


    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private Animator anim;
    private SpriteRenderer sr;

    public GameObject GOisGround;
    public GameObject GOIsCanStay;
    public GameObject GOIsWallRight;
    public LayerMask Ground;
    public LayerMask Dialog;
    private void Awake()
    {
        IsSide = true;
        IsSite = false;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        GravityDev = rb.gravityScale;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R)) // Reset
        {
            gameObject.transform.position = new Vector2(0, 0);
        }

        IsGround = Physics2D.OverlapCircle(GOisGround.transform.position, 0.1f, Ground); //Check ground
        //IsCanStay = Physics2D.OverlapCircle(GOIsCanStay.transform.position, 0.5f, Ground); //Can stay or not
        IsWallRight = Physics2D.OverlapCircle(GOIsWallRight.transform.position, 0.1f, Ground); //Check walls in right side
        IsEnemy = Physics2D.OverlapCircle(atkPos.transform.position, 0.5f, enemy);
        IsDialog = Physics2D.OverlapCircle(gameObject.transform.position, 0.5f, Dialog);

        if (health <= 0)
            Destroy(gameObject);

        if (IsSide)
            gameObject.transform.localScale = new Vector2(-1, 1);
        else
            gameObject.transform.localScale = new Vector2(1, 1);

        if (Input.GetKey(KeyCode.A)) // Walk in left
        {
            if (!BlockMove)
            {
                if (!IsAttack)
                {
                    transform.Translate(Vector3.left * speed * Time.deltaTime);
                    IsSide = true;
                    anim.SetBool("IsRun", true);
                }
            }
        }
        else if (Input.GetKey(KeyCode.D)) // Walk in right
        {
            if (!BlockMove)
            {
                if (!IsAttack)
                {
                    transform.Translate(Vector3.right * speed * Time.deltaTime);
                    IsSide = false;
                    anim.SetBool("IsRun", true);
                }
            }
        }
        else
            anim.SetBool("IsRun", false);

        if (!BlockMove)
        {
            if (!IsAttack)
            {
                if (Input.GetKey(KeyCode.Space)) // Jump
                {
                    if (IsGround)
                        JumpControl = true;
                }
                else
                    JumpControl = false;
                if (JumpControl)
                {
                    if (TimeJump > 0)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        TimeJump -= Time.deltaTime;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Space) && !JumpControl && !IsGround && JumpCount > 0)
                {
                    JumpCount--;
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce + 1);
                }
                else if (IsGround)
                {
                    TimeJump = StartTimeJump;
                    JumpCount = StartJumpCount;
                    DashCount = StartDashCount;
                }
            }
        }

        if (IsWallRight && !IsGround) //Check in wall
        {
            JumpCount = StartJumpCount;
            DashCount = StartDashCount;
            IsWall = true;
            //anim.SetBool("InWall", true);
        }
        else
        {
            IsWall = false;
            //anim.SetBool("InWall", false);
        }

        if (IsWall && !IsGround) //Can ships are being built
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, SlideSpeed);

            if (Input.GetKey(KeyCode.W))
            {
                rb.velocity = new Vector2(rb.velocity.x, upDownSpeed);
            }
        }
        else if (!IsGround && !IsWall)
            rb.gravityScale = GravityDev;

        if (IsWall && !IsGround && Input.GetKey(KeyCode.Space))//Jump
        {
            BlockMove = true;
            TimerJumpWall = JumpWallTime;
            IsWallJumping = true;
        }
        if (BlockMove && TimerJumpWall <= 0)
        {
            BlockMove = false;
            IsWallJumping = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
            TimerJumpWall -= Time.deltaTime;
        if (IsWallJumping && IsWallRight)
        {
            if (IsSide)
                rb.velocity = new Vector2(jumpAngle.x, jumpAngle.y);
            else
                rb.velocity = new Vector2(jumpAngle.x * -1, jumpAngle.y);
        }

        if (IsDialog && Input.GetKeyDown(KeyCode.E))//Активация диалогов(надо дороботать, как их различать, чтобы можно было бы ставить нормальный текст)
        {
            GameObject gm = Physics2D.OverlapCircle(gameObject.transform.position, 0.5f, Dialog).GetComponent<GameObject>();

            if (gm.tag == "1")
                ForUI.ActivatedDialog(1);
        }

        if (Input.GetKeyDown(KeyCode.Q) && TimeBTWDash <= 0) //Use dash
        {
            if (!IsAttack)
                Dash();
        }
        else
        {
            TimeBTWDash -= Time.deltaTime;
            if (!BlockMove)
            {
                if (TimeAFTDash <= 0 && IsDash)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    IsDash = false;
                }
                else
                    TimeAFTDash -= Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) //Crouch
        {
            if (!IsSite)
            {
                IsSite = true;
                bc.offset = new Vector2(0, -0.151821f);
                bc.size = new Vector2(1, 1.103642f);
            }
            else if (!IsCanStay)
            {
                IsSite = false;
                bc.offset = new Vector2(0, 0.3f);
                bc.size = new Vector2(1, 2);
            }
        }

        if (rb.velocity.y < FixYSpeed) //Fix speed in Y
            rb.velocity = new Vector2(rb.velocity.x, FixYSpeed);

        if (Input.GetKey(KeyCode.F) && !IsGround && !IsWallRight && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -2);
        }

        if (timeBTWattack <= 0)
        {
            if (Input.GetMouseButtonDown(0) && IsGround)
            {
                if (AttackNum == 2)
                    AttackNum = 0;
                AttackNum++;
                IsAttack = true;
                TimeAFTAttack = StartAFTAttack;
                timeBTWattack = startTimeBTWAttack;
                anim.SetInteger("AttackCount", AttackNum);

                if (IsEnemy)
                    Physics2D.OverlapCircle(atkPos.transform.position, 0.5f, enemy).GetComponent<Enemy>().Damage(weaponDamage);
            }
        }
        else
        {
            timeBTWattack -= Time.deltaTime;
        }
        if (TimeAFTAttack <= 0)
        {
            AttackNum = 0;
            IsAttack = false;
            anim.SetInteger("AttackCount", AttackNum);
        }
        else
            TimeAFTAttack -= Time.deltaTime;
    }

    public void Dash() //Dash
    {
        if (!IsSide) //Take side 
        {
            if (!IsGround && DashCount > 0) //Check ground and count dash
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                IsDash = true;
                DashCount--;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround) //Check ground
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed, 0);
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }

        }
        else 
        {
            if (!IsGround && DashCount > 0)
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                DashCount--;
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
            else if (IsGround)
            {
                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(DashSpeed * -1, rb.velocity.y);
                else
                    rb.velocity = new Vector2(DashSpeed * -1, 0);
                IsDash = true;
                TimeBTWDash = StartTimeBTWDash;
                TimeAFTDash = StartTimeAFTDash;
            }
        }
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

