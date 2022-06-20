using System.Diagnostics;
using System.IO;
using TracerLibrary;
using ZMachineLibrary;

namespace ZMachineConsole
{
    class Program
    {
        private static readonly ZMachineIO consoleIO = new ConsoleIO();

        static void Main(string[] args)
        {
            Debug.WriteLine("Enter Main()");
            int pos = 0;
            Parameter<string> filePath = new Parameter<string>("");
            Parameter<string> filename = new Parameter<string>("");
            Parameter<string> fileExtension = new Parameter<string>();

            // Get the default path directory

            filePath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            pos = filePath.Value.ToString().LastIndexOf('\\');
            filePath.Value = filePath.Value.ToString().Substring(0, pos);
            filePath.Source = Parameter<string>.SourceType.None;

            // Check if the config file has been paased in and overwrite the registry

            if (args.Length == 1)
            {
                string filenamePath = args[0];
                filenamePath = filenamePath.Trim('"');
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    fileExtension.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filePath.Source = Parameter<string>.SourceType.Command;
                    filenamePath = filenamePath.Substring(0, pos);
                }
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    filePath.Value = filenamePath.Substring(0, pos);
                    filePath.Source = Parameter<string>.SourceType.Command;
                    filename.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filename.Source = Parameter<string>.SourceType.Command;
                }
                else
                {
                    filename.Value = filenamePath;
                    filename.Source = Parameter<string>.SourceType.Command;
                }
                TraceInternal.TraceVerbose("Use command value " + filenamePath);
            }
            else
            {
                for (int item = 1; item < args.Length; item++)
                {
                    switch (args[item])
                    {
                        case "/N":
                        case "--name":
                            filename.Value = args[item + 1];
                            filename.Value = filename.Value.ToString().TrimStart('"');
                            filename.Value = filename.Value.ToString().TrimEnd('"');
                            filename.Source = Parameter<string>.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Name=" + filename);
                            break;
                        case "/P":
                        case "--path":
                            filePath.Value = args[item + 1];
                            filePath.Value = filePath.Value.ToString().TrimStart('"');
                            filePath.Value = filePath.Value.ToString().TrimEnd('"');
                            filePath.Source = Parameter<string>.SourceType.Command;
                            TraceInternal.TraceVerbose("Use command value Path=" + filePath);
                            break;
                    }
                }
            }
            TraceInternal.TraceInformation("Use Path=" + filePath.Value + " Name= " + filename.Value);

            if ((filePath.Value.ToString().Length > 0) && (filename.Value.ToString().Length > 0))
            {
                pos = filename.Value.ToString().LastIndexOf('.');

                string filenamePath = filePath.Value.ToString() + Path.DirectorySeparatorChar + filename.Value+ "." +  fileExtension.Value;
                Machine machine = new Machine(consoleIO);
                FileStream fs = File.OpenRead(filenamePath);
                machine.LoadFile(fs);
                System.Console.Title = filename.Value.ToString() + " - Zmachine[" + machine.Version + "]";
                machine.Run();
            }
            Debug.WriteLine("Exit Main()");
        }

    }
}
