using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerImpact : MonoBehaviour
{
    public GameObject ImpactPrefab;

    public float TimeToDestroy;
    public float TimeToDestroyEffect;

    private int Damage;

    private float Y_Rotation;

    // Start is called before the first frame update
    void Start()
    {
        GameObject effect = Instantiate(ImpactPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));

        Destroy(effect, TimeToDestroyEffect);
        Destroy(this.gameObject, TimeToDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(Y_Rotation, Damage);
        }
    }

    public void SetDamage(int amount)
    {
        Damage = amount;
    }

    public void SetRotation(float rot)
    {
        Y_Rotation = rot;
    }
}
