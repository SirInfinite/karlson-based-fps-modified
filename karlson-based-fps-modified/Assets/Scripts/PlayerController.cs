using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] public Camera playerCamera;
    [SerializeField] public float initialFOV;
    [SerializeField] public float lookSpeed;
    [SerializeField] public float lookXLimit;

    [Header("Jump")]
    [SerializeField] public float jumpPower;
    [SerializeField] public float gravity;

    [Header("Height")]
    [SerializeField] public float defaultHeight;
    [SerializeField] public float crouchHeight;

    [Header("Speeds")]
    [SerializeField] public float crouchSpeed;
    [SerializeField] private float defaultWalkSpeed = 12f;
    [SerializeField] private float defaultRunSpeed = 24f;
    private float walkSpeed;
    private float runSpeed;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera.fieldOfView = initialFOV;
    }
    void ProcessInput()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (transform.forward * curSpeedX) + (transform.right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
    }

    void Update()
    {
        ProcessInput();

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && canMove && characterController.height != 1)
        {
            playerCamera.fieldOfView = initialFOV + 3.5f;
        }
        else if (Input.GetKey(KeyCode.C) && canMove)
        {
            playerCamera.fieldOfView = initialFOV - 4f;
        }
        else
        {
            playerCamera.fieldOfView = initialFOV;
        }

        if (Input.GetKey(KeyCode.C) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = defaultWalkSpeed;
            runSpeed = defaultRunSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}