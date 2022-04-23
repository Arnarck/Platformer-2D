using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject[] Potions;
    
    public float TimeToDestroy;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerWeapon"))
        {
            int result = Random.Range(0, 12);

            if (result >= 0 && result < 3)
            {
                Instantiate(Potions[0], transform.position, Quaternion.identity);
            }
            else if (result >= 3 && result < 6)
            {
                Instantiate(Potions[1], transform.position, Quaternion.identity);
            }
            else if (result >= 6 && result < 11)
            {
                int randomResistancePotion = Random.Range(2, Potions.Length);
                Instantiate(Potions[randomResistancePotion], transform.position, Quaternion.identity);
            }

            GetComponent<BoxCollider2D>().enabled = false;
            animator.SetTrigger("Destroy");
            Destroy(this.gameObject, TimeToDestroy);
        }
    }
}
