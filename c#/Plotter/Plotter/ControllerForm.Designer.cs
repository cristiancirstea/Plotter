namespace Plotter
{
    partial class ControllerForm
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
            this.edPositionX = new System.Windows.Forms.NumericUpDown();
            this.edPositionY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.edPositionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edPositionY)).BeginInit();
            this.SuspendLayout();
            // 
            // edPositionX
            // 
            this.edPositionX.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edPositionX.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.edPositionX.Location = new System.Drawing.Point(12, 37);
            this.edPositionX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.edPositionX.Name = "edPositionX";
            this.edPositionX.Size = new System.Drawing.Size(120, 38);
            this.edPositionX.TabIndex = 9;
            this.edPositionX.ValueChanged += new System.EventHandler(this.edPositionX_ValueChanged);
            // 
            // edPositionY
            // 
            this.edPositionY.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edPositionY.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.edPositionY.Location = new System.Drawing.Point(179, 37);
            this.edPositionY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.edPositionY.Name = "edPositionY";
            this.edPositionY.Size = new System.Drawing.Size(120, 38);
            this.edPositionY.TabIndex = 10;
            this.edPositionY.ValueChanged += new System.EventHandler(this.edPositionY_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "OX";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "OY";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 102);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(45, 17);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Pen";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbLog.Location = new System.Drawing.Point(0, 125);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(514, 299);
            this.tbLog.TabIndex = 14;
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 424);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.edPositionY);
            this.Controls.Add(this.edPositionX);
            this.Name = "ControllerForm";
            this.Text = "ControllerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControllerForm_FormClosing);
            this.Load += new System.EventHandler(this.ControllerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edPositionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edPositionY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown edPositionX;
        private System.Windows.Forms.NumericUpDown edPositionY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox tbLog;
    }
}