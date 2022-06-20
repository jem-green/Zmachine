using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using ZMachineLibrary;

namespace ZMachineForm
{
    public class TextBoxIO : IConsoleIO
    {
	    #region Event handling

        /// <summary>
        /// Occurs when the Zmachine recives a message.
        /// </summary>
        public event EventHandler<TextEventArgs> TextReceived;

        /// <summary>
        /// Handles the actual event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextReceived(TextEventArgs e)
        {
            EventHandler<TextEventArgs> handler = TextReceived;
            if (handler != null)
                handler(this, e);
        }

        #endregion
        #region Fields

        // Formatting constraints

        private int _consoleHeight = 80;
        private int _consoleWidth = 75;
        private int _zoneWidth = 15;
        private int _compactWidth = 3;
        private string _input = "";
        private string _output = "";
        protected readonly object _lockObject = new Object();
		private int _lines;
		private readonly ConsoleColor _defaultFore;
		private readonly ConsoleColor _defaultBack;
        private int _window = 1;
        private Cursor[] _cursors;
		
        #endregion
        #region Constructor

        public TextBoxIO()
		{

            _cursors = new Cursor[2];    // Top and bottom region
            _cursors[0] = new Cursor();
            _cursors[1] = new Cursor();
            _window = 1;                // Lower window
		}

        #endregion
        #region Properties

        public int Compact
        {
            get
            {
                return (_compactWidth);
            }
            set
            {
                _compactWidth = value;
            }
        }

        public int Height
        {
            get
            {
                return (_consoleHeight);
            }
			set
            {
                _consoleHeight = value;
            }
        }

		public string Input
        {
            set
            {
                // need to wait here while the input is being read
                lock (_lockObject)
                {
                    _input = _input + value;
                }
            }
        }


        public string Output
        {
            get
            {
                string temp;
                // need to wait here while the output is being written
                lock (_lockObject)
                {
                    temp = _output;
                    _output = "";
                }
                return (temp);
            }
        }

        public int Left
        {
            get
            {
                return (_cursors[_window].Left);
            }
        }

        public int Top
        {
            get
            {
                return (_cursors[_window].Top);
            }
        }

        public int Zone
        {
            get
            {
                return (_zoneWidth);
            }
            set
            {
                _zoneWidth = value;
            }
        }

        public int Width
        {
            get
            {
                return (_consoleWidth);
            }
            set
            {
                _consoleWidth = value;
            }
        }

        public int Window
        {
            get
            {
                return (_window);
            }
            set
            {
                _window = value;
            }
        }



        #endregion
        #region Methods

        public void Clear()
        {
            _input = "";
            _output = "";
        }

        public Cursor Position(int window)
        {
            return (_cursors[window]);
        }

        public void Print(string text)
		{
			lock (_lockObject)
            {
                text = text.Replace("\n", "\r\n");
                _output = _output + text;
            }
            TextEventArgs args = new TextEventArgs(_window, TextEventArgs.TextMode.Add, text);
            OnTextReceived(args);         
		}

		public string Read(int max)
		{
            string value = "";
            do
            {
                while (_input.Length == 0)
                {
                    System.Threading.Thread.Sleep(250); // Loop until input is entered.
                }
                int pos = 0;
                lock (_lockObject)
                {
                    pos = _input.IndexOf('\n');
                    if (pos < 0)
                    {
                        pos = _input.IndexOf('\r');
                    }
                    if (pos > -1)
                    {
                        // read the input to the first \n or \r then trim the remaining
                        value = _input.Substring(0, pos);
                        _input = _input.Substring(pos + 1, _input.Length - pos - 1);
                    }
                }
            }
            while (value.Length == 0);
            _window = 1;
            return (value);
        }

		public char ReadChar()
		{
            throw new NotImplementedException("ReadChar");
            ////return Console.ReadKey(true).KeyChar;
            return (' ');
		}

		public void SetCursor(ushort line, ushort column, ushort window)
		{
            _window = window;
            _cursors[_window].Left = column;
            _cursors[_window].Top = line;
        }

		public void SetWindow(ushort window)
		{
            if (window == 0)
            {
                // indicates that the cursor is reset
                _window = 0;
                _cursors[_window].Left = 0;
                _cursors[_window].Top = 0;
            }
            else if (window == 1)
            {
                _window = 1;
            }
        }

		public void EraseWindow(ushort window)
        {
            TextEventArgs args = new TextEventArgs(_window, TextEventArgs.TextMode.Clear, "");
            OnTextReceived(args);
        }

		public void BufferMode(bool buffer)
		{
            throw new NotImplementedException("BufferMode");
		}

		public void SplitWindow(ushort lines)
		{
            _lines = lines;
            TextEventArgs args = new TextEventArgs(_window, TextEventArgs.TextMode.Lines, "");
            OnTextReceived(args);
        }

		public void ShowStatus()
		{
            throw new NotImplementedException("ShowStatus");
        }

		public void SetTextStyle(TextStyle textStyle)
		{
            switch (textStyle)
			{
				case TextStyle.Roman:
                    {
                        break;
                    }
				case TextStyle.Reverse:
                    {
                        break;
                    }
				case TextStyle.Bold:
                    {
                        break;
                    }
				case TextStyle.Italic:
                    {
                        break;
                    }
				case TextStyle.FixedPitch:
                    {
                        break;
                    }
				default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(textStyle), textStyle, null);
                    }
			}
		}

		public void SetColor(ZColor foreground, ZColor background)
		{
            throw new NotImplementedException("SetColor");
		}

		public void SoundEffect(ushort number)
		{
            if (number == 1)
            {
                SystemSounds.Beep.Play();
            }
            else if (number == 2)
            {
                SystemSounds.Exclamation.Play();
            }
            else
            {
                throw new Exception("Sound > 2");
            }
		}

		public void Quit()
		{
            throw new NotImplementedException("Quit");
        }

		private ConsoleColor ZColorToConsoleColor(ZColor c, bool fore)
		{
			switch(c)
			{
				case ZColor.PixelUnderCursor:
				case ZColor.Current:
					return fore ? _defaultFore : _defaultBack;
                case ZColor.Default:
					return fore ? _defaultFore : _defaultBack;
				case ZColor.Black:
					return ConsoleColor.Black;
				case ZColor.Red:
					return ConsoleColor.Red;
				case ZColor.Green:
					return ConsoleColor.Green;
				case ZColor.Yellow:
					return ConsoleColor.Yellow;
				case ZColor.Blue:
					return ConsoleColor.Blue;
				case ZColor.Magenta:
					return ConsoleColor.Magenta;
				case ZColor.Cyan:
					return ConsoleColor.Cyan;
				case ZColor.White:
					return ConsoleColor.White;
				case ZColor.DarkishGrey:
					return ConsoleColor.DarkGray;
				case ZColor.LightGrey:
					return ConsoleColor.Gray;
				case ZColor.MediumGrey:
					return ConsoleColor.Gray;
				case ZColor.DarkGrey:
					return ConsoleColor.DarkGray;
			}
			return _defaultFore;
		}

		public bool Save(Stream s)
		{
			FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "save"));
			s.CopyTo(fs);
			fs.Close();
			return true;
		}

		public Stream Restore()
		{
			FileStream fs = File.OpenRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "save"));
			return fs;
		}
	}
    #endregion
}
