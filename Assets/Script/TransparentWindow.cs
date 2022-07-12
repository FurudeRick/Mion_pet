using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

namespace Assets.Script
{
    public class TransparentWindow : MonoBehaviour
    {
        public GameObject dialogBox;
        public TextMeshProUGUI dialogBoxText;
        private bool isDialogBox = false;
        private bool isTop = false;
        [SerializeField] private Material m_Material;

        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Flags]
        public enum MenuFlags : uint
        {
            MF_STRING = 0,
            MF_POPUP = 0x10,
            MF_BYPOSITION = 0x400,
            MF_SEPARATOR = 0x800,
            MF_REMOVE = 0x1000,
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("Dwmapi.dll")]
        private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy,
            int uFlags);

        [DllImport("user32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref MARGINS lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool AppendMenu(IntPtr hMenu, MenuFlags uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenu(IntPtr hMenu, int wFlags, int x, int y, int nReserved, IntPtr hwnd, ref RECT lprc);


        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const uint WS_POPUP = 0x80000000;
        const uint WS_VISIBLE = 0x10000000;

        const uint WS_EX_TOPMOST = 0x00000008;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;

        const int SWP_FRAMECHANGED = 0x0020;
        const int SWP_SHOWWINDOW = 0x0040;
        const int SWP_HIDEWINDOW = 0x0080;
        const int SWP_NOSIZE = 1;
        const int SWP_NOMOVE = 2;
        const int LWA_ALPHA = 2;

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        const int TPM_CENTERALIGN = 0x0004;
        const int TPM_VCENTERALIGN = 0x0010;
        const int TPM_RETURNCMD = 0x0100;

        private IntPtr HWND_TOPMOST = new IntPtr(-1);
        private IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private IntPtr _hwnd;
        private IntPtr hmnuPopup;
        private IntPtr hFileMenu;
        private RECT rect;

        private int x = 0, y = 0;
        private int rx = 0, ry = 0;

        //创建多线程
        ThreadStart childref;
        Thread childThread;

        Encoding utf8 = Encoding.UTF8;
        Encoding utf16 = Encoding.Unicode;
        Encoding gb = Encoding.GetEncoding("gb2312");

        void Start()
        {
            UnityEngine.Debug.Log(System.Text.Encoding.Default);
#if !UNITY_EDITOR
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        _hwnd = GetActiveWindow();
        int fWidth = UnityEngine.Screen.width;
        int fHeight = UnityEngine.Screen.height;
        SetWindowLong(_hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        //SetWindowLong(_hwnd, GWL_EXSTYLE, WS_EX_TOPMOST | WS_EX_LAYERED | WS_EX_TRANSPARENT);//若想鼠标穿透，则将这个注释恢复即可
        DwmExtendFrameIntoClientArea(_hwnd, ref margins);
        SetWindowPos(_hwnd, HWND_TOPMOST, 0, 0, 200, 200, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
        ShowWindowAsync(_hwnd, 5); //Forces window to show in case of unresponsive app    // SW_SHOWMAXIMIZED(3)
        
#endif
            // 增加pop菜单
            hmnuPopup = CreatePopupMenu();
            AppendMenu(hmnuPopup, MenuFlags.MF_STRING, 1001, "窗口");
            AppendMenu(hmnuPopup, MenuFlags.MF_SEPARATOR, 0, "");
            AppendMenu(hmnuPopup, MenuFlags.MF_STRING, 1002, "隐藏/显示对话框");
            AppendMenu(hmnuPopup, MenuFlags.MF_SEPARATOR, 0, "");
            AppendMenu(hmnuPopup, MenuFlags.MF_STRING, 1003, "取消TOP/TOP");
            AppendMenu(hmnuPopup, MenuFlags.MF_SEPARATOR, 0, "");
            AppendMenu(hmnuPopup, MenuFlags.MF_STRING, 1004, "test");
            rect = new RECT();
        }

        void OnRenderImage(RenderTexture from, RenderTexture to)
        {
            Graphics.Blit(from, to, m_Material);
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ReleaseCapture();
                SendMessage(_hwnd, 0xA1, 0x02, 0);
                SendMessage(_hwnd, 0x0202, 0, 0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                //获取窗体位置
                MARGINS mARGINS = new MARGINS();
                GetWindowRect(_hwnd, ref mARGINS);
                x = mARGINS.cxLeftWidth; y = mARGINS.cxRightWidth;
                //下拉式菜单栏
                int ret = TrackPopupMenu(hmnuPopup, TPM_CENTERALIGN | TPM_VCENTERALIGN | TPM_RETURNCMD, x+135, y+84, 0, _hwnd, ref rect);

                switch (ret)
                {
                    case 1001:
                        dialogBoxText.text = "因为版本原因不能显示中文,请日记记录到文件,可以尝试把脚本的改成全英文";
                        childref = new ThreadStart(RunPythonScript);
                        childThread = new Thread(childref);
                        childThread.Start();
                        break;
                    case 1002:
                        if(isDialogBox == true)
                        {
                            dialogBox.SetActive(isDialogBox);
                            isDialogBox = false;
                        }
                        else if (isDialogBox == false)
                        {
                            dialogBox.SetActive(isDialogBox);
                            isDialogBox = true;
                        }
                        break;
                    case 1003:
                        if (isTop == true)
                        {
                            SetWindowPos(_hwnd, HWND_TOPMOST, x, y, 200, 200, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
                            isTop = false;
                        }
                        else if (isTop == false)
                        {
                            SetWindowPos(_hwnd, HWND_NOTOPMOST, x, y, 200, 200, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
                            isTop = true;
                        }
                        break;
                    case 1004:
                        break;
                }

            }
        }
        public void RunPythonScript()
        {
            Process p = new Process();
            p.StartInfo.FileName = @"D:\mydocument\project\unity\desk_pet\Mion_pet\Build\PyRPA.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.Close();
        }
        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            dialogBoxText.text = outLine.Data.ToString();
        }
    }
}