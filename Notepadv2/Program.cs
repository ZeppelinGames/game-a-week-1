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

        static Random rnd = new Random();

        static TaskPoolGlobalHook globalHook;

        static NotepadActor _player = new Player();
        private static List<NotepadActor> actors = new List<NotepadActor>() {
            //new BombEnemy(),
            //new AnimatedNotepad(),
            new AnimatedTextNotepad()
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
            await globalHook.RunAsync();
        }

        static void CloseApplication() {
            Console.WriteLine("Quitting...");

            globalHook.KeyTyped -= GlobalHook_KeyTyped;

            // Clean up windows
            for (int i = 0; i < actors.Count; i++) {
                actors[i].Close();
            }
            _player.Close();

            Environment.Exit(0);
        }

        private static void GlobalHook_KeyTyped(object? sender, KeyboardHookEventArgs e) {
            e.SuppressEvent = true;

            if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcEscape) {
                CloseApplication();
                return;
            }

            if(e.Data.KeyCode == SharpHook.Native.KeyCode.VcSpace) {
                _player.SetText(">:)");
            }
        }

        private static void Update(object? state) {
            _player.Update(0.016f);
            for (int i = 0; i < actors.Count; i++) {
                actors[i].Update(0.016f);
            }
        }
    }
}