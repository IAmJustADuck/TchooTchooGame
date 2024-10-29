using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Movements")]
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float crouchingSpeed = 1.5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 1f;
    private Coroutine crouchCoroutine;

    private CharacterController characterController;
    private bool isCrouching = false;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;

    private float rotationX = 0;

    [Header("Ground")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool wasGrounded = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleCrouch();
    }

    #region Movement
    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isCrouching ? crouchingSpeed : (isRunning ? runningSpeed : walkingSpeed);

        Vector3 moveDirection = (transform.forward * moveVertical + transform.right * moveHorizontal).normalized * speed;

        if (Grounded())
        {
            if (!wasGrounded)
            {
                velocity.y = 0;
            }

            if (Input.GetButtonDown("Jump") && !isCrouching)
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move((moveDirection + velocity) * Time.deltaTime);
        wasGrounded = Grounded();
    }
    #endregion

    #region Grounded Check
    private bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }
    #endregion

    #region Crouch
    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Grounded())
        {
            isCrouching = true;
            if (crouchCoroutine != null)
            {
                StopCoroutine(crouchCoroutine);
            }
            crouchCoroutine = StartCoroutine(CrouchToHeight(crouchHeight));
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
        {
            isCrouching = false;
            if (crouchCoroutine != null)
            {
                StopCoroutine(crouchCoroutine);
            }
            crouchCoroutine = StartCoroutine(CrouchToHeight(standingHeight));
        }
    }
    #endregion

    #region Rotation
    private void HandleRotation()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, rotationY, 0);
    }
    #endregion

    #region Crouch Animation
    private IEnumerator CrouchToHeight(float targetHeight)
    {
        float startHeight = transform.localScale.y;
        float startCenterY = characterController.center.y;

        float elapsedTime = 0f;
        float duration = 0.17f;

        while (elapsedTime < duration)
        {
            float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            transform.localScale = new Vector3(1, newHeight, 1);

            characterController.center = new Vector3(0, (newHeight / 2) - (standingHeight / 2), 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, targetHeight, 1);
        characterController.center = new Vector3(0, (targetHeight / 2) - (standingHeight / 2), 0);
    }
    #endregion
}
