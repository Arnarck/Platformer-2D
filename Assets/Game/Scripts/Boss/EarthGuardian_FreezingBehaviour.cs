using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGuardian_FreezingBehaviour : MonoBehaviour
{
    public GameObject FreezingEffect;

    public Color FreezingColor;
    public Color F_WD_Color;

    public int Damage;
    public int ChargeLimit;
    public int MaxChargeLimit;
    public int ChargeAddition;

    public bool isFreezing;

    private EarthGuardian earthGuardian;
    private BossInterface bossInterface;

    public int Charge;

    // Start is called before the first frame update
    void Start()
    {
        earthGuardian = GetComponent<EarthGuardian>();
        bossInterface = GameObject.Find("Canvas").GetComponent<BossInterface>();

        bossInterface.SetMaxFreezingBar(ChargeLimit);
        bossInterface.SetFreezingBarValue(Charge);
    }

    public void ChargeUp()
    {
        Charge++;
        bossInterface.SetFreezingBarValue(Charge);

        if (Charge >= ChargeLimit)
        {
            StartCoroutine("Freeze");
        }
    }

    IEnumerator Freeze()
    {
        isFreezing = true;
        earthGuardian.FreezeCharacter();
        FreezingEffect.SetActive(true);

        for (int i = 0; i < ChargeLimit; i++)
        {
            yield return new WaitForSeconds(1f);

            Charge--;
            earthGuardian.LoseLife(Damage);
            bossInterface.SetFreezingBarValue(Charge);
        }

        Charge = 0;

        if (ChargeLimit < MaxChargeLimit)
        {
            ChargeLimit += ChargeAddition;
        }

        bossInterface.SetMaxFreezingBar(ChargeLimit);
        bossInterface.SetFreezingBarValue(Charge);

        isFreezing = false;
        FreezingEffect.SetActive(false);
        earthGuardian.ChangeColor();
    }

    public void CancelEffect()
    {
        isFreezing = false;

        StopCoroutine("Freeze");
        FreezingEffect.SetActive(false);
        earthGuardian.ChangeColor();
    }
}
