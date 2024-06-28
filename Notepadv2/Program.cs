using SharpHook;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Notepadv2 {
    class Program {
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        static TaskPoolGlobalHook globalHook;
        private static List<NotepadActor> actors = new List<NotepadActor>() {
            //new BombEnemy(),
            //new AnimatedNotepad(),
            new AnimatedTextNotepad(),
            new NotepadButton()
        };

        public static void Main(string[] args) {
            Console.WriteLine("Press any key in this window or ESCAPE to end process");
            Console.WriteLine("Running...");
            
            ShowWindow(GetConsoleWindow(), SW_MINIMIZE);

            // Setup hooks
            RunHook();

            // Setup loop timer
            Timer timer = new Timer(Update, null, 0, 16);

            Console.ReadLine();

            // _player.Close();
            CloseApplication();
        }

        static async void RunHook() {
            globalHook = new TaskPoolGlobalHook();
            globalHook.KeyTyped += GlobalHook_KeyTyped;
            globalHook.MouseClicked += GlobalHook_MouseClicked;
            await globalHook.RunAsync();
        }

        private static void GlobalHook_MouseClicked(object? sender, MouseHookEventArgs e) {
           e.SuppressEvent = true;

            for (int i = 0; i < actors.Count; i++) {
                actors[i].HandleMouse(e.Data);
            }
        }


        private static void GlobalHook_KeyTyped(object? sender, KeyboardHookEventArgs e) {
            e.SuppressEvent = true;

            if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcEscape) {
                CloseApplication();
                return;
            }

            for (int i = 0; i < actors.Count; i++) {
                actors[i].HandleInput(e.Data);
            }
        }

        static void CloseApplication() {
            Console.WriteLine("Quitting...");

            // Deregister hooks
            globalHook.KeyTyped -= GlobalHook_KeyTyped;
            globalHook.MouseClicked -= GlobalHook_MouseClicked;

            // Clean up windows
            for (int i = 0; i < actors.Count; i++) {
                actors[i].Close();
            }

            Environment.Exit(0);
        }

        private static void Update(object? state) {
            for (int i = 0; i < actors.Count; i++) {
                actors[i].Update(0.016f);
            }
        }
    }
}