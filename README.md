# MSI Claw Fan PRM Monitor
Поддержка:
- MSI Claw A1M (1st Gen)
- MSI Claw A2VM 7
- MSI Claw A2VM 8 AI+

Приложение для отображения текущей скорости вентиляторов Fan_1 и Fan_2.
Передача показаний в "Счетчик производительности Windows" (Windows Performance Counters) для дальнейшего отображения в RTSS.
## Скачать

https://github.com/korehin/MSI_Claw_Fan_PRM/releases/download/release/MSI_Claw_Fan_PRM.zip

## Требования

- ✅ Windows 10/11
- ✅ .NET 8.0 Runtime (обычно установлен по умолчанию)
- ✅ Visual Studio 2022 (для сборки)

## Как собрать EXE

### 1️⃣ Запустите build.bat

### 2️⃣ Найдите готовый EXE
```
1) Рядом с build.bat
2) Либо по пути: bin\x64\Release\net8.0-windows\win-x64\publish
```

## Основные функции

- 🟢 Отображение скорости вентиляторов в RPM
- 📊 Обновление каждую 1 секунду
- 🔗 Автоматическая передача данных в "Счетчик производительности Windows" (Windows Performance Counters)
- 📡 Возможность добавления в автозапуск Windows
- 📡 Возможность сворачивания в трей при запуске
- 
## Настройка RivaTuner Statistics Server (RTSS)
- Кнопка Setup
- Плагины -> OverlayEditor.dll
- Источники -> Редактировать
- Добавить -> Счетчик производительности Windows
- Найти в списке MSI Claw -> Fan1 PRM и Fan2 PRM

======================================
УСТАНОВКА И НАСТРОЙКА ГОТОВОГО РЕШЕНИЯ
======================================

## МОЖНО ВЗЯТЬ ГОТОВЫЙ OVERLAY для RTSS

Claw8ai_full_NPU_v1.1.ovl

 ## Настройка HWinfo
- Установить hwi64_844.exe
- В настройках Hwinfo поставить галки:
Показывать датчики при запуске
Поддержка совместно используемой памяти

## Если не работает
 Попробуйте запустить от Администратора.

### Ошибка при сборке
1. Убедитесь, что установлен .NET 8.0 SDK
2. Выполните `Tools → Get Tools and Features → Individual Components` и установите:
   - .NET 8.0 Runtime
   - Windows Forms support for .NET

## Лицензия

MIT License
