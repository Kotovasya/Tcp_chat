namespace Server.Views
{
    partial class ServerGUI
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
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Location = new System.Drawing.Point(12, 59);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(748, 500);
            this.txtConsole.TabIndex = 0;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(295, 579);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(169, 33);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start server";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(368, 21);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(70, 22);
            this.portBox.TabIndex = 2;
            this.portBox.Text = "2020";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(259, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Порт сервера:";
            // 
            // ServerGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 629);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.txtConsole);
            this.Name = "ServerGUI";
            this.Text = "ServerGUI";
            this.Load += new System.EventHandler(this.ServerGUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label1;
    }
}