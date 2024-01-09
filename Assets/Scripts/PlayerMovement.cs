using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5.0f;

    private Vector2 moveInput;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start() {

    }

    private void Update() {
        FlipSprite();
        SetWalkingAnimation();
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void FlipSprite() {
        bool isMovingLeft = moveInput.x < 0;
        if (moveInput.x != 0) {
            spriteRenderer.flipX = isMovingLeft;
        }
    }

    private void SetWalkingAnimation() {
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isMoving);
    }

    private void Move() {
        float moveX = moveInput.x * moveSpeed;
        float moveY = moveInput.y * moveSpeed;
        rigidBody.velocity = new Vector2(moveX, moveY);
    }
}
