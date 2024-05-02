using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private new CapsuleCollider2D collider;

    [Header("Movement")]
    [SerializeField] float desiredSpeed = 5;
    [SerializeField] float accelTime = .15f;

    [Header("Jumping")]
    [SerializeField] float jumpStrength = 5;
    [SerializeField][Range(0, 1)] float jumpDamping = .4f;
    [SerializeField] float jumpBuffer = .15f;
    [SerializeField] float coyoteTime = .15f;

    [Header("Ground Check")]
    [SerializeField] float gcDist = 5;
    [SerializeField] float gcRadius = .45f;
    [SerializeField] float gcDistToFeet = 1;
    [SerializeField] float maxGroundedDist = .05f;
    [SerializeField] LayerMask gcLayers;

    public bool isGrounded { get; private set; }
    private RaycastHit2D gcHit;
    private float moveInput;
    private float timeSinceGrounded = Mathf.Infinity;
    private float timeSinceJumpInput = Mathf.Infinity;
    private float timeSinceDampenInput = Mathf.Infinity;
    public float lastNonZeroMoveInput = 1;

    private void Update()
    {
        Move(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            Jump();

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
            DampenJump();
    }

    private void Awake()
    {
        Physics2D.queriesHitTriggers = false;

        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
        rb.sharedMaterial = new PhysicsMaterial2D();
        rb.sharedMaterial.friction = 0;
        collider.sharedMaterial = rb.sharedMaterial;
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        HandleJumping();
        HandleJumpDamping();
        HandleMovement();
        HandleSlopes();

        timeSinceGrounded += Time.deltaTime;
        timeSinceJumpInput += Time.deltaTime;
        timeSinceDampenInput += Time.deltaTime;
    }

    private void HandleGroundCheck()
    {
        gcHit = Physics2D.CircleCast(transform.position, gcRadius, -transform.up, gcDist, gcLayers);

        var distanceToGround = gcHit.collider ? gcHit.distance - (gcDistToFeet - gcRadius) : Mathf.Infinity;

        isGrounded = distanceToGround < maxGroundedDist
            && Vector2.Dot(rb.velocity, transform.up) < .1f;

        if (isGrounded)
            timeSinceGrounded = 0;
    }

    private void HandleJumping()
    {
        if (timeSinceJumpInput > jumpBuffer)
            return;
        if (!isGrounded && timeSinceGrounded > coyoteTime)
            return;
        if (Vector2.Dot(rb.velocity, transform.up) > .1f)
            return;

        Debug.Log("Jump");

        var movementVel = Vector3.Project(rb.velocity, transform.right);
        rb.velocity = movementVel + transform.up * jumpStrength;
        timeSinceJumpInput = Mathf.Infinity;
        isGrounded = false;
    }

    private void HandleJumpDamping()
    {
        if (timeSinceDampenInput > jumpBuffer)
            return;
        if (Vector2.Dot(rb.velocity, transform.up) < 0)
            return;

        Vector2 verticalVel = Vector3.Project(rb.velocity, transform.up);
        rb.velocity -= verticalVel;
        rb.velocity += verticalVel * (1 - jumpDamping);
        timeSinceDampenInput = Mathf.Infinity;
    }

    private void HandleMovement()
    {
        var moveDir = transform.right * moveInput;
        Vector2 movementVel = Vector3.Project(rb.velocity, transform.right);
        var desiredVel = moveDir * desiredSpeed;
        var nonMovementVel = rb.velocity - movementVel;

        var accel = accelTime == 0 ? Mathf.Infinity : desiredSpeed / accelTime * Time.deltaTime;

        movementVel = Vector3.MoveTowards(movementVel, desiredVel, accel);

        rb.velocity = movementVel + nonMovementVel;
    }

    private void HandleSlopes()
    {
        if (!isGrounded) return;

        Vector2 upSlopeDir = Vector3.Cross(Vector3.Cross(gcHit.normal, transform.up), gcHit.normal).normalized;

        Debug.DrawLine(transform.position, (Vector2)transform.position + upSlopeDir, Color.blue);

        rb.velocity += Vector2.Dot(-upSlopeDir, Physics2D.gravity * rb.gravityScale) * Time.deltaTime * upSlopeDir;
        //rb.velocity += Physics2D.gravity.magnitude * Time.deltaTime * upSlopeDir;
    }

    public void Move(float moveInput)
    {
        this.moveInput = moveInput;
        if (moveInput != 0) lastNonZeroMoveInput = moveInput;
    }

    public void Jump()
    {
        timeSinceJumpInput = 0;
        timeSinceDampenInput = Mathf.Infinity;
    }

    public void DampenJump()
    {
        timeSinceDampenInput = 0;
        timeSinceJumpInput = Mathf.Infinity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, gcRadius);
        Gizmos.DrawLine(transform.position, transform.position - transform.up * gcDistToFeet);
    }
}
