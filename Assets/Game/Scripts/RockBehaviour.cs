using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RockBehaviour : MonoBehaviour
{
    public int Damage;
    public float Speed;

    private Animator animator;
    private Rigidbody2D rigidBody;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = Vector2.down * Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(180f, Damage);
        }

        if (collision.gameObject.layer == 8)
        {
            Speed = 0f;
            animator.SetTrigger("Destroy");
            GetComponent<CircleCollider2D>().enabled = false;
            Destroy(this.gameObject, 0.34f);
        }
    }
}
