using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGuardianMagic : MonoBehaviour
{
    public int Damage;
    public int PoisoningChances;
    public int BleedChances;

    public float Speed;
    public float TimeToDestroy;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(this.gameObject, TimeToDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerController.isAlive)
            {
                int PoisoningResult = Random.Range(0, PoisoningChances);
                int BleedResult = Random.Range(0, BleedChances);
                int choice = 0;

                if (choice == PoisoningResult && playerController.CanPoison)
                {
                    playerController.Poison();
                }

                if (choice == BleedResult && playerController.CanBleed)
                {
                    playerController.Bleed();
                }
                
                playerController.TakeDamage(transform.rotation.y, Damage);
            }

            Speed = 0f;
            animator.SetTrigger("Impact");
            Destroy(this.gameObject, 0.35f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Speed * Time.deltaTime, Space.Self);
    }
}
