using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stompSound;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 1.3f;
    public bool lerpCrouch;
    public bool crouching;
    public bool sprinting;
    public float crouchTimer;
    private InputManager inputManager;

    [Header("Stomp Bounce")]
    public Enemy currentStompTarget;
    public bool canStomp = false;
    public float stompBounceForce = 10f;
    private bool isOnFloor = false;
    private float stompCooldown = 0f;
    private bool hasJumped = false;
    private float stompAbilityCooldown = 1f; // ADD THIS
    private float lastStompTime = -1f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        CheckFloor();

        if (isGrounded && isOnFloor)                            // CHANGE: only reset when on floor
        {
            hasJumped = false;
            canStomp = false;
            currentStompTarget = null;
        }
        else if (!isGrounded)
        {
            canStomp = true;                                    // true when in air
        }

        if (stompCooldown > 0f)
            stompCooldown -= Time.deltaTime;

        bool jumpPressed = inputManager.onFoot.Jump.triggered;

        if (jumpPressed)
        {
            if (canStomp && currentStompTarget != null)
                DoStompBounce();
            else if (isGrounded && isOnFloor && !hasJumped)
            {
                hasJumped = true;
                Jump();
            }
            else if (!isGrounded && Time.time >= lastStompTime + stompAbilityCooldown) // only start if cooldown is up
            {
                lastStompTime = Time.time;
            }
        }
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else controller.height = Mathf.Lerp(controller.height, 2, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            // Push player away horizontally from the enemy
            Vector3 pushDir = transform.position - hit.gameObject.transform.position;
            pushDir.y = 0f;
            pushDir.Normalize();

            controller.Move(pushDir * speed * Time.deltaTime);
        }
    }

    private void CheckFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            isOnFloor = hit.collider.CompareTag("Floor") ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain");
        }
        else
        {
            isOnFloor = false;
        }
    }

    public float GetVelocityY()
    {
        return playerVelocity.y;
    }

    //recieve the inputs for out InputManager.cs and apply them to our character controller.
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void DoStompBounce()
    {
        if (currentStompTarget == null) return;
        if (Time.time < lastStompTime + stompAbilityCooldown) return; // block stomp if on cooldown

        lastStompTime = Time.time;

        Enemy enemy = currentStompTarget;
        enemy.lastHitType = ZombieCounter.AttackType.Stomp;
        enemy.TakeDamage(20, false, true); // fromStomp = true

        playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        stompCooldown = 0.5f;
        hasJumped = false;
        currentStompTarget = null;

        PlaySound(stompSound);

        PaintSplatter paint = enemy.GetComponent<PaintSplatter>();
        if (paint != null)
            paint.SplatterOnHit(enemy.transform.position);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);  // slight pitch variation
        audioSource.PlayOneShot(clip);
    }

    public float GetStompCooldownProgress()                    // ADD THIS
    {
        if (Time.time >= lastStompTime + stompAbilityCooldown)
            return 1f;
        return (Time.time - lastStompTime) / stompAbilityCooldown;
    }

    public float GetStompCooldownRemaining()                   // ADD THIS
    {
        return Mathf.Max(0f, (lastStompTime + stompAbilityCooldown) - Time.time);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
            speed = 8;
        else
            speed = 5;
    }
}
