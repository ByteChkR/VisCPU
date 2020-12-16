
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
            this.tbConsoleIn = new System.Windows.Forms.TextBox();
            this.btnSendInput = new System.Windows.Forms.Button();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.fbdSelectDir = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tslPercentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslCurrentTask = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelConsole.SuspendLayout();
            this.panelConsoleIn.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.statusStrip.SuspendLayout();
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
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslPercentage,
            this.tsProgress,
            this.tslCurrentTask});
            this.statusStrip.Location = new System.Drawing.Point(0, 29);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(838, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsProgress
            // 
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // tslPercentage
            // 
            this.tslPercentage.Name = "tslPercentage";
            this.tslPercentage.Size = new System.Drawing.Size(35, 17);
            this.tslPercentage.Text = "100%";
            // 
            // tslCurrentTask
            // 
            this.tslCurrentTask.Name = "tslCurrentTask";
            this.tslCurrentTask.Size = new System.Drawing.Size(62, 17);
            this.tslCurrentTask.Text = "Task: none";
            // 
            // HLLiveOutputViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 489);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelConsole);
            this.MinimumSize = new System.Drawing.Size(850, 449);
            this.Name = "HLLiveOutputViewerForm";
            this.Text = "HL Live Output Viewer";
            this.panelConsole.ResumeLayout(false);
            this.panelConsoleIn.ResumeLayout(false);
            this.panelConsoleIn.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
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
    }
}

