namespace FanSpeedMonitor
{
    public partial class MainForm : Form
    {
        private Label labelFanSpeed = null!;
        private Label labelStatus = null!;
        private Timer refreshTimer = null!;
        private IntPtr deviceHandle = IntPtr.Zero;
        private IntPtr[]? fanHandles = null;
        private uint fanCount = 0;

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            InitializeIntelAPI();
        }

        private void SetupUI()
        {
            this.Text = "Fan Speed Monitor - MSI Claw A2VM 8 AI+";
            this.Width = 500;
            this.Height = 250;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Основная метка со скоростью вентилятора
            labelFanSpeed = new Label
            {
                Text = "Получение данных...",
                Font = new Font("Arial", 48, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 120,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                ForeColor = Color.LimeGreen
            };
            this.Controls.Add(labelFanSpeed);

            // Статус подключения
            labelStatus = new Label
            {
                Text = "Статус: Инициализация...",
                Font = new Font("Arial", 12),
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                ForeColor = Color.Yellow
            };
            this.Controls.Add(labelStatus);

            // Таймер для обновления
            refreshTimer = new Timer();
            refreshTimer.Interval = 500; // Обновление каждые 500мс
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            UpdateFanSpeed();
        }

        private void InitializeIntelAPI()
        {
            try
            {
                // Инициализируем драйвер
                ctl_result_t initResult = NativeAPI.ctlInit(IntPtr.Zero, IntPtr.Zero);
                if (initResult != ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    labelStatus.Text = $"Ошибка инициализации: {initResult}";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                // Получаем адаптеры
                uint adapterCount = 0;
                ctl_result_t enumResult = NativeAPI.ctlEnumDeviceAdapters(out adapterCount, null);

                if (enumResult != ctl_result_t.CTL_RESULT_SUCCESS || adapterCount == 0)
                {
                    labelStatus.Text = "GPU адаптеры не найдены";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                IntPtr[] adapters = new IntPtr[adapterCount];
                enumResult = NativeAPI.ctlEnumDeviceAdapters(out adapterCount, adapters);

                if (enumResult != ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    labelStatus.Text = "Ошибка получения адаптеров";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                deviceHandle = adapters[0];

                // Получаем вентиляторы
                ctl_result_t fanEnumResult = NativeAPI.ctlEnumFans(deviceHandle, out fanCount, null);

                if (fanEnumResult != ctl_result_t.CTL_RESULT_SUCCESS || fanCount == 0)
                {
                    labelStatus.Text = "Вентиляторы не найдены";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                fanHandles = new IntPtr[fanCount];
                fanEnumResult = NativeAPI.ctlEnumFans(deviceHandle, out fanCount, fanHandles);

                if (fanEnumResult != ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    labelStatus.Text = "Ошибка получения вентиляторов";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                labelStatus.Text = $"Статус: ? OK | Вентиляторов: {fanCount}";
                labelStatus.ForeColor = Color.LimeGreen;

                // Запускаем обновление
                refreshTimer.Start();
            }
            catch (DllNotFoundException ex)
            {
                labelStatus.Text = $"Ошибка: ControlLib.dll не найдена в System32";
                labelStatus.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Ошибка: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void UpdateFanSpeed()
        {
            try
            {
                if (fanHandles == null || fanHandles.Length == 0)
                    return;

                int fanSpeedRPM = 0;
                ctl_result_t result = NativeAPI.ctlFanGetState(
                    fanHandles[0],
                    ctl_fan_speed_units_t.CTL_FAN_SPEED_UNITS_RPM,
                    out fanSpeedRPM
                );

                if (result == ctl_result_t.CTL_RESULT_SUCCESS)
                {
                    labelFanSpeed.Text = $"{fanSpeedRPM} RPM";
                    labelStatus.ForeColor = Color.LimeGreen;
                }
                else
                {
                    labelFanSpeed.Text = "--";
                    labelStatus.Text = $"Ошибка чтения: {result}";
                    labelStatus.ForeColor = Color.Red;
                }
            }
            catch (DllNotFoundException)
            {
                labelFanSpeed.Text = "--";
                labelStatus.Text = "Ошибка: ControlLib.dll не найдена";
                labelStatus.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                labelFanSpeed.Text = "--";
                labelStatus.Text = $"Исключение: {ex.Message}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
            try
            {
                NativeAPI.ctlClose();
            }
            catch { }
            base.OnFormClosing(e);
        }
    }
}
