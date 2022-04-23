using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public int RestoreLifePotion_Count;
    public int RestoreManaPotion_Count;
    public int PoisonInvulnerabilityPotion_Count;
    public int BleedInvulnerabilityPotion_Count;
    public int FreezeInvulnerabilityPotion_Count;
    public int BurnInvulnerabilityPotion_Count;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        playerController = GetComponent<PlayerController>();
    }

    public void AddPotion(string potion)
    {
        if (potion.Equals("RestoreLife"))
        {
            RestoreLifePotion_Count++;
        }
        else if (potion.Equals("RestoreMana"))
        {
            RestoreManaPotion_Count++;
        }
        else if (potion.Equals("PoisonInvulnerability"))
        {
            PoisonInvulnerabilityPotion_Count++;
        }
        else if (potion.Equals("BleedInvulnerability"))
        {
            BleedInvulnerabilityPotion_Count++;
        }
        else if (potion.Equals("FreezeInvulnerability"))
        {
            FreezeInvulnerabilityPotion_Count++;
        }
        else if (potion.Equals("BurnInvulnerability"))
        {
            BurnInvulnerabilityPotion_Count++;
        }
    }

    public void ConsumePotion(string potion)
    {
        if (potion.Equals("RestoreLife"))
        {
            if (playerController.HitPoints < playerController.MaxLife && RestoreLifePotion_Count > 0)
            {
                RestoreLifePotion_Count--;
                playerController.RestoreLife(50);
            }
        }
        else if (potion.Equals("RestoreMana"))
        {
            if (playerController.ManaPoints < playerController.MaxMana && RestoreManaPotion_Count > 0)
            {
                RestoreManaPotion_Count--;
                playerController.RestoreMana(10);
            }
        }
        else if (potion.Equals("PoisonInvulnerability"))
        {
            if (PoisonInvulnerabilityPotion_Count > 0)
            {
                PoisonInvulnerabilityPotion_Count--;
                playerController.PoisonResistancePotion();
            }
        }
        else if (potion.Equals("BleedingInvulnerability"))
        {
            if (BleedInvulnerabilityPotion_Count > 0)
            {
                BleedInvulnerabilityPotion_Count--;
                playerController.BleedingResistancePotion();
            }
        }
        else if (potion.Equals("FreezingInvulnerability"))
        {
            if (FreezeInvulnerabilityPotion_Count > 0)
            {
                FreezeInvulnerabilityPotion_Count--;
                playerController.FreezingResistancePotion();
            }
        }
        else if (potion.Equals("BurnInvulnerability"))
        {
            if (BurnInvulnerabilityPotion_Count > 0)
            {
                BurnInvulnerabilityPotion_Count--;
                playerController.BurnsResistancePotion();
            }
        }
    }
}
