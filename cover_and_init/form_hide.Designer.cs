namespace cover_and_init
{
    partial class form_hide
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
            this.label_Y = new System.Windows.Forms.Label();
            this.label_X = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_Y
            // 
            this.label_Y.AutoSize = true;
            this.label_Y.Location = new System.Drawing.Point(12, 57);
            this.label_Y.Name = "label_Y";
            this.label_Y.Size = new System.Drawing.Size(22, 12);
            this.label_Y.TabIndex = 3;
            this.label_Y.Text = "Y : ";
            // 
            // label_X
            // 
            this.label_X.AutoSize = true;
            this.label_X.Location = new System.Drawing.Point(12, 28);
            this.label_X.Name = "label_X";
            this.label_X.Size = new System.Drawing.Size(22, 12);
            this.label_X.TabIndex = 2;
            this.label_X.Text = "X : ";
            // 
            // form_hide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(135, 92);
            this.Controls.Add(this.label_Y);
            this.Controls.Add(this.label_X);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "form_hide";
            this.Text = "form_hide";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_hide_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label_Y;
        public System.Windows.Forms.Label label_X;
    }
}