using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public static PlayerXP instance;

    public int XPToLevelUp;
    public int LevelUpLifeBonus;
    public int LevelUpManaBonus;

    private PlayerController PlayerRef;
    private PlayerInterface playerInterface;

    public int CurrentXP;
    public int Level = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PlayerRef = GetComponent<PlayerController>();
        playerInterface = GameObject.Find("Canvas").GetComponent<PlayerInterface>();

        playerInterface.SetMaxXP(XPToLevelUp);
        playerInterface.SetLevelText(Level);
    }

    public void EarnXP(int amount)
    {
        CurrentXP += amount;

        if (CurrentXP >= XPToLevelUp)
        {
            int remainingXP = CurrentXP - XPToLevelUp;
            CurrentXP = remainingXP;

            LevelUp();
        }

        playerInterface.SetXPValue(CurrentXP);
    }

    public void LevelUp()
    {
        Level++;

        PlayerRef.IncreaseLife(LevelUpLifeBonus);
        PlayerRef.IncreaseMana(LevelUpManaBonus);

        playerInterface.SetLevelText(Level);
    }
}
