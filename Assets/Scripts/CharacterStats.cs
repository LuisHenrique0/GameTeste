using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;
    public int baseAttackPower;
    public int baseDefensePower;
    public int currentDefensePower;
    public int speed;
    public float criticalChance;

    public void Initialize(string name, int health, int mana, int attack, int defense, int characterSpeed)
    {
        characterName = name;
        maxHealth = health;
        currentHealth = health;
        maxMana = mana;
        currentMana = mana;
        baseAttackPower = attack;
        baseDefensePower = defense;
        currentDefensePower = defense;
        speed = characterSpeed;
        criticalChance = Random.Range(0.05f, 0.15f);
    }

    public bool IsAlive() => currentHealth > 0;

    public void TakeDamage(int incomingDamage)
    {
        int finalDamage = Mathf.Max(incomingDamage - currentDefensePower, 1);
        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
    }

    public bool CanUseSkill(int manaCost)
    {
        return currentMana >= manaCost;
    }

    public void ConsumeSkillMana(int manaCost)
    {
        currentMana = Mathf.Max(currentMana - manaCost, 0);
    }

    public void ResetDefense()
    {
        currentDefensePower = baseDefensePower;
    }
}