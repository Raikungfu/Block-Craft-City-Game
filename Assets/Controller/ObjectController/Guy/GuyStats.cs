using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuyStats : MonoBehaviour
{
    public int level;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;
    public int exp;
    public int currentExp = 0;
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI levelText;

    private TextMeshProUGUI healthText;
    private TextMeshProUGUI manaText;
    private TextMeshProUGUI expText;
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;

    public float regenRate = 1f;
    void Start()
    {
        if (healthSlider != null)
        {
            healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (manaSlider != null)
        {
            manaText = manaSlider.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (expSlider != null)
        {
            expText = expSlider.GetComponentInChildren<TextMeshProUGUI>();
        }
        userNameText.text = "Rai Yugi";
        levelText.text = "Lv. " + level;
        StartStats(level);
        StartCoroutine(RegenerateHealth());
    }

    private void Update()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (manaSlider.value != currentMana)
        {
            manaSlider.value = currentMana;
        }

        if (expSlider.value != currentExp)
        {
            expSlider.value = currentExp;
        }
        UpdateStats(level);
    }

    public void StartStats(int level)
    {
        maxHealth = CalculateMaxHealth(level);
        maxMana = CalculateMaxMana(level);
        exp = CalculateLevelExp(level);
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateUI();
    }

    public void UpdateStats(int newLevel)
    {
        level = newLevel;
        maxHealth = CalculateMaxHealth(level);
        maxMana = CalculateMaxMana(level);
        exp = CalculateLevelExp(level);
        UpdateUI();
    }

    private int CalculateMaxHealth(int level)
    {
        return 1000 + level * 500;
    }

    private int CalculateMaxMana(int level)
    {
        return 50 + level * 10;
    }

    private int CalculateLevelExp(int level)
    {
        return level * level * 1000;
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + "/" + maxHealth;
        }

        if (manaText != null)
        {
            manaText.text = "Mana: " + currentMana + "/" + maxMana;
        }

        if (expText != null)
        {
            expText.text = "Exp: " + currentExp + "/" + exp;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }

        if (expSlider != null)
        {
            expSlider.maxValue = exp;
            expSlider.value = currentExp;
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenRate);

            if (currentHealth < maxHealth)
            {
                currentHealth += (int)regenRate * level * 10;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
                currentMana += (int)regenRate * level;
                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
                UpdateUI();
            }
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        UpdateExperienceUI();

        if (currentExp >= exp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp -= exp;
        exp = CalculateNextLevelExp(level);
        UpdateLevelUI();
    }

    private int CalculateNextLevelExp(int level)
    {
        return CalculateLevelExp(level);
    }

    private void UpdateExperienceUI()
    {
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }
}
