using UnityEngine;
using UnityEngine.UI;  // Для работы с UI элементами

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public Dropdown graphicsQualityDropdown; // Выпадающий список для графики
    public Slider volumeSlider; // Слайдер для звука
    public GameObject mainMenuPanel; // Панель главного меню
    public GameObject optionsPanel;  // Панель настроек
    public Button backButton;  // Кнопка "Назад"

    private void Start()
    {
        // Инициализация значений
        InitializeSettings();

        // Привязка кнопки "Назад" к функции
        backButton.onClick.AddListener(BackToMainMenu);
    }

    // Метод для изменения качества графики
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Метод для изменения громкости
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;  // Устанавливаем глобальную громкость
    }

    // Инициализация настроек по умолчанию
    private void InitializeSettings()
    {
        // Устанавливаем начальные значения для графики и громкости
        graphicsQualityDropdown.value = 2;  // Среднее качество по умолчанию
        volumeSlider.value = 1f;  // Максимальная громкость по умолчанию

        // Применяем настройки
        SetGraphicsQuality(graphicsQualityDropdown.value);
        SetVolume(volumeSlider.value);
    }

    // Метод для возврата в главное меню
    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);  // Скрыть панель настроек
        mainMenuPanel.SetActive(true);  // Показать главное меню
    }
}
