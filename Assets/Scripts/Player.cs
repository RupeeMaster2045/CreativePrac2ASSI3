using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{

    public float speed;
    public float JumpPower;
    public float groundedY;
    private bool isFacingRight = true;
    private float horizontal;
    private bool HoldingEgg = false;

    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    public Animator animator;

    public UnityEvent OnlandEvent;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask deathLayer;
    [SerializeField] private TrailRenderer tr;

    private void Awake()
    {
        if (OnlandEvent == null)
            OnlandEvent = new UnityEvent();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (TouchedDeathPlane())
        {
            Debug.Log("you've been reset");
            rb.position = new Vector2(0, 0);
            animator.SetBool("CarryingEgg", false);
            animator.SetBool("PickingUp", false);
            animator.SetBool("DroppingEgg", false);
            HoldingEgg = false;
        }

        // Move
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpPower);

            jumpBufferCounter = 0f;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();
    }

    // Dash coroutine
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float normalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        animator.SetBool("IsDashing", true);
        yield return new WaitForSeconds(dashingTime);
        animator.SetBool("IsDashing", false);
        tr.emitting = false;
        rb.gravityScale = normalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public bool TouchedDeathPlane()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, deathLayer);
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }


    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

    }

    private IEnumerator OnTriggerStay2D(Collider2D collision)
    {
       // if (collision.gameObject.CompareTag("EggPickup") && Input.GetKeyDown(KeyCode.E))
       // {
       //     animator.SetBool("PickingUp", true);
       //     HoldingEgg = true;
       //     Debug.Log("Egg picked up");
       //     yield return new WaitForSeconds(1);
       //     animator.SetBool("CarryingEgg", true);
       // }

        if (collision.gameObject.CompareTag("EggPickup") && HoldingEgg == false)
        {
            //Debug.Log("Can pick up an egg!!!!!!!!!!");
            animator.SetBool("PickingUp", true);
            HoldingEgg = true;
            Debug.Log("Egg picked up");
            yield return new WaitForSeconds(0.4f);
            animator.SetBool("CarryingEgg", true);
        }

       // if (collision.gameObject.CompareTag("EggGoal") && Input.GetKeyDown(KeyCode.E))
       // {
       //     animator.SetBool("CarryingEgg", false);
       //     HoldingEgg = false;
       //     Debug.Log("Egg dropped");
       //     animator.SetBool("PickingUp", false);
       // }

        if (collision.gameObject.CompareTag("EggGoal") && HoldingEgg == true)
        {
            //Debug.Log("Can drop an egg!!!!!!!!!!!");
            animator.SetBool("DroppingEgg", true);
            HoldingEgg = false;
            Debug.Log("Egg dropped");
            yield return new WaitForSeconds(0.4f);
            animator.SetBool("CarryingEgg", false);
            animator.SetBool("PickingUp", false);
            animator.SetBool("DroppingEgg", false);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("IsJumping", false);
        }
    }
}
