namespace OcrTool;

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
            this.components = new System.ComponentModel.Container();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.btnRecognize = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Location = new System.Drawing.Point(12, 12);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(120, 30);
            this.btnOpenImage.TabIndex = 0;
            this.btnOpenImage.Text = "打开图像";            
            this.btnOpenImage.UseVisualStyleBackColor = true;
            // 
            // btnRecognize
            // 
            this.btnRecognize.Location = new System.Drawing.Point(138, 12);
            this.btnRecognize.Name = "btnRecognize";
            this.btnRecognize.Size = new System.Drawing.Size(120, 30);
            this.btnRecognize.TabIndex = 1;
            this.btnRecognize.Text = "开始识别";            
            this.btnRecognize.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(380, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(410, 60);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(360, 350);
            this.txtResult.TabIndex = 3;
            this.txtResult.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "图像预览";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(410, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "识别结果";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnRecognize);
            this.Controls.Add(this.btnOpenImage);
            this.Name = "Form1";
            this.Text = "OCR识别工具";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    #endregion
}
