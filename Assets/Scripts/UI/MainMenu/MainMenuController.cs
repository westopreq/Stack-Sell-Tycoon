using UnityEngine;
using UnityEngine.SceneManagement; // Для работы с сценами
using UnityEngine.UI;  // Для работы с UI элементами

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel; // Панель главного меню
    public GameObject optionsPanel;  // Панель настроек
    public Button startButton; // Кнопка для старта новой игры
    public Button exitButton; // Кнопка для выхода из игры
    public Button optionsButton; // Кнопка для открытия настроек

    private void Start()
    {
        // Инициализация UI
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Привязываем методы к кнопкам
        startButton.onClick.AddListener(StartNewGame);
        exitButton.onClick.AddListener(ExitGame);
        optionsButton.onClick.AddListener(OpenOptionsMenu);
    }

    // Метод для старта новой игры
    public void StartNewGame()
    {
        Debug.Log("Новая игра началась");
        // Загрузка основной сцены игры (например, SampleScene)
        SceneManager.LoadScene("SampleScene");
    }

    // Метод для выхода из игры
    public void ExitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }

    // Метод для открытия меню настроек
    public void OpenOptionsMenu()
    {
        optionsPanel.SetActive(true);  // Показываем панель настроек
        mainMenuPanel.SetActive(false);  // Скрываем главное меню
    }

    // Метод для возврата в главное меню
    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);  // Скрываем панель настроек
        mainMenuPanel.SetActive(true);  // Показываем главное меню
    }
}
