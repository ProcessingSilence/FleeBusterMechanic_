using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public float jumpForce;

    public float moveSpeed;
    // This controller just uses simple control methods to prove that the placement of the player is remembered.
    // I do not recommend using this control method for your final game.
    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        // Jump        
        if (Input.GetKeyDown(KeyCode.Space))
            rb2d.AddForce(Vector2.up * jumpForce);
        
        // Horizontal movement
        if (Input.GetKey(KeyCode.A))
            rb2d.AddForce(-Vector2.right * moveSpeed);
        else if (Input.GetKey(KeyCode.D))
            rb2d.AddForce(Vector2.right * moveSpeed);
        else
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);

        // Fall death.
        if (transform.position.y < -5)
        {
            rb2d.velocity = new Vector2(0,0);
            transform.position = new Vector2(0,2);
        }
    }
}
