using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;
using Cinemachine;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public GameObject self;

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    public float maxSpeed;
    public float maxSpeedWallJump;
    public float maxSpeedDefault;
    public Vector2 lastDirection;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    public float jumpLimit;
    public float jumpDragPrecision = 0.4f;
    [HideInInspector] public float force;
    public float wallJumpYForce;

    private float jumpTimer;
    
    [Header("Components")] [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    [Header("Cameras")] public CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Physics")] public float linearDragDeceleration = 4f;
    public float linearDragMultiplier = 0.15f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    public float fallMultiplierDivider = 2.0f;
    public float gravityOnWall = 0.1f;
    public float airJumpTime = 0.25f;
    public bool INPUTENABLED = true;
    public RigidbodyConstraints2D startConstraints;

    private bool canAirJump;
    private bool canRunCoroutineAirJump;
    private bool isJumping;
    
    [Header("States")] public bool onGround;

    [Header("Collision")] public float groundLength = 0.6f;
    public Vector3 colliderOffset;
    public float cubeRightDisplacement = 0.1f;
    public float cubeLeftDisplacement = 0.1f;
    public float cubeRight2Displacement = 0.1f;
    public float cubeLeft2Displacement = 0.1f;
    public float cubeRightSizeX = 0.5f;
    public float cubeRightSizeY = 0.5f;
    public float cubeLeftSizeX = 0.5f;
    public float cubeLeftSizeY = 0.5f;
    public LayerMask groundLayer;

    private bool canVerifyWallJump;
    private bool canCheckRight = true;
    private bool canCheckLeft = true;
    private bool isTouchingLeft;
    private bool isTouchingRight;

    [Header("Audio")] public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioMixer GlobalMixer;
    [HideInInspector] public AudioSource source;
    public AudioSource sourceStep;
    public AudioClip stepSound;

    private void Start()
    {
        maxSpeedDefault = maxSpeed;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();

        spriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (INPUTENABLED)
        {
            rb.constraints = startConstraints;
            force = maxSpeedWallJump;

            onGround = Physics2D.Raycast(transform.position + colliderOffset,
                           Vector2.down, groundLength, groundLayer) ||
                       Physics2D.Raycast(transform.position - colliderOffset,
                           Vector2.down, groundLength, groundLayer) ||
                       Physics2D.Raycast(transform.position + colliderOffset,
                           Vector2.down, groundLength) ||
                       Physics2D.Raycast(transform.position - colliderOffset,
                           Vector2.down, groundLength);

            if (Input.GetButtonDown("Jump"))
            {
                jumpTimer = Time.time + jumpDelay;
            }
            
            if (onGround || !isTouchingLeft || !isTouchingRight)
            {
                gravity = 1f;
                isJumping = false;
            }

            if (onGround)
            {
                if (isTouchingLeft || isTouchingRight)
                {
                    isTouchingLeft = false;
                    isTouchingRight = false;
                }

                canCheckLeft = true;
                canCheckRight = true;
                //topCoroutine(VerifyLanding());
                canRunCoroutineAirJump = true;
            }
            else
            {
                //StartCoroutine(VerifyLanding());
            }
            
            animator.SetBool("isGrounded", onGround);

            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (direction.x > 0 || direction.x < 0)
            {
                //StepSound(); 
            }

            VerifyWallJump();
            airTime();
            Flip();

            if (isTouchingRight)
            {
                gravity = gravityOnWall;
              
                if (Input.GetButtonDown("Jump"))
                {
                    Flip();
                    JumpInvert(-1, 0);
                    gravity = .1f;

                    StartCoroutine(ResetWalkSpeed());

                    canCheckRight = false;
                    canCheckLeft = true;
                    isTouchingRight = false;
                }
            }

            if (isTouchingLeft)
            {
                gravity = gravityOnWall;

                if (Input.GetButtonDown("Jump"))
                {
                    Flip();
                    JumpInvert(1, 1);
                    gravity = .1f;

                    StartCoroutine(ResetWalkSpeed());

                    canCheckRight = true;
                    canCheckLeft = false;
                    isTouchingLeft = false;
                }
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    IEnumerator ResetWalkSpeed()
    {
        yield return new WaitForSeconds(0.1f);
        maxSpeed = maxSpeedDefault;
    }
    
    /*void StepSound()
    {
        if (onGround && sourceStep.isPlaying == false && rb.velocity.x != 0)
        {
            //sourceStep.pitch = Random.Range(0.5f, 0.7f);
            sourceStep.Play(2205);
        }
    }*/

    /*IEnumerator VerifyLanding()
    {
        yield return new WaitForSeconds(0.02f);
        
        if (onGround)
        {
            animator.SetBool("isLanded", true);
            
            yield return new WaitForSeconds(0.15f);
            animator.SetBool("isLanded", false);
        }
        else
        {
            animator.SetBool("isLanded", false);
        }
    }*/

    void airTime()
    {
        if (onGround == false)
        {
            if (canRunCoroutineAirJump)
            {
                StartCoroutine(airTimeCoroutine());
                canRunCoroutineAirJump = false;
            }

            if (canAirJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (isJumping == false && rb.velocity.y < -5f)
                    {
                        Jump();
                        canAirJump = false;
                        StopCoroutine(airTimeCoroutine());
                    }
                }
            }
        }
    }

    IEnumerator airTimeCoroutine()
    {
        canAirJump = true;
        yield return new WaitForSeconds(airJumpTime);
        canAirJump = false;
    }
    
    void FixedUpdate()
    {
        moveCharacter(direction.x);

        if (jumpTimer > Time.time && onGround && !isTouchingLeft && !isTouchingRight)
        {
            Jump();
        }
        
        modifyPhysics();
        
        SetlastDirection();
    }

    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, 
                rb.velocity.y);
        }
        
        animator.SetFloat("Horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Vertical", rb.velocity.y);
    }

    void SetlastDirection()
    {
        if (direction != Vector2.zero)
        {
            lastDirection = direction;
        }
    }
    
    void Jump()
    {
        if (isJumping == false)
        {
            //source.pitch = Random.Range(0.5f, 1.3f);
            //source.PlayOneShot(jumpSound);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpTimer = 0;
            isJumping = true;
        }
    }
    
    void JumpInvert(float axis, int x)
    {
        //source.pitch = Random.Range(0.5f, 1.3f);
        //source.PlayOneShot(jumpSound);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        
        maxSpeed = maxSpeedWallJump;
        
        rb.AddForce(new Vector2(axis * force, jumpSpeed * wallJumpYForce), ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);
        //animator.SetBool("isSkid", changingDirections);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < jumpDragPrecision || changingDirections)
            {
                rb.drag = linearDragDeceleration;
            }
            else
            {
                rb.drag = 0f;
            }

            rb.gravityScale = 0;
        }
        else if(!isTouchingLeft || !isTouchingRight)
        {
            rb.gravityScale = gravity;
            rb.drag = linearDragDeceleration * linearDragMultiplier;

            if (rb.velocity.y <= jumpLimit)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }

            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / fallMultiplierDivider);
            }
           
        }
    }

    void Flip()
    {
        if (lastDirection.x < 0)
        {
            transform.DOScale(new Vector3(-1, 1, 1), 0.2f);
        }
        else if (lastDirection.x > 0)
        {
            transform.DOScale(new Vector3(1, 1, 1), 0.2f);
        }
    }

    void VerifyWallJump()
    {
        if (!onGround)
        {
            if (canCheckRight)
            {
                isTouchingRight = Physics2D.OverlapBox(
                    new Vector2(self.transform.position.x + cubeRightDisplacement,
                        self.transform.position.y + cubeRight2Displacement),
                    new Vector2(cubeRightSizeX, cubeRightSizeY), 0f, groundLayer);

            }

            if (canCheckLeft)
            {
                isTouchingLeft = Physics2D.OverlapBox(
                    new Vector2(self.transform.position.x + cubeLeftDisplacement, self.transform.position.y),
                    new Vector2(cubeLeftSizeX, cubeLeftSizeY), 0f, groundLayer);
            }
        }
        
        //animator.SetBool("isWallJump", isTouchingLeft || isTouchingRight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,
            transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,
            transform.position - colliderOffset + Vector3.down * groundLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector2(self.transform.position.x + cubeRightDisplacement, self.transform.position.y + cubeRight2Displacement),
            new Vector2(cubeRightSizeX, cubeRightSizeY));
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(self.transform.position.x + cubeLeftDisplacement, self.transform.position.y + cubeLeft2Displacement),
            new Vector2(cubeLeftSizeX, cubeLeftSizeY));
    }
}