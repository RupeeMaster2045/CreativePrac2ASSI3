using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask deathLayer;
    [SerializeField] private TrailRenderer tr;


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
        }

        // Move
        horizontal = Input.GetAxisRaw("Horizontal");
        //animator.SetFloat("Speed", Mathf.Abs(horizontal));

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
        yield return new WaitForSeconds(dashingTime);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + new Vector3(0, groundedY), Vector2.down * .1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EggPickup") && Input.GetKeyDown(KeyCode.E))
        {
            HoldingEgg = true;
            Debug.Log("Egg picked up");
        }

        if (collision.gameObject.CompareTag("EggPickup"))
        {
            Debug.Log("Can pick up an egg!!!!!!!!!!");
        }

        if (collision.gameObject.CompareTag("EggGoal") && Input.GetKeyDown(KeyCode.E))
        { 
            HoldingEgg = false;
            Debug.Log("Egg dropped");
        }

        if (collision.gameObject.CompareTag("EggGoal"))
        {
            Debug.Log("Can drop an egg!!!!!!!!!!!");
        }
    }
}
