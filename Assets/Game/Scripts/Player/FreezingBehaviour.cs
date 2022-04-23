using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreezingBehaviour : MonoBehaviour
{
    public GameObject FreezeEffect;
    public GameObject PotionTimer;
    public GameObject PotionsTimersUI;

    public int FreezingTime;
    public int FreezingDamage;

    public float InvulnerabilityTime;
    public float MovementDivider;

    public bool Freeze_IsRunning;
    public bool FreezingInvulnerability_IsRunning;

    private PlayerController PlayerRef;
    private Image PotionTimerImage;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GetComponent<PlayerController>();
        PotionTimerImage = PotionTimer.transform.GetChild(0).GetComponent<Image>();
    }

    IEnumerator Freeze()
    {
        Freeze_IsRunning = true;

        FreezeEffect.SetActive(true);

       // UIManager.instance.FreezeText.SetActive(true);

        PlayerRef.isFreezing = true;
        PlayerRef.ActiveEffects += 1;
        PlayerRef.spriteRenderer.color = PlayerRef.FreezeColor;
        PlayerRef.Speed = PlayerRef.initialSpeed / MovementDivider;
        PlayerRef.JumpForce = PlayerRef.initialJumpForce / MovementDivider;


        for (int i = 0; i < FreezingTime; i++)
        {
            yield return new WaitForSeconds(1f);
            PlayerRef.LoseLife(FreezingDamage);
        }

        CancelEffect();
    }

    public void CancelEffect()
    {
        Freeze_IsRunning = false;

        FreezeEffect.SetActive(false);

        //UIManager.instance.FreezeText.SetActive(false);

        PlayerRef.isFreezing = false;
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

    IEnumerator FreezingInvulnerability()
    {
        FreezingInvulnerability_IsRunning = true;

        PlayerRef.CanFreeze = false;

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

        PlayerRef.CanFreeze = true;

        FreezingInvulnerability_IsRunning = false;
    }
}
