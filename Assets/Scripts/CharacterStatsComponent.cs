using UnityEngine;

public class CharacterStatsComponent : MonoBehaviour
{
    public CharacterStats Stats { get; private set; }

    void Awake()
    {
        Stats = new CharacterStats();
    }

    public void Initialize(string name, int health, int mana, int attack, int defense, int characterSpeed)
    {
        Stats.Initialize(name, health, mana, attack, defense, characterSpeed);
    }

    public void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
    }

    public bool IsAlive()
    {
        return Stats.IsAlive();
    }
}