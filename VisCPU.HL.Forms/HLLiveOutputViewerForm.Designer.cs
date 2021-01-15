
namespace VisCPU.HL.Forms
{
    partial class HLLiveOutputViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tcCodeOut = new System.Windows.Forms.TabControl();
            this.tcCodeIn = new System.Windows.Forms.TabControl();
            this.tmrOutUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelConsole = new System.Windows.Forms.Panel();
            this.rtbConsoleOut = new System.Windows.Forms.RichTextBox();
            this.panelConsoleIn = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsiEditorOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsiTerminateRuntime = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiOpenRuntimePath = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiOpenBuildArgPath = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiOpenRunArgPath = new System.Windows.Forms.ToolStripMenuItem();
            this.clearProjectDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDependencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishToLocalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tslPercentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tslCurrentTask = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbConsoleIn = new System.Windows.Forms.TextBox();
            this.btnSendInput = new System.Windows.Forms.Button();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.fbdSelectDir = new System.Windows.Forms.FolderBrowserDialog();
            this.panelConsole.SuspendLayout();
            this.panelConsoleIn.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcCodeOut
            // 
            this.tcCodeOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCodeOut.Location = new System.Drawing.Point(0, 0);
            this.tcCodeOut.Name = "tcCodeOut";
            this.tcCodeOut.SelectedIndex = 0;
            this.tcCodeOut.Size = new System.Drawing.Size(435, 256);
            this.tcCodeOut.TabIndex = 1;
            // 
            // tcCodeIn
            // 
            this.tcCodeIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCodeIn.Location = new System.Drawing.Point(0, 0);
            this.tcCodeIn.Name = "tcCodeIn";
            this.tcCodeIn.SelectedIndex = 0;
            this.tcCodeIn.Size = new System.Drawing.Size(478, 256);
            this.tcCodeIn.TabIndex = 0;
            // 
            // panelConsole
            // 
            this.panelConsole.Controls.Add(this.rtbConsoleOut);
            this.panelConsole.Controls.Add(this.panelConsoleIn);
            this.panelConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelConsole.Location = new System.Drawing.Point(0, 256);
            this.panelConsole.Name = "panelConsole";
            this.panelConsole.Size = new System.Drawing.Size(913, 233);
            this.panelConsole.TabIndex = 4;
            // 
            // rtbConsoleOut
            // 
            this.rtbConsoleOut.BackColor = System.Drawing.Color.Black;
            this.rtbConsoleOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsoleOut.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rtbConsoleOut.Location = new System.Drawing.Point(0, 0);
            this.rtbConsoleOut.Name = "rtbConsoleOut";
            this.rtbConsoleOut.ReadOnly = true;
            this.rtbConsoleOut.Size = new System.Drawing.Size(913, 182);
            this.rtbConsoleOut.TabIndex = 2;
            this.rtbConsoleOut.Text = "";
            // 
            // panelConsoleIn
            // 
            this.panelConsoleIn.Controls.Add(this.statusStrip);
            this.panelConsoleIn.Controls.Add(this.tbConsoleIn);
            this.panelConsoleIn.Controls.Add(this.btnSendInput);
            this.panelConsoleIn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelConsoleIn.Location = new System.Drawing.Point(0, 182);
            this.panelConsoleIn.Name = "panelConsoleIn";
            this.panelConsoleIn.Size = new System.Drawing.Size(913, 51);
            this.panelConsoleIn.TabIndex = 1;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiEditorOptions,
            this.tslPercentage,
            this.tsProgress,
            this.tslCurrentTask});
            this.statusStrip.Location = new System.Drawing.Point(0, 27);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(838, 24);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsiEditorOptions
            // 
            this.tsiEditorOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsiEditorOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiTerminateRuntime,
            this.tsiRefresh,
            this.tsiRun,
            this.tsiBuild,
            this.tsiConfig,
            this.clearProjectDirectoryToolStripMenuItem,
            this.modulesToolStripMenuItem});
            this.tsiEditorOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsiEditorOptions.Name = "tsiEditorOptions";
            this.tsiEditorOptions.Size = new System.Drawing.Size(96, 22);
            this.tsiEditorOptions.Text = "Editor Options";
            this.tsiEditorOptions.ToolTipText = "Editor Options";
            // 
            // tsiTerminateRuntime
            // 
            this.tsiTerminateRuntime.Name = "tsiTerminateRuntime";
            this.tsiTerminateRuntime.Size = new System.Drawing.Size(217, 22);
            this.tsiTerminateRuntime.Text = "Terminate Runtime Process";
            this.tsiTerminateRuntime.Click += new System.EventHandler(this.tsiTerminateRuntime_Click);
            // 
            // tsiRefresh
            // 
            this.tsiRefresh.Name = "tsiRefresh";
            this.tsiRefresh.Size = new System.Drawing.Size(217, 22);
            this.tsiRefresh.Text = "Refresh";
            this.tsiRefresh.Click += new System.EventHandler(this.tsiRefresh_Click);
            // 
            // tsiRun
            // 
            this.tsiRun.Name = "tsiRun";
            this.tsiRun.Size = new System.Drawing.Size(217, 22);
            this.tsiRun.Text = "Run";
            this.tsiRun.Click += new System.EventHandler(this.tsiRun_Click);
            // 
            // tsiBuild
            // 
            this.tsiBuild.Name = "tsiBuild";
            this.tsiBuild.Size = new System.Drawing.Size(217, 22);
            this.tsiBuild.Text = "Build";
            this.tsiBuild.Click += new System.EventHandler(this.tsiBuild_Click);
            // 
            // tsiConfig
            // 
            this.tsiConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiOpenRuntimePath,
            this.tsiOpenBuildArgPath,
            this.tsiOpenRunArgPath});
            this.tsiConfig.Name = "tsiConfig";
            this.tsiConfig.Size = new System.Drawing.Size(217, 22);
            this.tsiConfig.Text = "Configuration";
            // 
            // tsiOpenRuntimePath
            // 
            this.tsiOpenRuntimePath.Name = "tsiOpenRuntimePath";
            this.tsiOpenRuntimePath.Size = new System.Drawing.Size(229, 22);
            this.tsiOpenRuntimePath.Text = "Open Runtime Path Config";
            this.tsiOpenRuntimePath.Click += new System.EventHandler(this.tsiOpenRuntimePath_Click);
            // 
            // tsiOpenBuildArgPath
            // 
            this.tsiOpenBuildArgPath.Name = "tsiOpenBuildArgPath";
            this.tsiOpenBuildArgPath.Size = new System.Drawing.Size(229, 22);
            this.tsiOpenBuildArgPath.Text = "Open Build Argument Config";
            this.tsiOpenBuildArgPath.Click += new System.EventHandler(this.tsiOpenBuildArgPath_Click);
            // 
            // tsiOpenRunArgPath
            // 
            this.tsiOpenRunArgPath.Name = "tsiOpenRunArgPath";
            this.tsiOpenRunArgPath.Size = new System.Drawing.Size(229, 22);
            this.tsiOpenRunArgPath.Text = "Open Run Argument Config";
            this.tsiOpenRunArgPath.Click += new System.EventHandler(this.tsiOpenRunArgPath_Click);
            // 
            // clearProjectDirectoryToolStripMenuItem
            // 
            this.clearProjectDirectoryToolStripMenuItem.Name = "clearProjectDirectoryToolStripMenuItem";
            this.clearProjectDirectoryToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.clearProjectDirectoryToolStripMenuItem.Text = "Clear Project Directory";
            this.clearProjectDirectoryToolStripMenuItem.Click += new System.EventHandler(this.clearProjectDirectoryToolStripMenuItem_Click);
            // 
            // modulesToolStripMenuItem
            // 
            this.modulesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDependencyToolStripMenuItem,
            this.buildModuleToolStripMenuItem,
            this.restoreToolStripMenuItem,
            this.publishToLocalToolStripMenuItem});
            this.modulesToolStripMenuItem.Name = "modulesToolStripMenuItem";
            this.modulesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.modulesToolStripMenuItem.Text = "Modules";
            // 
            // addDependencyToolStripMenuItem
            // 
            this.addDependencyToolStripMenuItem.Name = "addDependencyToolStripMenuItem";
            this.addDependencyToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.addDependencyToolStripMenuItem.Text = "Add Dependency";
            this.addDependencyToolStripMenuItem.Click += new System.EventHandler(this.addDependencyToolStripMenuItem_Click);
            // 
            // buildModuleToolStripMenuItem
            // 
            this.buildModuleToolStripMenuItem.Name = "buildModuleToolStripMenuItem";
            this.buildModuleToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.buildModuleToolStripMenuItem.Text = "Build Module";
            this.buildModuleToolStripMenuItem.Click += new System.EventHandler(this.buildModuleToolStripMenuItem_Click);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // publishToLocalToolStripMenuItem
            // 
            this.publishToLocalToolStripMenuItem.Name = "publishToLocalToolStripMenuItem";
            this.publishToLocalToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.publishToLocalToolStripMenuItem.Text = "Publish to Local";
            this.publishToLocalToolStripMenuItem.Click += new System.EventHandler(this.publishToLocalToolStripMenuItem_Click);
            // 
            // tslPercentage
            // 
            this.tslPercentage.Name = "tslPercentage";
            this.tslPercentage.Size = new System.Drawing.Size(35, 19);
            this.tslPercentage.Text = "100%";
            // 
            // tsProgress
            // 
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(100, 18);
            // 
            // tslCurrentTask
            // 
            this.tslCurrentTask.Name = "tslCurrentTask";
            this.tslCurrentTask.Size = new System.Drawing.Size(62, 19);
            this.tslCurrentTask.Text = "Task: none";
            // 
            // tbConsoleIn
            // 
            this.tbConsoleIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbConsoleIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbConsoleIn.Location = new System.Drawing.Point(0, 0);
            this.tbConsoleIn.Name = "tbConsoleIn";
            this.tbConsoleIn.Size = new System.Drawing.Size(838, 28);
            this.tbConsoleIn.TabIndex = 2;
            // 
            // btnSendInput
            // 
            this.btnSendInput.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSendInput.Location = new System.Drawing.Point(838, 0);
            this.btnSendInput.Name = "btnSendInput";
            this.btnSendInput.Size = new System.Drawing.Size(75, 51);
            this.btnSendInput.TabIndex = 1;
            this.btnSendInput.Text = "Send";
            this.btnSendInput.UseVisualStyleBackColor = true;
            this.btnSendInput.Click += new System.EventHandler(this.btnSendInput_Click);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.tcCodeOut);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(478, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(435, 256);
            this.panelRight.TabIndex = 5;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.tcCodeIn);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(478, 256);
            this.panelLeft.TabIndex = 6;
            // 
            // fbdSelectDir
            // 
            this.fbdSelectDir.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // HLLiveOutputViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 489);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelConsole);
            this.MinimumSize = new System.Drawing.Size(849, 448);
            this.Name = "HLLiveOutputViewerForm";
            this.Text = "HL Live Output Viewer";
            this.Load += new System.EventHandler(this.HLLiveOutputViewerForm_Load);
            this.panelConsole.ResumeLayout(false);
            this.panelConsoleIn.ResumeLayout(false);
            this.panelConsoleIn.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrOutUpdate;
        private System.Windows.Forms.TabControl tcCodeOut;
        private System.Windows.Forms.TabControl tcCodeIn;
        private System.Windows.Forms.Panel panelConsole;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelConsoleIn;
        private System.Windows.Forms.RichTextBox rtbConsoleOut;
        private System.Windows.Forms.TextBox tbConsoleIn;
        private System.Windows.Forms.Button btnSendInput;
        private System.Windows.Forms.FolderBrowserDialog fbdSelectDir;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tslPercentage;
        private System.Windows.Forms.ToolStripProgressBar tsProgress;
        private System.Windows.Forms.ToolStripStatusLabel tslCurrentTask;
        private System.Windows.Forms.ToolStripDropDownButton tsiEditorOptions;
        private System.Windows.Forms.ToolStripMenuItem tsiTerminateRuntime;
        private System.Windows.Forms.ToolStripMenuItem tsiRefresh;
        private System.Windows.Forms.ToolStripMenuItem tsiRun;
        private System.Windows.Forms.ToolStripMenuItem tsiBuild;
        private System.Windows.Forms.ToolStripMenuItem tsiConfig;
        private System.Windows.Forms.ToolStripMenuItem tsiOpenRuntimePath;
        private System.Windows.Forms.ToolStripMenuItem tsiOpenBuildArgPath;
        private System.Windows.Forms.ToolStripMenuItem tsiOpenRunArgPath;
        private System.Windows.Forms.ToolStripMenuItem clearProjectDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDependencyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem publishToLocalToolStripMenuItem;
    }
}

