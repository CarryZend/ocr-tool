using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Tesseract;

namespace OcrTool;

public partial class Form1 : Form
{
    // 控件声明
    private Button btnOpenImage;
    private Button btnRecognize;
    private PictureBox pictureBox1;
    private RichTextBox txtResult;
    private Label label1;
    private Label label2;

    public Form1()
    {
        InitializeComponent();
        // 绑定事件
        this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
        this.btnRecognize.Click += new System.EventHandler(this.btnRecognize_Click);
    }

    // 打开图像按钮事件
    private void btnOpenImage_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // 显示图像
                pictureBox1.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
                // 清空之前的识别结果
                txtResult.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开图像文件: " + ex.Message);
            }
        }
    }

    // 识别按钮事件
    private void btnRecognize_Click(object sender, EventArgs e)
    {
        if (pictureBox1.Image == null)
        {
            MessageBox.Show("请先打开一张图像");
            return;
        }

        try
        {
            // 显示正在识别的提示
            txtResult.Text = "正在识别，请稍候...";
            Application.DoEvents();

            // 保存图像到临时文件
            string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_ocr.png");
            pictureBox1.Image.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Png);

            // 图像预处理
            string preprocessedImagePath = ImagePreprocessor.PreprocessImage(tempImagePath);
            MessageBox.Show("图像预处理完成，临时文件: " + preprocessedImagePath);

            // 使用相对路径构建tessdata目录路径
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // 从应用程序运行目录向上导航三级到达OcrTool目录
            string tessdataPath = Path.Combine(appDirectory, "..", "..", "..", "tessdata");
            // 规范化路径，处理相对路径符号
            tessdataPath = Path.GetFullPath(tessdataPath);            
            // 初始化Tesseract引擎
            try
            {
                // 检查语言数据文件是否存在
                string chiSimPath = Path.Combine(tessdataPath, "chi_sim.traineddata");
                string engPath = Path.Combine(tessdataPath, "eng.traineddata");
                // MessageBox.Show($"语言数据文件检查:\n中文: {File.Exists(chiSimPath)}\n英文: {File.Exists(engPath)}");

                using (var engine = new TesseractEngine(tessdataPath, "chi_sim+eng", EngineMode.Default))
                {
                    // 使用预处理后的图像
                    using (var img = Pix.LoadFromFile(preprocessedImagePath))
                    {
                        // 显示图像信息
                        MessageBox.Show($"图像尺寸: {img.Width}x{img.Height}\n位深度: {img.Depth}位");

                        using (var page = engine.Process(img))
                        {
                            string text = page.GetText();
                            MessageBox.Show("识别结果长度: " + text.Length);
                            if (string.IsNullOrEmpty(text))
                            {
                                txtResult.Text = "识别结果为空";
                            }
                            else
                            {
                                txtResult.Text = text;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tesseract引擎初始化或识别出错: " + ex.Message);
                throw;
            }

            // 删除临时文件
            if (File.Exists(tempImagePath))
            {
                File.Delete(tempImagePath);
            }
            // 删除预处理图像文件
            // if (File.Exists(preprocessedImagePath))
            // {
            //     File.Delete(preprocessedImagePath);
            // }
        }
        catch (Exception ex)
        {
            txtResult.Text = "识别出错: " + ex.Message;
        }
    }
}
