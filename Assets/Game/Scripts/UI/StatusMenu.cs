using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenu : MonoBehaviour
{
    public Text CurrentHP_Text;
    public Text MaxHP_Text;

    public Text CurrentMP_Text;
    public Text MaxMP_Text;

    public Text CurrentLevel_Text;
    public Text CurrentXP_Text;

    public Text CurrentMeleeDamage_Text;
    public Text CurrentMagicDamage_Text;

    public Text PoisoningStatus_Text;
    public Text BleedStatus_Text;
    public Text BurnStatus_Text;
    public Text FreezingStatus_Text;

    private GameObject player;
    private PlayerController playerController;
    private PlayerXP playerXP;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerXP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerXP>();
    }

    public void UpdateStatus()
    {
        CurrentHP_Text.text = playerController.HitPoints.ToString();
        MaxHP_Text.text = playerController.MaxLife.ToString();

        CurrentMP_Text.text = playerController.ManaPoints.ToString();
        MaxMP_Text.text = playerController.MaxMana.ToString();

        CurrentLevel_Text.text = playerXP.Level.ToString();
        CurrentXP_Text.text = playerXP.CurrentXP.ToString();

        CurrentMeleeDamage_Text.text = player.transform.GetChild(0).GetChild(0).GetComponent<WeaponBehaviour>().Damage.ToString();
        CurrentMagicDamage_Text.text = player.transform.GetChild(1).GetChild(0).GetComponent<StaffBehaviour>().Damage.ToString();



        if (playerController.isPoisoned)
        {
            PoisoningStatus_Text.text = "SIM";
            PoisoningStatus_Text.color = Color.green;
        }
        else
        {
            PoisoningStatus_Text.text = "NÃO";
            PoisoningStatus_Text.color = Color.red;
        }

        if (playerController.isBleeding)
        {
            BleedStatus_Text.text = "SIM";
            BleedStatus_Text.color = Color.green;
        }
        else
        {
            BleedStatus_Text.text = "NÃO";
            BleedStatus_Text.color = Color.red;
        }

        if (playerController.isBurning)
        {
            BurnStatus_Text.text = "SIM";
            BurnStatus_Text.color = Color.green;
        }
        else
        {
            BurnStatus_Text.text = "NÃO";
            BurnStatus_Text.color = Color.red;
        }

        if (playerController.isFreezing)
        {
            FreezingStatus_Text.text = "SIM";
            FreezingStatus_Text.color = Color.green;
        }
        else
        {
            FreezingStatus_Text.text = "NÃO";
            FreezingStatus_Text.color = Color.red;
        }
    }
}
