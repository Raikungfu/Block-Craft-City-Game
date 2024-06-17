using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlowingSphere : MonoBehaviour
{
    public float hoverHeight = 0.3f;
    public float hoverSpeed = 0.5f;
    public float rotationSpeed = 50.0f;
    public Light glowLight;
    public ParticleSystem glowEffect;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;

        if (glowLight == null)
            glowLight = GetComponentInChildren<Light>();

        if (glowEffect == null)
            glowEffect = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GuyInventory inventory = other.GetComponent<GuyInventory>();
            if (inventory != null)
            {
                if (CompareTag("Weapon"))
                {
                    inventory.AddWeapon(gameObject.name);
                    ToggleEffect(false);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void ToggleEffect(bool state)
    {
        if (glowLight != null)
            glowLight.enabled = state;

        if (glowEffect != null)
        {
            if (state)
                glowEffect.Play();
            else
                glowEffect.Stop();
        }
    }
}
