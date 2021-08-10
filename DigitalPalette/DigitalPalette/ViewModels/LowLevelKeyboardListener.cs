using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DigitalPalette.ViewModels
{
    public class LowLevelKeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _Mouseproc;
        private LowLevelKeyboardProc _Keyboardproc;

        private IntPtr _MousehookID = IntPtr.Zero;
        private IntPtr _KeyboardhookID = IntPtr.Zero;

        public delegate void DoEventHandler();
        public event DoEventHandler doEvent;

        public LowLevelKeyboardListener()
        {
            _Keyboardproc = HookCallback;
            _Mouseproc = HookCallback1;
        }

        public void HookKeyboard()
        {
            _KeyboardhookID = SetHook(_Keyboardproc, 0);
            _MousehookID = SetHook(_Mouseproc, 1);
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_KeyboardhookID);
            UnhookWindowsHookEx(_MousehookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc, int i)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                if (i == 0)
                {
                    // 키보드 후킹
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
                else
                {
                    // 마우스 후킹
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        bool ctrlPressed = false;
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            #region [키보드 후킹]
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode == 162 || vkCode == 163) //162 is Left Ctrl, 163 is Right Ctrl
                {
                    ctrlPressed = true;
                }
                else
                {
                    ctrlPressed = false;
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == 162 || vkCode == 163)
                {
                    ctrlPressed = false;
                }
            }
            #endregion
            return CallNextHookEx(_KeyboardhookID, nCode, wParam, lParam);
        }

        private IntPtr HookCallback1(int nCode, IntPtr wParam, IntPtr lParam)
        {
            #region [마우스 후킹]
            if (wParam == (IntPtr)WM_LBUTTONDOWN && ctrlPressed)
            {
                if(doEvent != null)
                {
                    doEvent();
                    return (IntPtr)1;
                }
            }
            #endregion
            return CallNextHookEx(_MousehookID, nCode, wParam, lParam);
        }

    }

}
