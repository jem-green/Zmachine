using System;
using System.IO;
using ZMachineLibrary;

namespace ZMachineApp
{
    public class RichTextBoxIO : IConsoleIO
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

        private int consoleWidth = 75;
        private int zoneWidth = 15;
        private int compactWidth = 3;
        private int hpos = 0;
        private int vpos = 0;
        private string input = "";
        private string output = "";
        protected readonly object lockObject = new Object();
		private int _lines;
		private readonly ConsoleColor _defaultFore;
		private readonly ConsoleColor _defaultBack;
        private int _window;

        #endregion
        #region Constructors

        public RichTextBoxIO()
		{
			//Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
			//Console.SetCursorPosition(0, Console.WindowHeight-1);
			//_defaultFore = Console.ForegroundColor;
			//_defaultBack = Console.BackgroundColor;
		}

        #endregion
        #region Properties

        public int Compact
        {
            get
            {
                return (compactWidth);
            }
            set
            {
                compactWidth = value;
            }
        }

        public string Input
        {
            set
            {
                // need to wait here while the input is being read
                lock (lockObject)
                {
                    input = input + value;
                }
            }
        }

        public string Output
        {
            get
            {
                string temp;
                // need to wait here while the output is being written
                lock (lockObject)
                {
                    temp = output;
                    output = "";
                }
                return (temp);
            }
        }
		
		public int Hpos
        {
            get
            {
                return (hpos);
            }
        }

        public int Vpos
        {
            get
            {
                return (vpos);
            }
        }

        public int Width
        {
            get
            {
                return (consoleWidth);
            }
            set
            {
                consoleWidth = value;
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

        public int Zone
        {
            get
            {
                return (zoneWidth);
            }
            set
            {
                zoneWidth = value;
            }
        }


        #endregion
        #region Methods

        public void Clear()
        {
        }

        public Cursor Position(int window)
        {
            return (new Cursor(hpos,vpos));
        }

        public void Print(string s)
		{
			lock (lockObject)
            {
                s = s.Replace("\n", "\r\n");
                output = output + s;
            }
            TextEventArgs args = new TextEventArgs(_window, TextEventArgs.TextMode.Add, s);
            OnTextReceived(args);
        }

		public string Read(int max)
		{
            string value = "";
            do
            {
                while (input.Length == 0)
                {
                    System.Threading.Thread.Sleep(250); // Loop until input is entered.
                }
                int pos = 0;
                lock (lockObject)
                {
                    pos = input.IndexOf('\n');
                    if (pos < 0)
                    {
                        pos = input.IndexOf('\r');
                    }
                    if (pos > -1)
                    {
                        // read the input to the first \n or \r then trim the remaining
                        value = input.Substring(0, pos);
                        input = input.Substring(pos + 1, input.Length - pos - 1);
                    }
                }
            }
            while (value.Length == 0);
            return (value);
        }

		public char ReadChar()
		{
            //return Console.ReadKey(true).KeyChar;
            return (' ');
		}

		public void SetCursor(ushort line, ushort column, ushort window)
		{
			//Console.SetCursorPosition(column-1, line-1);
		}

		public void SetWindow(ushort window)
		{
            //if (window == 0)
            //{
            //   Console.SetCursorPosition(0, Console.WindowHeight - 1);
            //}
		}

		public void EraseWindow(ushort window)
		{
		//	ConsoleColor c = Console.BackgroundColor;
		//	Console.BackgroundColor = _defaultBack;
		//	Console.Clear();
		//	Console.BackgroundColor = c;
		}

		public void BufferMode(bool buffer)
		{
		}

		public void SplitWindow(ushort lines)
		{
			_lines = lines;
		}

		public void ShowStatus()
		{
		}

		public void SetTextStyle(TextStyle textStyle)
		{
			switch(textStyle)
			{
				case TextStyle.Roman:
                    {
                        Console.ResetColor();
                        break;
                    }
				case TextStyle.Reverse:
                    {
                        ConsoleColor temp = Console.BackgroundColor;
                        Console.BackgroundColor = Console.ForegroundColor;
                        Console.ForegroundColor = temp;
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
			Console.ForegroundColor = ZColorToConsoleColor(foreground, true);
			Console.BackgroundColor = ZColorToConsoleColor(background, false);
		}

		public void SoundEffect(ushort number)
		{
            if (number == 1)
            {
                Console.Beep(2000, 300);
            }
            else if (number == 2)
            {
                Console.Beep(250, 300);
            }
            else
            {
                throw new Exception("Sound > 2");
            }
		}

		public void Quit()
		{
		}

		private ConsoleColor ZColorToConsoleColor(ZColor c, bool fore)
		{
			switch(c)
			{
				case ZColor.PixelUnderCursor:
				case ZColor.Current:
					return fore ? Console.ForegroundColor : Console.BackgroundColor;
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
			return Console.ForegroundColor;
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
