using System.Runtime.InteropServices;

namespace FanSpeedMonitor
{
    public enum ctl_result_t : uint
    {
        CTL_RESULT_SUCCESS = 0,
        CTL_RESULT_ERROR_NOT_INITIALIZED = 1,
        CTL_RESULT_ERROR_INVALID_NULL_HANDLE = 2,
        CTL_RESULT_ERROR_INVALID_NULL_POINTER = 3,
        CTL_RESULT_ERROR_DEVICE_LOST = 4,
        CTL_RESULT_ERROR_UNSUPPORTED_FEATURE = 5,
    }

    public enum ctl_fan_speed_units_t : uint
    {
        CTL_FAN_SPEED_UNITS_RPM = 0,
        CTL_FAN_SPEED_UNITS_PERCENT = 1
    }

    public static class NativeAPI
    {
        private const string DLL_NAME = "ControlLib.dll";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ctl_result_t ctlInit(IntPtr loaderHandle, IntPtr pInitDesc);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ctl_result_t ctlClose();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ctl_result_t ctlEnumDeviceAdapters(out uint adapterCount, IntPtr[]? adapters);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ctl_result_t ctlEnumFans(IntPtr hDevice, out uint fanCount, IntPtr[]? fanHandles);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern ctl_result_t ctlFanGetState(IntPtr hFan, ctl_fan_speed_units_t units, out int pSpeed);
    }
}
