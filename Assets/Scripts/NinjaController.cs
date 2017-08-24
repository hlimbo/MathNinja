using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class NinjaController : MonoBehaviour {

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField]
    private int direction;
    [SerializeField]
    private Vector2 moveAccel;

    public float moveSpeed;
    public Vector2 maxSpeed;

    public float jumpSpeed;
    public float maxJumpSpeed;
    public float minJumpSpeed;
    private Vector2 jumpAccel;
    [SerializeField]
    private bool beginJump = false;
    public float jumpTime;
    [SerializeField]
    private float beginJumpTime;
    [SerializeField]
    private float endJumpTime;
    public float beginJumpY;
    public LayerMask floorMask;

    [SerializeField]
    private Vector2 rbVelocity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        direction = 0;
	}
	
	void Update ()
    {
        direction = (int)Input.GetAxisRaw("Horizontal");

        //this is accelerating the player every frame
        moveAccel = new Vector2(direction * moveSpeed, 0.0f) * Time.deltaTime;
        rb.AddForce(moveAccel, ForceMode2D.Impulse);

        animator.SetBool("isRunning", moveAccel.x != 0.0f);

        if (direction < 0)
            sr.flipX = true;
        else if (direction > 0)
            sr.flipX = false;

        //clamp speed
        if (Mathf.Abs(rb.velocity.x) > maxSpeed.x)
            rb.velocity = new Vector2(maxSpeed.x * direction, rb.velocity.y);

        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (!beginJump)
            {
                beginJump = true;
                beginJumpY = rb.position.y;
                beginJumpTime = Time.time;
            }
        }

        if (beginJump)
        {
            float timeElapsed = Time.time - beginJumpTime;
            if (timeElapsed <= jumpTime)
            {
                jumpAccel = new Vector2(0.0f, jumpSpeed) * Time.deltaTime;
                if (rb.velocity.y > maxJumpSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
                }
                else
                {
                    rb.AddForce(jumpAccel, ForceMode2D.Impulse);
                }
            }
            else if(timeElapsed > jumpTime)
            {
                //check if on floor
                Vector2 feetPos = new Vector2(rb.position.x, rb.position.y - sr.bounds.extents.y);
                bool onFloor = Physics2D.OverlapCircle(feetPos,0.1f, floorMask);
                if (onFloor)
                {
                    beginJump = false;
                }
            }
        }

        //set jumping animation
        Vector2 feetPos2 = new Vector2(rb.position.x, rb.position.y - sr.bounds.extents.y);
        bool onFloor2 = Physics2D.OverlapCircle(feetPos2,0.1f,floorMask);
        animator.SetBool("isJumping", !onFloor2);

        //debug
        rbVelocity = rb.velocity;
    }
}
