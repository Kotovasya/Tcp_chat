namespace Client.Views
{
    partial class Chat
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
            this.nameList = new System.Windows.Forms.ListBox();
            this.messages = new System.Windows.Forms.ListBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.crButton = new System.Windows.Forms.Button();
            this.joinCRButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nameList
            // 
            this.nameList.FormattingEnabled = true;
            this.nameList.ItemHeight = 16;
            this.nameList.Location = new System.Drawing.Point(12, 73);
            this.nameList.Name = "nameList";
            this.nameList.Size = new System.Drawing.Size(120, 404);
            this.nameList.TabIndex = 0;
            this.nameList.SelectedIndexChanged += new System.EventHandler(this.NameList_SelectedIndexChanged);
            // 
            // messages
            // 
            this.messages.FormattingEnabled = true;
            this.messages.ItemHeight = 16;
            this.messages.Location = new System.Drawing.Point(138, 75);
            this.messages.Name = "messages";
            this.messages.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.messages.Size = new System.Drawing.Size(650, 372);
            this.messages.TabIndex = 1;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(138, 455);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(543, 22);
            this.messageTextBox.TabIndex = 2;
            this.messageTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MessageTextBox_KeyUp);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(687, 455);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(101, 28);
            this.sendButton.TabIndex = 3;
            this.sendButton.Text = "Отправить";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // crButton
            // 
            this.crButton.Location = new System.Drawing.Point(384, 23);
            this.crButton.Name = "crButton";
            this.crButton.Size = new System.Drawing.Size(139, 31);
            this.crButton.TabIndex = 4;
            this.crButton.Text = "Создать комнату";
            this.crButton.UseVisualStyleBackColor = true;
            this.crButton.Click += new System.EventHandler(this.CreateCR_Click);
            // 
            // joinCRButton
            // 
            this.joinCRButton.Location = new System.Drawing.Point(12, 23);
            this.joinCRButton.Name = "joinCRButton";
            this.joinCRButton.Size = new System.Drawing.Size(134, 29);
            this.joinCRButton.TabIndex = 5;
            this.joinCRButton.Text = "Присоединиться";
            this.joinCRButton.UseVisualStyleBackColor = true;
            this.joinCRButton.Visible = false;
            this.joinCRButton.Click += new System.EventHandler(this.JoinCRButton_Click);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 491);
            this.Controls.Add(this.joinCRButton);
            this.Controls.Add(this.crButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.messages);
            this.Controls.Add(this.nameList);
            this.Name = "Chat";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chat_FormClosing);
            this.Load += new System.EventHandler(this.Chat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox nameList;
        private System.Windows.Forms.ListBox messages;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button crButton;
        private System.Windows.Forms.Button joinCRButton;
    }
}