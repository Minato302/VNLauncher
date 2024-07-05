#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VNLauncher.FunctionalClasses
{
    public struct KeyCode
    {
        public Int32? vkCode;
        public IntPtr wParam;
        public KeyCode(Int32? vkCode, IntPtr wParam)
        {
            this.vkCode = vkCode;
            this.wParam = wParam;
        }

        public override Boolean Equals(Object? rhs)
        {
            (Int32?, IntPtr) param = ((Int32?, IntPtr))rhs!;
            if (vkCode == null)
            {
                return wParam == param.Item2;
            }
            return vkCode == param.Item1 && wParam == param.Item2;
        }
        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public struct InputKey
    {
        private KeyCode code;
        private String name;
        public KeyCode Code => code;
        public String Name => name;
        public InputKey(KeyCode code, String name)
        {
            this.code = code;
            this.name = name;
        }
    }
    public class InputConverter
    {
        private List<InputKey> keys;
        public InputConverter()
        {
            keys = new List<InputKey>();
            for (Char ch = 'a'; ch <= 'z'; ch++)
            {
                keys.Add(new InputKey(new KeyCode(ch - 'a' + 65, new IntPtr(257)), "键盘按键" + ch.ToString().ToUpper()));
            }
            keys.Add(new InputKey(new KeyCode(9, new IntPtr(257)), "键盘按键Tab"));
            keys.Add(new InputKey(new KeyCode(13, new IntPtr(257)), "键盘按键回车"));
            keys.Add(new InputKey(new KeyCode(32, new IntPtr(257)), "键盘按键空格"));
            keys.Add(new InputKey(new KeyCode(160, new IntPtr(257)), "键盘按键左Shift"));
            keys.Add(new InputKey(new KeyCode(161, new IntPtr(257)), "键盘按键右Shift"));
            keys.Add(new InputKey(new KeyCode(164, new IntPtr(257)), "键盘按键左Alt"));
            keys.Add(new InputKey(new KeyCode(165, new IntPtr(257)), "键盘按键右Alt"));
            for (Int32 i = 0; i <= 9; i++)
            {
                keys.Add(new InputKey(new KeyCode(48 + i, new IntPtr(257)), "键盘按键" + i.ToString()));
            }
            keys.Add(new InputKey(new KeyCode(219, new IntPtr(257)), "键盘按键["));
            keys.Add(new InputKey(new KeyCode(221, new IntPtr(257)), "键盘按键]"));
            keys.Add(new InputKey(new KeyCode(220, new IntPtr(257)), "键盘按键\\"));
            keys.Add(new InputKey(new KeyCode(186, new IntPtr(257)), "键盘按键;"));
            keys.Add(new InputKey(new KeyCode(222, new IntPtr(257)), "键盘按键'"));
            keys.Add(new InputKey(new KeyCode(188, new IntPtr(257)), "键盘按键<"));
            keys.Add(new InputKey(new KeyCode(190, new IntPtr(257)), "键盘按键>"));
            keys.Add(new InputKey(new KeyCode(191, new IntPtr(257)), "键盘按键?'"));

            keys.Add(new InputKey(new KeyCode(37, new IntPtr(257)), "键盘按键←"));
            keys.Add(new InputKey(new KeyCode(38, new IntPtr(257)), "键盘按键↑"));
            keys.Add(new InputKey(new KeyCode(39, new IntPtr(257)), "键盘按键→"));
            keys.Add(new InputKey(new KeyCode(40, new IntPtr(257)), "键盘按键↓"));

            for (Int32 i = 1; i <= 12; i++)
            {
                keys.Add(new InputKey(new KeyCode(i + 111, new IntPtr(257)), "键盘按键F" + i.ToString()));
            }

            keys.Add(new InputKey(new KeyCode(null, new IntPtr(514)), "鼠标左键"));
            keys.Add(new InputKey(new KeyCode(null, new IntPtr(517)), "鼠标右键"));
            keys.Add(new InputKey(new KeyCode(null, new IntPtr(520)), "滚轮键"));
            keys.Add(new InputKey(new KeyCode(null, new IntPtr(524)), "鼠标侧键"));
        }
        public InputKey ConvertToInputKey(Int32? vkCode, IntPtr wParam)
        {
            foreach (InputKey key in keys)
            {
                if (key.Code.Equals((vkCode, wParam)))
                {
                    return key;
                }
            }
            return new InputKey(new KeyCode(0, new IntPtr(0)), "非法按键");
        }
        public InputKey ConvertToInputKey(String name)
        {
            foreach (InputKey key in keys)
            {
                if (key.Name.Equals(name))
                {
                    return key;
                }
            }
            return new InputKey(new KeyCode(0, new IntPtr(0)), "非法按键");
        }
    }
    public class KeyTriggers
    {
        private List<(InputKey, Action)> triggers;
        public KeyTriggers()
        {
            triggers = new List<(InputKey, Action)>();
        }
        public Action? GetAction(String name)
        {
            foreach ((InputKey, Action) trigger in triggers)
            {
                if (trigger.Item1.Name.Equals(name))
                {
                    return trigger.Item2;
                }
            }
            return null;
        }
        public void AddTrigger(String keyName, Action action)
        {
            InputConverter converter = new InputConverter();

            triggers.Add((converter.ConvertToInputKey(keyName), action));
        }
    }
    public class GlobalHook
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(Int32 idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(Int32 idHook, LowLevelMouseProc lpfn, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, Int32 nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean SwitchToThisWindow(IntPtr hWnd, Boolean fAltTab);

        private delegate IntPtr LowLevelKeyboardProc(Int32 nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc procKey;

        private delegate IntPtr LowLevelMouseProc(Int32 nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc procMouse;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetCursorPos(out WindowsHandler.POINT lpPoint);

        private IntPtr SetKeyHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName!), 0);
            }
        }
        public static WindowsHandler.POINT? GetMousePosition()
        {
            if (GetCursorPos(out WindowsHandler.POINT lpPoint))
            {
                return lpPoint;
            }
            else
            {
                return null;
            }
        }


        private IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(14, proc, GetModuleHandle(curModule.ModuleName!), 0);
            }
        }

        private IntPtr keyHookID = IntPtr.Zero;
        private IntPtr mouseHookID = IntPtr.Zero;

        private KeyTriggers keyTriggers;
        private IntPtr HookCallback(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            InputConverter converter = new InputConverter();
            Int32 vkCode = Marshal.ReadInt32(lParam);

            keyTriggers.GetAction(converter.ConvertToInputKey(vkCode, wParam).Name)?.Invoke();
            return CallNextHookEx(keyHookID, nCode, wParam, lParam);
        }

        public GlobalHook(List<(String, Action)> triggers)
        {
            procKey = HookCallback;
            procMouse = HookCallback;

            keyHookID = SetKeyHook(procKey);
            mouseHookID = SetMouseHook(procMouse);
            keyTriggers = new KeyTriggers();
            foreach((String, Action) trigger in triggers)
            {
                keyTriggers.AddTrigger(trigger.Item1, trigger.Item2);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean UnhookWindowsHookEx(Int32 idHook);
        public void UninstallHook()
        {
            if (keyHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(keyHookID);
                keyHookID = IntPtr.Zero;
            }

            if (mouseHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(mouseHookID);
                mouseHookID = IntPtr.Zero;
            }
        }

    }
    public class GlobalKeyReader
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(Int32 idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(Int32 idHook, LowLevelMouseProc lpfn, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, Int32 nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(String lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean SwitchToThisWindow(IntPtr hWnd, Boolean fAltTab);

        private delegate IntPtr LowLevelKeyboardProc(Int32 nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc procKey;

        private delegate IntPtr LowLevelMouseProc(Int32 nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc procMouse;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetCursorPos(out WindowsHandler.POINT lpPoint);

        private IntPtr SetKeyHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName!), 0);
            }
        }
        public static WindowsHandler.POINT? GetMousePosition()
        {
            if (GetCursorPos(out WindowsHandler.POINT lpPoint))
            {
                return lpPoint;
            }
            else
            {
                return null;
            }
        }


        private IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule!)
            {
                return SetWindowsHookEx(14, proc, GetModuleHandle(curModule.ModuleName!), 0);
            }
        }

        private IntPtr keyHookID = IntPtr.Zero;
        private IntPtr mouseHookID = IntPtr.Zero;
        private Action<InputKey> action;
        private Boolean isReading;
        public void ReadAKey()
        {
            isReading = true;
        }
        private IntPtr HookCallback(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            Int32 vkCode = Marshal.ReadInt32(lParam);
            InputConverter converter = new InputConverter();
            InputKey key = converter.ConvertToInputKey(vkCode, wParam);
            if (key.Name != "非法按键" && isReading)
                {
                action(key);
                isReading = false;
            }
            return CallNextHookEx(keyHookID, nCode, wParam, lParam);
        }

        public GlobalKeyReader(Action<InputKey> action)
        {
            procKey = HookCallback;
            procMouse = HookCallback;
            this.action = action;

            keyHookID = SetKeyHook(procKey);
            mouseHookID = SetMouseHook(procMouse);
            isReading = false;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean UnhookWindowsHookEx(Int32 idHook);
        public void UninstallHook()
        {
            if (keyHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(keyHookID);
                keyHookID = IntPtr.Zero;
            }

            if (mouseHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(mouseHookID);
                mouseHookID = IntPtr.Zero;
            }
        }
    }
}
