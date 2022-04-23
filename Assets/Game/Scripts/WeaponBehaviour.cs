using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public GameObject WeaponSwoosh;
    public int Damage;
    public int BleedingChances;

    public bool DealBleedDamage;

    private Collider2D Collider;
    private Animator animator;

    private string Wielder;

    private void Start()
    {
        Collider = GetComponent<Collider2D>();

        animator = GetComponent<Animator>();

        Wielder = transform.parent.parent.tag;
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
                collision.GetComponent<EarthGuardian>().TakeDamage(transform.eulerAngles.y, Damage);
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController PlayerRef = collision.GetComponent<PlayerController>();

                if (DealBleedDamage)
                {
                    int chance = Random.Range(0, BleedingChances);
                    int result = 0;

                    if (result == chance && PlayerRef.CanBleed && PlayerRef.CanTakeDamage)
                    {
                        PlayerRef.Bleed();
                    }
                }

                PlayerRef.TakeDamage(transform.rotation.eulerAngles.y, Damage);
            }
        }
    }

    public void AttackAnimation(bool state)
    {
        Collider.enabled = state;
        animator.SetBool("Attack", state);
        WeaponSwoosh.SetActive(state);
    }
}
