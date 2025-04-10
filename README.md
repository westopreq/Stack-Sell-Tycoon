# Stack & Sell Tycoon - Тестовое задание

Этот репозиторий содержит код для прототипа 3D idle tycoon игры под названием **Stack & Sell Tycoon**, разработанной с использованием **Unity** (версия 2022.3.37f1). Игра фокусируется на управлении супермаркетом, где игроки могут строить и улучшать различные комнаты, управлять товарами, сотрудниками и финансами.

## Обзор проекта

В **Stack & Sell Tycoon** игроки могут:
- Строить комнаты в своем супермаркете и расширять пространство.
- Управлять товарами, включая хранение, выставление и продажу.
- Отслеживать финансы, включая доходы, расходы и баланс.
- Проводить маркетинговые кампании для привлечения клиентов.
- Улучшать и расширять супермаркет, добавляя этажи, секции и улучшая товары.
- Управлять магазином и продажами товаров NPC.

## Реализованные функции

- **Система строительства комнат**:
  - Автоматически определяет доступные зоны для строительства.
  - Возможность размещать новые комнаты, расширяясь от начальной.
  - Комнаты отображаются с превью до их размещения.

- **Система товаров**:
  - Поддержка хранения и выставления товаров на полках.
  - Товары могут быть перемещены между инвентарями с помощью перетаскивания или дабл-клика.

- **Инвентарь**:
  - Инвентарь реализован в стиле игр **Heroes of Might and Magic**, **Thief** и **SCUM**.
  - При перетаскивании карточки товара из инвентаря и отпускании её вне инвентаря, она превращается в предмет в игровом мире.
  - Перенос предметов, их ротация и возможность добавления реализованы в отдельных скриптах для простоты интеграции.

- **Магазин**:
  - В игре реализована система магазина, где NPC может покупать товары.
  - При покупке NPC платит по умолчанию на **+25%** от стоимости товара с округлением до целого числа.

- **Система инвентаря**:
  - Поддержка различных типов инвентарей (например, полки, склад).
  - Визуальное отображение товаров в интерфейсе инвентаря.
  
- **Основной UI**:
  - В игровом интерфейсе есть кнопки для приостановки игры, управления инвентарем и открытия различных панелей.

- **Сохранение и загрузка**:
  - Система сохранения и загрузки данных игры, включая состояние объектов, инвентарь и планировку комнат.
  - Без перезагрузки сцен при сохранении или загрузке для сохранения плавности игрового процесса.
  - **Сохранение находится на стадии бета-версии и требует доработки.**

## Как начать

### Требования

- Unity 2022.3.37f1 или более поздняя версия
- Базовые знания Unity и C#

### Как запустить

1. Клонируйте этот репозиторий на свой компьютер.
2. Откройте проект в Unity.
3. Нажмите `Play` для запуска игры в редакторе Unity.

### Как играть

1. Откройте панель магазина, где можно приобрести товары.
2. Для покупки товара откройте панель игрока через интерфейс. Дважды щелкните по полке, чтобы выбрать её инвентарь.
3. Перетащите товар из инвентаря игрока в инвентарь полки.
4. ИИ NPC подходит к кассе и оплачивает товар, внося **+25%** от стоимости товара (с округлением до целого числа).

### Управление

- **Дважды щелкните по предмету**, чтобы перенести его в инвентарь.
- **Правая кнопка мыши (Mouse 2)**: Сделать инвентарь выбранного объекта активным.
- **Левая кнопка мыши (Mouse 1)**: Удерживайте для перетаскивания предметов.
- **Q, E**: Ротация предмета при удержании предмета.

### Будущие улучшения

- Возможность добавления стоимости за расширение секций и строительства, как отдельный режим (аналогично играм серии **Sims** или **Inzoi**).
- Получение бонусов с повышением уровня, таких как открытие новых комнат, доступ к этажам, новые товары, полки, а также добавление развлекательных зон (например, качалок).
- Возможность добавить воровство и правоохранительную систему для создания дополнительных вызовов в игре.
- Реализация режима от первого лица для NPC.
- Улучшение ИИ для более сложного поведения клиентов и сотрудников.

**Примечание:** Этот проект является тестовым заданием, и реализация многих функций находится на начальной стадии. Хотя есть много идей для дальнейшего улучшения, я сосредоточился на том, чтобы реализовать основные механики, которых не было в примерах, отправленных рекрутеру.

## Лицензия

Лицензия на использование этого проекта указана в отдельном файле LICENSE.
