using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpField : MonoBehaviour
{
    public float JumpForceAddition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerController>().isJumping)
        {
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();

            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0f);
            playerRigidbody.AddForce(Vector2.up * JumpForceAddition, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
