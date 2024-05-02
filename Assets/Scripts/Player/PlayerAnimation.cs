using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GrabManager grabManager;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] SpriteRenderer handsSprite;

    [Header("Player Sprites")]
    [SerializeField] Sprite playerIdleSprite;
    [SerializeField] Sprite playerThrowSprite;

    void Update()
    {
        var flipX = playerMovement.lastNonZeroMoveInput < 0;
        playerSprite.flipX = flipX;
        handsSprite.flipX = flipX;

        var isGrabbing = grabManager.IsGrabbing();
        handsSprite.enabled = isGrabbing
            || (!playerMovement.isGrounded && Vector2.Dot(playerMovement.rb.velocity, Vector2.up) < -5); // Put hands up if falling faster than -5m/s
        playerSprite.sprite = handsSprite.enabled ? playerThrowSprite : playerIdleSprite;

        animator.SetBool("isGrounded", playerMovement.isGrounded);
        animator.SetBool("isMoving", Vector3.Project(playerMovement.rb.velocity, transform.right).magnitude > .1f);
    }
}
