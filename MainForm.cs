using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;

namespace MSI_Claw_Fan_PRM
{
    public class MainForm : Form
    {
        private readonly Label lblValue;
        private readonly CheckBox chkAutostart;
        private readonly CheckBox chkStartMinimized;
        private readonly System.Windows.Forms.Timer timer;

        private PerformanceCounter? perfCounter1;
        private PerformanceCounter? perfCounter2;

        private readonly Icon _shieldIcon;

        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip trayMenu;

        private volatile bool reallyClose = false;

        private const string AppName     = "MSI_Claw_Fan_PRM";
        private const string RegAutostart = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string RegSettings  = @"SOFTWARE\MSI_Claw_Fan_PRM";

        private static bool IsRussian()
        {
            return CultureInfo.CurrentUICulture
                .TwoLetterISOLanguageName
                .Equals("ru", StringComparison.OrdinalIgnoreCase);
        }

        private bool GetAutostart()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    RegAutostart, false);

                return key?.GetValue(AppName) != null;
            }
            catch
            {
                return false;
            }
        }

        private void SetAutostart(bool enable)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    RegAutostart, true);

                if (key == null)
                    return;

                if (enable)
                {
                    string exePath =
                        Process.GetCurrentProcess()
                            .MainModule?
                            .FileName ?? string.Empty;

                    if (!string.IsNullOrEmpty(exePath))
                        key.SetValue(AppName, $"\"{exePath}\"");
                }
                else
                {
                    // Используем true для throwOnMissingValue - не выбрасывать исключение
                    if (key.GetValue(AppName) != null)
                        key.DeleteValue(AppName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при изменении автозапуска: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool GetStartMinimized()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    RegSettings, false);

                return key?.GetValue("StartMinimized") is int v && v == 1;
            }
            catch
            {
                return false;
            }
        }

        private void SetStartMinimized(bool enable)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.CreateSubKey(
                    RegSettings, true);

                key?.SetValue(
                    "StartMinimized",
                    enable ? 1 : 0,
                    RegistryValueKind.DWord);
            }
            catch
            {
                // Игнорируем ошибки
            }
        }

        public MainForm()
        {
            Text = "MSI Claw Fan PRM";

            Width = 500;
            Height = 240;

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;

            WindowState =
                FormWindowState.Normal;

            StartPosition =
                FormStartPosition.CenterScreen;

            // Копируем ресурс в MemoryStream, чтобы иконка не зависела
            // от времени жизни исходного потока манифеста
            var rawStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("MSI_Claw_Fan_PRM.shield.ico");

            if (rawStream == null)
                throw new InvalidOperationException("Icon resource not found");

            var memStream = new System.IO.MemoryStream();
            rawStream.CopyTo(memStream);
            rawStream.Dispose();
            memStream.Position = 0;

            _shieldIcon = new Icon(memStream);
            this.Icon = _shieldIcon;

            lblValue = new Label
            {
                Left = 20,
                Top = 20,
                Width = 440,
                Height = 100,
                AutoSize = false,
                Font = new Font("Consolas", 22)
            };

            Controls.Add(lblValue);

            bool ru = IsRussian();

            chkAutostart = new CheckBox
            {
                Left = 20,
                Top = 130,
                Width = 440,
                Height = 24,
                Text = ru
                    ? "Автозапуск вместе с Windows"
                    : "Autorun with Windows",
                Font = new Font("Segoe UI", 10),
                Checked = GetAutostart()
            };

            chkAutostart.CheckedChanged +=
                (s, e) => SetAutostart(chkAutostart.Checked);

            Controls.Add(chkAutostart);

            chkStartMinimized = new CheckBox
            {
                Left = 20,
                Top = 160,
                Width = 440,
                Height = 24,
                Text = ru
                    ? "Запускать свёрнутым в трей"
                    : "Minimize to tray",
                Font = new Font("Segoe UI", 10),
                Checked = GetStartMinimized()
            };

            chkStartMinimized.CheckedChanged +=
                (s, e) => SetStartMinimized(chkStartMinimized.Checked);

            Controls.Add(chkStartMinimized);

            CreateCategory();

            try
            {
                perfCounter1 =
                    new PerformanceCounter(
                        "MSI Claw",
                        "Fan1 RPM",
                        false);

                perfCounter2 =
                    new PerformanceCounter(
                        "MSI Claw",
                        "Fan2 RPM",
                        false);
            }
            catch
            {
                // Если не удалось создать счётчики, продолжаем работу без них
                perfCounter1 = null;
                perfCounter2 = null;
            }

            timer =
                new System.Windows.Forms.Timer();

            timer.Interval = 1000;

            timer.Tick += Timer_Tick;

            trayMenu =
                new ContextMenuStrip();

            trayMenu.Items.Add(
                ru ? "Выход" : "Exit",
                null,
                ExitClick);

            trayIcon =
                new NotifyIcon();

            trayIcon.Icon = _shieldIcon;

            trayIcon.Text =
                "MSI Fan Monitor";

            trayIcon.Visible = true;

            trayIcon.ContextMenuStrip =
                trayMenu;

            trayIcon.DoubleClick +=
                TrayDoubleClick;

            Resize += MainForm_Resize;

            FormClosing +=
                MainForm_FormClosing;

            Shown += MainForm_Shown;
        }

        private void MainForm_Shown(
            object? sender,
            EventArgs e)
        {
            timer.Start();

            if (GetStartMinimized())
            {
                Hide();

                WindowState =
                    FormWindowState.Minimized;
            }
        }

        private void MainForm_Resize(
            object? sender,
            EventArgs e)
        {
            if (WindowState ==
               FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void MainForm_FormClosing(
            object? sender,
            FormClosingEventArgs e)
        {
            if (!reallyClose)
            {
                e.Cancel = true;

                Hide();

                WindowState =
                    FormWindowState.Minimized;
            }
        }

        private void TrayDoubleClick(
            object? sender,
            EventArgs e)
        {
            Show();

            WindowState =
                FormWindowState.Normal;

            Activate();
        }

        private void ExitClick(
            object? sender,
            EventArgs e)
        {
            reallyClose = true;

            trayIcon.Visible = false;

            Application.Exit();
        }

        private void CreateCategory()
        {
            try
            {
                string[] required = { "Fan1 RPM", "Fan2 RPM" };

                bool needRecreate = false;

                if (PerformanceCounterCategory.Exists("MSI Claw"))
                {
                    var category =
                        new PerformanceCounterCategory("MSI Claw");

                    var existing = category.GetCounters();

                    var existingNames =
                        new System.Collections.Generic.HashSet<string>();

                    foreach (var c in existing)
                        existingNames.Add(c.CounterName);

                    foreach (var name in required)
                    {
                        if (!existingNames.Contains(name))
                        {
                            needRecreate = true;
                            break;
                        }
                    }

                    if (needRecreate)
                        PerformanceCounterCategory.Delete("MSI Claw");
                }

                if (!PerformanceCounterCategory.Exists("MSI Claw"))
                {
                    CounterCreationDataCollection counters = new();

                    foreach (var name in required)
                    {
                        counters.Add(
                            new CounterCreationData(
                                name,
                                $"MSI Claw {name}",
                                PerformanceCounterType.NumberOfItems32));
                    }

                    PerformanceCounterCategory.Create(
                        "MSI Claw",
                        "MSI Claw Sensors",
                        PerformanceCounterCategoryType.SingleInstance,
                        counters);
                }
            }
            catch { }
        }

        private ManagementBaseObject? GetEmb(
            ManagementBaseObject p,
            string prop)
        {
            return p[prop]
                as ManagementBaseObject;
        }

        private byte[] ReadBytes(
            ManagementBaseObject p)
        {
            object value = p["Bytes"];

            if (value is byte[] b)
                return b;

            return Array.Empty<byte>();
        }

        private void PutBytes(
            ManagementBaseObject p,
            byte[] pkg)
        {
            p["Bytes"] = pkg;
        }

        private byte[] WmiGetFan(
            ManagementScope scope,
            byte idx)
        {
            byte[] pkg = new byte[32];

            pkg[0] = idx;

            using ManagementObject pMO =
                new ManagementObject(
                    scope,
                    new ManagementPath(
                    "MSI_ACPI.InstanceName='ACPI\\PNP0C14\\0_0'"),
                    null);

            ManagementBaseObject? pIn =
                pMO.GetMethodParameters(
                "Get_Fan");

            ManagementBaseObject? pData =
                GetEmb(
                pIn,
                "Data");

            bool ok = pData != null;

            if (!ok)
            {
                ManagementBaseObject? pWO =
                    pMO.InvokeMethod(
                    "Get_WMI",
                    null,
                    null);

                pIn = pWO;

                pData =
                    GetEmb(
                    pIn,
                    "Data");

                ok = pData != null;
            }

            if (!ok)
                return Array.Empty<byte>();

            if (pData == null)
                return Array.Empty<byte>();

            PutBytes(
                pData,
                pkg);

            pIn["Data"] = pData;

            ManagementBaseObject? pOut =
                pMO.InvokeMethod(
                    "Get_Fan",
                    pIn,
                    null);

            if (pOut == null)
                return Array.Empty<byte>();

            ManagementBaseObject? pDO =
                GetEmb(
                pOut,
                "Data");

            if (pDO == null)
                return Array.Empty<byte>();

            return ReadBytes(pDO);
        }

        private (int, int) ReadFan()
        {
            try
            {
                ManagementScope scope =
                    new ManagementScope(
                    @"root\WMI");

                scope.Connect();

                byte[] bytes =
                    WmiGetFan(
                    scope,
                    0);

                int rpm1 = 0;
                int rpm2 = 0;

                if (bytes.Length > 2)
                {
                    int tach1 =
                        bytes[2];

                    if (tach1 > 0)
                        rpm1 =
                            480000 /
                            tach1;
                }

                if (bytes.Length > 4)
                {
                    int tach2 =
                        bytes[4];

                    if (tach2 > 0)
                        rpm2 =
                            480000 /
                            tach2;
                }

                return (rpm1, rpm2);
            }
            catch
            {
                return (0, 0);
            }
        }

        private void Timer_Tick(
            object? sender,
            EventArgs e)
        {
            var fans =
                ReadFan();

            int fan1 =
                fans.Item1;

            int fan2 =
                fans.Item2;

            int fan1r = (int)(Math.Round(fan1 / 50.0) * 50);
            int fan2r = (int)(Math.Round(fan2 / 50.0) * 50);

            lblValue.Text =
                $"FAN1 RPM : {fan1r}" +
                Environment.NewLine +
                $"FAN2 RPM : {fan2r}";

            if (perfCounter1 != null)
                perfCounter1.RawValue = fan1r;

            if (perfCounter2 != null)
                perfCounter2.RawValue = fan2r;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Останавливаем таймер
                timer?.Stop();
                timer?.Dispose();

                // Освобождаем Performance Counters
                perfCounter1?.Dispose();
                perfCounter2?.Dispose();

                // Освобождаем иконку в трее
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }

                // Освобождаем меню
                trayMenu?.Dispose();

                // Освобождаем иконку
                _shieldIcon?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}