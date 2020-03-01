namespace QM
{
    /// <summary>
    /// Provides methods to prevent system from entering sleep mode.
    /// </summary>
    /// source=https://stackoverflow.com/questions/24869508/moving-mouse-to-stop-monitor-from-sleeping
    public static class PowerHelper
    {
        public static void ForceSystemAwake()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                                  NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                                  NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                                                  NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void ResetSystemDefault()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
        }
    }
}
