using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.Forms
{

    public partial class HLLiveOutputViewerForm : Form
    {

        private const string CONSOLE_BIN_CONFIG_PATH = "runtime_path.txt";
        private const string CONSOLE_BUILD_DEFAULT_ARGS = "[build] -export -i {0} -steps HL-expr bin -clean false";
        private const string CONSOLE_RUN_DEFAULT_ARGS = "[build;run] -export -i {0} -steps HL-expr bin -clean false";
        private const string CONSOLE_RUN_ARG_CONFIG_PATH = "runtime_args.txt";
        private const string CONSOLE_BUILD_ARG_CONFIG_PATH = "runtime_build_args.txt";
        private const string ENTRY_VHL_FILE = "src";
        private const string ENTRY_FILE_NAME = ENTRY_VHL_FILE + ".vhl";
        private const string SOURCE_FOLDER = "src";

        private readonly Dictionary<string, RichTextBox> CodeInPages = new Dictionary<string, RichTextBox>();
        private readonly Dictionary<string, RichTextBox> CodeOutPages = new Dictionary<string, RichTextBox>();
        private bool IsBuilding;
        private TextWriter ConsoleIn;
        private Process ConsoleProcess;

        private string BuildArgs => File.ReadAllText(CONSOLE_BUILD_ARG_CONFIG_PATH);

        private string RunArgs => File.ReadAllText(CONSOLE_RUN_ARG_CONFIG_PATH);

        #region Public

        public HLLiveOutputViewerForm()
        {
            if (!File.Exists(CONSOLE_BUILD_ARG_CONFIG_PATH))
            {
                File.WriteAllText(CONSOLE_BUILD_ARG_CONFIG_PATH, CONSOLE_BUILD_DEFAULT_ARGS);
            }

            if (!File.Exists(CONSOLE_RUN_ARG_CONFIG_PATH))
            {
                File.WriteAllText(CONSOLE_RUN_ARG_CONFIG_PATH, CONSOLE_RUN_DEFAULT_ARGS);
            }

            InitializeComponent();

            EventManager.Initialize();
            EventManager < WarningEvent >.OnEventReceive += x => WriteConsoleOut( $"[WARNING] [{x.EventKey}] {x.Message}" );
            Logger.OnLogReceive += (x, y) => WriteConsoleOut($"[LOG] [{x}] {y}");

            DisableConsoleIn();

            tbConsoleIn.KeyUp += ConsoleInKeyHandler;

            Directory.CreateDirectory(SOURCE_FOLDER);

            if (!File.Exists(GetEntryPath()))
            {
                File.WriteAllText(GetEntryPath(), "");
            }

            CheckForIllegalCrossThreadCalls = false;
            (RichTextBox rIn, RichTextBox rOut) = GetPage(Path.Combine(SOURCE_FOLDER, ENTRY_VHL_FILE));
            BuildHL(GetEntryPath());
            SizeChanged += HLLiveOutputViewerForm_SizeChanged;
            Closing += (a,b) => ConsoleProcess?.Kill();
        }

        #endregion

        #region Private

        private void btnSendInput_Click(object sender, EventArgs e)
        {
            WriteConsoleIn(tbConsoleIn.Text);
            tbConsoleIn.Text = "";
        }

        private void BuildHL(string src)
        {
            try
            {
                ExpressionParser p = new ExpressionParser();
                string file = File.ReadAllText(src);

                string newFile = Path.Combine(
                                              Path.GetDirectoryName(Path.GetFullPath(src)),
                                              Path.GetFileNameWithoutExtension(src)
                                             ) +
                                 ".vasm";

                HLCompilation c = p.Parse(file, Path.GetDirectoryName(Path.GetFullPath(src)));
                c.OnCompiledIncludedScript += OnFileCompiled;

                File.WriteAllText(
                                  newFile,
                                  c.Parse()
                                 );

                HLCompilation.ResetCounter();
                UpdateOutputPages();
            }
            catch (Exception e)
            {
            }
        }

        private void BuildProgram(string consolePath)
        {
            RunConsoleProcess(
                              consolePath,
                              string.Format(BuildArgs, GetEntryPath())
                             );
        }

        private void ConsoleInKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                WriteConsoleIn(tbConsoleIn.Text);
                tbConsoleIn.Text = "";
            }
        }

        private void DisableConsoleIn()
        {
            ConsoleIn = null;
            panelConsoleIn.Enabled = false;
        }

        private void EnableConsoleIn(TextWriter tw)
        {
            ConsoleIn = tw;
            panelConsoleIn.Enabled = true;
        }

        private (RichTextBox input, RichTextBox output) GeneratePages(string name)
        {
            rtbConsoleOut.AppendText($"Adding Pages: {name}\n");
            TabPage pIn = new TabPage(name + ".vhl");
            TabPage pOut = new TabPage(name + ".vasm");
            RichTextBox tbIn = new RichTextBox();
            RichTextBox tbOut = new RichTextBox();
            tbIn.Parent = pIn;
            tbIn.Dock = DockStyle.Fill;
            tbOut.Parent = pOut;
            tbOut.Dock = DockStyle.Fill;
            tbOut.ReadOnly = true;
            tbIn.AcceptsTab = true;

            tbIn.TextChanged += (sender, args) =>
                                {
                                    File.WriteAllText(name + ".vhl", tbIn.Text);
                                    BuildHL(name + ".vhl");
                                };

            tbIn.KeyUp += ProcessKeyUpEvent;
            tcCodeIn.TabPages.Add(pIn);
            tcCodeOut.TabPages.Add(pOut);

            return (tbIn, tbOut);
        }

        private string GetEntryPath()
        {
            return Path.Combine(SOURCE_FOLDER, ENTRY_FILE_NAME);
        }

        private (RichTextBox input, RichTextBox output) GetPage(string name)
        {
            RichTextBox inp;
            RichTextBox outp;

            if (CodeInPages.ContainsKey(name))
            {
                inp = CodeInPages[name];
                outp = CodeOutPages[name];
            }
            else
            {
                if (string.IsNullOrEmpty(name) || !File.Exists(name + ".vhl"))
                {
                    return (null, null);
                }

                (inp, outp) = GeneratePages(name);

                inp.Text = File.ReadAllText(name + ".vhl");

                if (File.Exists(name + ".vasm"))
                {
                    outp.Text = File.ReadAllText(name + ".vasm");
                }
                else
                {
                    outp.Text = $"File: '{name}.vasm' not found";
                }

                CodeInPages[name] = inp;
                CodeOutPages[name] = outp;
            }

            return (inp, outp);
        }

        private void HLLiveOutputViewerForm_SizeChanged(object sender, EventArgs e)
        {
            panelRight.Width = Width / 2;
        }

        private void OnConsoleProcessExit(object sender, EventArgs e)
        {
            DisableConsoleIn();
            ConsoleProcess = null;
            IsBuilding = false;
            UpdateOutputPages();
        }

        private void OnFileCompiled(string arg1, string arg2)
        {
            BuildHL(arg1);

            Uri u = new Uri(
                            Path.Combine(Path.GetDirectoryName(arg1), Path.GetFileNameWithoutExtension(arg1)),
                            UriKind.Absolute
                           );

            Uri u1 = new Uri(Path.GetFullPath(SOURCE_FOLDER), UriKind.Absolute);
            Uri ret = u1.MakeRelativeUri(u);
            GetPage(ret.OriginalString);
        }

        private void ProcessKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (File.Exists(CONSOLE_BIN_CONFIG_PATH))
                {
                    string cPath = File.ReadAllText(CONSOLE_BIN_CONFIG_PATH);

                    if (File.Exists(cPath))
                    {
                        RunProgram(cPath);
                    }
                    else
                    {
                        MessageBox.Show(
                                        $"No Console Runtime Set. Create {CONSOLE_BIN_CONFIG_PATH} with path to Console"
                                       );
                    }

                }
            }
            else if (e.KeyCode == Keys.F6)
            {
                if (File.Exists(CONSOLE_BIN_CONFIG_PATH))
                {
                    string cPath = File.ReadAllText(CONSOLE_BIN_CONFIG_PATH);

                    if (File.Exists(cPath))
                    {
                        BuildProgram(cPath);
                    }
                    else
                    {
                        MessageBox.Show(
                                        $"No Console Runtime Set. Create {CONSOLE_BIN_CONFIG_PATH} with path to Console"
                                       );
                    }
                }
            }
            else if(e.KeyCode == Keys.F7 && ConsoleProcess != null)
            {
                ConsoleProcess.Kill();
                WriteConsoleOut("");
                WriteConsoleOut("Console Process Killed");
            }
        }

        private void RunConsoleProcess(string path, string args)
        {
            if (IsBuilding)
            {
                return;
            }

            rtbConsoleOut.Text = "";
            IsBuilding = true;
            ProcessStartInfo psi = new ProcessStartInfo(path, args);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            Process p = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            ConsoleProcess = p;

            p.OutputDataReceived += WriteConsoleOutput;
            p.ErrorDataReceived += WriteConsoleError;
            p.Exited += OnConsoleProcessExit;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            EnableConsoleIn(p.StandardInput);
        }

        private void RunProgram(string consolePath)
        {
            RunConsoleProcess(
                              consolePath,
                              string.Format(RunArgs, GetEntryPath())
                             );
        }

        private void UpdateInputPages()
        {
            foreach (KeyValuePair<string, RichTextBox> keyValuePair in CodeInPages)
            {
                UpdatePage(keyValuePair.Key + ".vhl", keyValuePair.Value);
            }
        }

        private void UpdateOutputPages()
        {
            foreach (KeyValuePair<string, RichTextBox> keyValuePair in CodeOutPages)
            {
                UpdatePage(keyValuePair.Key + ".vasm", keyValuePair.Value);
            }
        }

        private void UpdatePage(string file, RichTextBox target)
        {
            if (File.Exists(file))
            {
                target.Text = File.ReadAllText(file);
            }
            else
            {
                target.Text = $"File: '{file}' not found";
            }
        }

        private void WriteConsoleError(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            rtbConsoleOut.AppendText($"[ERR]{e.Data}\n");
            rtbConsoleOut.ScrollToCaret();
        }

        private void WriteConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            WriteConsoleOut( $"[LOG]{e.Data}" );
        }
        
        private void WriteConsoleOut(string line)
        {
            rtbConsoleOut.AppendText($"{line}\n");
            rtbConsoleOut.ScrollToCaret();

        }

        private void WriteConsoleIn(string line)
        {
            if (ConsoleIn == null)
            {
                return;
            }

            ConsoleIn.WriteLine(line);
        }

        #endregion

    }

}
