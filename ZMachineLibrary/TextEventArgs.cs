using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace ZMachineLibrary
{
    public class TextEventArgs : EventArgs
    {
        #region Fields
        private string _text = "";
        private int _window = 0;
        private TextMode _textMode = TextMode.Add;

        public enum TextMode : int
        {
            Add = 0,
            Delete = 1,
            Clear = 2,
            Lines = 3
        }

        #endregion
        #region Constructor
        public TextEventArgs(int window, TextMode textMode ,string text)
        {
            _window = window;
            _textMode = textMode;
            _text = text;
        }
        #endregion
        #region Properties

        public int Window
        {
            set
            {
                _window = value;
            }
            get
            {
                return (_window);
            }
        }

        public TextMode Mode
        {
            set
            {
                _textMode = value;
            }
            get
            {
                return (_textMode);
            }
        }

        public string Text
        {
            set
            {
                _text = value;
            }
            get
            {
                return (_text);
            }
        }


        #endregion
    }
}

