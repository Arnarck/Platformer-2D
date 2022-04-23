using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffBehaviour : MonoBehaviour
{
    public enum TypeOf
    {
        Ice,
        Fire
    }

    public TypeOf Projectile;

    public GameObject FireProjectilePrefab;
    public GameObject IceProjectilePrefab;

    public int Damage;

    private Animator animator;
    private PlayerController playerController;
    private PlayerInterface playerInterface;

    private string Wielder;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Wielder = transform.parent.parent.tag;

        if (Wielder.Equals("Player"))
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerInterface = GameObject.Find("Canvas").GetComponent<PlayerInterface>();

            if (Projectile == TypeOf.Fire)
            {
                playerInterface.SetProjectile("Fire");
            }
            else
            {
                playerInterface.SetProjectile("Ice");
            }
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(2)) && Wielder.Equals("Player") && !playerController.isAttacking)
        {
            if (Projectile == TypeOf.Fire)
            {
                Projectile = TypeOf.Ice;
                playerInterface.SetProjectile("Ice");
            }
            else
            {
                Projectile = TypeOf.Fire;
                playerInterface.SetProjectile("Fire");
            }
        }
    }

    public void AttackAnimation(bool state)
    {
        animator.SetBool("Attack", state);
    }

    public void ShotProjectile(Quaternion direction)
    {
        GameObject shot;

        if (Projectile == TypeOf.Ice)
        {
            shot = Instantiate(IceProjectilePrefab, transform.GetChild(0).transform.position, direction);
        }
        else
        {
            shot = Instantiate(FireProjectilePrefab, transform.GetChild(0).transform.position, direction);
        }

        shot.GetComponent<ProjectileBehaviour>().SetWielder(Wielder);
        shot.GetComponent<ProjectileBehaviour>().SetDamage(Damage);
    }
}
