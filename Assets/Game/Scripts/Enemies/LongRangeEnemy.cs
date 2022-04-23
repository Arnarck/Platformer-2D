using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LongRangeEnemy : MonoBehaviour
{
    public BoxCollider2D FootCollider;

    public int HitPoints;
    public int XP_Earned;

    public float Speed;
    public float FieldOfView;
    public float HeightOfView;
    public float HearingArea;
    public float MeleeAttackArea;
    public float AttackAnimationTime;
    public float AttackCoolDownTime;
    public float DamageRecoil;
    public float TimeToRecovery;
    //public float InvincibilityTime;
    public float DistanceToStop;

    private Transform PlayerRef;
    private StaffBehaviour Staff;
    private WeaponBehaviour Weapon;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Vector3 StartPos;

    private bool CanAttack = true;
    private bool CanMove = true;
    private bool CanTakeDamage = true;

    private bool isAlive = true;
    private bool isTakingDamage;
    private bool isSeeingPlayer;
    private bool isMoving;
    private bool isStaffEquiped = true;
    private bool isAttacking;

    private bool TimeToAttack_IsRunning;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerRef = GameObject.Find("Player").GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetStaff();
        SetWeapon();

        StartPos = transform.position;
    }






    private void FixedUpdate()
    {
        if (isAlive && !isTakingDamage && !UIManager.instance.GameIsPaused)
        {
            if (CanMove && isMoving)
            {
                Movement();
            }
        }
    }

    void Movement()
    {
        animator.SetBool("isWalking", true);

        if (transform.rotation.eulerAngles.y == 0)
        {
            rigidBody.velocity = new Vector2(Speed, rigidBody.velocity.y);
        }
        else
        {
            rigidBody.velocity = new Vector2(-Speed, rigidBody.velocity.y);
        }
    }






    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && FootCollider.IsTouching(collision))
        {
            CanMove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && !FootCollider.IsTouching(collision))
        {
            CanMove = false;
            StopMoving();
        }
    }

    public void TakeDamage(float direction, int damage)
    {
        if (isAlive && CanTakeDamage)
        {
            //Checks if the enemy is attacking
            if (TimeToAttack_IsRunning)
            {
                StopCoroutine("TimeToAttack");
                TimeToAttack_IsRunning = false;

                if (isStaffEquiped)
                {
                    animator.SetBool("MagicalAttack", false);
                    Staff.AttackAnimation(false);
                }
                else
                {
                    animator.SetBool("MeleeAttack", false);
                    Weapon.AttackAnimation(false);
                }
            }

            isTakingDamage = true;
            isMoving = false;
            isAttacking = false;
            CanAttack = false;
            CanMove = false;
            CanTakeDamage = false;
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
            animator.SetBool("isWalking", false);

            if (direction == 0)
            {
                rigidBody.AddForce(Vector2.right * DamageRecoil, ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(Vector2.left * DamageRecoil, ForceMode2D.Impulse);
            }

            StartCoroutine("DamageRecovery");
            LoseLife(damage);

            if (isAlive)
            {
                animator.SetTrigger("Hit");
            }
        }
    }






    // Update is called once per frame
    void Update()
    {
        if (isAlive && !isTakingDamage && !UIManager.instance.GameIsPaused)
        {
            float difference = transform.position.x - PlayerRef.position.x;

            DetectPlayer();

            if (isSeeingPlayer)
            {
                if (Mathf.Abs(difference) <= MeleeAttackArea && !isAttacking)
                {
                    isMoving = true;
                    ChangeWeapon("Melee");
                }
                else if (Mathf.Abs(difference) > MeleeAttackArea && !isAttacking)
                {
                    StopMoving();
                    ChangeWeapon("Staff");

                    if (CanAttack)
                    {
                        Attack();
                    }
                }
                
                if (Mathf.Abs(difference) <= DistanceToStop)
                {
                    StopMoving();

                    if (CanAttack)
                    {
                        Attack();
                    }
                }
            }
            else if (!isSeeingPlayer && !isAttacking)
            {
                ChangeWeapon("Staff");
                GoHome();
            }
        }
    }

    void DetectPlayer()
    {
        float Y_DistanceFromPlayer = PlayerRef.position.y - transform.position.y;
        //if it's value is >= 0, the player is on the enemies's right, else, he's on the enemie's left
        float X_DistanceFromPlayer = PlayerRef.position.x - transform.position.x;

        //Checks if the player is at an altitude where the Goblin can see him
        if (Y_DistanceFromPlayer >= -0.5f && Y_DistanceFromPlayer <= HeightOfView)
        {
            //Checks if the goblin is looking to the right. Else, he is looking to the left
            if (transform.rotation.eulerAngles.y == 0)
            {
                //Checks if the player is in the Goblin's field of view
                if (X_DistanceFromPlayer >= 0f && X_DistanceFromPlayer <= FieldOfView)
                {
                    isSeeingPlayer = true;
                }
                //Checks if the Goblin is hearing the player coming form behind him
                else if (X_DistanceFromPlayer < 0f && X_DistanceFromPlayer >= -HearingArea)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    isSeeingPlayer = true;
                }
                //if the goblin isn't seeing or hearing the player, he'll go home
                else
                {
                    isSeeingPlayer = false;
                }
            }
            //The goblin is looking to the left. So, the conditions are reversed
            else
            {
                if (X_DistanceFromPlayer <= 0f && X_DistanceFromPlayer >= -FieldOfView)
                {
                    isSeeingPlayer = true;
                }
                else if (X_DistanceFromPlayer > 0f && X_DistanceFromPlayer <= HearingArea)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    isSeeingPlayer = true;
                }
                else
                {
                    isSeeingPlayer = false;
                }
            }
        }
        //if the player is at an altitude that the goblin can't see him, the goblin will stop
        else
        {
            isSeeingPlayer = false;
            StopMoving();
        }
    }

    void Attack()
    {
        CanAttack = false;
        isAttacking = true;

        if (isStaffEquiped)
        {
            animator.SetBool("MagicalAttack", true);
            Staff.AttackAnimation(true);
        }
        else
        {
            animator.SetBool("MeleeAttack", true);
            Weapon.AttackAnimation(true);
        }

        StartCoroutine("TimeToAttack");
    }



    //Coroutines



    IEnumerator TimeToAttack()
    {
        TimeToAttack_IsRunning = true;
        yield return new WaitForSeconds(AttackAnimationTime);

        isAttacking = false;
        
        if (isStaffEquiped)
        {
            Staff.ShotProjectile(transform.rotation);
            animator.SetBool("MagicalAttack", false);
            Staff.AttackAnimation(false);
        }
        else
        {
            animator.SetBool("MeleeAttack", false);
            Weapon.AttackAnimation(false);
        }

        yield return new WaitForSeconds(AttackCoolDownTime);
        TimeToAttack_IsRunning = false;

        CanAttack = true;
    }

    IEnumerator DamageRecovery()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(TimeToRecovery);
        spriteRenderer.color = Color.white;

        isTakingDamage = false;
        CanAttack = true;
        CanMove = true;

        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        //yield return new WaitForSeconds(InvincibilityTime);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        CanTakeDamage = true;
    }






    void SetStaff()
    {
        Staff = transform.GetChild(0).GetChild(0).GetComponent<StaffBehaviour>();
    }

    void SetWeapon()
    {
        Weapon = transform.GetChild(1).GetChild(0).GetComponent<WeaponBehaviour>();
        //Weapon.gameObject.SetActive(false);
    }

    void ChangeWeapon(string weapon)
    {
        if (weapon.Equals("Melee"))
        {
            isStaffEquiped = false;
            Staff.gameObject.SetActive(false);
            Weapon.gameObject.SetActive(true);
        }
        else
        {
            isStaffEquiped = true;
            Staff.gameObject.SetActive(true);
            Weapon.gameObject.SetActive(false);
        }
    }

    void StopMoving()
    {
        isMoving = false;

        animator.SetBool("isWalking", false);
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
    }

    void GoHome()
    {
        if (StartPos.x - 0.5f > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            animator.SetBool("isWalking", true);
            CanMove = true;
            isMoving = true;
        }
        else if (StartPos.x + 0.5f < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            animator.SetBool("isWalking", true);
            CanMove = true;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }
    }

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