using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsMenu : MonoBehaviour
{
    public GameObject PotionDetails;

    public Text RestoreLifeAmount_Text;
    public Text RestoreManaAmount_Text;
    public Text PoisonInvulnerabilityAmount_Text;
    public Text BleedInvulnerabilityAmount_Text;
    public Text BurnInvulnerabilityAmount_Text;
    public Text FreezeInvulnerabilityAmount_Text;

    public Text PotionName_Text;
    public Text PotionDecription_Text;
    public Text PotionDuration_Text;

    private PlayerInventory playerInventory;

    private string potionSelected;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }


    public void UpdatePotionsAmount()
    {
        RestoreLifeAmount_Text.text = playerInventory.RestoreLifePotion_Count.ToString();
        RestoreManaAmount_Text.text = playerInventory.RestoreManaPotion_Count.ToString();
        PoisonInvulnerabilityAmount_Text.text = playerInventory.PoisonInvulnerabilityPotion_Count.ToString();
        BleedInvulnerabilityAmount_Text.text = playerInventory.BleedInvulnerabilityPotion_Count.ToString();
        BurnInvulnerabilityAmount_Text.text = playerInventory.BurnInvulnerabilityPotion_Count.ToString();
        FreezeInvulnerabilityAmount_Text.text = playerInventory.FreezeInvulnerabilityPotion_Count.ToString();
    }

    public void LoadPotionDetails(string potionName)
    {
        PotionDetails.SetActive(true);
        potionSelected = potionName;

        if (potionName.Equals("RestoreLife"))
        {
            PotionName_Text.text = "Poção de Cura.";
            PotionDecription_Text.text = "Restaura 50 pontos de vida do jogador.";
            PotionDuration_Text.text = "Duração: Instantâneo.";
        }
        else if (potionName.Equals("RestoreMana"))
        {
            PotionName_Text.text = "Poção de Mana.";
            PotionDecription_Text.text = "Restaura 10 pontos de mana do jogador.";
            PotionDuration_Text.text = "Duração: Instantâneo.";
        }
        else if (potionName.Equals("PoisonInvulnerability"))
        {
            PotionName_Text.text = "Poção de Invulnerabilidade a envenenamento.";
            PotionDecription_Text.text = "Anula o efeito de envenenamento (caso esteja ativo) e garante invulnerabilidade a envenenamento por um determinado período de tempo.";
            PotionDuration_Text.text = "Duração: 10 segundos.";
        }
        else if (potionName.Equals("BleedingInvulnerability"))
        {
            PotionName_Text.text = "Poção de Invulnerabilidade a Sangramento.";
            PotionDecription_Text.text = "Anula o efeito de sangramento (caso esteja ativo) e garante invulnerabilidade a sangramento por um determinado período de tempo.";
            PotionDuration_Text.text = "Duração: 10 segundos.";
        }
        else if (potionName.Equals("BurnInvulnerability"))
        {
            PotionName_Text.text = "Poção de Invulnerabilidade a Queimaduras.";
            PotionDecription_Text.text = "Anula o efeito de queimaduras (caso esteja ativo) e garante invulnerabilidade a queimaduras por um determinado período de tempo.";
            PotionDuration_Text.text = "Duração: 10 segundos.";
        }
        else if (potionName.Equals("FreezingInvulnerability"))
        {
            PotionName_Text.text = "Poção de Invulnerabilidade a Congelamento.";
            PotionDecription_Text.text = "Anula o efeito de congelamento (caso esteja ativo) e garante invulnerabilidade a congelamento por um determinado período de tempo.";
            PotionDuration_Text.text = "Duração: 10 segundos.";
        }
    }

    public void ConsumePotion()
    {
        playerInventory.ConsumePotion(potionSelected);
        UpdatePotionsAmount();
    }
}
