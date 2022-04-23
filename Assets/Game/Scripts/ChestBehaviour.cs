using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestBehaviour : MonoBehaviour
{
    public enum TypeOf
    {
        MeleeWeapon,
        Staff
    }

    public TypeOf Reward;

    public GameObject RewardPrefab;
    public GameObject RewardMenu;

    public Image RewardImage;
    public Sprite RewardSprite;

    public Text RewardNameText;
    public string RewardName;

    public Color RewardColor;

    private PlayerController playerController;

    private bool CanInteract;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CanInteract = true;
            UIManager.instance.InteractText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CanInteract = false;
            UIManager.instance.InteractText.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !playerController.isAttacking && CanInteract)
        {
            GiveReward();

            UIManager.instance.PauseGame(RewardMenu);
            UIManager.instance.CanUnpauseGame = false;

            RewardImage.sprite = RewardSprite;
            RewardNameText.text = RewardName;
            RewardNameText.color = RewardColor;

            Destroy(this.gameObject);
        }
    }

    public void GiveReward()
    {
        GameObject NewWeapon;

        NewWeapon = Instantiate(RewardPrefab);

        if (Reward == TypeOf.MeleeWeapon)
        {
            Destroy(playerController.gameObject.transform.GetChild(0).GetChild(0).gameObject);

            NewWeapon.transform.parent = playerController.gameObject.transform.GetChild(0);
            NewWeapon.transform.position = new Vector3(0f, 0f, 0f);

            playerController.ChangeWeapon("Melee");
        }
        else
        {
            Destroy(playerController.gameObject.transform.GetChild(1).GetChild(0).gameObject);

            NewWeapon.transform.parent = playerController.gameObject.transform.GetChild(1);
            NewWeapon.transform.position = new Vector3(0f, 0.135f, 0f);

            playerController.ChangeWeapon("Magic");
        }

    }
}
