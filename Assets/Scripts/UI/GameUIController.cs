using UnityEngine;
using UnityEngine.UI;
using TMPro; // Для TextMeshProUGUI
using UnityEngine.SceneManagement; // Для загрузки сцен

public class GameUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject otherInventoryPanel;
    public GameObject anotherPanel;
    public GameObject shopPanel; // Ссылка на панель магазина

    [Header("Buttons")]
    public Button exitToMenuButton;
    public Button pauseButton;
    public Button toggleOtherInventoryButton;
    public Button toggleAnotherPanelButton;
    public Button openShopButton;
    public Button saveButton; // Кнопка для сохранения

    [Header("Pause Button Text")]
    public TextMeshProUGUI pauseButtonText;

    private bool isPaused = false;

    private void Start()
    {
        // Привязка событий к кнопкам
        exitToMenuButton.onClick.AddListener(ExitToMenu);
        pauseButton.onClick.AddListener(TogglePause);
        toggleOtherInventoryButton.onClick.AddListener(ToggleOtherInventoryPanel);
        toggleAnotherPanelButton.onClick.AddListener(ToggleAnotherPanel);
        openShopButton.onClick.AddListener(OpenShop); // Привязываем метод открытия/закрытия магазина
        saveButton.onClick.AddListener(SaveGame); // Привязка метода сохранения
    }

    private void ExitToMenu()
    {
        Debug.Log("Выйти в меню");

        // Здесь загружаем сцену с главным меню
        SceneManager.LoadScene("MainMenu");
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        if (pauseButtonText != null)
            pauseButtonText.text = isPaused ? "Play" : "Pause";
    }

    private void ToggleOtherInventoryPanel()
    {
        if (otherInventoryPanel != null)
            otherInventoryPanel.SetActive(!otherInventoryPanel.activeSelf);
    }

    private void ToggleAnotherPanel()
    {
        if (anotherPanel != null)
            anotherPanel.SetActive(!anotherPanel.activeSelf);
    }

    private void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(!shopPanel.activeSelf); // Переключаем видимость панели магазина
        }
    }

    private void SaveGame()
    {
        // Логика для сохранения игры
        Debug.Log("Игра сохранена");
        
        // Пример сохранения, если у вас есть система сохранений:
        // SaveSystem.SaveGameData();
    }
}
