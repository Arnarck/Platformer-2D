using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurningBehaviour : MonoBehaviour
{
    public GameObject BurnEffect;
    public GameObject PotionTimer;
    public GameObject PotionsTimersUI;

    public int BurningTime;
    public int BurningDamage;

    public float InvulnerabilityTime;
    public float MovementMultiplier;

    public bool Burn_IsRunning;
    public bool BurnInvulnerability_IsRunning;

    private PlayerController PlayerRef;
    private Image PotionTimerImage;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GetComponent<PlayerController>();
        PotionTimerImage = PotionTimer.transform.GetChild(0).GetComponent<Image>();
    }

    IEnumerator Burn()
    {
        Burn_IsRunning = true;

        BurnEffect.SetActive(true);

        //UIManager.instance.BurningText.SetActive(true);

        PlayerRef.isBurning = true;
        PlayerRef.ActiveEffects += 1;
        PlayerRef.Speed = PlayerRef.initialSpeed * MovementMultiplier;
        PlayerRef.JumpForce = PlayerRef.initialJumpForce * MovementMultiplier;

        for (int i = 0; i < BurningTime; i++)
        {
            yield return new WaitForSeconds(0.5f);
            PlayerRef.LoseLife(BurningDamage);
        }

        CancelEffect();
    }

    public void CancelEffect()
    {
        Burn_IsRunning = false;

        BurnEffect.SetActive(false);

        //UIManager.instance.BurningText.SetActive(false);

        PlayerRef.isBurning = false;
        PlayerRef.ActiveEffects -= 1;
        PlayerRef.spriteRenderer.color = Color.white;
        PlayerRef.Speed = PlayerRef.initialSpeed;
        PlayerRef.JumpForce = PlayerRef.initialJumpForce;


        if (PlayerRef.ActiveEffects > 1)
        {
            PlayerRef.spriteRenderer.color = PlayerRef.MultipleEffectsColor;
        }
        else if (PlayerRef.isPoisoned)
        {
            PlayerRef.spriteRenderer.color = PlayerRef.PoisoningColor;
        }
        else if (PlayerRef.isBleeding)
        {
            PlayerRef.spriteRenderer.color = PlayerRef.BleedingColor;
        }
        else if (PlayerRef.isBurning)
        {
            PlayerRef.spriteRenderer.color = PlayerRef.BurningColor;
        }
        else if (PlayerRef.isFreezing)
        {
            PlayerRef.spriteRenderer.color = PlayerRef.FreezeColor;
        }
        else
        {
            PlayerRef.spriteRenderer.color = Color.white;
        }
    }

    IEnumerator BurnsInvulnerability()
    {
        BurnInvulnerability_IsRunning = true;

        PlayerRef.CanBurn = false;

        PotionTimer.SetActive(true);
        PotionTimer.transform.SetParent(PotionsTimersUI.transform);
        PotionTimerImage.fillAmount = 1f;

        for (int i = 0; i < InvulnerabilityTime; i++)
        {
            yield return new WaitForSeconds(1f);
            PotionTimerImage.fillAmount -= .1f;
        }

        PotionTimer.transform.SetParent(null);
        PotionTimer.transform.position = Vector3.zero;
        PotionTimer.SetActive(false);

        PlayerRef.CanBurn = true;

        BurnInvulnerability_IsRunning = false;
    }
}
