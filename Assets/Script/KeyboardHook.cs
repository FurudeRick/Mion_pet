using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Assets.Script
{
    class KeyboardHook
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hmod, uint dwThreadID);

        // 卸载钩子 
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        // 获取模块句柄
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(String modulename);

        public const int WM_KEYDOWN = 0x0100;
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_SYSKEYDOWN = 0x0104;
        protected const int WM_KEYUP = 0x101;
        protected const int WM_SYSKEYUP = 0x105;
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public delegate int HookHandlerDelegate(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam);
        //钩子回掉委托实例
        private static HookHandlerDelegate proc;
        //钩子句柄
        private static IntPtr hKeyboardHook;

        static bool isdown = false;

        //钩子要实现的功能
        private static int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            if (nCode >= 0 && (wparam == (IntPtr)WM_KEYDOWN || wparam == (IntPtr)WM_SYSKEYDOWN))
            {
                if (lparam.vkCode == 162)
                {
                    isdown = true;
                }

            }
            else if (nCode >= 0 && (wparam == (IntPtr)WM_KEYUP || wparam == (IntPtr)WM_SYSKEYUP))
            {
                if (lparam.vkCode == 162)
                {
                    isdown = false;
                }
                if (isdown == true && lparam.vkCode == 72)
                {
                    showWindows();
                }
            }
            return 0;
        }
        public static void HookStart()
        {
            if (hKeyboardHook == IntPtr.Zero)
            {
                // 创建HookProc实例 
                proc = new HookHandlerDelegate(HookCallback);
                using (Process curPro = Process.GetCurrentProcess())
                using (ProcessModule curMod = curPro.MainModule)
                {
                    //定义全局钩子 
                    hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curMod.FileName), 0);
                }

                if (hKeyboardHook == IntPtr.Zero)
                {
                    HookStop();
                    throw new Exception("钩子设置失败");
                }
            }

        }

        public static void HookStop()
        {
            bool retKeyboard = true;
            if (hKeyboardHook != IntPtr.Zero)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = IntPtr.Zero;
            }
            if (!(retKeyboard)) throw new Exception("卸载钩子失败");

        }
        public static void showWindows()
        {
            Interface form1 = new Interface();
            form1.Show();
        }
    }
}
