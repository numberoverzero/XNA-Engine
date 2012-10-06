#region File Description

//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using System;
using Engine.Input.Managers;
using Engine.Input.Managers.SinglePlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Engine.Screen
{
    /// <summary>
    ///   A popup message box screen, used to display "are you sure?"
    ///   confirmation messages.
    /// </summary>
    /// <remarks>
    ///   This public class is somewhat similar to one of the same name in the 
    ///   GameStateManagement sample.
    /// </remarks>
    public class MessageBoxScreen : GameScreen
    {
        private const float VPad = 0.1f;
        private const float HPad = 0.1f;
        private const float BorderPad = 0.2f;
        
        private readonly string _cancel;
        private readonly string _confirm;
        private readonly string _message;
        
        private readonly OptionLayout _layout;
        
        private Rectangle _backgroundBorderRect;
        private Rectangle _backgroundRect;
        
        private Vector2 _cancelPos;
        private Vector2 _confirmPos;
        
        private bool _confirmSelected;

        private SpriteFont _messageFont;
        private Vector2 _msgPos;
        private SpriteFont _optionFont;
        private Vector2 _popupTextSize = Vector2.Zero;

        /// <summary>
        ///   Constructor.
        /// </summary>
        public MessageBoxScreen(string message, string confirm, string cancel, OptionLayout layout)
        {
            _message = message;
            _confirm = confirm;
            _cancel = cancel;
            _layout = layout;
            _confirmSelected = false;
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.25);
            TransitionOffTime = TimeSpan.FromSeconds(0.25);

            CalculateLayout();
        }

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        private void CalculateLayout()
        {
            var msgSize = _messageFont.MeasureString(_message);
            var confirmSize = _optionFont.MeasureString(_confirm);
            var cancelSize = _optionFont.MeasureString(_cancel);

            var screenCenter = Dimensions/2;


            var optSize = Vector2.Zero;
            switch (_layout)
            {
                case OptionLayout.Horizontal:
                    optSize = new Vector2((1 + HPad)*(confirmSize.X + cancelSize.X),
                                          Math.Max(confirmSize.Y, cancelSize.Y));
                    break;
                case OptionLayout.Vertical:
                    optSize = new Vector2(Math.Max(confirmSize.X, cancelSize.X),
                                          (1 + VPad)*(confirmSize.Y + cancelSize.Y));
                    break;
            }
            _popupTextSize = new Vector2(Math.Max(msgSize.X, optSize.X), (1 + VPad)*(msgSize.Y + optSize.Y));

            var origin = screenCenter - _popupTextSize/2;
            _msgPos = new Vector2(screenCenter.X - msgSize.X/2, origin.Y);


            //var optionOrigin =
            var padHeight = _popupTextSize.Y - _popupTextSize.Y/(1 + VPad);
            var optionOrigin = new Vector2(origin.X, origin.Y + msgSize.Y + padHeight);

            switch (_layout)
            {
                case OptionLayout.Horizontal:
                    _confirmPos = optionOrigin;
                    _cancelPos = new Vector2(optionOrigin.X + _popupTextSize.X - cancelSize.X, optionOrigin.Y);
                    break;
                case OptionLayout.Vertical:
                    var optionVPad = _popupTextSize.Y - msgSize.Y - padHeight - confirmSize.Y - cancelSize.Y;
                    _confirmPos = new Vector2(screenCenter.X - confirmSize.X/2, optionOrigin.Y);
                    _cancelPos = new Vector2(screenCenter.X - cancelSize.X/2, origin.Y + _popupTextSize.Y);
                    break;
            }

            _backgroundRect = new Rectangle((int) (screenCenter.X - _popupTextSize.X/2),
                                            (int) (screenCenter.Y - _popupTextSize.Y/2), (int) _popupTextSize.X,
                                            (int) _popupTextSize.Y);
            // Draw the background rectangles
            _backgroundRect.X -= (int) (BorderPad/2*_backgroundRect.Width);
            _backgroundRect.Y -= (int) (BorderPad/2*_backgroundRect.Height);
            _backgroundRect.Width += (int) (BorderPad*_backgroundRect.Width);
            _backgroundRect.Height += (int) (BorderPad*_backgroundRect.Height);

            _backgroundBorderRect = new Rectangle(_backgroundRect.X - 1, _backgroundRect.Y - 1,
                                                  _backgroundRect.Width + 2, _backgroundRect.Height + 2);
        }

        /// <summary>
        ///   Loads graphics content for this screen. This uses the shared ContentManager
        ///   provided by the ScreenManager, so the content will remain loaded forever.
        ///   Whenever a subsequent MessageBoxScreen tries to load this same content,
        ///   it will just get back another reference to the already loaded instance.
        /// </summary>
        public override void LoadContent()
        {
            _optionFont = ScreenManager.Content.Load<SpriteFont>("Fonts/MessageBoxOption");
            _messageFont = ScreenManager.Content.Load<SpriteFont>("Fonts/MessageBoxMessage");
        }

        /// <summary>
        ///   Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            if (input.IsPressed("menu_up") || input.IsPressed("menu_down") || input.IsPressed("menu_left") || input.IsPressed("menu_right"))
                _confirmSelected = !_confirmSelected;

            if (!input.IsPressed("menu_select")) return;
            var handler = _confirmSelected ? Accepted : Cancelled;
            handler(this, EventArgs.Empty);
            ExitScreen();
        }

        /// <summary>
        ///   Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha*2/3);

            // Fade the popup alpha during transitions.
            var confirmColor = _confirmSelected ? ScreenManager.TextSelectColor : ScreenManager.TextColor;
            var cancelColor = _confirmSelected ? ScreenManager.TextColor : ScreenManager.TextSelectColor;

            ScreenManager.DrawRectangle(_backgroundBorderRect, new Color(128, 128, 128,
                                                                         (byte) (192.0f*TransitionAlpha/255.0f)));
            ScreenManager.DrawRectangle(_backgroundRect, new Color(0, 0, 0,
                                                                   (byte) (232.0f*TransitionAlpha/255.0f)));

            // Draw the message box text.
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(_messageFont, _message, _msgPos, ScreenManager.TextColor);
            ScreenManager.SpriteBatch.DrawString(_optionFont, _confirm, _confirmPos, confirmColor);
            ScreenManager.SpriteBatch.DrawString(_optionFont, _cancel, _cancelPos, cancelColor);
            ScreenManager.SpriteBatch.End();
        }
    }
}