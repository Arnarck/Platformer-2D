using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BleedingBehaviour : MonoBehaviour
{
    public GameObject BleedEffect;
    public GameObject PotionTimer;
    public GameObject PotionsTimersUI;

    public int MovingDamage;
    public int StoppedDamage;

    public float TimeToStopBleeding;
    public float InvulnerabilityTime;

    public bool Bleed_IsRunning;
    public bool StopBleeding_IsRunning;
    public bool BleedingInvulnerability_IsRunning;

    private PlayerController PlayerRef;
    private Image PotionTimerImage;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GetComponent<PlayerController>();
        PotionTimerImage = PotionTimer.transform.GetChild(0).GetComponent<Image>();
    }

    IEnumerator Bleed()
    {
        Bleed_IsRunning = true;

        BleedEffect.SetActive(true);

       // UIManager.instance.BleedText.SetActive(true);

        PlayerRef.isBleeding = true;
        PlayerRef.ActiveEffects += 1;

        while (PlayerRef.isBleeding)
        {
            yield return new WaitForSeconds(1f);

            if (PlayerRef.isMoving)
            {
                PlayerRef.LoseLife(MovingDamage);
            }
            else
            {
                PlayerRef.LoseLife(StoppedDamage);
            }
        }

        Bleed_IsRunning = false;
    }

    public void CancelEffect()
    {
        Bleed_IsRunning = false;

        BleedEffect.SetActive(false);

        //UIManager.instance.BleedText.SetActive(false);

        PlayerRef.isBleeding = false;
        PlayerRef.ActiveEffects -= 1;

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

    IEnumerator StopBleeding()
    {
        StopBleeding_IsRunning = true;

        yield return new WaitForSeconds(TimeToStopBleeding);

        StopCoroutine("Bleed");
        CancelEffect();

        StopBleeding_IsRunning = false;
    }

    IEnumerator BleedingInvulnerability()
    {
        BleedingInvulnerability_IsRunning = true;

        PlayerRef.CanBleed = false;

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

        PlayerRef.CanBleed = true;

        BleedingInvulnerability_IsRunning = false;
    }
}
