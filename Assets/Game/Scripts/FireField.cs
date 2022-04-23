using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireField : MonoBehaviour
{
    public int Damage;
    public float TimeToDestroy;
    public bool CanDestroy;

    // Start is called before the first frame update
    void Start()
    {
        if (CanDestroy)
        {
            Destroy(this.gameObject, TimeToDestroy);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerController.CanBurn && playerController.CanTakeDamage)
            {
                if (!playerController.isBurning)
                {
                    playerController.Burn();
                }
                playerController.TakeDamage(180f, Damage);
            }
        }
        else if (collision.CompareTag("Patrol_Enemy"))
        {
            collision.GetComponent<PatrolEnemy>().TakeDamage(0f, 1000);
        }
        else if (collision.CompareTag("Pursuit_Enemy"))
        {
            collision.GetComponent<PursuitEnemy>().TakeDamage(0f, 1000);
        }
        else if (collision.CompareTag("Mage_Enemy"))
        {
            collision.GetComponent<LongRangeEnemy>().TakeDamage(0f, 1000);
        }
    }
}
