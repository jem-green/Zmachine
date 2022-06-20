using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZMachineLibrary;
using System.Threading;
using ZMachineForm.Properties;
using System.IO;
using System.Diagnostics;
using TracerLibrary;

namespace ZMachineForm
{
    public partial class ConsoleForm : Form
    {
        // Prepare the zMachine
        static ZMachineIO textBoxIO = null;
        Machine machine = null;
        int pos = 0;
        bool stopped = true;

        
        // Declare a delegate used to communicate with the UI thread
        public delegate void UpdateTextDelegate(int window, TextEventArgs.TextMode textMode, string text);
        public UpdateTextDelegate updateTextDelegate;

        // Declare our worker thread
        private Thread workerThread = null;

        // Manage the inputs
        string value = "";

        // Most recently used
        protected MruStripMenu mruMenu;

        public ConsoleForm(string filepath, string name, string extension)
        {
            Debug.WriteLine("In ConsoleForm()");

            InitializeComponent();

            this.Icon = Resources.Infocom;

            textBoxIO = new TextBoxIO();
            textBoxIO.TextReceived += new EventHandler<TextEventArgs>(OnMessageReceived);

            // Initialise the delegate
            updateTextDelegate = new UpdateTextDelegate(UpdateText);

            // Add most recent used
            mruMenu = new MruStripMenuInline(fileMenuItem, recentFileToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), 4);
            LoadFiles();

            if ((filepath.Length > 0) && (name.Length > 0))
            {
                lowerTextBox.Text = "";
                string filenamePath = "";
                filenamePath = filepath + Path.DirectorySeparatorChar + name + "." + extension;
                try
                {
                	machine = new Machine(textBoxIO);
                	FileStream fs = File.OpenRead(filenamePath);
                    mruMenu.AddFile(filenamePath);
                	machine.LoadFile(fs);
                    this.Text = name + " -Zmachine[" + machine.Version + "]";
                    stopped = false;
                    this.workerThread = new Thread(new ThreadStart(this.Run));
                    this.workerThread.Start();
                    lowerTextBox.Visible = true;
                    lowerTextBox.Enabled = true;
                    upperTextBox.Visible = true;
                    upperTextBox.Enabled = true;
                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }

			Debug.WriteLine("Out ConsoleForm()");
        }

        private void OnMruFile(int number, String filenamePath)
        {
            Debug.WriteLine("In OnMruFile");

            string path = "";
            string filename = "";
            string extension = "";

            lowerTextBox.Enabled = false;
            lowerTextBox.Visible = false;
            upperTextBox.Visible = false;
            upperTextBox.Enabled = false;

            if (File.Exists(filenamePath) == true)
            {
                mruMenu.SetFirstFile(number);
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filenamePath = filenamePath.Substring(0, pos);
                }
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path = filenamePath.Substring(0, pos);
                    filename = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    path = filenamePath;
                }
                TraceInternal.TraceVerbose("Use Path=" + path + " Name= " + filename + " Extension=" +  extension);

                lowerTextBox.Text = "";
                
                filenamePath = path + Path.DirectorySeparatorChar + filename + "." + extension;
                try
                {
                    machine = new Machine(textBoxIO);
                    FileStream fs = File.OpenRead(filenamePath);
                    machine.LoadFile(fs);
                    this.Text = filename + " -Zmachine[" + machine.Version + "]";
                    stopped = false;
                    this.workerThread = new Thread(new ThreadStart(this.Run));
                    this.workerThread.Start();
                    lowerTextBox.Visible = true;
                    lowerTextBox.Enabled = true;
                    upperTextBox.Visible = true;
                    upperTextBox.Enabled = true;
                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            else
            {
                mruMenu.RemoveFile(number);
            }

            Debug.WriteLine("Out OnMruFile");
        }

        private void UpdateText(int window, TextEventArgs.TextMode textMode, string text)
        {
            Debug.WriteLine("In UpdateText()");

            switch (textMode)
            {
                case TextEventArgs.TextMode.Lines:
                    {
                        upperTextBox.Height = 0;
                        lowerTextBox.Top = consoleMenuStrip.Height + upperTextBox.Height;
                        lowerTextBox.Height = ClientSize.Height - consoleMenuStrip.Height - upperTextBox.Height;
                        break;
                    }
                case TextEventArgs.TextMode.Add:
                    {
                        if (window == 0)
                        {
                            this.upperTextBox.AppendText(text);
                        }
                        else
                        {
                            this.lowerTextBox.AppendText(text);
                        }
                        break;
                    }
            }
            Debug.WriteLine("Out UpdateText()");
        }

        // Define the event handlers.
        private void OnMessageReceived(object source, TextEventArgs e)
        {
            Debug.WriteLine("In OnMessageReceived()");

            if (e.Text.Length > 0)
            {
                this.Invoke(updateTextDelegate, e.Window, e.Mode, e.Text);
            }

            Debug.WriteLine("Out OnMessageReceived()");
        }

        private void Run()
        {
            machine.Run();
        }

        /// <summary>
        /// Intercept the key press events and manage the content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LowerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("In LowerTextBox_KeyPress()");

            char chr = e.KeyChar;
            if (chr == '\r')
            {
                value = value + chr;
                textBoxIO.Input = value;
                this.lowerTextBox.AppendText("\r" + "\n");
                value = "";
            }
            else if (chr == '\b')
            {
                this.lowerTextBox.Text = this.lowerTextBox.Text.Substring(0, this.lowerTextBox.Text.Length - 1);
                this.lowerTextBox.SelectionStart = lowerTextBox.Text.Length;
                this.lowerTextBox.ScrollToCaret();
                value = value.Substring(0, value.Length - 1);
            }
            else
            {
                value = value + chr;
                this.lowerTextBox.AppendText(Convert.ToString(chr));
            }

            Debug.WriteLine("Out LowerTextBox_KeyPress()");
        }

        private void FileOpenMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FileOpenMenuItem_Click()");

            string path = "";
            string filename = "";
            lowerTextBox.Enabled = false;
            lowerTextBox.Visible = false;
            if (stopped == false)
            {
                workerThread.Abort();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "zMachine (*.z?)|*.z?|Specific (*.zil)|*.zil|Generic (*.dat)|*.dat|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filenamePath = openFileDialog.FileName;
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path = filenamePath.Substring(0, pos);
                    filename = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    filename = filenamePath;
                }
               TraceInternal.TraceVerbose("Use Name=" + filename + " Path=" + path);

                lowerTextBox.Text = "";
                filenamePath = path + Path.DirectorySeparatorChar + filename;

                try
                {
                    machine = new Machine(textBoxIO);
                    FileStream fs = File.OpenRead(filenamePath);
                    mruMenu.AddFile(filenamePath);
                    machine.LoadFile(fs);
                    this.Text = filename + " -Zmachine[" + machine.Version + "]";
                    stopped = false;
                    this.workerThread = new Thread(new ThreadStart(this.Run));
                    textBoxIO.Clear();
                    this.workerThread.Start();
                    lowerTextBox.Visible = true;
                    lowerTextBox.Enabled = true;
                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            Debug.WriteLine("Out FileOpenMenuItem_Click()");
        }

        private void FormatFontMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FormatFontMenuItem_Click()");

            FontDialog fontDialog = new FontDialog
            {
                Font = lowerTextBox.Font,
                ShowColor = true,
                Color = lowerTextBox.ForeColor
            };

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                Font font = fontDialog.Font;
                Color color = fontDialog.Color;
                lowerTextBox.Font = font;
                lowerTextBox.ForeColor = color;
                Properties.Settings.Default.ConsoleFont = font;
                Properties.Settings.Default.ConsoleFontColor = color;
                // Save settings
                Settings.Default.Save();
            }

            Debug.WriteLine("In FormatFontMenuItem_Click()");
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_Load()");

            Settings.Default.Upgrade();

            // Set window location
            if (Settings.Default.ConsoleLocation != null)
            {
                this.Location = Settings.Default.ConsoleLocation;
            }

            // Fixed windows size

            this.Width = textBoxIO.Width;

            // Set window size
            if (Settings.Default.ConsoleSize != null)
            {
                this.Size = Settings.Default.ConsoleSize;
            }

            // Set Console font
            if (Settings.Default.ConsoleFont != null)
            {
                this.lowerTextBox.Font = Settings.Default.ConsoleFont;
                upperTextBox.Font = Settings.Default.ConsoleFont;
            }

            // Set Console font color
            if (Settings.Default.ConsoleFontColor != null)
            {
                this.lowerTextBox.ForeColor = Settings.Default.ConsoleFontColor;
                this.upperTextBox.ForeColor = Settings.Default.ConsoleFontColor;
            }

            // Set Console color
            if (Settings.Default.ConsoleColor != null)
            {
                this.lowerTextBox.BackColor = Settings.Default.ConsoleColor;
                this.upperTextBox.BackColor = Settings.Default.ConsoleColor;
            }

            Debug.WriteLine("Out ConsoleForm_Load()");

        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_FormClosing()");

            // Need to stop the thread
            // think i will try a better approach

            if (stopped == false)
            {
                workerThread.Abort();
            }

            // Copy window location to app settings
            Settings.Default.ConsoleLocation = this.Location;

            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.ConsoleSize = this.Size;
            }
            else
            {
                Settings.Default.ConsoleSize = this.RestoreBounds.Size;
            }

            // Copy console font type to app settings
            Settings.Default.ConsoleFont = this.lowerTextBox.Font;

            // Copy console font color to app settings
            Settings.Default.ConsoleFontColor = this.lowerTextBox.ForeColor;

            // Copy console color to app settings
            Settings.Default.ConsoleColor = this.lowerTextBox.BackColor;

            // Safe Mru
            SaveFiles();

            // Save settings
            Settings.Default.Save();

            // Upgrade settings
            Settings.Default.Reload();

            Debug.WriteLine("Out ConsoleForm_FormClosing()");
        }

        private void FileExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormatColorMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FormatColorMenuItem_Click()");

            ColorDialog colorDialog = new ColorDialog
            {
                Color = lowerTextBox.BackColor
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color color = colorDialog.Color;
                lowerTextBox.BackColor = color;
                Properties.Settings.Default.ConsoleColor = color;
            }

            Debug.WriteLine("Out FormatColorMenuItem_Click()");
        }

        private void LoadFiles()
        {
            Debug.WriteLine("In LoadFiles()");

            Debug.WriteLine("Files " + Properties.Settings.Default.FileCount);

            string file = "";
            string property = "";
            for (int i = 0; i < 4; i++)
            {
                property = "File" + (i + 1);
                file = (string)Properties.Settings.Default[property];
                if (file != "")
                {
                    mruMenu.AddFile(file);
                    Debug.WriteLine("Load " + file);
                }
            }

            Debug.WriteLine("Out LoadFiles()");
        }

        public void SaveFiles()
        {
            Debug.WriteLine("In SaveFiles()");

            string[] files = mruMenu.GetFiles();
            string property = "";
            Properties.Settings.Default["FileCount"] = files.Length;
            Debug.WriteLine("Files=" + files.Length);
            for (int i=0; i < 4; i++)
            {
                property = "File" + (i + 1);
                if (i < files.Length)
                {
                    Properties.Settings.Default[property] = files[i];
                    Debug.WriteLine("Save " + property + "="+ files[i]);
                }
                else
                {
                    Properties.Settings.Default[property] = "";
                    Debug.WriteLine("Save " + property + "=");
                }
            }

            Debug.WriteLine("Out SaveFiles()");
        }

        private void ConsoleForm_Resize(object sender, EventArgs e)
        {

            // Reset some of the controls to test

            upperTextBox.Height = 0;
            lowerTextBox.Top = consoleMenuStrip.Height + upperTextBox.Height;
            lowerTextBox.Left = 0;
            lowerTextBox.Width = ClientSize.Width;
            lowerTextBox.Height = ClientSize.Height - consoleMenuStrip.Height - upperTextBox.Height;
        }
    }
}