﻿using System.Collections;
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

    public enum PLAYER_STATE
    {
        FLOOR,
        JUMPING,
        FALLING
    }

    public float jumpHeight;
    [Tooltip("SpeedFactor controls how fast player jumps off from floor.This value will be clamped by its jumpSpeed if this value is ridiculously high")]
    public float speedFactor;
    [SerializeField]
    private  float recordedJumpHeight;
    [SerializeField]
    private float jumpSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField]
    private PLAYER_STATE pState;
    private float beginJumpY;
    private float beginJumpTime;

    public float timeElapsed;

    [SerializeField]
    private Vector2 jumpVelocity;
    public float jumpTime;

    public LayerMask floorMask;

    [SerializeField]
    private Vector2 rbVelocity;

	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        Debug.Log("Physics2D gravity: " + Physics2D.gravity);
        jumpSpeed = CalculateJumpVerticalSpeed(jumpHeight);
        Debug.Log("Jump Speed: " + jumpSpeed);
        speedFactor = speedFactor > jumpSpeed ? jumpSpeed : speedFactor;

        jumpTime = Mathf.Sqrt(2 * jumpHeight / Mathf.Abs(Physics2D.gravity.y));
        Debug.Log("jump Time: " + jumpTime);

        jumpVelocity = new Vector2(0.0f, jumpSpeed);

        pState = PLAYER_STATE.FALLING;
    }
	
	// Update is called once per frame
	void Update ()
    {

        rbVelocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if(Input.GetButtonDown("Jump"))
        {
            if(pState == PLAYER_STATE.FLOOR)
            {
                pState = PLAYER_STATE.JUMPING;
                beginJumpY = rb.position.y;
                beginJumpTime = Time.time;
            }
        }

        //fixed height jump
        if(pState == PLAYER_STATE.JUMPING)
        {
            timeElapsed = Time.time - beginJumpTime;
            recordedJumpHeight = rb.position.y - beginJumpY;
            if (recordedJumpHeight < jumpHeight)
            {
                if(rb.velocity.y < jumpVelocity.y)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelocity.y * speedFactor + rb.velocity.y);
                }
                else
                {
                    //goal: is to slowly decelerate y velocity down to 0 using some sort of lerp ~ done
                    //goal2: I need to find the t param in Mathf.Lerp that satisfies the amount of time required to reach jumpHeight
                    //goal3: I need to slowly decelerate the y velocity until I reach jumpHeight where newVelocity = 0.0f when deltaY >= jumpHeight
                    //so hacky but it works :D

                    //if designer desires the initial jump velocity to accelerate during the first second of the jump and then decelerate afterwards...
                    //this functionality of the fixed height jump must be reworked in order to meet those requirements

                    //this is grabbing the percentage between 0 and jumpHeight * 2 as a decimal which is used to decelerate the jumpVelocity every frame
                    //until it reaches roughly the jumpHeight specified in the script parameter
                    float t = Mathf.InverseLerp(0.0f, jumpHeight * 2, recordedJumpHeight);
                    //this is applying the deceleration of the jumpVelocity using the inverse lerp value "t" where t is a percentage between 0.0f and jumpHeight * 2
                    float newVelocity = Mathf.Lerp(jumpVelocity.y * 2, 0.0f, t);
                    rb.velocity = new Vector2(rb.velocity.x, newVelocity);

                   // Debug.Log("t: " + t + " |  newVelocity: " + newVelocity);
                }
            }
            else
            {
                //disrupting velocity -- looks like player hit an invisible ceiling if y velocity set to 0 immediately
                // rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                pState = PLAYER_STATE.FALLING;
            }
        }

        //see if player will fall on the floor this frame
        if(pState == PLAYER_STATE.FALLING)
        { 
            Vector2 footPos = new Vector2(rb.position.x, rb.position.y - sr.bounds.extents.y);
            bool isOnFloor = Physics2D.OverlapCircle(footPos, 0.1f, floorMask);
            if (isOnFloor)
            {
                Debug.Log("recordedJumpHeight: " + recordedJumpHeight);
                pState = PLAYER_STATE.FLOOR;
            }
        }

    }

    //this calculates a reasonable jump velocity to work with
    public float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2f * targetJumpHeight * Mathf.Abs(Physics2D.gravity.y));
    }
}
