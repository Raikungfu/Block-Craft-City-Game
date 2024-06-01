using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GuyMoverment : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;

    private CharacterController characterController;
    private float ySpeed; 
    private float originalStepOffset;
    private Animator animator;
    private GuyStats stats;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
        stats = GetComponent<GuyStats>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
        }
        else
        {
            characterController.stepOffset = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;

            if (characterController.isGrounded || CheckIfGrounded())
            {
                ySpeed = jumpSpeed;
            } else if (stats.level > 20 && Physics.Raycast(transform.position, Vector3.down, out hit) && hit.distance <= ySpeed)
            {
                ySpeed = jumpSpeed;
            }
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

    }

    void HandleAnimation()
    {
        bool isJumping = !characterController.isGrounded;
        animator.SetBool("IsJumping", isJumping); 
    }

    bool CheckIfGrounded()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 10f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, characterController.height / 2 + groundCheckDistance, groundMask))
        {
            return true;
        }
        return false;
    }

}
