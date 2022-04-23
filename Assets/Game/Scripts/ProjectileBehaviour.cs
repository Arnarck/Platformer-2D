using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public enum Projectile
    {
        Fire,
        Ice
    }

    public Projectile ProjectileType;

    public int ActivateEffectChances;

    public float Speed;
    public float TimeToDestroy;

    private Animator animator;

    private int Damage;

    private string Wielder;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(this.gameObject, TimeToDestroy);
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Wielder.Equals("Player"))
        {
            if (collision.CompareTag("Patrol_Enemy"))
            {
                collision.GetComponent<PatrolEnemy>().TakeDamage(transform.rotation.y, Damage);
            }
            else if (collision.CompareTag("Pursuit_Enemy"))
            {
                collision.GetComponent<PursuitEnemy>().TakeDamage(transform.rotation.y, Damage);
            }
            else if (collision.CompareTag("Mage_Enemy"))
            {
                collision.GetComponent<LongRangeEnemy>().TakeDamage(transform.rotation.y, Damage);
            }
            else if (collision.CompareTag("Boss"))
            {
                EarthGuardian_BurningBehaviour bossBurningBehaviour = collision.GetComponent<EarthGuardian_BurningBehaviour>();

                EarthGuardian_FreezingBehaviour bossFreezingBehaviour = collision.GetComponent<EarthGuardian_FreezingBehaviour>();

                //Checks if the boss's burning charge can be filled
                if (ProjectileType == Projectile.Fire && !bossBurningBehaviour.isBurning && !bossBurningBehaviour.isWeakenedDefense && !bossFreezingBehaviour.isFreezing)
                {
                    bossBurningBehaviour.ChargeUp();
                }

                //Checks if the boss's freezing charge can be filled
                if (ProjectileType == Projectile.Ice && !bossFreezingBehaviour.isFreezing && !bossBurningBehaviour.isBurning)
                {
                    bossFreezingBehaviour.ChargeUp();
                }


                collision.GetComponent<EarthGuardian>().TakeDamage(transform.eulerAngles.y, Damage);
            }

            Speed = 0f;
            Destroy(this.gameObject, 0.1f);
            animator.SetTrigger("Impact");
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController playerController = collision.GetComponent<PlayerController>();

                if (playerController.CanFreeze && ProjectileType == Projectile.Ice && playerController.CanTakeDamage)
                {
                    playerController.Freeze();
                }
                else if (playerController.CanBurn && ProjectileType == Projectile.Fire && playerController.CanTakeDamage)
                {
                    playerController.Burn();
                }

                playerController.TakeDamage(transform.rotation.eulerAngles.y, Damage);

                Speed = 0f;
                Destroy(this.gameObject, 0.1f);
                animator.SetTrigger("Impact");
            }
        }
    }






    // Update is called once per frame
    void Update()
    {
        if (!UIManager.instance.GameIsPaused)
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime, Space.Self);
        }
    }

    public void SetWielder(string name)
    {
        Wielder = name;

        if (Wielder.Equals("Player"))
        {
            this.tag = "PlayerWeapon";
        }
    }

    public void SetDamage(int amount)
    {
        Damage = amount;
    }
}
