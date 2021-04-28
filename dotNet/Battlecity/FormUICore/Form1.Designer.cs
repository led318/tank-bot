
namespace FormUICore
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.fieldPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.checkBoxRunProcessing = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(1349, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // fieldPanel
            // 
            this.fieldPanel.BackColor = System.Drawing.Color.White;
            this.fieldPanel.Location = new System.Drawing.Point(462, 12);
            this.fieldPanel.Name = "fieldPanel";
            this.fieldPanel.Size = new System.Drawing.Size(250, 125);
            this.fieldPanel.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(419, 293);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(12, 537);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.Size = new System.Drawing.Size(419, 329);
            this.logTextBox.TabIndex = 3;
            // 
            // checkBoxRunProcessing
            // 
            this.checkBoxRunProcessing.AutoSize = true;
            this.checkBoxRunProcessing.Checked = true;
            this.checkBoxRunProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRunProcessing.Location = new System.Drawing.Point(12, 507);
            this.checkBoxRunProcessing.Name = "checkBoxRunProcessing";
            this.checkBoxRunProcessing.Size = new System.Drawing.Size(131, 24);
            this.checkBoxRunProcessing.TabIndex = 4;
            this.checkBoxRunProcessing.Text = "Run processing";
            this.checkBoxRunProcessing.UseVisualStyleBackColor = true;
            this.checkBoxRunProcessing.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1422, 878);
            this.Controls.Add(this.checkBoxRunProcessing);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fieldPanel);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel fieldPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.CheckBox checkBoxRunProcessing;
    }
}

