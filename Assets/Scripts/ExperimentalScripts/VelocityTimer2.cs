using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * s = displacement
 * u = initial velocity
 * v = final velocity
 * a = acceleration
 * t = time
 *
 *
 */ 

//utlizing kinematic equation (suvat): 
/*
 * v^2 = u^2 + 2*a*s;
 * v = sqrt(2*a*s) where u = 0;
 * 
 */ 
public class VelocityTimer2 : MonoBehaviour {

    public float jumpHeight;
    public float recordedJumpHeight;
    public float jumpSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool beginJump;
    private float beginJumpY;
    private float beginJumpTime;

    public float timeElapsed;

    private Vector2 jumpVelocity;
    public float jumpTime;

	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        Debug.Log("Physics2D gravity: " + Physics2D.gravity);
        jumpSpeed = CalculateJumpVerticalSpeed(jumpHeight);
        Debug.Log("Jump Speed: " + jumpSpeed);

        jumpTime = Mathf.Sqrt(2 * jumpHeight / Mathf.Abs(Physics2D.gravity.y));
        Debug.Log("jump Time: " + jumpTime);

        jumpVelocity = new Vector2(0.0f, jumpSpeed);
	}
	
	// Update is called once per frame
	void Update ()
    {

        if(Input.GetButtonDown("Jump"))
        {
            if(!beginJump)
            {
                beginJump = true;
                beginJumpY = rb.position.y;
                beginJumpTime = Time.time;
               
            }
        }

        //fixed height jump
        if(beginJump)
        {
            timeElapsed = Time.time - beginJumpTime;
            float deltaY = rb.position.y - beginJumpY;
            if(jumpTime / 2 > timeElapsed)//deltaY < jumpHeight)
            {
                //decelerate until jumpVelocity reaches 0
                //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.99f);
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                Debug.Log("deltaY: " + deltaY);
                beginJump = false;
            }
        }
		
	}

    public float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2f * targetJumpHeight * Mathf.Abs(Physics2D.gravity.y));
    }
}
