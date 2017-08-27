using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class NinjaController : MonoBehaviour {

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D playerCollider;

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
    private bool beginJump;
    public float jumpTime;
    [SerializeField]
    private float beginJumpTime;
    public float beginJumpY;
    public LayerMask floorMask;

    [SerializeField]
    private Vector2 rbVelocity;

    [SerializeField]
    private bool onFloor;

    [SerializeField]
    private bool isDead;
    public static bool IsDead { get; private set; }

    private Camera mainCam;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        IsDead = isDead = false;
    }

    void Start ()
    {
        direction = 0;
        onFloor = false;
        beginJump = false;
        mainCam = Camera.main;

        StartCoroutine(CheckIfDead());
	}
	
	void Update ()
    {
        if(isDead)
        {
            //should player fall through the floor when dead?
            playerCollider.isTrigger = true;
            animator.SetBool("isDead", isDead);
            return;
        }

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
                animator.SetBool("isJumping", true);
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
                onFloor = Physics2D.OverlapCircle(feetPos,0.1f, floorMask);
                if (onFloor)
                {
                    beginJump = false;
                    animator.SetBool("isJumping", false);
                }
            }
        }

        //debug
        rbVelocity = rb.velocity;
    }

    //need to sync death animation with number event manager coroutine loop
    private IEnumerator CheckIfDead()
    {
        while(!isDead)
        {
            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration)
            {
                if(NumberEventManager.attempt_answer != null)
                {
                    break;
                }
                //checks if player dies by falling off camera bounds
                else if (mainCam.orthographic)
                {
                    Transform cameraTransform = mainCam.GetComponent<Transform>();
                    Debug.Assert(cameraTransform != null);
                    if (transform.position.y < cameraTransform.position.y - mainCam.orthographicSize ||
                        transform.position.y > cameraTransform.position.y + mainCam.orthographicSize)
                    {
                        IsDead = isDead = true;
                        break;
                    }
                }

                yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
            }

            //checks if player dies by grabbing the wrong answer
            if (NumberEventManager.attempt_answer != null)
            {
                IsDead = isDead = (NumberEventManager.HasCorrectAnswer) ? false : true;
            }

            yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        }
    }
}
