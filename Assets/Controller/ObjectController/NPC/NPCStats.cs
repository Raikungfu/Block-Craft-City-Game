using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCStats : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    public Slider healthSlider;
    private TextMeshProUGUI healthText;
    public Transform healthBarPosition;
    public float respawnTime = 5f;
    public int expAmount = 5;
    private Vector3 initialPosition;

    void Start()
    {
        if (healthSlider != null)
        {
            healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        }
        currentHealth = maxHealth;
        initialPosition = transform.position;
        UpdateUI();
    }

    void Update()
    {
        if (healthBarPosition != null && healthSlider != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(healthBarPosition.position);
            healthSlider.transform.position = screenPosition;

            if (screenPosition.z < 0)
            {
                screenPosition.z = 0;
                healthSlider.transform.position = screenPosition;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateUI();

        if (currentHealth == 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                GuyStats playerStats = player.GetComponent<GuyStats>();
                if (playerStats != null)
                {
                    playerStats.GainExperience(expAmount);
                }
                else
                {
                    Debug.LogWarning("GuyStats component not found on the player.");
                }
            }
            else
            {
                Debug.LogWarning("Player not found.");
            }
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnTime);
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = initialPosition;
        gameObject.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GuyAction player = other.GetComponent<GuyAction>();
            if (player != null && player.currentWeapon != null)
            {
                float damage = player.currentWeapon.attackPower * player.GetComponent<GuyStats>().level;
                TakeDamage(damage);
            }
        }
        else if (CompareTag("Weapon"))
        {

        }
    }
}
