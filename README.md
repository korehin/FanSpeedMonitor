# Fan Speed Monitor для MSI Claw A2VM 8 AI+

Приложение для отображения текущей скорости вентилятора видеокарты Intel Arc Graphics 140V.

## Требования

- ✅ Windows 10/11
- ✅ .NET 6.0 Runtime (обычно установлен по умолчанию)
- ✅ Visual Studio 2022 (для сборки)
- ✅ Последний драйвер Intel GPU
- ✅ ControlLib.dll в `C:\Windows\System32\` (идет с драйвером)

## Как собрать EXE

### 1️⃣ Откройте проект в Visual Studio 2022
```
File → Open Project or Solution → выберите папку этого проекта
```

### 2️⃣ Соберите проект
```
Build → Build Solution (Ctrl+Shift+B)
```

### 3️⃣ Найдите готовый EXE
```
bin\Release\net6.0-windows\FanSpeedMonitor.exe
```

## Как запустить через командную строку

```bash
git clone https://github.com/korehin/FanSpeedMonitor.git
cd FanSpeedMonitor
dotnet build --configuration Release
```

Готовый exe будет в: `bin\Release\net6.0-windows\FanSpeedMonitor.exe`

## Основные функции

- 🟢 Отображение скорости вентилятора в RPM крупными зеленными цифрами
- 📊 Обновление каждые 500мс
- 🔗 Автоматическое подключение к Intel GPU
- 📡 Индикатор статуса подключения
- 🎨 Темный интерфейс для удобства

## Если не работает

### Ошибка: "ControlLib.dll не найдена"
1. Проверьте наличие ControlLib.dll в `C:\Windows\System32\`
2. Переустановите последний драйвер Intel GPU

### Ошибка: "GPU адаптеры не найдены"
1. Проверьте, что GPU активен в BIOS
2. Проверьте в Device Manager, что GPU видна
3. Обновите драйвер

### Ошибка при сборке в Visual Studio
1. Убедитесь, что установлен .NET 6.0 SDK
2. Выполните `Tools → Get Tools and Features → Individual Components` и установите:
   - .NET 6.0 Runtime
   - Windows Forms support for .NET

## Лицензия

MIT License

## Благодарности

- Intel GPU Control Library: https://github.com/intel/drivers.gpu.control-library
- ToothNClaw: https://github.com/BassemMohsen/ToothNClaw
- HandheldCompanion: https://github.com/Valkirie/HandheldCompanion
