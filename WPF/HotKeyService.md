```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Utils
{
    public class HotKeyService
    {
        /// <remarks>
        ///https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-registerhotkey
        /// </remarks>>
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        /// <remarks>
        ///https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-unregisterhotkey
        /// </remarks>
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <remarks>
        /// https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-hotkey
        /// </remarks>
        public const int WmHotKey = 0x0312;


        private static readonly Dictionary<int, List<Action>> HotKeys;


        static HotKeyService()
        {
            HotKeys = new Dictionary<int, List<Action>>();
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcherOnThreadFilterMessage;
        }

        private static void ComponentDispatcherOnThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (handled || msg.message != WmHotKey)
                return;

            var param = (int)msg.wParam;

            if (!HotKeys.TryGetValue(param, out var actions)) return;

            handled = actions.Any();
            foreach (var action in actions)
            {
                action?.Invoke();
            }
        }

        private static int GetId(Key key, KeyModifier modifier)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var id = virtualKey + ((int)modifier * 0x10000);

            return id;
        }

        /// <summary>
        /// Registers <paramref name="action"/> to hot-key combination.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifier"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Register(Key key, KeyModifier modifier, Action action)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var id = GetId(key, modifier);

            var rv = RegisterHotKey(IntPtr.Zero, id, (uint)modifier, (uint)virtualKey);

            if (!HotKeys.ContainsKey(id))
                HotKeys.Add(id, new List<Action>() { action });
            else
                HotKeys[id].Add(action);

            return rv;
        }

        /// <summary>
        /// Unregisters <paramref name="action"/> from specified hot-key combination.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifier"></param>
        /// <param name="action"></param>
        public static void Unregister(Key key, KeyModifier modifier, Action action)
        {
            var id = GetId(key, modifier);

            if (HotKeys.TryGetValue(id, out var actions))
                actions.Remove(action);
        }

        /// <summary>
        /// Unregisters all actions from specified hot-key combination.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifier"></param>
        public static void Unregister(Key key, KeyModifier modifier)
        {
            var id = GetId(key, modifier);

            if (HotKeys.TryGetValue(id, out var actions))
                actions.Clear();
        }

        /// <summary>
        /// Unregisters all actions from all hot-key combinations.
        /// </summary>
        public static void Unregister()
        {
            var ids = HotKeys.Keys.ToList();
            foreach (var id in ids)
            {
                if (HotKeys.TryGetValue(id, out var actions))
                    actions.Clear();
            }
        }
    }
}
```