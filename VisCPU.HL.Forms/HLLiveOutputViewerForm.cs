using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.Forms
{

    public partial class HLLiveOutputViewerForm : Form
    {

        private readonly ConcurrentQueue<Action> taskQueue = new ConcurrentQueue<Action>();
        private Task taskProcessor = null;
        private const string CONSOLE_BUILD_DEFAULT_ARGS = "[build] -export -i {0} -steps HL-expr bin -clean false";
        private const string CONSOLE_RUN_DEFAULT_ARGS = "[build;run] -export -i {0} -steps HL-expr bin -clean false";

        private readonly Dictionary<string, RichTextBox> m_CodeInPages = new Dictionary<string, RichTextBox>();
        private readonly Dictionary<string, RichTextBox> m_CodeOutPages = new Dictionary<string, RichTextBox>();
        private bool m_IsBuilding;
        private TextWriter m_ConsoleIn;
        private Process m_ConsoleProcess;
        private string m_SourceFolder = null;

        private bool m_EnableIO = true;
        private readonly ConcurrentQueue<Action> EnableIOQueue = new ConcurrentQueue<Action>();
        private readonly ConcurrentQueue<Action> UIThreadQueue = new ConcurrentQueue<Action>();

        private readonly object m_StartLock = new object();
        private bool m_StartFlag = false;

        private static string ConsoleBinConfigPath => Path.Combine(Application.StartupPath, "runtime_path.txt");

        private string ConsoleRuntimePath =>
            Path.Combine( Application.StartupPath, File.ReadAllText( ConsoleBinConfigPath ) );

        private static string ConsoleRunArgConfigPath => Path.Combine(Application.StartupPath, "runtime_args.txt");

        private static string ConsoleBuildArgConfigPath =>
            Path.Combine(Application.StartupPath, "runtime_build_args.txt");

        private static string LastWorkingDirConfig => Path.Combine(Application.StartupPath, "last_working_dir.txt");

        private string EntryVhlFile => Path.Combine(m_SourceFolder, "src");

        private string EntryFileName => EntryVhlFile + ".vhl";

        private string BuildArgs => File.ReadAllText(ConsoleBuildArgConfigPath);

        private string RunArgs => File.ReadAllText(ConsoleRunArgConfigPath);

        #region Public

        
        public HLLiveOutputViewerForm(string file = null)
        {
            if (!File.Exists(ConsoleBuildArgConfigPath))
            {
                File.WriteAllText(ConsoleBuildArgConfigPath, CONSOLE_BUILD_DEFAULT_ARGS);
            }

            if (!File.Exists(ConsoleRunArgConfigPath))
            {
                File.WriteAllText(ConsoleRunArgConfigPath, CONSOLE_RUN_DEFAULT_ARGS);
            }

            InitializeComponent();

            OpenFolder(file);

            EventManager.RegisterDefaultHandlers();

            EventManager<WarningEvent>.OnEventReceive +=
                x => WriteConsoleOut($"[WARNING] [{x.EventKey}] {x.Message}");

            Logger.OnLogReceive += (x, y) => WriteConsoleOut($"[LOG] [{x}] {y}");

            DisableConsoleIn();

            tbConsoleIn.KeyUp += ConsoleInKeyHandler;

            Directory.CreateDirectory(m_SourceFolder);

            if (!File.Exists(GetEntryPath()))
            {
                File.WriteAllText(GetEntryPath(), "");
            }

            CheckForIllegalCrossThreadCalls = false;
            (RichTextBox rIn, RichTextBox rOut) = GetPage(Path.Combine(m_SourceFolder, EntryVhlFile));

            SizeChanged += HLLiveOutputViewerForm_SizeChanged;
            Closing += (a, b) => m_ConsoleProcess?.Kill();
        }

        #endregion

        #region Private

        private void btnSendInput_Click(object sender, EventArgs e)
        {
            WriteConsoleIn(tbConsoleIn.Text);
            tbConsoleIn.Text = "";
        }

        private void BuildAndRunProject()
        {
            if (File.Exists(ConsoleBinConfigPath))
            {

                if (File.Exists(ConsoleRuntimePath))
                {
                    RunConsoleProcess(
                                      ConsoleRuntimePath,
                                      string.Format(RunArgs, GetEntryPath())
                                     );
                }
                else
                {
                    MessageBox.Show(
                                    $"No Console Runtime Set. File {ConsoleRuntimePath} does not exist"
                                   );
                }
            }
            else
            {
                MessageBox.Show(
                                $"No Console Runtime Set. Create {ConsoleBinConfigPath} with path to Console"
                               );
            }
        }

        private void BuildProject()
        {
            if (File.Exists(ConsoleBinConfigPath))
            {

                if (File.Exists(ConsoleRuntimePath))
                {
                    RunConsoleProcess(
                                      ConsoleRuntimePath,
                                      string.Format(BuildArgs, GetEntryPath())
                                     );
                }
                else
                {
                    MessageBox.Show(
                                    $"No Console Runtime Set. File {ConsoleRuntimePath} does not exist"
                                   );
                }
            }
            else
            {
                MessageBox.Show(
                                $"No Console Runtime Set. Create {ConsoleBinConfigPath} with path to Console"
                               );
            }
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
            m_ConsoleIn = null;
            EnableConsoleInControls(false);
        }

        private void EnableConsoleIn(TextWriter tw)
        {
            m_ConsoleIn = tw;
            EnableConsoleInControls(true);
        }

        private void EnableConsoleInControls(bool enable)
        {
            tbConsoleIn.Enabled = enable;
            btnSendInput.Enabled = enable;
        }

        private void EnqueueTask(Action t)
        {
            taskQueue.Enqueue(t);

            if (taskProcessor == null)
            {
                taskProcessor = Task.Run(ProcessTasks);
            }
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
                                    if (m_EnableIO)
                                    {
                                        UpdateIO(name, tbIn);
                                    }
                                    else
                                    {
                                        EnableIOQueue.Enqueue(() => UpdateIO(name, tbIn));
                                    }
                                };

            tbIn.KeyUp += ProcessKeyUpEvent;
            tcCodeIn.TabPages.Add(pIn);
            tcCodeOut.TabPages.Add(pOut);

            return (tbIn, tbOut);
        }

        private string GetEntryPath()
        {
            return Path.Combine(m_SourceFolder, EntryFileName);
        }

        private (RichTextBox input, RichTextBox output) GetPage(string name)
        {
            RichTextBox inp;
            RichTextBox outp;

            if (m_CodeInPages.ContainsKey(name))
            {
                inp = m_CodeInPages[name];
                outp = m_CodeOutPages[name];
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

                m_CodeInPages[name] = inp;
                m_CodeOutPages[name] = outp;
            }

            return (inp, outp);
        }

        private void HLLiveOutputViewerForm_Load(object sender, EventArgs e)
        {
            Startup();
        }

        private void HLLiveOutputViewerForm_SizeChanged(object sender, EventArgs e)
        {
            panelRight.Width = Width / 2;
        }

        private void OnConsoleProcessExit(object sender, EventArgs e)
        {
            DisableConsoleIn();
            m_ConsoleProcess = null;
            m_IsBuilding = false;
            UpdateOutputPages();
        }

        private void OnFileCompiled(string arg1, string arg2)
        {
            RunEditorInternalHLBuild(arg1);

            UIThreadQueue.Enqueue(
                                  () =>
                                  {
                                      Uri u = new Uri(
                                                      Path.Combine(
                                                                   Path.GetDirectoryName(arg1),
                                                                   Path.GetFileNameWithoutExtension(arg1)
                                                                  ),
                                                      UriKind.Absolute
                                                     );

                                      Uri u1 = new Uri(
                                                       Path.GetFullPath(m_SourceFolder),
                                                       UriKind.Absolute
                                                      );

                                      Uri ret = u1.MakeRelativeUri(u);
                                      GetPage(ret.OriginalString);
                                  }
                                 );
        }

        private void OpenFolder(string file)
        {
            if ( file != null )
            {
                File.WriteAllText( LastWorkingDirConfig, file );
            }
            
            if (File.Exists(LastWorkingDirConfig))
            {
                m_SourceFolder = File.ReadAllText(LastWorkingDirConfig);

                if (Directory.Exists(m_SourceFolder))
                {
                    Directory.SetCurrentDirectory(m_SourceFolder);

                    return;
                }
            }

            DialogResult r = DialogResult.None;

            while (r != DialogResult.OK)
            {
                r = fbdSelectDir.ShowDialog();
            }

            m_SourceFolder = fbdSelectDir.SelectedPath + "/";
            Directory.SetCurrentDirectory(m_SourceFolder);
            File.WriteAllText(LastWorkingDirConfig, m_SourceFolder);
        }

        private void ProcessIOQueue()
        {
            foreach (Action action in EnableIOQueue)
            {
                action();
            }
        }

        private void ProcessKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                BuildAndRunProject();
            }
            else if (e.KeyCode == Keys.F6)
            {
                BuildProject();
            }
            else if (e.KeyCode == Keys.F7 && m_ConsoleProcess != null)
            {
                TerminateRuntimeProcess();
            }
            else if (e.KeyCode == Keys.F8)
            {
                RefreshEditor();
            }
            else if (e.KeyCode == Keys.F9)
            {
                CleanProjectFolder();
            }
        }

        private void ProcessTasks()
        {
            while (!taskQueue.IsEmpty)
            {
                if (taskQueue.TryDequeue(out Action res))
                {
                    res();
                }
            }
        }

        private void ProcessUIThreadQueue()
        {
            while (!UIThreadQueue.IsEmpty)
            {
                if (UIThreadQueue.TryDequeue(out Action res))
                {
                    res();
                }
            }
        }

        private void RefreshEditor()
        {
            EnqueueTask(
                        () =>
                        {
                            RunEditorInternalHLBuild(GetEntryPath());
                            ProcessUIThreadQueue();
                            UpdateInputPages();
                            UpdateOutputPages();
                            WriteConsoleOut("");
                            WriteConsoleOut("Updated Pages");
                        }
                       );
        }

        private void RunConsoleProcess(string path, string args)
        {
            if (m_IsBuilding)
            {
                return;
            }

            rtbConsoleOut.Text = "";
            m_IsBuilding = true;
            ProcessStartInfo psi = new ProcessStartInfo(path, args);
            psi.WorkingDirectory = Path.GetDirectoryName(path);
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

            m_ConsoleProcess = p;

            p.OutputDataReceived += WriteConsoleOutput;
            p.ErrorDataReceived += WriteConsoleError;
            p.Exited += OnConsoleProcessExit;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            EnableConsoleIn(p.StandardInput);
        }

        private void RunEditorInternalHLBuild(string src)
        {
            try
            {
                SetStatus("Building " + src, 0);
                ExpressionParser p = new ExpressionParser();
                string file = File.ReadAllText(src);

                string newFile = Path.Combine(
                                              Path.GetDirectoryName(Path.GetFullPath(src)),
                                              Path.GetFileNameWithoutExtension(src)
                                             ) +
                                 ".vasm";

                HLCompilation c = p.Parse(file, Path.GetDirectoryName(Path.GetFullPath(src)));
                c.OnCompiledIncludedScript += OnFileCompiled;
                SetStatus("Updating Pages " + src, 0.75f);

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

            SetStatus("Build Complete " + src, 1);
        }

        private void SetStatus(string taskDesc, float process)
        {
            tsProgress.Value = (int)(process * 100);
            tslPercentage.Text = $"{Math.Round(process * 100, 1)}%";
            tslCurrentTask.Text = $"Task: {taskDesc}";
            Application.DoEvents();
        }

        private void Startup()
        {
            EnqueueTask(
                        () =>
                        {
                            RunEditorInternalHLBuild(GetEntryPath());

                            lock (m_StartLock)
                            {
                                m_StartFlag = true;
                            }
                        }
                       );

            while (true)
            {
                lock (m_StartLock)
                {
                    if (m_StartFlag)
                    {
                        break;
                    }
                }

                Application.DoEvents();
            }

            m_StartFlag = false;
            ProcessUIThreadQueue();
        }

        private void TerminateRuntimeProcess()
        {
            m_ConsoleProcess?.Kill();
            WriteConsoleOut("");
            WriteConsoleOut("Console Process Killed");
        }

        private void tsiBuild_Click(object sender, EventArgs e)
        {
            BuildProject();
        }

        private void tsiOpenBuildArgPath_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ConsoleBuildArgConfigPath))
            {
                File.Create(ConsoleBuildArgConfigPath);
            }

            Process.Start(ConsoleBuildArgConfigPath);
        }

        private void tsiOpenRunArgPath_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ConsoleRunArgConfigPath))
            {
                File.Create(ConsoleRunArgConfigPath);
            }

            Process.Start(ConsoleRunArgConfigPath);
        }

        private void tsiOpenRuntimePath_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ConsoleBinConfigPath))
            {
                File.Create(ConsoleBinConfigPath);
            }

            Process.Start(ConsoleBinConfigPath);
        }

        private void tsiRefresh_Click(object sender, EventArgs e)
        {
            RefreshEditor();
        }

        private void tsiRun_Click(object sender, EventArgs e)
        {
            BuildAndRunProject();
        }

        private void tsiTerminateRuntime_Click(object sender, EventArgs e)
        {
            TerminateRuntimeProcess();
        }

        private void UpdateInputPages()
        {
            m_EnableIO = false;

            foreach (KeyValuePair<string, RichTextBox> keyValuePair in m_CodeInPages)
            {
                UpdatePage(keyValuePair.Key + ".vhl", keyValuePair.Value);
            }

            m_EnableIO = true;
            ProcessIOQueue();
        }

        private void UpdateIO(string name, RichTextBox rtb)
        {
            File.WriteAllText(name + ".vhl", rtb.Text);

            //RunEditorInternalHLBuild( name + ".vhl" );
        }

        private void UpdateOutputPages()
        {
            foreach (KeyValuePair<string, RichTextBox> keyValuePair in m_CodeOutPages)
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

        private void WriteConsoleIn(string line)
        {
            if (m_ConsoleIn == null)
            {
                return;
            }

            WriteConsoleOut(line);
            m_ConsoleIn.WriteLine(line);
        }

        private void WriteConsoleOut(string line)
        {
            rtbConsoleOut.AppendText($"{line}\n");
            rtbConsoleOut.ScrollToCaret();
        }

        private void WriteConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            WriteConsoleOut($"[LOG]{e.Data}");
        }

        #endregion

        private void CleanProjectFolder()
        {
            IEnumerable<string> sourceFiles = Directory.
                                              GetFiles(m_SourceFolder, "*.*", SearchOption.AllDirectories).
                                              Select(Path.GetFullPath);

            foreach (string sourceFile in sourceFiles)
            {
                if (Path.GetExtension(sourceFile) == ".vhl")
                {
                    continue;
                }


                WriteConsoleOut("Deleting File: " + sourceFile);
                File.Delete(sourceFile);
            }

        }

        private void clearProjectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CleanProjectFolder();
        }
    }

}
