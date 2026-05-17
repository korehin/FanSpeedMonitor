# MSI Claw Fan PRM Monitor
Поддержка:
MSI Claw A1M (1st Gen)
MSI Claw A2VM 7
MSI Claw A2VM 8 AI+

Приложение для отображения текущей скорости вентиляторов Fan_1 и Fan_2. И передачи показаний в Performance Counters для дальнейшего отображения в RTSS.

## Требования

- ✅ Windows 10/11
- ✅ .NET 8.0 Runtime (обычно установлен по умолчанию)
- ✅ Visual Studio 2022 (для сборки)

## Как собрать EXE

### 1️⃣ Скачайте source 
```
Запустите build.bat
```

### 2️⃣ Найдите готовый EXE
```
1) Рядом с build.bat
2) Либо по пути: bin\x64\Release\net8.0-windows\win-x64\publish
```

## Основные функции

- 🟢 Отображение скорости вентиляторов в RPM
- 📊 Обновление каждые 1 секунду
- 🔗 Автоматическая передача данных в "Счетчик производительности Windows" (Performance Counters)
- 📡 Возможность добавления в автозапуск
- 📡 Возможность автозапуска свернутого в трей
## RivaTuner Statistics Server (RTSS)
- Setup
- Плагины -> OverlayEditor.dll
- Источники -> Редактировать
- Добавить -> Счетчик производительности Windows
- Найти MSI Claw -> Fan1 PRM и Fan2 PRM

## Если не работает
 Попробуйте запустить от Администратора.

### Ошибка при сборке в Visual Studio
1. Убедитесь, что установлен .NET 8.0 SDK
2. Выполните `Tools → Get Tools and Features → Individual Components` и установите:
   - .NET 8.0 Runtime
   - Windows Forms support for .NET

## Лицензия

MIT License
