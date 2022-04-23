using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EarthGuardian : MonoBehaviour
{
    public GameObject MagicProjectilePrefab;
    public Transform MagicOriginPosition;

    public SpriteRenderer spriteRenderer;

    public Color MainColor;
    public Color DamageColor;

    public int HitPoints;

    public float Speed;
    public float minHeightOfView;
    public float minDistanceFromPlayer;
    public float minPunchAttackDistance;
    public float minMagicalAttackDistance;

    public float BasicAttack_AnimTime;
    //public float BasicAttack_CDTime;
    public float CloseAttack_AnimTime;
    //public float CloseAttack_CDTime;
    public float MagicalAttack_AnimTime;
    //public float MagicalAttack_CDTime;
    public float CommomAttacks_CDTime;

    public float SpecialAttack_AnimTime;
    public float SpecialAttack_CDTime;

    public float DamageRecovery_Time;

    public bool isAlive = false;

    private Animator animator;
    private Rigidbody2D rigidBody;
    private Transform PlayerRef;
    private Vector3 StartPoint;
    private BossInterface bossInterface;
    private EarthGuardianHammer MeleeWeapon;
    private EarthGuardian_BurningBehaviour burningBehaviour;
    private EarthGuardian_FreezingBehaviour freezingBehaviour;
    private LevelComplete levelComplete;

    private int MaxHitPoints;

    private bool isSeeingPlayer;
    private bool CanMove = true;
    private bool CanAttack;
    private bool CanSpecialAttack;
    private bool isAttacking;

    private bool WillChangeScenario = true;
    private bool SpecialAttacksActivated;

    private bool ChangeScenarioAttackAnimation_IsRunning;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        PlayerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bossInterface = GameObject.Find("Canvas").GetComponent<BossInterface>();
        burningBehaviour = GetComponent<EarthGuardian_BurningBehaviour>();
        freezingBehaviour = GetComponent<EarthGuardian_FreezingBehaviour>();
        levelComplete = GameObject.Find("LevelComplete").GetComponent<LevelComplete>();

        StartPoint = transform.position;
        MaxHitPoints = HitPoints;
        MeleeWeapon = transform.GetChild(0).GetChild(0).GetComponent<EarthGuardianHammer>();

        bossInterface.SetMaxLife(HitPoints);
    }

    private void FixedUpdate()
    {
        if (isAlive && !freezingBehaviour.isFreezing)
        {
            if (CanMove)
            {
                Movement();
            }
            else
            {
                StopMoving();
            }
        }
    }

    public void Movement()
    {
        animator.SetBool("isWalking", true);

        if (transform.rotation.y == 0)
        {
            rigidBody.velocity = new Vector2(Speed, rigidBody.velocity.y);
        }
        else
        {
            rigidBody.velocity = new Vector2(-Speed, rigidBody.velocity.y);
        }
    }

    public void TakeDamage(float direction, int amount)
    {
        if (isAlive)
        {
            //Checks if the boss is under the "weakened defense" effect. If he is, he takes x3 damage
            if (burningBehaviour.isWeakenedDefense)
            {
                LoseLife(amount * burningBehaviour.WeakenedDefense_DamageMultiplier);
            }

            //Checks if the player is attacking the boss from behind. If he is, the boss takes twice as much damage.
            if (direction == transform.eulerAngles.y && !isSeeingPlayer)
            {
                LoseLife(amount * 2);
            }

            //If the boss isn't under the "weakened" effect, and if the player isn't attacking from behind, the boss will take the basic damage
            if (!(direction == transform.eulerAngles.y && !isSeeingPlayer) && !burningBehaviour.isWeakenedDefense)
            {
                LoseLife(amount);
            }

            StartCoroutine("DamageRecovery");

            if (isAlive)
            {
                //Checks if the boss is taking damage from behind. If he is, he will rotate to the direction that he received damage
                if (direction == transform.eulerAngles.y && !isSeeingPlayer && !freezingBehaviour.isFreezing)
                {
                    transform.rotation = Quaternion.Euler(0f, 180f - direction, 0f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive && !freezingBehaviour.isFreezing)
        {
            //distance between the player and the guardian
            float difference = Mathf.Abs(PlayerRef.position.x - transform.position.x);

            DetectPlayer();

            //Checks if the guardian is seeing the player. Else, if he's not attacking, he will go back to the spawn point.
            if (isSeeingPlayer)
            {
                AttackManager(difference);

                //Checks if the player is at a certain distance from the boss, or if the boss is attacking. If one of these conditions is true, the boss will stop in front of the player. Else, he will keep moving
                if (difference <= minDistanceFromPlayer || isAttacking)
                {
                    CanMove = false;
                }
                else
                {
                    CanMove = true;
                }
            }
            else if (!isAttacking)
            {
                GoHome();
            }
        }
    }

    public void DetectPlayer()
    {
        float difference = PlayerRef.position.x - transform.position.x;

        //Checks whether the player is in the Boss's field of vision
        if (PlayerRef.position.y >= minHeightOfView)
        {
            //Checks if the Boss is looking at the same direction that the player is located
            if (transform.rotation.y == 0f && difference >= 0f)
            {
                isSeeingPlayer = true;
            }
            else if (transform.rotation.y != 0f && difference <= 0f)
            {
                isSeeingPlayer = true;
            }
            else
            {
                isSeeingPlayer = false;
            }
        }
        else
        {
            isSeeingPlayer = false;
        }
    }

    public void StopMoving()
    {
        animator.SetBool("isWalking", false);
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
    }

    public void GoHome()
    {
        float distance = StartPoint.x - transform.position.x;

        if (distance > 1f)
        {
            CanMove = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (distance < -1f)
        {
            CanMove = true;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            CanMove = false;
        }
    }

    public void AttackManager(float difference)
    {
        //Checks if the guardian can attack. Else, checks if he can do the special attack
        if (CanAttack && !isAttacking)
        {
            //Allows the boss to do the special attacks when his life is 1/3 of the total or less.
            if (!SpecialAttacksActivated && HitPoints <= MaxHitPoints / 2)
            {
                SpecialAttacksActivated = true;
                CanSpecialAttack = true;
            }

            //Allows the boss to use the attack that will change the scenario when his life is 2/3 of the total or less. This attack will be used only one time.
            if (WillChangeScenario && HitPoints <= MaxHitPoints * 3 / 4)
            {
                WillChangeScenario = false;
                ChangeScenarioAttack();
            }
            //Checks if the boss can use the special attack. If he can, he will use this attack
            else if (CanSpecialAttack)
            {
                SpecialAttack();
            }
            //Checks if the player is close to the boss. if he is, the boss will use the close attack
            else if (difference <= minPunchAttackDistance)
            {
                CloseAttack();
            }
            //Checks if the player is at a median distance from the boss. If he is, the boss will use the basic attack
            else if (difference > minPunchAttackDistance && difference < minMagicalAttackDistance)
            {
                BasicAttack();
            }
            //Checks if the player is far from the boss. If he is, the boss will use the long range attack
            else
            {
                MagicalAttack();
            }
        }
    }

    public void BasicAttack()
    {
        isAttacking = true;
        CanAttack = false;

        animator.SetBool("BasicAttack", true);
        MeleeWeapon.BasicAttackAnimation(true);

        StartCoroutine("BasicAttackAnimation");
    }

    public void CloseAttack()
    {
        isAttacking = true;
        CanAttack = false;

        animator.SetBool("CloseAttack", true);
        MeleeWeapon.CloseAttackAnimation(true);

        StartCoroutine("CloseAttackAnimation");
    }

    public void MagicalAttack()
    {
        isAttacking = true;
        CanAttack = false;

        animator.SetBool("MagicalAttack", true);

        StartCoroutine("MagicalAttackAnimation");
    }

    public void SpecialAttack()
    {
        isAttacking = true;
        CanAttack = false;
        CanSpecialAttack = false;

        animator.SetBool("SpecialAttack", true);
        MeleeWeapon.SpecialAttackAnimation(true);

        StartCoroutine("SpecialAttackAnimation");
    }

    public void ChangeScenarioAttack()
    {
        isAttacking = true;
        CanAttack = false;

        animator.SetBool("SpecialAttack", true);
        MeleeWeapon.ChangeScenarioAttackAnimation(true);

        StartCoroutine("ChangeScenarioAttackAnimation");
    }



    //========================== COROUTINES ==========================



    IEnumerator BasicAttackAnimation()
    {
        yield return new WaitForSeconds(BasicAttack_AnimTime);

        isAttacking = false;
        animator.SetBool("BasicAttack", false);
        MeleeWeapon.BasicAttackAnimation(false);

        StartCoroutine("CommomAttacksCoolDown");

        //yield return new WaitForSeconds(Attack_CDTime);

        //CanAttack = true;
    }

    IEnumerator CloseAttackAnimation()
    {
        yield return new WaitForSeconds(CloseAttack_AnimTime);

        isAttacking = false;
        animator.SetBool("CloseAttack", false);
        MeleeWeapon.CloseAttackAnimation(false);

        StartCoroutine("CommomAttacksCoolDown");

        //yield return new WaitForSeconds(Attack_CDTime);

        //CanAttack = true;
    }

    IEnumerator MagicalAttackAnimation()
    {
        yield return new WaitForSeconds(MagicalAttack_AnimTime);

        Instantiate(MagicProjectilePrefab, MagicOriginPosition.position, transform.rotation);
        isAttacking = false;
        animator.SetBool("MagicalAttack", false);

        StartCoroutine("CommomAttacksCoolDown");

        //yield return new WaitForSeconds(Attack_CDTime);

        //CanAttack = true;
    }

    IEnumerator SpecialAttackAnimation()
    {
        yield return new WaitForSeconds(SpecialAttack_AnimTime);

        isAttacking = false;
        animator.SetBool("SpecialAttack", false);
        MeleeWeapon.SpecialAttackAnimation(false);

        StartCoroutine("CommomAttacksCoolDown");
        StartCoroutine("SpecialAttacksCoolDown");

        //yield return new WaitForSeconds(SpecialAttack_CDTime);

        //CanSpecialAttack = true;
    }

    IEnumerator ChangeScenarioAttackAnimation()
    {
        ChangeScenarioAttackAnimation_IsRunning = true;

        yield return new WaitForSeconds(SpecialAttack_AnimTime);

        isAttacking = false;
        animator.SetBool("SpecialAttack", false);
        MeleeWeapon.ChangeScenarioAttackAnimation(false);

        ChangeScenarioAttackAnimation_IsRunning = false;

        StartCoroutine("CommomAttacksCoolDown");
    }

    public IEnumerator CommomAttacksCoolDown()
    {
        yield return new WaitForSeconds(CommomAttacks_CDTime);

        CanAttack = true;
    }

    IEnumerator SpecialAttacksCoolDown()
    {
        yield return new WaitForSeconds(SpecialAttack_CDTime);

        CanSpecialAttack = true;
    }

    IEnumerator DamageRecovery()
    {
        spriteRenderer.color = DamageColor;

        yield return new WaitForSeconds(DamageRecovery_Time);

        ChangeColor();
    }



    //========================== METHODS CALLED IN SEVERAL WAYS ==========================



    public void FreezeCharacter()
    {
        CanMove = false;
        isSeeingPlayer = false;
        StopMoving();

        animator.SetBool("CloseAttack", false);
        animator.SetBool("BasicAttack", false);
        animator.SetBool("MagicalAttack", false);
        animator.SetBool("SpecialAttack", false);

        MeleeWeapon.BasicAttackAnimation(false);
        MeleeWeapon.CloseAttackAnimation(false);
        MeleeWeapon.SpecialAttackAnimation(false);

        if (ChangeScenarioAttackAnimation_IsRunning)
        {
            WillChangeScenario = true;
        }
    }

    public void ChangeColor()
    {
        if (burningBehaviour.isWeakenedDefense && freezingBehaviour.isFreezing)
        {
            spriteRenderer.color = freezingBehaviour.F_WD_Color;
        }
        else if (burningBehaviour.isBurning)
        {
            spriteRenderer.color = burningBehaviour.burningColor;
        }
        else if (burningBehaviour.isWeakenedDefense)
        {
            spriteRenderer.color = burningBehaviour.WeakenedDefenseColor;
        }
        else if (freezingBehaviour.isFreezing)
        {
            spriteRenderer.color = freezingBehaviour.FreezingColor;
        }
        else
        {
            spriteRenderer.color = MainColor;
        }
    }

    public void LoseLife(int amount)
    {
        HitPoints -= amount;

        if (HitPoints <= 0)
        {
            HitPoints = 0;
            isAlive = false;
            animator.SetTrigger("Die");
            StopAllCoroutines();
            FreezeCharacter();

            if (freezingBehaviour.isFreezing)
            {
                freezingBehaviour.CancelEffect();
            }

            if (burningBehaviour.isBurning || burningBehaviour.isWeakenedDefense)
            {
                burningBehaviour.CancelEffects();
            }

            Destroy(rigidBody);
            Destroy(GetComponent<BoxCollider2D>());

            levelComplete.StartCoroutine("DelayToLevelComplete");
        }

        bossInterface.SetLifeValue(HitPoints);
    }
}
