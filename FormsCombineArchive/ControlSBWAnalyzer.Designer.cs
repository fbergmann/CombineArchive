namespace FormsCombineArchive
{
    partial class ControlSBWAnalyzer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdEditVisually = new System.Windows.Forms.Button();
            this.cmdEditScript = new System.Windows.Forms.Button();
            this.cmdSimulate = new System.Windows.Forms.Button();
            this.cmdTranslate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAnalyzer = new System.Windows.Forms.ComboBox();
            this.cmdGo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdEditVisually
            // 
            this.cmdEditVisually.Location = new System.Drawing.Point(13, 12);
            this.cmdEditVisually.Name = "cmdEditVisually";
            this.cmdEditVisually.Size = new System.Drawing.Size(126, 23);
            this.cmdEditVisually.TabIndex = 1;
            this.cmdEditVisually.Text = "Edit (visually)";
            this.cmdEditVisually.UseVisualStyleBackColor = true;
            this.cmdEditVisually.Click += new System.EventHandler(this.OnEditVisuallyClicked);
            // 
            // cmdEditScript
            // 
            this.cmdEditScript.Location = new System.Drawing.Point(145, 12);
            this.cmdEditScript.Name = "cmdEditScript";
            this.cmdEditScript.Size = new System.Drawing.Size(126, 23);
            this.cmdEditScript.TabIndex = 2;
            this.cmdEditScript.Text = "Edit (script)";
            this.cmdEditScript.UseVisualStyleBackColor = true;
            this.cmdEditScript.Click += new System.EventHandler(this.OnEditScriptClicked);
            // 
            // cmdSimulate
            // 
            this.cmdSimulate.Location = new System.Drawing.Point(277, 12);
            this.cmdSimulate.Name = "cmdSimulate";
            this.cmdSimulate.Size = new System.Drawing.Size(100, 23);
            this.cmdSimulate.TabIndex = 3;
            this.cmdSimulate.Text = "Simulate";
            this.cmdSimulate.UseVisualStyleBackColor = true;
            this.cmdSimulate.Click += new System.EventHandler(this.OnSimulateClicked);
            // 
            // cmdTranslate
            // 
            this.cmdTranslate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdTranslate.Location = new System.Drawing.Point(383, 12);
            this.cmdTranslate.Name = "cmdTranslate";
            this.cmdTranslate.Size = new System.Drawing.Size(100, 23);
            this.cmdTranslate.TabIndex = 4;
            this.cmdTranslate.Text = "Translate";
            this.cmdTranslate.UseVisualStyleBackColor = true;
            this.cmdTranslate.Click += new System.EventHandler(this.OnTranslateClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Edit with: ";
            // 
            // cmbAnalyzer
            // 
            this.cmbAnalyzer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAnalyzer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnalyzer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbAnalyzer.FormattingEnabled = true;
            this.cmbAnalyzer.Location = new System.Drawing.Point(69, 41);
            this.cmbAnalyzer.Name = "cmbAnalyzer";
            this.cmbAnalyzer.Size = new System.Drawing.Size(333, 21);
            this.cmbAnalyzer.Sorted = true;
            this.cmbAnalyzer.TabIndex = 6;
            // 
            // cmdGo
            // 
            this.cmdGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGo.Location = new System.Drawing.Point(408, 41);
            this.cmdGo.Name = "cmdGo";
            this.cmdGo.Size = new System.Drawing.Size(75, 23);
            this.cmdGo.TabIndex = 7;
            this.cmdGo.Text = "Go!";
            this.cmdGo.UseVisualStyleBackColor = true;
            this.cmdGo.Click += new System.EventHandler(this.OnGoClicked);
            // 
            // ControlSBWAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdGo);
            this.Controls.Add(this.cmbAnalyzer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdTranslate);
            this.Controls.Add(this.cmdSimulate);
            this.Controls.Add(this.cmdEditScript);
            this.Controls.Add(this.cmdEditVisually);
            this.Name = "ControlSBWAnalyzer";
            this.Size = new System.Drawing.Size(493, 72);
            this.Load += new System.EventHandler(this.ControlSBWAnalyzer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdEditVisually;
        private System.Windows.Forms.Button cmdEditScript;
        private System.Windows.Forms.Button cmdSimulate;
        private System.Windows.Forms.Button cmdTranslate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAnalyzer;
        private System.Windows.Forms.Button cmdGo;
    }
}
