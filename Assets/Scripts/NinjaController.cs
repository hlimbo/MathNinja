using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class NinjaController : MonoBehaviour {

    public enum AIR_STATE
    {
        FLOOR,
        JUMPING,
        FALLING
    }

    //component variables required for this gameobject to properly function
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D playerCollider;

    //running variables
    [SerializeField]
    private Vector2 moveAccel;
    public float moveSpeed;
    public Vector2 maxSpeed;
    [SerializeField]
    private int direction = 0;


    //jump variables
    public float jumpHeight;
    public float timeToJumpApex;
    [SerializeField]
    private float gravity;
    private float beginJumpY;
    private float beginJumpTime;
    [SerializeField]
    private float recordedJumpHeight;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private AIR_STATE aState;
    [SerializeField]
    private float timeElapsed;

    public static float JumpHeight { get; private set; }

    public LayerMask floorMask;

    [SerializeField]
    private Vector2 rbVelocity;

    [SerializeField]
    private bool isDead;
    public static bool IsDead { get; private set; }
    
    [Header("Debugging Settings")]
    [Tooltip("Use this cheat to ignore collisions and fly on the map. Useful for Debugging Timers")]
    public bool enableFlyMode;

    private Camera mainCam;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        IsDead = isDead = false;
    }

    void Start ()
    {
        JumpHeight = jumpHeight;
        aState = AIR_STATE.FALLING;
        mainCam = Camera.main;

        //calculate jump speed based on jump height and the gravity using a kinematic equation
        //jumpSpeed = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(Physics2D.gravity.y));

        //sqrt((2 * d) / a) = t
        //jump time = sqrt(2 * jumpHeight / gravity);
        //jump speed = jump height / jump time

        //know jumpHeight and timetoJumpApex
        //need to solve for gravity and jumpSpeed
        //d = vi * t + (1/2) * a * t^2
        //solve for gravity
        gravity = (2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        //temp ~ 
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -gravity);
        //solve for jumpSpeed
        jumpSpeed = gravity * timeToJumpApex;

        //debugging
        if (enableFlyMode)
        {
            rb.bodyType = RigidbodyType2D.Static;
            playerCollider.enabled = false;
            transform.position = new Vector2(mainCam.transform.position.x, mainCam.transform.position.y - mainCam.orthographicSize / 2);
            return;
        }

        // StartCoroutine(CheckIfDead());
        WorldEventSystem.OnTimerInterrupted += IsPlayerOutOfBounds;
        WorldEventSystem.OnPlayerActionEvaluated += EvaluatePlayerDeath;
    }

    //events will be bound here
    //private void OnEnable()
    //{
    //    WorldEventSystem.OnTimerInterrupted += IsPlayerOutOfBounds;
    //    WorldEventSystem.OnPlayerActionEvaluated += EvaluatePlayerDeath;
    //}

    //events will be unbound here
    private void OnDestroy()
    {
        WorldEventSystem.OnTimerInterrupted -= IsPlayerOutOfBounds;
        WorldEventSystem.OnPlayerActionEvaluated -= EvaluatePlayerDeath;
    }

    void Update()
    {
        //debugging
        if (enableFlyMode)
            return;

        if(isDead)
        {
            //should player fall through the floor when dead?
            playerCollider.isTrigger = true;
            animator.SetBool("isDead", isDead);
            return;
        }

        //todo remove horizontal movement control (only allow the player to move to the right)
        //this is accelerating the player every frame
        direction = (int)Input.GetAxisRaw("Horizontal");
        if (direction < 0)
        {
            moveAccel = new Vector2(moveSpeed * 2 * Time.deltaTime * direction, 0.0f);
            rb.AddForce(moveAccel, ForceMode2D.Impulse);
            //clamp speed
            if (Mathf.Abs(rb.velocity.x) > maxSpeed.x * 2)
                rb.velocity = new Vector2(maxSpeed.x * direction * 2, rb.velocity.y);
        }
        else
        {
            moveAccel = new Vector2(moveSpeed * Time.deltaTime * direction, 0.0f);
            rb.AddForce(moveAccel, ForceMode2D.Impulse);
            //clamp speed
            if (Mathf.Abs(rb.velocity.x) > maxSpeed.x)
                rb.velocity = new Vector2(maxSpeed.x * direction, rb.velocity.y);
        }

        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (aState == AIR_STATE.FLOOR)
            {
                aState = AIR_STATE.JUMPING;
                beginJumpY = rb.position.y;
                beginJumpTime = Time.time;
            }
        }

        //fixed jump height
        if (aState == AIR_STATE.JUMPING)
        {
            timeElapsed = Time.time - beginJumpTime;
            recordedJumpHeight = rb.position.y - beginJumpY;

            //floaty jump but the jump heights recorded are very accurate
            //if (recordedJumpHeight < jumpHeight)
            //{
            //    if (rb.velocity.y < jumpSpeed)
            //    {
            //        //increase the rate of the jump velocity by some jumpFactor percentage every frame
            //        float jumpFactor = 0.25f;
            //        Vector2 jumpAccel = new Vector2(0.0f, jumpSpeed * jumpFactor + rb.velocity.y);
            //        rb.AddForce(jumpAccel, ForceMode2D.Impulse);
            //    }
            //    else //otherwise,if rb.velocity.y exceeds or matches the jumpSpeed
            //    {
            //        //slowly decelerate the y velocity down to zero which will smoothly transition the player between jumping and falling
            //        float jumpVelPercent = Mathf.InverseLerp(0.0f, jumpHeight * 2, recordedJumpHeight);
            //        float newJumpVelocity = Mathf.Lerp(0.0f, jumpSpeed * 2, 1.0f - jumpVelPercent);
            //        rb.velocity = new Vector2(rb.velocity.x, newJumpVelocity);
            //    }
            //}

            //jumps are snappy but gravity affects friction along x-axis
            //but recordedJumpHeight is always less than specified jumpHeight
            if(timeElapsed < timeToJumpApex)
            {
                //jumpModifier is used to make sure ninja reaches target jumpHeight
                float jumpModifier = 1.5f;
                float jumpVelPercent = Mathf.InverseLerp(0.0f, jumpHeight * jumpModifier, recordedJumpHeight);
                float newJumpVelocity = Mathf.Lerp(0.0f, jumpSpeed, 1.0f - jumpVelPercent);
                rb.velocity = new Vector2(rb.velocity.x, newJumpVelocity);
            }
            else
            {
                aState = AIR_STATE.FALLING;
            }

            recordedJumpHeight = rb.position.y - beginJumpY;
        }

        if(aState == AIR_STATE.FALLING)
        {
            Vector2 footPos = new Vector2(rb.position.x, rb.position.y - sr.bounds.extents.y);
            bool isOnFloor = Physics2D.OverlapCircle(footPos, 0.1f, floorMask);
            if (isOnFloor)
                aState = AIR_STATE.FLOOR;
        }

        //animation related stuff
        sr.flipX = (direction < 0) ? true : (direction > 0) ? false : sr.flipX;
        animator.SetBool("isRunning", moveAccel.x != 0.0f);
        animator.SetBool("isJumping", aState != AIR_STATE.FLOOR);

        //debug
        rbVelocity = rb.velocity;
    }

    //function bound to event OnTimerInterrupted
    private bool IsPlayerOutOfBounds()
    {
        Debug.Assert(mainCam.orthographic);
        Transform camTransform = mainCam.GetComponent<Transform>();
        Debug.Assert(camTransform != null);
        bool result = transform.position.y < camTransform.position.y - mainCam.orthographicSize;
        //player death static variable gets determined here
        IsDead = isDead = result;
        return result;
    }

    //function bound to event OnPostTimerElapsed
    //note: unsure if this code will look better after refactor
    private void EvaluatePlayerDeath()
    {
        //do a busy wait here... since I do not know if NinjaController's coroutine or NumberEventManager's coroutine will be called first.
        //to ensure that the problem gets evaluated by NumberEventManager's coroutine check
        //note: unsure if a busy wait or script execution order needs to be modified in order to one function run first
        //while (NumberEventManager.ProblemState == NumberEventManager.Problem_State.ANSWER_PENDING)
        //{
        //    yield return new WaitForEndOfFrame();
        //}

        //check if player lives or dies depending on what answer was picked
        switch (NumberEventManager.ProblemState)
        {
            case NumberEventManager.Problem_State.CORRECT_ANSWER:
                IsDead = isDead = false;
                break;
            case NumberEventManager.Problem_State.WRONG_ANSWER:
                IsDead = isDead = true;
                break;
        }
    }

    //need to sync death animation with number event manager coroutine loop
    private IEnumerator CheckIfDead()
    {
        while(!isDead)
        {
            //need to sync all coroutines that rely on NumberEventManager's timing
            yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);

            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration)
            {
                if(NumberEventManager.user_answer != NumberEventManager.NO_ANSWER)
                {
                    break;
                }
                //checks if player dies by falling off camera bounds
                else if (mainCam.orthographic)
                {
                    if (IsPlayerOutOfBounds())
                    {
                        IsDead = isDead = true;
                        break;
                    }
                }

                yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
            }

            //unsure if this busy wait will still be required to ensure order remains stable
            while (NumberEventManager.ProblemState == NumberEventManager.Problem_State.ANSWER_PENDING)
            {
                yield return new WaitForEndOfFrame();
            }

            EvaluatePlayerDeath();

            yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.tag.Equals("Floor"))
    //    {
    //        direction = 1;
    //    }
    //}
}
