using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    public GameObject IceProjectileIcon;
    public GameObject FireProjectileIcon;

    public Slider LifeBar_Slider;
    public Slider ManaBar_Slider;
    public Slider XPBar_Slider;

    public Text LevelText_Text;


    //LIFE BAR
    public void SetMaxLife(int maxValue)
    {
        LifeBar_Slider.maxValue = maxValue;
        LifeBar_Slider.value = maxValue;
    }

    public void SetLifeValue(int value)
    {
        LifeBar_Slider.value = value;
    }


    //MANA BAR
    public void SetMaxMana(int maxValue)
    {
        ManaBar_Slider.maxValue = maxValue;
        ManaBar_Slider.value = maxValue;
    }

    public void SetManaValue(int value)
    {
        ManaBar_Slider.value = value;
    }


    //XP BAR
    public void SetMaxXP(int maxValue)
    {
        XPBar_Slider.maxValue = maxValue;
    }

    public void SetXPValue(int value)
    {
        XPBar_Slider.value = value;
    }


    //LEVEL TEXT
    public void SetLevelText(int level)
    {
        LevelText_Text.text = level.ToString();
    }


    //PROJECTILES
    public void SetProjectile(string Projectile)
    {
        if (Projectile.Equals("Ice"))
        {
            IceProjectileIcon.SetActive(true);
            FireProjectileIcon.SetActive(false);
        }
        else
        {
            FireProjectileIcon.SetActive(true);
            IceProjectileIcon.SetActive(false);
        }
    }
}
