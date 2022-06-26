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

        const int VK_LSHIFT = 160;
        const int VK_LCONTROL = 162;
        const int VK_LMENU = 164;
        const int key0 = 48;
        const int key1 = 49;
        const int key2 = 50;
        const int key3 = 51;
        const int key4 = 52;
        const int key5 = 53;
        const int key6 = 54;
        const int key7 = 55;
        const int key8 = 56;
        const int key9 = 57;
        const int keyA = 65;
        const int keyB = 66;
        const int keyC = 67;
        const int keyD = 68;
        const int keyE = 69;
        const int keyF = 70;
        const int keyG = 71;
        const int keyH = 72;
        const int keyI = 73;
        const int keyJ = 74;
        const int keyK = 75;
        const int keyL = 76;
        const int keyM = 77;
        const int keyN = 78;
        const int keyO = 79;
        const int keyP = 80;
        const int keyQ = 81;
        const int keyR = 82;
        const int keyS = 83;
        const int keyT = 84;
        const int keyU = 85;
        const int keyV = 86;
        const int keyW = 87;
        const int keyX = 88;
        const int keyY = 89;
        const int keyZ = 90;
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

        //钩子绑定
        private static int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            //keyCombination_2(nCode, wparam, ref lparam, showWindows.showWindowInterface, VK("ctrl"), VK("h"));
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

        //参数Fun为要实现的函数
        public static void keyCombination_2(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam, Action Fun,int key1,int key2)
        {

            if (nCode >= 0 && (wparam == (IntPtr)WM_KEYDOWN || wparam == (IntPtr)WM_SYSKEYDOWN))
            {
                if (lparam.vkCode == key1)
                {
                    isdown = true;
                }

            }
            else if (nCode >= 0 && (wparam == (IntPtr)WM_KEYUP || wparam == (IntPtr)WM_SYSKEYUP))
            {
                if (lparam.vkCode == key1)
                {
                    isdown = false;
                }
                if (isdown == true && lparam.vkCode == key2)
                {
                    //要实现的功能 Fun()
                }
            }
        }

        //获取配置文件的key值
        public int[] getKey(string key)
        {
            int[] key_map = new int[2];
            string[] key1_2 = key.Split('+');
            key_map[0] = VK(key1_2[0]);
            key_map[1] = VK(key1_2[1]);
            return key_map;
        }

        private static int VK(string key)
        {
            int key_map=0;
            switch(key)
            {
                case "ctrl":
                    key_map = VK_LCONTROL;
                    break;
                case "alt":
                    key_map = VK_LMENU;
                    break;
                case "shift":
                    key_map = VK_LSHIFT;
                    break;
                case "0":
                    key_map = key0;
                    break;
                case "1":
                    key_map = key1;
                    break;
                case "2":
                    key_map = key2;
                    break;
                case "3":
                    key_map = key3;
                    break;
                case "4":
                    key_map = key4;
                    break;
                case "5":
                    key_map = key5;
                    break;
                case "6":
                    key_map = key6;
                    break;
                case "7":
                    key_map = key7;
                    break;
                case "8":
                    key_map = key8;
                    break;
                case "9":
                    key_map = key9;
                    break;
                case "a":
                    key_map = keyA;
                    break;
                case "b":
                    key_map = keyB;
                    break;
                case "c":
                    key_map = keyC;
                    break;
                case "d":
                    key_map = keyD;
                    break;
                case "e":
                    key_map = keyE;
                    break;
                case "f":
                    key_map = keyF;
                    break;
                case "g":
                    key_map = keyG;
                    break;
                case "h":
                    key_map = keyH;
                    break;
                case "i":
                    key_map = keyI;
                    break;
                case "j":
                    key_map = keyJ;
                    break;
                case "k":
                    key_map = keyK;
                    break;
                case "l":
                    key_map = keyL;
                    break;
                case "m":
                    key_map = keyM;
                    break;
                case "n":
                    key_map = keyN;
                    break;
                case "o":
                    key_map = keyO;
                    break;
                case "p":
                    key_map = keyP;
                    break;
                case "q":
                    key_map = keyQ;
                    break;
                case "r":
                    key_map = keyR;
                    break;
                case "s":
                    key_map = keyS;
                    break;
                case "t":
                    key_map = keyT;
                    break;
                case "u":
                    key_map = keyU;
                    break;
                case "v":
                    key_map = keyV;
                    break;
                case "w":
                    key_map = keyW;
                    break;
                case "x":
                    key_map = keyX;
                    break;
                case "y":
                    key_map = keyY;
                    break;
                case "z":
                    key_map = keyZ;
                    break;

                default:
                    key_map = 0;
                    break;
            }
            return key_map;
        }
    }
}
