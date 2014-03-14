using System;

namespace LogitechWrapper
{
    /// <summary>
    ///     The button class. Handles all interaction with a single
    ///     button based on the provided identifier. Normally a hex value.
    /// </summary>
    public class Button
    {
        private bool _currentState;
        private bool _lastState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Button" /> class.
        /// </summary>
        /// <param name="buttonIdentifier">The button identifier.</param>
        public Button(int buttonIdentifier)
        {
            Identifier = buttonIdentifier;
        }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Identifier { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this button is down.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this button is down; otherwise, <c>false</c>.
        /// </value>
        public bool IsDown
        {
            get { return LogitechGSDK.LogiLcdIsButtonPressed(Identifier); }
        }

        /// <summary>
        ///     Gets a value indicating whether this button is up.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this button is up; otherwise, <c>false</c>.
        /// </value>
        public bool IsUp
        {
            get { return !LogitechGSDK.LogiLcdIsButtonPressed(Identifier); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is clicked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is clicked; otherwise, <c>false</c>.
        /// </value>
        public bool IsClicked
        {
            get { return (!_lastState && _currentState); }
        }

        /// <summary>
        /// Occurs when the button is clicked (Changes from Up to Down).
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        ///     Updates this buttons pressed values.
        /// </summary>
        public void Update()
        {
            _lastState = _currentState;
            _currentState = LogitechGSDK.LogiLcdIsButtonPressed(Identifier);

            if (IsClicked)
                OnClicked();
        }

        protected virtual void OnClicked()
        {
            EventHandler handler = Clicked;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}