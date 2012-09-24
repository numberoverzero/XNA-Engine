// Used (with modification) from http://stackoverflow.com/a/10222878
// SO answer by Niko Drašković

#region Using Statements

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Keys = Microsoft.Xna.Framework.Input.Keys;

#endregion

namespace Engine.Input.EventInput
{
    /// <summary>
    ///   Hooks up windows dlls for keyboard event capture
    /// </summary>
    public class KeyboardLayout
    {
        private const uint KLF_ACTIVATE = 1; //activate the layout
        private const int KL_NAMELENGTH = 9; // length of the keyboard buffer
        private const string LANG_EN_US = "00000409";
        private const string LANG_HE_IL = "0001101A";

        [DllImport("user32.dll")]
        private static extern long LoadKeyboardLayout(
            string pwszKLID, // input locale identifier
            uint Flags // input locale identifier options
            );

        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(
            StringBuilder pwszKLID //[out] string that receives the name of the locale identifier
            );

        /// <summary>
        ///   The name of the keyboard layout
        /// </summary>
        /// <returns> </returns>
        public static string getName()
        {
            var name = new StringBuilder(KL_NAMELENGTH);
            GetKeyboardLayoutName(name);
            return name.ToString();
        }
    }

    /// <summary>
    ///   EventArgs of a character press
    ///   (includes info about modifers such as alt, as well as repeat count)
    /// </summary>
    public class CharacterEventArgs : EventArgs
    {
        /// <summary>
        ///   Construct a new character event arg for a key press
        /// </summary>
        /// <param name="character"> </param>
        /// <param name="lParam"> </param>
        public CharacterEventArgs(char character, int lParam)
        {
            Character = character;
            Param = lParam;
        }

        /// <summary>
        ///   The character that was entered
        /// </summary>
        public char Character { get; private set; }

        /// <summary>
        ///   Extra info such as modifers that were pressed
        /// </summary>
        public int Param { get; private set; }

        /// <summary>
        ///   How many times the key was registered
        /// </summary>
        public int RepeatCount
        {
            get { return Param & 0xffff; }
        }

        /// <summary>
        ///   True if this was an extended key
        /// </summary>
        public bool ExtendedKey
        {
            get { return (Param & (1 << 24)) > 0; }
        }

        /// <summary>
        ///   Was alt depressed when the key was entered
        /// </summary>
        public bool AltPressed
        {
            get { return (Param & (1 << 29)) > 0; }
        }

        /// <summary>
        ///   The state the key was in before this event
        /// </summary>
        public bool PreviousState
        {
            get { return (Param & (1 << 30)) > 0; }
        }

        /// <summary>
        ///   Is this a transition from a different previous state
        /// </summary>
        public bool TransitionState
        {
            get { return (Param & (1 << 31)) > 0; }
        }
    }

    /// <summary>
    ///   EventArgs of a Keys press
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        /// <summary>
        ///   Construct a new Key event arg for a key press
        /// </summary>
        /// <param name="keyCode"> </param>
        public KeyEventArgs(Keys keyCode)
        {
            KeyCode = keyCode;
        }

        /// <summary>
        ///   The key that was pressed
        /// </summary>
        public Keys KeyCode { get; private set; }
    }

    /// <summary>
    ///   EventHandler for char events
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);

    /// <summary>
    ///   EventHandler for Key events
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    /// <summary>
    ///   Converts win dll hooked input events into nice CharacterEvents and KeyEvents
    /// </summary>
    public static class EventInput
    {
        //various Win32 constants that we need
        private const int GWL_WNDPROC = -4;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_CHAR = 0x102;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_INPUTLANGCHANGE = 0x51;
        private const int WM_GETDLGCODE = 0x87;
        private const int WM_IME_COMPOSITION = 0x10f;
        private const int DLGC_WANTALLKEYS = 4;
        private static bool initialized;
        private static IntPtr prevWndProc;
        private static WndProc hookProcDelegate;
        private static IntPtr hIMC;

        /// <summary>
        ///   Event raised when a character has been entered.
        /// </summary>
        public static event CharEnteredHandler CharEntered;

        /// <summary>
        ///   Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>
        ///   Event raised when a key has been released.
        /// </summary>
        public static event KeyEventHandler KeyUp;

        //Win32 functions that we're using
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam,
                                                    IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        /// <summary>
        ///   Initialize the TextInput with the given GameWindow.
        /// </summary>
        /// <param name="window"> The XNA window to which text input should be linked. </param>
        public static void Initialize(GameWindow window)
        {
            if (initialized)
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");

            hookProcDelegate = HookProc;
            prevWndProc = (IntPtr) SetWindowLong(window.Handle, GWL_WNDPROC,
                                                 (int) Marshal.GetFunctionPointerForDelegate(hookProcDelegate));

            hIMC = ImmGetContext(window.Handle);
            initialized = true;
        }

        private static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(prevWndProc, hWnd, msg, wParam, lParam);

            switch (msg)
            {
                case WM_GETDLGCODE:
                    returnCode = (IntPtr) (returnCode.ToInt32() | DLGC_WANTALLKEYS);
                    break;

                case WM_KEYDOWN:
                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs((Keys) wParam));
                    break;

                case WM_KEYUP:
                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs((Keys) wParam));
                    break;

                case WM_CHAR:
                    if (CharEntered != null)
                    {
                        CharEntered(null, new CharacterEventArgs((char) wParam, lParam.ToInt32()));
                        KeyDown(null, new KeyEventArgs((Keys) wParam));
                    }
                    break;

                case WM_IME_SETCONTEXT:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, hIMC);
                    break;

                case WM_INPUTLANGCHANGE:
                    ImmAssociateContext(hWnd, hIMC);
                    returnCode = (IntPtr) 1;
                    break;
            }

            return returnCode;
        }

        #region Nested type: WndProc

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }

    /// <summary>
    ///   Receives event-driven input in the form of strings, characters, and Keys
    /// </summary>
    public interface IKeyboardSubscriber
    {
        /// <summary>
        ///   Does this Subscriber have the (possibly exclusive) focus
        /// </summary>
        bool Selected { get; set; }

        /// <summary>
        ///   Handle a single character of input
        /// </summary>
        /// <param name="inputChar"> </param>
        void ReceiveTextInput(char inputChar);

        /// <summary>
        ///   Handle a string of input
        /// </summary>
        /// <param name="text"> </param>
        void ReceiveTextInput(string text);

        /// <summary>
        ///   Handle a special command
        /// </summary>
        /// <param name="command"> </param>
        void ReceiveCommandInput(char command);

        /// <summary>
        ///   Handle a Key input
        /// </summary>
        /// <param name="key"> </param>
        void ReceiveSpecialInput(Keys key);

        //or Focused
    }

    /// <summary>
    ///   Sends out read calls when input is handled from EventInput
    /// </summary>
    public static class KeyboardDispatcher
    {
        private static bool initialized;
        private static readonly List<IKeyboardSubscriber> _subscriber = new List<IKeyboardSubscriber>();
        private static string _pasteResult = "";

        /// <summary>
        ///   Initialize the KeyboardDispatcher by connecting it to a GameWindow
        /// </summary>
        /// <param name="window"> </param>
        public static void Initialize(GameWindow window)
        {
            if (!initialized)
            {
                EventInput.Initialize(window);
                EventInput.CharEntered += EventInput_CharEntered;
                EventInput.KeyDown += EventInput_KeyDown;
                initialized = true;
            }
        }

        private static void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (_subscriber.Count == 0)
                return;

            foreach (var subscriber in _subscriber)
                subscriber.ReceiveSpecialInput(e.KeyCode);
        }

        private static void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (_subscriber.Count == 0)
                return;
            if (char.IsControl(e.Character))
            {
                //ctrl-v
                if (e.Character == 0x16)
                {
                    //XNA runs in Multiple Thread Apartment state, which cannot recieve clipboard
                    var thread = new Thread(PasteThread);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    foreach (var subscriber in _subscriber)
                        subscriber.ReceiveTextInput(_pasteResult);
                }
                else
                {
                    foreach (var subscriber in _subscriber)
                        subscriber.ReceiveCommandInput(e.Character);
                }
            }
            else
            {
                foreach (var subscriber in _subscriber)
                    subscriber.ReceiveTextInput(e.Character);
            }
        }

        /// <summary>
        ///   Add a listener that receives messages when new keys are pressed
        /// </summary>
        /// <param name="subscriber"> </param>
        public static void RegisterListener(IKeyboardSubscriber subscriber)
        {
            _subscriber.Add(subscriber);
        }

        /// <summary>
        ///   Remove a listener
        /// </summary>
        /// <param name="subscriber"> </param>
        public static void UnregisterListener(IKeyboardSubscriber subscriber)
        {
            _subscriber.Remove(subscriber);
        }

        //Thread has to be in Single Thread Apartment state in order to receive clipboard

        [STAThread]
        private static void PasteThread()
        {
            if (Clipboard.ContainsText())
            {
                _pasteResult = Clipboard.GetText();
            }
            else
            {
                _pasteResult = "";
            }
        }
    }
}