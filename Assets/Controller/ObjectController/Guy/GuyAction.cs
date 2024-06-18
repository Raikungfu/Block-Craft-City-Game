using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuyAction : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f;
    public float jumpSpeed = 10f;

    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private Animator animator;
    private GuyStats stats;
    public float groundCheckDistance = 2f;
    public float lookSpeed = 2f;
    public LayerMask groundMask;

    public Transform[] cameraTransforms;

    public Weapon currentWeapon;
    private GuyInventory inventory;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
        stats = GetComponent<GuyStats>();
        inventory = GetComponent<GuyInventory>();
        groundMask = LayerMask.GetMask("mountain");
    }

    void Update()
    {
        Transform activeCamera = GetActiveCamera();
        if (activeCamera != null)
        {
            HandleMovement(activeCamera);
            HandleAnimation();
            HandleWeaponSwitch();
        }
    }

    Transform GetActiveCamera()
    {
        foreach (var camTransform in cameraTransforms)
        {
            if (camTransform == Camera.main.transform)
            {
                return camTransform;
            }
        }
        return null;
    }

    void HandleMovement(Transform activeCamera)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = activeCamera.forward;
        Vector3 cameraRight = activeCamera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementDirection = (horizontalInput * cameraRight + verticalInput * cameraForward).normalized;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

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
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else
        {
            characterController.stepOffset = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;

            if ((Physics.Raycast(transform.position, Vector3.down, out hit) && hit.distance <= 0.3) || (stats.level > 20 && hit.distance <= ySpeed))
            {
                ySpeed = jumpSpeed;
            }
        }

        if (Input.GetButtonDown("Slash") && !IsPointerOverUIObject() && currentWeapon)
        {
            animator.SetTrigger("Slash");
        }

        if (Input.GetButtonDown("Grab"))
        {
            animator.SetTrigger("Grab");
        }

        if (Input.GetButtonDown("PickUp") && !IsPointerOverUIObject())
        {
            animator.SetTrigger("PickUp");
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

    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void HandleWeaponSwitch()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                Weapon weapon = inventory.GetWeapon(i - 1);
                if (currentWeapon != weapon)
                {
                    if (weapon != null)
                    {
                        EquipWeapon(weapon);
                    }
                }
            }
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        Debug.Log("Weapon equipped: " + newWeapon.itemName);

        UpdatePlayerWeaponModel(newWeapon);
    }

    private void UpdatePlayerWeaponModel(Weapon weapon)
    {
        Transform weaponSlot = FindChildTransformByName(transform, "mixamorig:RightHandMiddle4");
        if (weaponSlot != null)
        {
            foreach (Transform child in weaponSlot)
            {
                Destroy(child.gameObject);
            }

            if (weapon.prefab != null)
            {
                GameObject instantiatedWeapon = Instantiate(weapon.prefab, weaponSlot);
                RotateWeaponTowardsCharacter(instantiatedWeapon);
            }
        }
    }

    private void RotateWeaponTowardsCharacter(GameObject weaponObject)
    {
        if (weaponObject != null)
        {
            Vector3 directionToCharacter = transform.position - weaponObject.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCharacter, Vector3.one);

            weaponObject.transform.rotation = lookRotation;
        }
    }

    private Transform FindChildTransformByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                Transform foundChild = FindChildTransformByName(child, name);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }
        return null;
    }
}
