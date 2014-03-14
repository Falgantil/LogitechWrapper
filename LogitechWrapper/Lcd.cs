using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LogitechWrapper
{
    /// <summary>
    ///     This is the main attraction of this library. This is the class
    ///     that allows you to utilize the functions of your LCD screen.
    /// </summary>
    public class Lcd
    {
        private Thread _buttonThread;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lcd" /> class.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="screenType">The screenType.</param>
        public Lcd(string appName, LcdScreenType screenType)
            : this(appName, (int) screenType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lcd" /> class.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="DllNotFoundException">
        ///     The DLL was found, but could not be loaded. This is most likely due to an
        ///     incompatibility issue (32-bit application but 64-bit dll, or vice versa).
        ///     or
        ///     The Logitech DLL is missing, or built for incorrect platform! Either place
        ///     it in System32, or in the same folder as the application!
        /// </exception>
        /// <exception cref="Exception">
        ///     Could not initialize the connection to your keyboard. Make sure the Logitech Gaming
        ///     Software is running, and you're supplying the correct Type.
        /// </exception>
        public Lcd(string appName, int type)
        {
            bool libraryExists = LogitechGSDK.LibraryExists("LogitechLcd");

            if (File.Exists("LogitechLcd.dll") && !libraryExists)
                throw new DllNotFoundException(
                    "The DLL was found, but could not be loaded. This is most likely due to an incompatibility issue (32-bit application but 64-bit dll, or vice versa).");

            if (!libraryExists)
                throw new DllNotFoundException("The Logitech DLL is missing, or built for incorrect platform! " +
                                               "Either place it in System32, or in the same folder as the application!");

            bool isConnected = LogitechGSDK.LogiLcdInit(appName, type);

            if (!isConnected)
                throw new Exception("Could not initialize the connection to your keyboard. " +
                                    "Make sure the Logitech Gaming Software is running, and you're supplying the correct Type.");

            Button1 = new Button(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0);
            Button2 = new Button(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1);
            Button3 = new Button(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2);
            Button4 = new Button(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3);

            UpdateLcd();
        }

        /// <summary>
        ///     Gets the first button, from left.
        /// </summary>
        /// <value>
        ///     The first button.
        /// </value>
        public Button Button1 { get; private set; }

        /// <summary>
        ///     Gets the second button, from left.
        /// </summary>
        /// <value>
        ///     The second button.
        /// </value>
        public Button Button2 { get; private set; }

        /// <summary>
        ///     Gets the third button, from left.
        /// </summary>
        /// <value>
        ///     The third button.
        /// </value>
        public Button Button3 { get; private set; }

        /// <summary>
        ///     Gets the fourth button, from left.
        /// </summary>
        /// <value>
        ///     The fourth button.
        /// </value>
        public Button Button4 { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically update the screen every time a WriteText request has been
        ///     called.
        ///     <para>This may not be desired, if you're performing several lines of writing in the same method.</para>
        /// </summary>
        /// <value>
        ///     <c>true</c> to automatically update screen; otherwise, <c>false</c>.
        /// </value>
        public bool AutoUpdate { get; set; }

        /// <summary>
        ///     Manages updating of the buttons. The interval defines (in milliseconds) how often it will check for changes in the
        ///     pressed state.
        ///     <para></para>
        /// </summary>
        /// <param name="interval">The interval.</param>
        public void HandleButtons(int interval)
        {
            if (_buttonThread != null && _buttonThread.IsAlive)
                _buttonThread.Abort();

            _buttonThread = new Thread(() =>
            {
                while (true)
                {
                    UpdateButtons();

                    if (_buttonThread.ThreadState == ThreadState.AbortRequested)
                        break;

                    Thread.Sleep(interval);
                }
            });
            _buttonThread.Start();
        }

        /// <summary>
        ///     Ends the handling of the buttons.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">You cannot end the handling, when it is not running!</exception>
        public void EndHandlingButtons()
        {
            if (_buttonThread == null || !_buttonThread.IsAlive)
                throw new InvalidOperationException("You cannot end the handling, when it is not running!");

            _buttonThread.Abort();
        }

        /// <summary>
        ///     Writes the text on the LCD screen on the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="text">The text.</param>
        public void WriteText(Line line, string text)
        {
            WriteText((int) line, text);
        }

        /// <summary>
        ///     Writes the text on the LCD screen on the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">line;The line needs to be 0, 1, 2 or 3!</exception>
        public void WriteText(int line, string text)
        {
            if (line > 3 || line < 0)
                throw new ArgumentOutOfRangeException("line", "The line needs to be 0, 1, 2 or 3!");

            LogitechGSDK.LogiLcdMonoSetText(line, text);

            if (AutoUpdate)
                LogitechGSDK.LogiLcdUpdate();
        }

        /// <summary>
        ///     Updates the LCD screen, showing everything in the cache.
        /// </summary>
        public void UpdateLcd()
        {
            LogitechGSDK.LogiLcdUpdate();
        }

        /// <summary>
        ///     Resizes and draws the image on the LCD screen.
        /// </summary>
        /// <param name="image">The image.</param>
        public void DrawImage(IImage image)
        {
            if (image.Width != 160 || image.Height != 42)
                image.Resize(160, 42);

            var imageBytes = new List<byte>();

            for (byte y = 0; y < 42; y++)
                for (byte x = 0; x < 160; x++)
                    imageBytes.Add(image.GetPixel(x, y));

            LogitechGSDK.LogiLcdMonoSetBackground(imageBytes.ToArray());
            LogitechGSDK.LogiLcdUpdate();
        }

        /// <summary>
        ///     Updates the buttons, checking for changes in the pressed state.
        /// </summary>
        public void UpdateButtons()
        {
            Button1.Update();
            Button2.Update();
            Button3.Update();
            Button4.Update();
        }
    }
}