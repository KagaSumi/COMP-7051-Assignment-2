// PlayerController.cs

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    private CapsuleCollider capsuleCollider;

    [Header("Look Settings")]
    public Camera playerCamera;
    public float lookSensitivity = 100.0f;
    public float maxLookUp = 90.0f;
    public float maxLookDown = -90.0f;

    [Header("Reset Logic")]
    public EnemyAI enemyToReset; // Reference to the enemy script

    private CharacterController controller;
    private PlayerControls playerControls;
    private Vector3 playerVelocity;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0.0f;
    private bool noClipActive = false;

    // Store initial spawn position and rotation
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        playerControls = new PlayerControls();

        // Store initial transform
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
        // Subscribe to the 'performed' event for button presses
        playerControls.Gameplay.ToggleNoClip.performed += ToggleNoClip;
        playerControls.Gameplay.Reset.performed += ResetPlayerAndEnemy;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
        playerControls.Gameplay.ToggleNoClip.performed -= ToggleNoClip;
        playerControls.Gameplay.Reset.performed -= ResetPlayerAndEnemy;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        // Read the continuous 2D move input from WASD or Left Stick
        moveInput = playerControls.Gameplay.Move.ReadValue<Vector2>();

        // --- NEW LOGIC: Check if NoClip is active ---
        if (noClipActive)
        {
            // Calculate movement direction relative to where the player is looking
            Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

            // Apply the movement directly to the transform's position
            transform.position += move * moveSpeed * Time.deltaTime;
        }
        else
        {
            // NORMAL MODE: Use the CharacterController

            // Ground the player if they are on the ground
            if (controller.isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = -2f; // A small downward force to keep them grounded
            }

            // Calculate horizontal movement
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            controller.Move(move * moveSpeed * Time.deltaTime);

            // Apply gravity
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void HandleLook()
    {
        // Read continuous input for looking
        lookInput = playerControls.Gameplay.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // Rotate the player body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera up/down
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, maxLookDown, maxLookUp);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    // This method is called by the Input System when the assigned button is pressed
    private void ToggleNoClip(InputAction.CallbackContext context)
    {
        noClipActive = !noClipActive;

        // Toggle both the CharacterController's collision and the child's collider
        controller.detectCollisions = !noClipActive;
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = !noClipActive;
        }

        Debug.Log("No-Clip Toggled: " + (noClipActive ? "ON" : "OFF"));
    }

    // This method is called by the Input System for the reset button
    private void ResetPlayerAndEnemy(InputAction.CallbackContext context)
    {
        Debug.Log("Reset button pressed. Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}