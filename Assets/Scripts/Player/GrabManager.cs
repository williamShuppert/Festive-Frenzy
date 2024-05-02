using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrabManager : MonoBehaviour
{
    [SerializeField] float throwStrength = 5;
    [SerializeField] float throwAngularVel = 500;
    [SerializeField] Vector2 throwDir = Vector2.right;
    [SerializeField] Transform grabPoint;
    [SerializeField] Vector2 grabSize = new Vector2(1, 1);
    [SerializeField] Vector2 grabOffset;

    public UnityEvent OnThrow;
    public UnityEvent OnPickup;
    
    private new Collider2D collider;
    private Collider2D grabbedCollider;
    private Collider2D prevGrabbedCollider;
    private PlayerMovement playerMovement;

    public bool IsGrabbing()
    {
        return grabbedCollider != null;
    }

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Throw
        if (Input.GetKeyDown(KeyCode.E) && grabbedCollider)
        {
            grabbedCollider.isTrigger = false;
            grabbedCollider.GetComponent<SpriteRenderer>().sortingOrder = 1;

            var rb = grabbedCollider.GetComponent<Rigidbody2D>();
            rb.velocity = Vector3.Project(playerMovement.rb.velocity, transform.right);
            rb.velocity += throwDir.normalized * throwStrength * new Vector2(playerMovement.lastNonZeroMoveInput, 1);
            rb.angularVelocity = throwAngularVel * -playerMovement.lastNonZeroMoveInput;

            prevGrabbedCollider = grabbedCollider;
            grabbedCollider = null;

            OnThrow.Invoke();
            return;
        }

        // Init Grab
        if (!grabbedCollider && Input.GetKeyDown(KeyCode.E))
        {
            grabbedCollider = GetClosestGrabbableCollider();

            if (!grabbedCollider) return;

            grabbedCollider.isTrigger = true;
            Physics2D.IgnoreCollision(grabbedCollider, collider, true);
            grabbedCollider.GetComponent<SpriteRenderer>().sortingOrder = 9;

            OnPickup.Invoke();
        }

        // Grabbing
        if (grabbedCollider)
        {
            if (!grabbedCollider.attachedRigidbody)
            {
                grabbedCollider = null;
                return;
            }

            grabbedCollider.transform.position = grabPoint.position;
            grabbedCollider.transform.eulerAngles = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!prevGrabbedCollider) return;

        if (!prevGrabbedCollider.bounds.Intersects(collider.bounds))
        {
            Physics2D.IgnoreCollision(prevGrabbedCollider, collider, false);
            prevGrabbedCollider = null;
        }
    }

    private Collider2D GetClosestGrabbableCollider()
    {
        var colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + grabOffset, grabSize, 0);

        Collider2D closestCollider = null;
        var minDist = Mathf.Infinity;
        foreach (var collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue; // Don't grab self
            if (!collider.attachedRigidbody || collider.attachedRigidbody.isKinematic || collider.attachedRigidbody.bodyType == RigidbodyType2D.Static)
                continue; // Don't grab non rigidbodies or static/kinematic ones

            //var dirToObj = (collider.transform.position - transform.position).normalized;
            //if (Vector2.Dot(collider.attachedRigidbody.velocity, dirToObj) > 0 && collider.attachedRigidbody.velocity.magnitude > 10)
            //    continue; // Don't grab objects moving fast enough away from player

            var dist = Vector2.Distance(collider.transform.position, transform.position);
            if (dist < minDist)
            {
                closestCollider = collider;
                minDist = dist;
            }
        }

        return closestCollider;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + grabOffset, grabSize);

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + throwDir);
    }
}
