using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Color MultipleEffectsColor;
    public Color PoisoningColor;
    public Color BleedingColor;
    public Color BurningColor;
    public Color FreezeColor;

    public BoxCollider2D JumpCollider;
    public SpriteRenderer spriteRenderer;

    public int MaxLife;
    public int MaxMana;
    public int HitPoints;
    public int ManaPoints;
    public int ThermalShockDamage;
    public int ActiveEffects;

    public float Speed;
    public float JumpForce;
    public float DamageRecoil;
    public float AttackCoolDownTime;
    public float MeleeAttackTime;
    public float MagicalAttackTime;
    public float TimeToRecovery;
    public float InvincibilityTime;
    public float RestoreManaTime;

    public float initialSpeed;
    public float initialJumpForce;

    public bool isAlive = true;
    public bool isMoving;
    public bool isJumping;
    public bool isAttacking;

    public bool CanPoison = true;
    public bool CanBleed = true;
    public bool CanBurn = true;
    public bool CanFreeze = true;
    public bool CanTakeDamage = true;
    public bool CanJump = true;

    public bool isPoisoned;
    public bool isBleeding;
    public bool isBurning;
    public bool isFreezing;

    private Animator animator;
    private Rigidbody2D rigidBody;
    private PlayerInterface playerInterface;
    private WeaponBehaviour Weapon;
    private StaffBehaviour Staff;

    private PoisoningBehaviour poisoningBehaviour;
    private BleedingBehaviour bleedingBehaviour;
    private FreezingBehaviour freezingBehaviour;
    private BurningBehaviour burningBehaviour;

    private float AttackAnimationTime;
    private float DamageRecoilDirection;

    private bool isTakingDamage;
    private bool isStaffEquiped;

    private bool TookDamage;

    private bool CanMove = true;
    private bool CanAttack = true;
    private bool HaveMana;

    private bool TimeToAttack_IsRunning;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.2f;
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerInterface = GameObject.Find("Canvas").GetComponent<PlayerInterface>();

        poisoningBehaviour = GetComponent<PoisoningBehaviour>();
        bleedingBehaviour = GetComponent<BleedingBehaviour>();
        freezingBehaviour = GetComponent<FreezingBehaviour>();
        burningBehaviour = GetComponent<BurningBehaviour>();

        initialSpeed = Speed;
        initialJumpForce = JumpForce;
        HitPoints = MaxLife;
        ManaPoints = MaxMana;

        playerInterface.SetMaxLife(MaxLife);
        playerInterface.SetMaxMana(MaxMana);

        SetWeapon();
        SetStaff();

        Staff.gameObject.SetActive(false);
        AttackAnimationTime = MeleeAttackTime;

        if (ManaPoints > 0)
        {
            HaveMana = true;
        }
    }



    //FixedUpdate and it's updates



    void FixedUpdate()
    {
        if (isAlive && !UIManager.instance.GameIsPaused)
        {
            if (CanMove)
            {
                Movement();
            }

            if (Input.GetKey(KeyCode.W) && CanJump && !isJumping)
            {
                Jump();
            }

            if (TookDamage)
            {
                TakeDamageRecoil();
            }
        }
    }

    //Allows the player to move after he press the A/D key
    void Movement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            animator.SetBool("isRunning", true);
            rigidBody.velocity = new Vector2(-Speed, rigidBody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            animator.SetBool("isRunning", true);
            rigidBody.velocity = new Vector2(Speed, rigidBody.velocity.y);
        }
        else
        {
            animator.SetBool("isRunning", false);
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
        }
    }

    //Allows the player to jump after he press the W key
    void Jump()
    {
        //CanJump = false;
        animator.SetBool("isJumping", true);
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
        rigidBody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    void TakeDamageRecoil()
    {
        if (DamageRecoilDirection == 0)
        {
            rigidBody.AddForce(new Vector2(DamageRecoil, 0f), ForceMode2D.Impulse);
        }
        else
        {
            rigidBody.AddForce(new Vector2(-DamageRecoil, 0f), ForceMode2D.Impulse);
        }

        TookDamage = false;
    }



    //Collision detections and it's methods



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && !collision.IsTouching(JumpCollider))
        {
            CanJump = false;
            isJumping = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Checks if the player's JumpCollider is colliding with the ground
        if (collider.gameObject.layer == 8 && collider.IsTouching(JumpCollider))
        {
            isJumping = false;
            animator.SetBool("isJumping", false);

            if (!isTakingDamage)
            {
                CanJump = true;
            }
        }
    }

    //Allows the player to take daamge after the enemy's attack collides with him
    public void TakeDamage(float direction, int DamageAmount)
    {
        if (isAlive && CanTakeDamage)
        {
            LoseLife(DamageAmount);

            //If the player is burning and freezing at the same time, the effects will be canceled and the player will receive thermal shock damage
            if (isBurning && isFreezing)
            {
                burningBehaviour.StopCoroutine("Burn");
                burningBehaviour.CancelEffect();

                freezingBehaviour.StopCoroutine("Freeze");
                freezingBehaviour.CancelEffect();

                LoseLife(ThermalShockDamage);
            }

            //Checks if the player is attacking at the time he takes damage
            if (TimeToAttack_IsRunning)
            {
                StopCoroutine("TimeToAttack");
                TimeToAttack_IsRunning = false;
                isAttacking = false;

                if (isStaffEquiped)
                {
                    animator.SetBool("MagicalAttack", false);
                    Staff.AttackAnimation(false);

                    if (isFreezing)
                    {
                        Speed = initialSpeed / freezingBehaviour.MovementDivider;
                    }
                    else if (isBurning)
                    {
                        Speed = initialSpeed * burningBehaviour.MovementMultiplier;
                    }
                    else
                    {
                        Speed = initialSpeed;
                    }
                }
                else
                {
                    Weapon.AttackAnimation(false);
                }
            }

            if (isAlive)
            {
                animator.SetTrigger("Hitted");
            }

            //Makes it impossible for the player to perform any action
            isTakingDamage = true;
            CanTakeDamage = false;
            CanJump = false;
            CanMove = false;
            CanAttack = false;
            rigidBody.velocity = new Vector2(0f, 0f);

            DamageRecoilDirection = direction;
            TookDamage = true;

            StartCoroutine("DamageRecovery");
        }
    }

    //Activates the player's poisoning effect
    public void Poison()
    {
        if (poisoningBehaviour.Poison_IsRunning)
        {
            poisoningBehaviour.StopCoroutine("Poison");
            ActiveEffects -= 1;
        }

        poisoningBehaviour.StartCoroutine("Poison");
    }

    //Activates the player's bleeding effect
    public void Bleed()
    {
        if (bleedingBehaviour.StopBleeding_IsRunning)
        {
            bleedingBehaviour.StopCoroutine("StopBleeding");
            bleedingBehaviour.StopBleeding_IsRunning = false;
        }

        if (!bleedingBehaviour.Bleed_IsRunning)
        {
            bleedingBehaviour.StartCoroutine("Bleed");
        }
    }

    public void Burn()
    {
        if (burningBehaviour.Burn_IsRunning)
        {
            burningBehaviour.StopCoroutine("Burn");
            ActiveEffects -= 1;
        }

        burningBehaviour.StartCoroutine("Burn");
    }

    public void Freeze()
    {
        if (freezingBehaviour.Freeze_IsRunning)
        {
            freezingBehaviour.StopCoroutine("Freeze");
            ActiveEffects -= 1;
        }

        freezingBehaviour.StartCoroutine("Freeze");
    }



    //Update and it's methods



    void Update()
    {
        if (isAlive && !UIManager.instance.GameIsPaused)
        {
            if ((Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1)) && !isAttacking)
            {
                if (isStaffEquiped)
                {
                    ChangeWeapon("Melee");
                }
                else
                {
                    ChangeWeapon("Magic");
                }
            }

            if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && CanAttack)
            {
                Attack();
            }

            //Checks if the player is moving
            if (isAttacking || isTakingDamage || animator.GetBool("isJumping") || animator.GetBool("isRunning"))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            //Checks if the player is under the bleeding effect
            if (isBleeding)
            {
                if (isMoving)
                {
                    //Checks if the coroutine that stops the bleeding is active. If it is, it will be stopped and the player will keep receiving bleeding damage
                    if (bleedingBehaviour.StopBleeding_IsRunning)
                    {
                        bleedingBehaviour.StopCoroutine("StopBleeding");
                        bleedingBehaviour.StopBleeding_IsRunning = false;
                    }
                }
                else
                {
                    //Checks if the coroutine that stops the bleeding is active. If it isn't, it will start and the player will be healed of the bleeding in 5 seconds (if he don't move)
                    if (!bleedingBehaviour.StopBleeding_IsRunning)
                    {
                        bleedingBehaviour.StartCoroutine("StopBleeding");
                    }
                }
            }
        }
    }

    //Allows the player to attack after he press the SPACE key
    void Attack()
    {
        if (isStaffEquiped && HaveMana)
        {
            isAttacking = true;
            CanAttack = false;

            Staff.AttackAnimation(true);
            animator.SetBool("MagicalAttack", true);
            Speed = 0f;

            StartCoroutine("TimeToAttack");
        }
        else if (!isStaffEquiped)
        {
            isAttacking = true;
            CanAttack = false;

            Weapon.AttackAnimation(true);
            animator.SetTrigger("MeleeAttack");

            StartCoroutine("TimeToAttack");
        }
    }



    //Coroutines (Executed after Update and it's methods)



    //Makes the player return to the white color after receive an attack, and makes he invincible during some time
    IEnumerator DamageRecovery()
    {
        spriteRenderer.color = new Color(255, 0, 0);

        yield return new WaitForSeconds(TimeToRecovery);

        if (ActiveEffects > 1)
        {
            spriteRenderer.color = MultipleEffectsColor;
        }
        else if (isPoisoned)
        {
            spriteRenderer.color = PoisoningColor;
        }
        else if (isBleeding)
        {
            spriteRenderer.color = BleedingColor;
        }
        else if (isBurning)
        {
            spriteRenderer.color = BurningColor;
        }
        else if (isFreezing)
        {
            spriteRenderer.color = FreezeColor;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

        if (isAlive)
        {
            isTakingDamage = false;
            CanAttack = true;
            CanJump = true;
            CanMove = true;

            //for (int i = 0; i < 6; i++)
            //{
            //    yield return new WaitForSeconds(0.15f);

            //    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.25f);

            //    yield return new WaitForSeconds(0.15f);

            //    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            //}

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

            yield return new WaitForSeconds(InvincibilityTime);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

            CanTakeDamage = true;
        }
        else
        {
            //if the player isn't alive and is recovery from attack (hitted animation played), then he will die. As the player have a friction = 0, he will slide after take an attack. So, to prevent this, his velocity in X = 0.
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
        }
    }

    //Stops the attack animation after some time, and then prevents the player from attacking for a while (attack cool down)
    IEnumerator TimeToAttack()
    {
        TimeToAttack_IsRunning = true;
        yield return new WaitForSeconds(AttackAnimationTime);
        isAttacking = false;

        if (isStaffEquiped)
        {
            LoseMana(1);
            Staff.ShotProjectile(transform.rotation);
            Staff.AttackAnimation(false);
            animator.SetBool("MagicalAttack", false);

            if (isFreezing)
            {
                Speed = initialSpeed / freezingBehaviour.MovementDivider;
            }
            else if (isBurning)
            {
                Speed = initialSpeed * burningBehaviour.MovementMultiplier;
            }
            else
            {
                Speed = initialSpeed;
            }
        }
        else
        {
            Weapon.AttackAnimation(false);
        }

        yield return new WaitForSeconds(AttackCoolDownTime);
        TimeToAttack_IsRunning = false;

        CanAttack = true;
    }

    IEnumerator RestoreMana()
    {
        while (ManaPoints < MaxMana)
        {
            yield return new WaitForSeconds(RestoreManaTime);
            ManaPoints++;
            playerInterface.SetManaValue(ManaPoints);
            HaveMana = true;
        }
    }



    //Methods that are executed from different places / button input



    //Updates the player's life (UI and Code) after he takes damage
    public void LoseLife(int amount)
    {
        HitPoints -= amount;

        if (HitPoints <= 0)
        {
            HitPoints = 0;
            isAlive = false;
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
            animator.SetTrigger("Die");
            StopAllCoroutines();
            Weapon.AttackAnimation(false);
            poisoningBehaviour.CancelEffect();
            bleedingBehaviour.CancelEffect();
            burningBehaviour.CancelEffect();
            freezingBehaviour.CancelEffect();
            UIManager.instance.StartCoroutine("StartGameOver");
        }

        playerInterface.SetLifeValue(HitPoints);
    }

    void LoseMana(int amount)
    {
        ManaPoints -= amount;
        StopCoroutine("RestoreMana");
        StartCoroutine("RestoreMana");

        if (ManaPoints <= 0)
        {
            ManaPoints = 0;
            HaveMana = false;
        }

        playerInterface.SetManaValue(ManaPoints);
    }

    //Reassign the weapon reference
    public void SetWeapon()
    {
        Weapon = transform.GetChild(0).GetChild(0).GetComponent<WeaponBehaviour>();
    }

    public void SetStaff()
    {
        Staff = transform.GetChild(1).GetChild(0).GetComponent<StaffBehaviour>();
    }

    public void ChangeWeapon(string newWeapon)
    {
        if (newWeapon.Equals("Magic"))
        {
            isStaffEquiped = true;
            Staff.gameObject.SetActive(true);
            Weapon.gameObject.SetActive(false);
            AttackAnimationTime = MagicalAttackTime;
        }
        else
        {
            isStaffEquiped = false;
            Weapon.gameObject.SetActive(true);
            Staff.gameObject.SetActive(false);
            AttackAnimationTime = MeleeAttackTime;
        }

    }

    public void ReassignWeaponReference()
    {
        if (Weapon == null)
        {
            SetWeapon();
        }
        else if (Staff == null)
        {
            SetStaff();
        }
    }

    public void RestoreLife(int amount)
    {
        HitPoints += amount;

        if (HitPoints > MaxLife)
        {
            HitPoints = MaxLife;
        }

        playerInterface.SetLifeValue(HitPoints);
    }

    public void RestoreMana(int amount)
    {
        HaveMana = true;
        ManaPoints += amount;

        if (ManaPoints >= MaxMana)
        {
            StopCoroutine("RestoreMana");
            ManaPoints = MaxMana;
        }

        playerInterface.SetManaValue(ManaPoints);
    }

    public void IncreaseLife(int amount)
    {
        MaxLife += amount;
        HitPoints = MaxLife;
        playerInterface.SetMaxLife(MaxLife);
    }

    public void IncreaseMana(int amount)
    {
        StopCoroutine("RestoreMana");
        HaveMana = true;
        MaxMana += amount;
        ManaPoints = MaxMana;
        playerInterface.SetMaxMana(MaxMana);
    }

    //Cancels the poisoning effects right after the player drink de potion
    public void PoisonResistancePotion()
    {
        //Checks if the player is poisoned. If he is, the effect is canceled.
        if (poisoningBehaviour.Poison_IsRunning)
        {
            poisoningBehaviour.StopCoroutine("Poison");
            poisoningBehaviour.CancelEffect();
        }

        //Checks if the player is already under the potion effect. If he is, the effect is reseted
        if (poisoningBehaviour.PoisonInvulnerability_IsRunning)
        {
            poisoningBehaviour.StopCoroutine("PoisonInvulnerability");
        }

        poisoningBehaviour.StartCoroutine("PoisonInvulnerability");
    }

    public void BleedingResistancePotion()
    {
        if (bleedingBehaviour.Bleed_IsRunning)
        {
            bleedingBehaviour.StopCoroutine("Bleed");
            bleedingBehaviour.CancelEffect();
        }

        if (bleedingBehaviour.StopBleeding_IsRunning)
        {
            bleedingBehaviour.StopCoroutine("StopBleeding");
            bleedingBehaviour.StopBleeding_IsRunning = false;
        }

        if (bleedingBehaviour.BleedingInvulnerability_IsRunning)
        {
            bleedingBehaviour.StopCoroutine("BleedingInvulnerability");
        }

        bleedingBehaviour.StartCoroutine("BleedingInvulnerability");
    }

    public void BurnsResistancePotion()
    {
        if (burningBehaviour.Burn_IsRunning)
        {
            burningBehaviour.StopCoroutine("Burn");
            burningBehaviour.CancelEffect();
        }

        if (burningBehaviour.BurnInvulnerability_IsRunning)
        {
            burningBehaviour.StopCoroutine("BurnsInvulnerability");
        }

        burningBehaviour.StartCoroutine("BurnsInvulnerability");
    }

    public void FreezingResistancePotion()
    {
        if (freezingBehaviour.Freeze_IsRunning)
        {
            freezingBehaviour.StopCoroutine("Freeze");
            freezingBehaviour.CancelEffect();
        }

        if (freezingBehaviour.FreezingInvulnerability_IsRunning)
        {
            freezingBehaviour.StopCoroutine("FreezingInvulnerability");
        }

        freezingBehaviour.StartCoroutine("FreezingInvulnerability");
    }
}
