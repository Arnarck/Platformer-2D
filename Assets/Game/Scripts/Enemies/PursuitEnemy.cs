using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PursuitEnemy : MonoBehaviour
{
    public BoxCollider2D FootCollider;
    
    public int HitPoints;
    public int XP_Earned;

    public float Speed;
    public float TimeToRecovery;
    //public float InvincibilityTime;
    public float DamageRecoil;
    public float FieldOfView;
    public float HeightOfView;
    public float HearingArea;
    public float AttackAnimationTime;
    public float AttackCoolDownTime;
    public float DistanceToStop;

    private bool isAlive = true;

    private WeaponBehaviour Weapon;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform PlayerRef;
    private Vector3 StartPos;

    private bool CanMove = true;
    private bool CanAttack = true;
    private bool CanTakeDamage = true;
    private bool isTakingDamage;
    private bool isMoving;
    private bool isSeeingPlayer;

    private bool TimeToAttack_IsRunning;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PlayerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        StartPos = transform.position;
        SetWeapon();
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
        animator.SetBool("isRunning", true);

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
                Weapon.AttackAnimation(false);
            }

            isTakingDamage = true;
            CanAttack = false;
            CanMove = false;
            CanTakeDamage = false;
            isMoving = false;
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
            animator.SetBool("isRunning", false);

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






    private void Update()
    {
        if (isAlive && !isTakingDamage && !UIManager.instance.GameIsPaused)
        {
            float difference = transform.position.x - PlayerRef.position.x;

            DetectPlayer();

            //If the goblin is very close to the player, he will stop in front of him and will be able to attack
            if (isSeeingPlayer && Mathf.Abs(difference) <= DistanceToStop)
            {
                StopMoving();

                if (CanAttack)
                {
                    Attack();
                }
            }
            //If the goblin can move and if he is seeing the player, he will move
            else if (isSeeingPlayer && CanMove)
            {
                isMoving = true;
            }
            //If the goblin isn't seeing the player, he will go back to home
            else if (!isSeeingPlayer)
            {
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

    void GoHome()
    {
        if (StartPos.x - 0.5f > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            animator.SetBool("isRunning", true);
            CanMove = true;
            isMoving = true;
        }
        else if (StartPos.x + 0.5f < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            animator.SetBool("isRunning", true);
            CanMove = true;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }
    }

    void Attack()
    {
        CanAttack = false;
        animator.SetTrigger("Attack");
        Weapon.AttackAnimation(true);
        StartCoroutine("TimeToAttack");
    }

    IEnumerator TimeToAttack()
    {
        TimeToAttack_IsRunning = true;
        yield return new WaitForSeconds(AttackAnimationTime);

        Weapon.AttackAnimation(false);

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

    void StopMoving()
    {
        isMoving = false;

        animator.SetBool("isRunning", false);
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
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

    void SetWeapon()
    {
        Weapon = transform.GetChild(0).GetChild(0).GetComponent<WeaponBehaviour>();
    }
}
