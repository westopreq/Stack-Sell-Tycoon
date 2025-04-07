using UnityEngine;
using TMPro;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance;

    [Header("UI элементы")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText;

    [Header("Начальные значения")]
    public int money = 0;

    private int experience = 0;
    private int level = 1;
    private int expToNextLevel = 100;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        if (money < 0) money = 0;
        UpdateUI();
    }

    public bool HasEnoughMoney(int amount)
    {
        return money >= amount; // Проверка, есть ли достаточно денег для покупки
    }

    public void AddExperience(int amount)
    {
        experience += amount;

        while (experience >= expToNextLevel)
        {
            experience -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        expToNextLevel = Mathf.CeilToInt(expToNextLevel * 1.1f);
        Debug.Log($"Уровень повышен до {level}! Следующий лвл через {expToNextLevel} XP");
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"$ {money}";

        if (experienceText != null)
            experienceText.text = $"{experience} / {expToNextLevel} EXP";

        if (levelText != null)
            levelText.text = $"Level {level}";
    }
}
