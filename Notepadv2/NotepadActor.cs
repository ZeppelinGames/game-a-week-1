using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Notepadv2 {
    public abstract class NotepadActor {

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const int WM_SETTEXT = 0x000C;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_SETFONT = 0x0030;
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern string SendMessage(IntPtr hWnd, int wMsg);

        [DllImport("user32.dll")]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT wRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
                                    int nOrientation, int fnWeight, bool fdwItalic,
                                    bool fdwUnderline, bool fdwStrikeOut, int fdwCharSet,
                                    int fdwOutputPrecision, int fdwClipPrecision,
                                    int fdwQuality, int fdwPitchAndFamily, string lpszFace);

        [DllImport("user32.dll")]
        extern private static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

        private Process? _notepadProcess = null;
        private IntPtr _notepadHandle = IntPtr.Zero;
        private string _text = "";
        private string _title = "???";

        private bool _isDirty = false;

        public int PositionX => _posX;
        public int PositionY => _posY;
        public int Width => _sizeX;
        public int Height => _sizeY;

        private int _posX = 0;
        private int _posY = 0;
        private int _sizeX = 200;
        private int _sizeY = 250;

        private int _moveX = 0;
        private int _moveY = 0;

        public NotepadActor(string windowTitle = "WINDOW", int positionX = 0, int positionY = 0, int sizeX = 200, int sizeY = 250, string innerText = "") {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = "notepad.exe",
                UseShellExecute = false,
            };

            ThreadStart ts = new ThreadStart(() => {
                _notepadProcess = Process.Start(startInfo);
                if (_notepadProcess != null) {
                    _notepadProcess.WaitForInputIdle();
                    _notepadHandle = FindWindowEx(_notepadProcess.MainWindowHandle, IntPtr.Zero, "Edit", null);

                    // Remove the buttons from notepad
                    int currentStyle = GetWindowLong(_notepadProcess.MainWindowHandle, GWL_STYLE);
                    SetWindowLong(_notepadProcess.MainWindowHandle, GWL_STYLE, (currentStyle & ~WS_SYSMENU));

                    UpdateFont(); 

                    _isDirty = true;
                }
            });
            Thread t = new Thread(ts);
            t.Start();

            SetPosition(positionX, positionY);
            SetSize(sizeX, sizeY);
            SetWindowTitle(windowTitle);
            SetText(innerText);
        }

        public void UpdateFont(int fontSize = 64, string fontFamily = "Consolas") {
            // Set System font
            IntPtr fontHandle = CreateFont(fontSize, 0, 0, 0, 1000, false, false, false, 0, 0, 0, 0, 0, fontFamily);
            SendMessage(_notepadHandle, WM_SETFONT, fontHandle, "");
            _isDirty = true;
        }

        public void SetText(string newText) {
            _text = newText;
            _isDirty = true;
        }

        public void AddToEndOfText(string a) {
            _text += a;
            _isDirty = true;
        }

        public void SetPosition(int x, int y) {
            _posX = x;
            _posY = y;
            _isDirty = true;
        }

        public void MoveActor(int x, int y) {
            _posX += x;
            _posY += y;
            _isDirty = true;
        }

        public void SetSize(int x , int y) {
            _sizeX = x;
            _sizeY = y;
            _isDirty = true;
        }

        public void SetWindowTitle(string title) {
            // Set title
            _title = title;
            _isDirty = true;
        }

        public virtual void Update(float dt) {
            if (_notepadProcess == null) {
                return;
            }

            RECT windowRect = new RECT();
            if (GetWindowRect(_notepadProcess.MainWindowHandle, ref windowRect)) {
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;
                if (width != _sizeX || height != _sizeY ||
                    windowRect.top != _posY || windowRect.left != _posX) {
                    _isDirty = true;
                }
            }

            if (_moveX != 0 || _moveY != 0) {
                _posX += _moveX;
                _posY += _moveY;

                _isDirty = true;
            }

            string test  = SendMessage(_notepadHandle, WM_GETTEXT);
            Debug.Write(test);

            if (_isDirty) {
                _isDirty = false;
                SendMessage(_notepadProcess.MainWindowHandle, WM_SETTEXT, 0, _title);

                // Set text content
                SendMessage(_notepadHandle, WM_SETTEXT, 0, _text);

                // Move and size window
                MoveWindow(_notepadProcess.MainWindowHandle, _posX, _posY, _sizeX, _sizeY, true);

            }
        }

        public void Close() {
            if (_notepadProcess != null) {
                _notepadProcess.CloseMainWindow();
            }
        }
    }
}
