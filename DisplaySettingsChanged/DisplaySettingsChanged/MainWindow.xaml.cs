using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace DisplaySettingsChanged
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 변수 정의
        public static bool tttttt = false;

        public const int ENUM_CURRENT_SETTINGS = -1;

        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_BADMODE = -2;

        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;

        public static DEVMODE originalMode;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            // 100% = 96, +25% = +24
            Console.WriteLine("{0}, {1}", dpiX, dpiY);

            // ChangeDisplaySettings(1920, 1080, 32);
        }

        #region 00. 
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            [MarshalAs(UnmanagedType.I4)]
            public int x;
            [MarshalAs(UnmanagedType.I4)]
            public int y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct DEVMODE
        {
            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            // In addition you can define the last character array as following:
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public Char[] dmDeviceName;

            // After the 32-bytes array
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            // Also can be defined as
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            //public Byte[] dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;
        }

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings(
            [param: MarshalAs(UnmanagedType.LPTStr)]
            string lpszDeviceName,
            [param: MarshalAs(UnmanagedType.U4)]
            int iModeNum,
            [In, Out]
            ref DEVMODE lpDevMode
        );

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings(
            [In, Out]
            ref DEVMODE lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwflags
        );
        #endregion

        public static void GetCurrentSettings()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            
            if(EnumDisplaySettings(null, -1, ref mode) == true)
            {
                Console.WriteLine("Device Name:{0}\n이거 나오냐? : {1}", mode.dmDeviceName, mode.dmLogPixels);
                Console.WriteLine("Current Mode:\n\t" +
                    "{0} by {1}, " +
                    "{2} bit, " +
                    "{3} degrees, " +
                    "{4} hertz",
                    mode.dmPelsWidth,
                    mode.dmPelsHeight,
                    mode.dmBitsPerPel,
                    mode.dmDisplayOrientation * 90,
                    mode.dmDisplayFrequency);
            }
        }
        public static void EnumerateSupportedModes()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            int modeIndex = 0; // 0 = The first mode

            Console.WriteLine("Supported Modes:");

            while (EnumDisplaySettings(null,
                modeIndex,
                ref mode) == true) // Mode found
            {
                Console.WriteLine("\t" +
                    "{0} by {1}, " +
                    "{2} bit, " +
                    "{3} degrees, " +
                    "{4} hertz",
                    mode.dmPelsWidth,
                    mode.dmPelsHeight,
                    mode.dmBitsPerPel,
                    mode.dmDisplayOrientation * 90,
                    mode.dmDisplayFrequency);

                modeIndex++; // The next mode
            }
        }
        public static void ChangeDisplaySettings(int width, int height, int bitCount)
        {
            originalMode = new DEVMODE();
            originalMode.dmSize = (ushort)Marshal.SizeOf(originalMode);

            // Retrieving current settings to edit them
            EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS,ref originalMode);

            // Making a copy of the current settings to allow reseting to the original mode
            DEVMODE newMode = originalMode;

            // Changing the settings
            newMode.dmPelsWidth = (uint)width;
            newMode.dmPelsHeight = (uint)height;
            newMode.dmBitsPerPel = (uint)bitCount;

            // Capturing the operation result
            int result = ChangeDisplaySettings(ref newMode, 0);

            if (result == DISP_CHANGE_SUCCESSFUL)
            {
                tttttt = true;
                Console.WriteLine("Succeeded.\n");

                // Inspecting the new mode
                GetCurrentSettings();

                Console.WriteLine();

                // Waiting for seeing the results
                // Console.ReadKey(true);

                // ChangeDisplaySettings(ref originalMode, 0);
            }
            else if (result == DISP_CHANGE_BADMODE)
                Console.WriteLine("Mode not supported.");
            else if (result == DISP_CHANGE_RESTART)
                Console.WriteLine("Restart required.");
            else
                Console.WriteLine("Failed. Error code = {0}", result);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (tttttt)
                ChangeDisplaySettings(ref originalMode, 0);
        }


    }
}
