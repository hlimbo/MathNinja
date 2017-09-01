using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Question: if given a duration for a jump to last... will the amount of velocities applied per frame stay constant?
//That is,if I give 3 seconds to apply multiple jump forces over several frames,will the number of velocities for the entire duration
//remain the same per run? (e.g. will I get 3 velocity values within 3 seconds of the simulation every single time or will it differentiate a little bit?)

//utilizing kinematic equation s = (u + v) * t / 2
[RequireComponent(typeof(Rigidbody2D))]
public class VelocityTimer : MonoBehaviour {

    [SerializeField]
    private float startTime;

    [Tooltip("Measured in seconds")]
    public float duration;

    public float jumpHeight;
    public float recordedJumpHeight;
    public List<float> yVelocities;

    //Conclusions
    //1. where acceleration being applied to velocity is constant
       //i. seems to me that the number of velocities applied over 3 seconds changes every time (not consistent)
       //ii. jumpHeights are not consistent because the amount of time a jump takes varies between x duration seconds as the minimum with some upperbound  close to x seconds
       //iii. The difference between displacement(using the kinematic equation to calculate the jumpheight) and actual jumpHeight can be as big as 0.64 unity meters
       //iv. was using Time.time to record the latest Time in Update()
    //2. where jumpHeight is given to calculate the number of seconds required to meet that jumpHeight requirement
    public float jumpAccel;
    public int jumpDirection;

    //velocity of the rigidbody is capped by maxVelocity;
    public float maxJumpVelocity;

    [SerializeField]
    [Tooltip("This adds acceleration to the gameobject's attached rigidbody's velocity every frame")]
    private Vector2 jumpForce;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    [SerializeField]
    private bool isJumping = false;

    [SerializeField]
    private float startYPosition;

    [SerializeField]
    private float elapsedTime;

    public LayerMask floorMask;

    public const float NO_JUMP_HEIGHT = -1;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        yVelocities = new List<float>();

        // jumpHeight = NO_JUMP_HEIGHT;
        recordedJumpHeight = NO_JUMP_HEIGHT;

        duration =(float)( 2 * jumpHeight )/ maxJumpVelocity;
        Debug.Log("Number of seconds required to reach " + jumpHeight + " unity meters is: " + duration + " seconds");
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if(!isJumping)
            {
                isJumping = true;
                startTime = Time.time;
                startYPosition = rb.position.y;
                recordedJumpHeight = NO_JUMP_HEIGHT;
            }
        }

        if(isJumping)
        {
            elapsedTime = Time.time - startTime;

            if(elapsedTime >= duration && rb.velocity.y != 0.0f)
            {
                if (rb.position.y - startYPosition > recordedJumpHeight)
                {
                    Debug.Log("Elapsed Jump time: " + elapsedTime);
                    recordedJumpHeight = rb.position.y - startYPosition;

                    //subtract the amount of upward force to negate jumping
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                }
            }

            if (rb.position.y - startYPosition < jumpHeight && elapsedTime < duration)
            {
                //clamp velocity if new jump velocity exceeds max jump velocity
                jumpForce = new Vector2(0.0f, jumpAccel * jumpDirection) * Time.deltaTime;
                if (rb.velocity.y + jumpForce.y >= maxJumpVelocity)
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
                }
                else
                {
                    rb.AddForce(jumpForce, ForceMode2D.Impulse);
                }

                yVelocities.Add(rb.velocity.y);
            }
            else if(rb.velocity.y == 0.0f)
            {

                isJumping = false;

                Debug.Log("Recorded Jump Height: " + recordedJumpHeight);
                Debug.Log("Jump Height: " + jumpHeight);

                //check if y-height is roughly equal to the kinematic equation s =  t * (u + ... + v) / n where n is the number of velocities applied over n frames
                float averageVelocity = 0;
                foreach (float yVelocity in yVelocities)
                {
                    averageVelocity += yVelocity;
                }

                averageVelocity /= yVelocities.Count;

                float s = averageVelocity * duration;
                Debug.Log("Displacement: " + s);

                Debug.Log("Difference: " + Mathf.Abs(s - jumpHeight));

                Debug.Log("Y-Velocities Count: " + yVelocities.Count);

                recordedJumpHeight = NO_JUMP_HEIGHT;
                yVelocities.Clear();
    
            }
        }

    }

}
