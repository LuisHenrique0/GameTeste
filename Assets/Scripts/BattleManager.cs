using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BattleManager : MonoBehaviour
{
    enum SkillType
    {
        NormalAttack,
        MediumSkill,
        SpecialSkill
    }

    [Header("Player Components")]
    public CharacterStatsComponent playerStatsComponent;
    public Slider playerHealthSlider;
    public Slider playerManaSlider;
    public Button normalAttackButton;
    public Button mediumSkillButton;
    public Button specialSkillButton;
    public Button defendButton;

    [Header("Enemy Components")]
    public CharacterStatsComponent enemyStatsComponent;
    public Slider enemyHealthSlider;

    [Header("Battle UI")]
    public TMP_Text battleLogText;

    [Header("Inventory Panel")]
    public GameObject inventoryPanel;
    public Button openInventoryButton;
    public Button closeInventoryButton;
    public Transform itemButtonsContainer;
    public GameObject itemButtonPrefab;

    [Header("Arrow Components")]
    public GameObject arrowPrefab;
    public Transform playerPosition;
    public Transform enemyPosition;

    private bool isPlayerTurn = true;
    private bool battleEnded = false;
    private bool isInventoryOpen = false;
    private int enemyTurnCounter = 0;
    private List<GameObject> itemButtons = new List<GameObject>();
    private Inventory inventory;

    void Start()
    {
        inventory = new Inventory();

        playerStatsComponent.Initialize("Heroi", 100, 100, 20, 10, 8);
        enemyStatsComponent.Initialize("Inimigo", 200, 0, 15, 8, 6);

        InitializeBattleUI();

        normalAttackButton.onClick.AddListener(() => PlayerAttack(SkillType.NormalAttack));
        mediumSkillButton.onClick.AddListener(() => PlayerAttack(SkillType.MediumSkill));
        specialSkillButton.onClick.AddListener(() => PlayerAttack(SkillType.SpecialSkill));
        defendButton.onClick.AddListener(PlayerDefend);

        openInventoryButton.onClick.AddListener(OpenInventoryPanel);
        closeInventoryButton.onClick.AddListener(CloseInventoryPanel);

        inventoryPanel.SetActive(false);
    }

    void InitializeBattleUI()
    {
        playerHealthSlider.maxValue = playerStatsComponent.Stats.maxHealth;
        playerManaSlider.maxValue = playerStatsComponent.Stats.maxMana;
        enemyHealthSlider.maxValue = enemyStatsComponent.Stats.maxHealth;

        UpdateBattleUI();
    }

    void OpenInventoryPanel()
    {
        if (!isPlayerTurn || battleEnded) return;

        isInventoryOpen = true;
        inventoryPanel.SetActive(true);
        RefreshInventoryPanel();
        
        normalAttackButton.interactable = false;
        mediumSkillButton.interactable = false;
        specialSkillButton.interactable = false;
        defendButton.interactable = false;
    }

    void CloseInventoryPanel()
    {
        isInventoryOpen = false;
        inventoryPanel.SetActive(false);
        
        if (isPlayerTurn && !battleEnded)
        {
            normalAttackButton.interactable = true;
            mediumSkillButton.interactable = true;
            specialSkillButton.interactable = true;
            defendButton.interactable = true;
        }
    }

    void RefreshInventoryPanel()
{
    for (int i = 0; i < inventory.Items.Count; i++)
    {
        var item = inventory.Items[i];

        if (i < itemButtons.Count)
        {
            GameObject buttonObj = itemButtons[i];
            Button itemButton = buttonObj.GetComponent<Button>();
            ItemButtonUI buttonUI = buttonObj.GetComponent<ItemButtonUI>();

          
            buttonUI.UpdateButton(item);

           
            itemButton.interactable = item.amount > 0;
        }
        else
        {
         
            GameObject buttonObj = Instantiate(itemButtonPrefab, itemButtonsContainer);
            Button itemButton = buttonObj.GetComponent<Button>();
            ItemButtonUI buttonUI = buttonObj.GetComponent<ItemButtonUI>();

            
            buttonUI.UpdateButton(item);

           
            itemButtons.Add(buttonObj);

           
            itemButton.onClick.AddListener(() => UseItemFromInventory(item.itemType));
        }
    }

    
    for (int i = inventory.Items.Count; i < itemButtons.Count; i++)
    {
        itemButtons[i].SetActive(false);
    }
}

    

    void UseItemFromInventory(Item.ItemType itemType)
    {
        if (!isPlayerTurn || battleEnded) return;

        var item = inventory.GetItem(itemType);
        if (item == null || item.amount <= 0)
        {
            UpdateBattleLog("Item não disponível!");
            return;
        }

        bool itemUsed = false;

        switch (itemType)
        {
            case Item.ItemType.HealthPotion:
                if (playerStatsComponent.Stats.currentHealth < playerStatsComponent.Stats.maxHealth)
                {
                    int healAmount = item.effectValue;
                    playerStatsComponent.Stats.currentHealth = Mathf.Min(
                        playerStatsComponent.Stats.currentHealth + healAmount,
                        playerStatsComponent.Stats.maxHealth
                    );
                    UpdateBattleLog($"Jogador usou {item.name} e recuperou {healAmount} de vida!");
                    itemUsed = true;
                }
                else
                {
                    UpdateBattleLog("Vida já está cheia!");
                }
                break;

            case Item.ItemType.ManaPotion:
                if (playerStatsComponent.Stats.currentMana < playerStatsComponent.Stats.maxMana)
                {
                    int manaAmount = item.effectValue;
                    playerStatsComponent.Stats.currentMana = Mathf.Min(
                        playerStatsComponent.Stats.currentMana + manaAmount,
                        playerStatsComponent.Stats.maxMana
                    );
                    UpdateBattleLog($"Jogador usou {item.name} e recuperou {manaAmount} de mana!");
                    itemUsed = true;
                }
                else
                {
                    UpdateBattleLog("Mana já está cheia!");
                }
                break;
        }

        if (itemUsed)
        {
            inventory.UseItem(itemType);
            RefreshInventoryPanel();
            UpdateBattleUI();
            CloseInventoryPanel();
            EndPlayerTurn();
        }
    }

    void PlayerAttack(SkillType skillType)
    {
        if (!isPlayerTurn || battleEnded) return;

        UpdateBattleLog("", true);

        int damage = 0;
        int manaCost = 0;
        string skillName = "";

        switch (skillType)
        {
            case SkillType.NormalAttack:
                damage = playerStatsComponent.Stats.baseAttackPower;
                manaCost = 5;
                skillName = "Ataque Normal";

                if (!playerStatsComponent.Stats.CanUseSkill(manaCost))
                {
                    UpdateBattleLog("Mana insuficiente!");
                    return;
                }

                playerStatsComponent.Stats.ConsumeSkillMana(manaCost);
                UpdateBattleUI();
                UpdateBattleLog($"Jogador usou {skillName}!");

                StartCoroutine(LaunchArrowAndEndTurn(damage));
                return;

            case SkillType.MediumSkill:
                damage = Mathf.RoundToInt(playerStatsComponent.Stats.baseAttackPower * 1.5f);
                manaCost = 15;
                skillName = "Habilidade Média";
                break;

            case SkillType.SpecialSkill:
                damage = Mathf.RoundToInt(playerStatsComponent.Stats.baseAttackPower * 2.5f);
                manaCost = 25;
                skillName = "Habilidade Especial";
                break;
        }

        if (!playerStatsComponent.Stats.CanUseSkill(manaCost))
        {
            UpdateBattleLog("Mana insuficiente!");
            return;
        }

        playerStatsComponent.Stats.ConsumeSkillMana(manaCost);
        ApplyDamageToEnemy(damage);
        UpdateBattleLog($"Jogador usou {skillName}! Causou {damage} de dano!");

        UpdateBattleUI();
        EndPlayerTurn();
    }

    IEnumerator LaunchArrowAndEndTurn(int damage)
    {
        if (arrowPrefab == null || playerPosition == null || enemyPosition == null)
        {
            Debug.LogWarning("O prefab da seta ou as posições não estão atribuídos.");
            yield break;
        }

        GameObject arrow = Instantiate(arrowPrefab, playerPosition.position, Quaternion.identity);
        Vector3 direction = (enemyPosition.position - playerPosition.position).normalized;

        ArrowController arrowScript = arrow.GetComponent<ArrowController>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction);
            arrowScript.damage = damage;
            arrowScript.battleManager = this;
        }

        while (arrow != null)
        {
            yield return null;
        }

        UpdateBattleUI();
        CheckForEndGame();
        if (!playerStatsComponent.IsAlive() || !enemyStatsComponent.IsAlive()) yield break;
        EndPlayerTurn();
    }

    public void ApplyDamageToEnemy(int damage)
    {
        enemyStatsComponent.TakeDamage(damage);
        UpdateBattleLog($"Inimigo sofreu {damage} de dano!");
        UpdateBattleUI();
        CheckForEndGame();
    }

    void PlayerDefend()
    {
        if (!isPlayerTurn || battleEnded) return;

        UpdateBattleLog("", true);

        playerStatsComponent.Stats.currentDefensePower = Mathf.RoundToInt(playerStatsComponent.Stats.baseDefensePower * 1.2f);
        UpdateBattleLog("Jogador está se defendendo! Defesa aumentada.");
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        if (battleEnded) return;

        isPlayerTurn = false;
        DisablePlayerButtons();
        Invoke("EnemyTurn", 2f);
    }

    void EnemyTurn()
    {
        if (battleEnded) return;

        UpdateBattleLog("", true);

        enemyTurnCounter++;

        int damage;
        string skillName;

        if (enemyTurnCounter % 3 == 0)
        {
            damage = Mathf.RoundToInt(enemyStatsComponent.Stats.baseAttackPower * 2f);
            skillName = "Ataque Especial";
        }
        else
        {
            damage = enemyStatsComponent.Stats.baseAttackPower;
            skillName = "Ataque Normal";
        }

        UpdateBattleLog($"Inimigo usou {skillName} e causou {damage} de dano!");
        playerStatsComponent.TakeDamage(damage);

        UpdateBattleUI();
        CheckForEndGame();
        if (!playerStatsComponent.IsAlive()) return;

        isPlayerTurn = true;
        EnablePlayerButtons();
    }

    void UpdateBattleUI()
    {
        playerHealthSlider.value = playerStatsComponent.Stats.currentHealth;
        playerManaSlider.value = playerStatsComponent.Stats.currentMana;
        enemyHealthSlider.value = enemyStatsComponent.Stats.currentHealth;
    }

    void UpdateBattleLog(string message, bool clearLog = false)
    {
        if (battleLogText != null)
        {
            if (clearLog)
            {
                battleLogText.text = "";
            }

            battleLogText.text += message + "\n";

            string[] lines = battleLogText.text.Split('\n');
            if (lines.Length > 5)
            {
                battleLogText.text = string.Join("\n", lines, lines.Length - 5, 5);
            }
        }
        else
        {
            Debug.Log(message);
        }
    }

    void CheckForEndGame()
    {
        if (!playerStatsComponent.IsAlive())
        {
            EndBattle(false);
        }
        else if (!enemyStatsComponent.IsAlive())
        {
            EndBattle(true);
        }
    }

    void EndBattle(bool playerWon)
    {
        battleEnded = true;
        DisablePlayerButtons();
        UpdateBattleLog(playerWon ? "Jogador venceu a batalha!" : "Inimigo venceu a batalha!");
    }

    void DisablePlayerButtons()
    {
        normalAttackButton.interactable = false;
        mediumSkillButton.interactable = false;
        specialSkillButton.interactable = false;
        defendButton.interactable = false;
        openInventoryButton.interactable = false;
        
        if (isInventoryOpen)
        {
            CloseInventoryPanel();
        }
    }

    void EnablePlayerButtons()
    {
        if (battleEnded) return;

        normalAttackButton.interactable = true;
        mediumSkillButton.interactable = true;
        specialSkillButton.interactable = true;
        defendButton.interactable = true;
        openInventoryButton.interactable = true;
    }
}