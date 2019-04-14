namespace SimFeedback.extension
{
    partial class FanatecExtControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBarMouseSpeed = new System.Windows.Forms.TrackBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.comboBoxJoysticks = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMouseSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarMouseSpeed
            // 
            this.trackBarMouseSpeed.BackColor = System.Drawing.SystemColors.Window;
            this.trackBarMouseSpeed.Location = new System.Drawing.Point(82, 54);
            this.trackBarMouseSpeed.Margin = new System.Windows.Forms.Padding(2);
            this.trackBarMouseSpeed.Maximum = 30;
            this.trackBarMouseSpeed.Minimum = 10;
            this.trackBarMouseSpeed.Name = "trackBarMouseSpeed";
            this.trackBarMouseSpeed.Size = new System.Drawing.Size(133, 45);
            this.trackBarMouseSpeed.TabIndex = 6;
            this.trackBarMouseSpeed.Value = 10;
            this.trackBarMouseSpeed.Scroll += new System.EventHandler(this.trackBarMouseSpeed_Scroll);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(21, 54);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(56, 24);
            this.buttonStart.TabIndex = 5;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // comboBoxJoysticks
            // 
            this.comboBoxJoysticks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxJoysticks.FormattingEnabled = true;
            this.comboBoxJoysticks.Location = new System.Drawing.Point(21, 23);
            this.comboBoxJoysticks.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxJoysticks.Name = "comboBoxJoysticks";
            this.comboBoxJoysticks.Size = new System.Drawing.Size(194, 21);
            this.comboBoxJoysticks.TabIndex = 4;
            this.comboBoxJoysticks.SelectedIndexChanged += new System.EventHandler(this.comboBoxJoysticks_SelectionChangeCommitted);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(229, 110);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fanatec Extension";
            // 
            // FanatecExtControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBarMouseSpeed);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.comboBoxJoysticks);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FanatecExtControl";
            this.Size = new System.Drawing.Size(238, 118);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMouseSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarMouseSpeed;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ComboBox comboBoxJoysticks;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
