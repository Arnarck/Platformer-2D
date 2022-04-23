using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBehaviour : MonoBehaviour
{
    public enum TypeOf
    {
        RestoreLife,
        RestoreMana,
        IncreaseAttackPower,
        FasterAttacks,
        PoisonInvulnerability,
        BleedInvulnerability,
        FreezeInvulnerability,
        BurnInvulnerability
    }

    public TypeOf Potion;

    private PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInventory.AddPotion(Potion.ToString());
            Destroy(this.gameObject);
        }
    }
}
