using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGuardian_BurningBehaviour : MonoBehaviour
{
    public GameObject BurningEffect;

    public Color burningColor;
    public Color WeakenedDefenseColor;

    public int Damage;
    public int Iterations;
    public int ChargeLimit;
    public int MaxChargeLimit;
    public int ChargeAddition;
    public int WeakenedDefense_DamageMultiplier;

    public bool isBurning;
    public bool isWeakenedDefense;

    private EarthGuardian earthGuardian;
    private BossInterface bossInterface;

    public int Charge;

    // Start is called before the first frame update
    void Start()
    {
        earthGuardian = GetComponent<EarthGuardian>();
        bossInterface = GameObject.Find("Canvas").GetComponent<BossInterface>();

        bossInterface.SetMaxBurnBar(ChargeLimit);
        bossInterface.SetBurnBarValue(Charge);
    }

    public void ChargeUp()
    {
        Charge++;
        bossInterface.SetBurnBarValue(Charge);

        if (Charge >= ChargeLimit)
        {
            StartCoroutine("Burn");
        }
    }

    IEnumerator Burn()
    {
        isBurning = true;
        BurningEffect.SetActive(true);

        for (int i = 0; i < Iterations; i++)
        {
            yield return new WaitForSeconds(0.5f);

            earthGuardian.LoseLife(Damage);
        }

        isBurning = false;
        BurningEffect.SetActive(false);

        if (earthGuardian.isAlive)
        {
            StartCoroutine("WeakenedDefense");
        }
    }

    IEnumerator WeakenedDefense()
    {
        isWeakenedDefense = true;
        earthGuardian.ChangeColor();

        for (int i = 0; i < ChargeLimit; i++)
        {
            yield return new WaitForSeconds(1f);

            Charge--;
            bossInterface.SetBurnBarValue(Charge);
        }

        Charge = 0;

        if (ChargeLimit < MaxChargeLimit)
        {
            ChargeLimit += ChargeAddition;
        }

        bossInterface.SetBurnBarValue(Charge);
        bossInterface.SetMaxBurnBar(ChargeLimit);

        isWeakenedDefense = false;
        earthGuardian.ChangeColor();
    }

    public void CancelEffects()
    {
        isBurning = false;
        isWeakenedDefense = false;

        StopCoroutine("Burn");
        StopCoroutine("WeakenedDefense");
        BurningEffect.SetActive(false);
        earthGuardian.ChangeColor();
    }
}
