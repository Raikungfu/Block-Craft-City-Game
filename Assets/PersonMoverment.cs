using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMoverment : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;

    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;



        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("IsRunning", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsRunning", false);
            if (characterController.isGrounded)
            {
                characterController.stepOffset = originalStepOffset;
                ySpeed = -0.5f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ySpeed = jumpSpeed;
                    animator.SetBool("IsJumping", true);
                }
                else
                {
                    animator.SetBool("IsJumping", false);
                }
            }
            else
            {
                characterController.stepOffset = 0;
            }
        }
    }
}
