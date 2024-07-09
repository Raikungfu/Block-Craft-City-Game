using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private GuyInventory inventory;
    private float velo = 0.0f;
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int VelocityXHash = Animator.StringToHash("X");
    private static readonly int VelocityYHash = Animator.StringToHash("Y");
    private bool isSlashing = false;
    private bool canSlash = true;
    public float slashCooldown = 5.0f;

    public GameObject WeaponsList;
    public GameObject InventoryList;

    public Weapon currentWeapon; 
    public Item currentBlock;

    public bool getIsSlashing() => isSlashing;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
        stats = GetComponent<GuyStats>();
        inventory = GetComponent<GuyInventory>();
        groundMask = LayerMask.GetMask("mountain");
        animator.SetFloat(VelocityHash, velo);
    }

    void Update()
    {
        Transform activeCamera = GetActiveCameraTransform();
        if (activeCamera != null)
        {
            HandleMovement(activeCamera);
            HandleAnimation();
            HandleWeaponSwitch();
        }
    }

    Transform GetActiveCameraTransform()
    {
        foreach (var camTransform in cameraTransforms)
        {
            if (camTransform.gameObject.active)
            {
                return camTransform;
            }
        }
        return null;
    }


    void HandleMovement(Transform activeCamera)
    {
        if (stats.IsDied) return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = activeCamera.forward;
        Vector3 cameraRight = activeCamera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 movementDirection = (horizontalInput * cameraRight + verticalInput * cameraForward).normalized;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            if (velo < speed) velo += Time.deltaTime * (speed / 5);
        }
        else
        {
            velo = 0.0f;
        }

        animator.SetFloat(VelocityHash, velo);
        animator.SetFloat(VelocityXHash, horizontalInput);
        animator.SetFloat(VelocityYHash, verticalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
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

        Vector3 velocity = movementDirection * velo;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        HandleOtherAction();
    }

    void HandleOtherAction()
    {
        if (Input.GetButtonDown("Slash") && currentWeapon && canSlash)
        {
            animator.SetTrigger("Slash");
            StartCoroutine(SlashCoroutine());
        }

        if (Input.GetButtonDown("Grab"))
        {
            animator.SetTrigger("Grab");
        }

        if (Input.GetButtonDown("PickUp"))
        {
            animator.SetTrigger("PickUp");
        }
    }

    private IEnumerator SlashCoroutine()
    {
        isSlashing = true;
        canSlash = false;
        yield return new WaitForSeconds(slashCooldown);
        isSlashing = false;
        canSlash = true;
    }

    void HandleAnimation()
    {
        bool isJumping = !characterController.isGrounded;
        animator.SetBool("IsJumping", isJumping);
    }


    void HandleWeaponSwitch()
    {
        for (int i = 1; i <= 3; i++)
        {
            Image childOverlay = WeaponsList.GetComponentsInChildren<Image>()[i];
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                Weapon weapon = inventory.GetWeapon(i - 1);

                Color overlayColor = new Color(226 / 255f, 84 / 255f, 84 / 255f, 1.0f);
                Color overlayColorDis = new Color(212 / 255f, 212 / 255f, 212 / 255f, 1.0f);
                if (weapon != null)
                {

                    if (currentWeapon != weapon)
                    {
                        changeColorOverlay(childOverlay, overlayColor);
                        EquipWeapon(weapon);
                    }
                    else
                    {
                        changeColorOverlay(childOverlay, overlayColorDis);
                        DestroyWeapon();
                    }
                }
                for (int j = 1; j <= 3; j++)
                {
                    if (i != j)
                    {
                        Image childOverlayDis = WeaponsList.GetComponentsInChildren<Image>()[j];
                        changeColorOverlay(childOverlayDis, overlayColorDis);
                    }
                }
            }
        }
    }

    void changeColorOverlay(Image overlayImage, Color color)
    {
        if (overlayImage != null)
        {
            overlayImage.color = color;
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;

        UpdatePlayerWeaponModel(newWeapon);
    }

    public void DestroyWeapon()
    {
        Transform weaponSlot = FindChildTransformByName(transform, "mixamorig:RightHandMiddle4");
        if (weaponSlot != null)
        {
            foreach (Transform child in weaponSlot)
            {
                Destroy(child.gameObject);
            }
        }
        currentWeapon = null;
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
                GameObject instantiatedWeapon = Instantiate(Resources.Load<GameObject>(weapon.prefab), weaponSlot);
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

    public void BuildBlock(Vector3 position)
    {
        if (currentBlock != null && currentBlock.amount > 0)
        {
            GameObject blockPrefab = Resources.Load<GameObject>(currentBlock.prefab);
            GlowingSphere script = blockPrefab.GetComponent<GlowingSphere>();
            if (script != null)
            {
                Destroy(script);
            }
            if (blockPrefab != null)
            {
                Instantiate(blockPrefab, position, Quaternion.identity);
                currentBlock.amount--;
                Debug.Log("Block built. Remaining amount: " + currentBlock.amount);
            }
            else
            {
                Debug.LogWarning("Block prefab not found!");
            }
        }
        else
        {
            Debug.LogWarning("No blocks available to build!");
        }
    }

    public void DestroyBlock(GameObject block)
    {
        if (currentWeapon != null)
        {
            Destroy(block);
            Debug.Log("Block destroyed with weapon: " + currentWeapon.itemName);
        }
        else
        {
            Debug.LogWarning("No weapon equipped to destroy the block!");
        }
    }
}
