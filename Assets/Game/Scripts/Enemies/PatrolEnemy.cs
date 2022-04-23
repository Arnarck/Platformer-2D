using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public CircleCollider2D AttackCollider;

    public int Damage;
    public int HitPoints;
    public int XP_Earned;
    public int PoisoningChances;

    public float Speed;
    public float DamageRecoil;
    public float TimeToRecovery;
    //public float InvincibilityTime;
    public float AttackCoolDownTime;
    public float AttackAnimationTime;
    public float PatrolAreaLimit;

    public bool DealPoisoningDamage;

    private GameObject PlayerRef;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    private Vector3 PatrolArea_Center;

    private float PatrolArea_LeftLimit;
    private float PatrolArea_RightLimit;

    private bool MoveToRight;
    private bool isAlive = true;
    private bool CanMove = true;
    private bool CanAttack = true;
    private bool canTakeDamage = true;
    private bool TimeToAttack_IsRunning;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();

        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PatrolArea_Center = transform.position;
        PatrolArea_LeftLimit = transform.position.x - PatrolAreaLimit;
        PatrolArea_RightLimit = transform.position.x + PatrolAreaLimit;
    }


    //FixedUpdate and it's methods


    void FixedUpdate()
    {
        if (CanMove && isAlive && !UIManager.instance.GameIsPaused)
        {
            Movement();
        }
    }

    void Movement()
    {
        animator.SetBool("isMoving", true);

        //Checks if the spider is in the bounds of the Patrol area
        if (transform.position.x <= PatrolArea_LeftLimit)
        {
            MoveToRight = true;
        }
        else if (transform.position.x >= PatrolArea_RightLimit)
        {
            MoveToRight = false;
        }

        //Checks which side the spider will rotate
        if (MoveToRight)
        {
            rigidBody.velocity = new Vector2(Speed, rigidBody.velocity.y);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            rigidBody.velocity = new Vector2(-Speed, rigidBody.velocity.y);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }


    //Collision detections and methods triggered by collisions


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.IsTouching(AttackCollider) && CanAttack && isAlive)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.isAlive)
            {
                Attack();

                if (DealPoisoningDamage && playerController.CanTakeDamage)
                {
                    int choice = 0;
                    int result = Random.Range(0, PoisoningChances);

                    if (result == choice && playerController.CanPoison)
                    {
                        playerController.Poison();
                    }

                }
                
                playerController.TakeDamage(transform.rotation.eulerAngles.y, Damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Checks if the Player is colliding only with the Attack Collider of the spider
        if (collision.CompareTag("Player") && collision.IsTouching(AttackCollider) && CanAttack && isAlive)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.isAlive)
            {
                Attack();

                if (DealPoisoningDamage && playerController.CanTakeDamage)
                {
                    int choice = 0;
                    int result = Random.Range(0, PoisoningChances);

                    if (result == choice && playerController.CanPoison)
                    {
                        playerController.Poison();
                    }

                }
                
                playerController.TakeDamage(transform.rotation.eulerAngles.y, Damage);
            }
        }
    }

    void Attack()
    {
        CanAttack = false;
        CanMove = false;

        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);

        animator.SetTrigger("Attack");
        StartCoroutine("TimeToAttack");
    }

    public void TakeDamage(float direction, int damage)
    {
        if (canTakeDamage && isAlive)
        {
            LoseLife(damage);

            //Checks if the spider is attacking at the moment that she receive damage
            if (TimeToAttack_IsRunning)
            {
                StopCoroutine("TimeToAttack");
                CanAttack = false;
            }


            CanAttack = false;
            CanMove = false;
            canTakeDamage = false;

            if (isAlive)
            {
                animator.SetTrigger("Hitted");
            }

            rigidBody.velocity = new Vector2(0f, 0f);

            if (direction == 0)
            {
                rigidBody.AddForce(Vector2.right * DamageRecoil, ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(Vector2.left * DamageRecoil, ForceMode2D.Impulse);
            }

            StartCoroutine("DamageRecovery");
        }
    }


    //Update and it's methods


    void Update()
    {
        if (PlayerRef != null && isAlive && !UIManager.instance.GameIsPaused)
        {
            CheckInvasion();
        }
    }

    void CheckInvasion()
    {
        float Distance_Y = PlayerRef.transform.position.y - transform.position.y;

        //Checks if the player is in the spider's patrol area, and if he is at an altitude that the spider can see him
        if (PlayerRef.transform.position.x >= PatrolArea_LeftLimit && PlayerRef.transform.position.x <= PatrolArea_RightLimit && (Distance_Y >= -0.5f && Distance_Y <= 2f))
        {
            if (transform.position.x <= PlayerRef.transform.position.x - 1.2f)
            {
                MoveToRight = true;
            }
            else if (transform.position.x >= PlayerRef.transform.position.x + 1.2f)
            {
                MoveToRight = false;
            }
        }
    }


    //COROUTINES - Executed After Update and it's methods


    IEnumerator DamageRecovery()
    {
        spriteRenderer.color = new Color(255, 0, 0);

        yield return new WaitForSeconds(TimeToRecovery);

        //rigidBody.velocity = new Vector2(0f, 0f);

        spriteRenderer.color = new Color(255, 255, 255);

        CanAttack = true;
        CanMove = true;

        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        //yield return new WaitForSeconds(InvincibilityTime);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        canTakeDamage = true;
    }

    IEnumerator TimeToAttack()
    {
        TimeToAttack_IsRunning = true;
        yield return new WaitForSeconds(AttackAnimationTime);

        CanMove = true;

        yield return new WaitForSeconds(AttackCoolDownTime);
        TimeToAttack_IsRunning = false;

        CanAttack = true;
    }


    //Methods that are executed from different places / button input


    void LoseLife(int amount)
    {
        HitPoints -= amount;

        if (HitPoints <= 0)
        {
            HitPoints = 0;
            PlayerXP.instance.EarnXP(XP_Earned);
            isAlive = false;

            animator.SetTrigger("Die");
            StopAllCoroutines();
            Destroy(this.gameObject, 1f);
        }
    }
}
