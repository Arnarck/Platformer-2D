using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisoningBehaviour : MonoBehaviour
{
    public GameObject PoisonEffect;
    public GameObject PotionTimer;
    public GameObject PotionsTimersUI;

    //public int PoisoningTime;
    public int MovingDamage;
    public int StoppedDamage;

    public float InvulnerabilityTime;

    public bool Poison_IsRunning;
    public bool PoisonInvulnerability_IsRunning;

    private PlayerController PlayerRef;
    private Image PotionTimerImage;

    
    // Start is called before the first frame update
    void Start()
    {
        PlayerRef = GetComponent<PlayerController>();
        PotionTimerImage = PotionTimer.transform.GetChild(0).GetComponent<Image>();
    }

    public IEnumerator Poison()
    {
        Poison_IsRunning = true;

        PoisonEffect.SetActive(true);

       // UIManager.instance.PoisonText.SetActive(true);

        PlayerRef.isPoisoned = true;
        PlayerRef.ActiveEffects += 1;

        //for (int i = 0; i < PoisoningTime; i++)
        while (PlayerRef.isPoisoned)
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
        //CancelEffect();
    }

    public void CancelEffect()
    {
        Poison_IsRunning = false;

        PoisonEffect.SetActive(false);

        //UIManager.instance.PoisonText.SetActive(false);

        PlayerRef.isPoisoned = false;
        PlayerRef.ActiveEffects -= 1;
        PlayerRef.spriteRenderer.color = Color.white;

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

    IEnumerator PoisonInvulnerability()
    {
        PoisonInvulnerability_IsRunning = true;

        PlayerRef.CanPoison = false;

        PotionTimer.SetActive(true);
        PotionTimer.transform.SetParent(PotionsTimersUI.transform);
        PotionTimerImage.fillAmount  = 1f;

        for (int i = 0; i < InvulnerabilityTime; i++)
        {
            yield return new WaitForSeconds(1f);
            PotionTimerImage.fillAmount -= .1f;
        }

        PotionTimer.transform.SetParent(null);
        PotionTimer.transform.position = Vector3.zero;
        PotionTimer.SetActive(false);

        PlayerRef.CanPoison = true;

        PoisonInvulnerability_IsRunning = false;
    }
}
