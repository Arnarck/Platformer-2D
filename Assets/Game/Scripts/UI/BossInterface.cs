using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossInterface : MonoBehaviour
{
    public Slider LifeBar_Slider;
    public Slider BurnBar_Slider;
    public Slider FreezingBar_Slider;


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


    //BURN BAR
    public void SetMaxBurnBar(int maxValue)
    {
        BurnBar_Slider.maxValue = maxValue;
    }

    public void SetBurnBarValue(int value)
    {
        BurnBar_Slider.value = value;
    }


    //FREEZING BAR
    public void SetMaxFreezingBar(int maxValue)
    {
        FreezingBar_Slider.maxValue = maxValue;
    }

    public void SetFreezingBarValue(int value)
    {
        FreezingBar_Slider.value = value;
    }
}
