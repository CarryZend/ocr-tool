using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OcrTool
{
    public static class ImagePreprocessor
    {
        /// <summary>
        /// 预处理图像以提高OCR识别率
        /// </summary>
        /// <param name="imagePath">图像文件路径</param>
        /// <param name="useSkeletonAlgorithm">是否使用骨架算法</param>
        /// <returns>预处理后的图像路径</returns>
        public static string PreprocessImage(string imagePath, bool useSkeletonAlgorithm = false)
        {
            // 创建临时文件路径
            string tempPath = Path.Combine(Path.GetTempPath(), "preprocessed_ocr.png");

            using (Bitmap originalBitmap = new Bitmap(imagePath))
            {
                // 0. 调整图像大小：按宽度300等比例缩放
                Bitmap resizedBitmap = ResizeImage(originalBitmap, 300);

                // 1. 色彩加深
                Bitmap enhancedBitmap = EnhanceColors(resizedBitmap);

                // 2. 转换为灰度图像
                Bitmap grayBitmap = ConvertToGrayscale(enhancedBitmap);

                // 3. 调整对比度
                Bitmap contrastBitmap = AdjustContrast(grayBitmap, 50.0f); // 增加对比度

                // 4. 去除噪点
                // Bitmap denoisedBitmap = resizedBitmap; //RemoveNoise(contrastBitmap);

                // 5. 二值化处理
                Bitmap binaryBitmap = BinarizeImage(contrastBitmap, 128);

                // 6. 可选：应用骨架算法
                Bitmap skeletonBitmap = binaryBitmap;
                // if (useSkeletonAlgorithm)
                // {
                //     skeletonBitmap = ExtractSkeleton(resizedBitmap);
                // }

                // 保存预处理后的图像
                skeletonBitmap.Save(tempPath, ImageFormat.Png);

                // 释放资源
                resizedBitmap.Dispose();
                enhancedBitmap.Dispose();
                grayBitmap.Dispose();
                contrastBitmap.Dispose();
                // denoisedBitmap.Dispose();
                if (skeletonBitmap != binaryBitmap)
                {
                    skeletonBitmap.Dispose();
                }
                binaryBitmap.Dispose();
            }

            return tempPath;
        }

        /// <summary>
        /// 色彩加深处理
        /// </summary>
        private static Bitmap EnhanceColors(Bitmap original)
        {
            Bitmap enhancedBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color color = original.GetPixel(i, j);

                    // 增强色彩饱和度和对比度
                    float saturationFactor = 1.5f; // 饱和度增加50%
                    float contrastFactor = 1.2f;  // 对比度增加20%

                    // RGB转HSV
                    float h, s, v;
                    ColorToHSV(color, out h, out s, out v);

                    // 增强饱和度
                    s = Math.Clamp(s * saturationFactor, 0f, 1f);

                    // 增强对比度 (对V通道进行调整)
                    v = Math.Clamp((v - 0.5f) * contrastFactor + 0.5f, 0f, 1f);

                    // HSV转RGB
                    Color enhancedColor = HSVToColor(h, s, v);

                    enhancedBitmap.SetPixel(i, j, enhancedColor);
                }
            }

            return enhancedBitmap;
        }

        /// <summary>
        /// 将RGB颜色转换为HSV颜色空间
        /// </summary>
        private static void ColorToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            // 计算亮度V
            v = max;

            // 计算饱和度S
            s = max == 0 ? 0 : delta / max;

            // 计算色相H
            if (delta == 0)
            {
                h = 0; // 灰色
            }
            else if (max == r)
            {
                h = ((g - b) / delta) % 6;
            }
            else if (max == g)
            {
                h = (b - r) / delta + 2;
            }
            else
            {
                h = (r - g) / delta + 4;
            }

            h *= 60; // 转换为度数
            if (h < 0)
            {
                h += 360;
            }
        }

        /// <summary>
        /// 将HSV颜色空间转换为RGB颜色
        /// </summary>
        private static Color HSVToColor(float h, float s, float v)
        {
            if (s == 0)
            {
                // 灰色
                int value = (int)(v * 255);
                return Color.FromArgb(value, value, value);
            }

            h /= 60; // 转换为区段
            int i = (int)Math.Floor(h);
            float f = h - i;
            float p = v * (1 - s);
            float q = v * (1 - s * f);
            float t = v * (1 - s * (1 - f));

            float r, g, b;
            switch (i)
            {
                case 0:
                    r = v; g = t; b = p;
                    break;
                case 1:
                    r = q; g = v; b = p;
                    break;
                case 2:
                    r = p; g = v; b = t;
                    break;
                case 3:
                    r = p; g = q; b = v;
                    break;
                case 4:
                    r = t; g = p; b = v;
                    break;
                default:
                    r = v; g = p; b = q;
                    break;
            }

            return Color.FromArgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255)
            );
        }

        /// <summary>
        /// 将图像转换为灰度
        /// </summary>
        private static Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color color = original.GetPixel(i, j);
                    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                    grayBitmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }

            return grayBitmap;
        }

        /// <summary>
        /// 调整图像对比度
        /// </summary>
        /// <param name="original">原始图像</param>
        /// <param name="contrast">对比度因子，1.0表示原始对比度，大于1增加对比度，小于1降低对比度</param>
        /// <returns>调整对比度后的图像</returns>
        private static Bitmap AdjustContrast(Bitmap original, float contrast)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            // 对比度计算公式: factor = (259 * (contrast + 255)) / (255 * (259 - contrast))
            float factor = (259 * (contrast + 255)) / (255 * (259 - contrast));

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color color = original.GetPixel(i, j);
                    // 应用对比度调整公式
                    int red = Math.Clamp((int)(factor * (color.R - 128) + 128), 0, 255);
                    int green = Math.Clamp((int)(factor * (color.G - 128) + 128), 0, 255);
                    int blue = Math.Clamp((int)(factor * (color.B - 128) + 128), 0, 255);
                    newBitmap.SetPixel(i, j, Color.FromArgb(red, green, blue));
                }
            }

            return newBitmap;
        }

        /// <summary>
        /// 图像二值化处理
        /// </summary>
        /// <param name="original">原始图像</param>
        /// <param name="threshold">二值化阈值</param>
        /// <returns>二值化后的图像</returns>
        private static Bitmap BinarizeImage(Bitmap original, int threshold = 128)
        {
            Bitmap binaryBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color color = original.GetPixel(i, j);
                    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                    binaryBitmap.SetPixel(i, j, gray < threshold ? Color.Black : Color.White);
                }
            }

            return binaryBitmap;
        }

        /// <summary>
        /// 提取图像骨架
        /// </summary>
        /// <param name="binaryBitmap">二值化后的图像</param>
        /// <returns>提取骨架后的图像</returns>
        private static Bitmap ExtractSkeleton(Bitmap binaryBitmap)
        {
            // 创建图像副本
            Bitmap skeletonBitmap = new Bitmap(binaryBitmap);
            Bitmap tempBitmap = new Bitmap(binaryBitmap);

            bool changed;
            int maxIterations = 100; // 设置最大迭代次数防止无限循环
            int iteration = 0;

            do
            {
                changed = false;
                iteration++;

                // 第一步：腐蚀和条件膨胀
                for (int i = 1; i < binaryBitmap.Width - 1; i++)
                {
                    for (int j = 1; j < binaryBitmap.Height - 1; j++)
                    {
                        Color current = tempBitmap.GetPixel(i, j);
                        if (current.ToArgb() == Color.White.ToArgb())
                            continue; // 只处理黑色像素

                        // 检查8个邻域像素
                        Color top = tempBitmap.GetPixel(i, j - 1);
                        Color bottom = tempBitmap.GetPixel(i, j + 1);
                        Color left = tempBitmap.GetPixel(i - 1, j);
                        Color right = tempBitmap.GetPixel(i + 1, j);
                        Color topLeft = tempBitmap.GetPixel(i - 1, j - 1);
                        Color topRight = tempBitmap.GetPixel(i + 1, j - 1);
                        Color bottomLeft = tempBitmap.GetPixel(i - 1, j + 1);
                        Color bottomRight = tempBitmap.GetPixel(i + 1, j + 1);

                        // 计算邻域中的白色像素数
                        int whiteCount = 0;
                        if (top.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (bottom.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (left.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (right.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (topLeft.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (topRight.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (bottomLeft.ToArgb() == Color.White.ToArgb()) whiteCount++;
                        if (bottomRight.ToArgb() == Color.White.ToArgb()) whiteCount++;

                        // 优化：扩展骨架候选条件，允许更多的白色像素
                        if (whiteCount >= 1 && whiteCount <= 7)
                        {
                            // 检查连接性
                            int transitions = 0;
                            bool[] neighbors = new bool[]
                            {
                                top.ToArgb() == Color.Black.ToArgb(),
                                topRight.ToArgb() == Color.Black.ToArgb(),
                                right.ToArgb() == Color.Black.ToArgb(),
                                bottomRight.ToArgb() == Color.Black.ToArgb(),
                                bottom.ToArgb() == Color.Black.ToArgb(),
                                bottomLeft.ToArgb() == Color.Black.ToArgb(),
                                left.ToArgb() == Color.Black.ToArgb(),
                                topLeft.ToArgb() == Color.Black.ToArgb(),
                                top.ToArgb() == Color.Black.ToArgb() // 重复第一个元素用于循环检测
                            };

                            for (int k = 0; k < 8; k++)
                            {
                                if (!neighbors[k] && neighbors[k + 1])
                                    transitions++;
                            }

                            // 优化：允许有1-2个过渡，以保留更多骨架细节
                            if (transitions >= 1 && transitions <= 2)
                            {
                                // 检查是否满足保留条件
                                bool canRemove = true;
                                // 优化：放宽保留条件，保留更多连接点
                                if ((left.ToArgb() == Color.Black.ToArgb() && right.ToArgb() == Color.Black.ToArgb()) ||
                                    (top.ToArgb() == Color.Black.ToArgb() && bottom.ToArgb() == Color.Black.ToArgb()))
                                {
                                    canRemove = false;
                                }

                                if (canRemove)
                                {
                                    skeletonBitmap.SetPixel(i, j, Color.White);
                                    changed = true;
                                }
                            }
                        }
                    }
                }

                // 更新临时位图
                tempBitmap = new Bitmap(skeletonBitmap);
            } while (changed && iteration < maxIterations);

            return skeletonBitmap;
        }

        /// <summary>
        /// 调整图像大小，按指定宽度等比例缩放
        /// </summary>
        /// <param name="original">原始图像</param>
        /// <param name="targetWidth">目标宽度</param>
        /// <returns>调整大小后的图像</returns>
        private static Bitmap ResizeImage(Bitmap original, int targetWidth)
        {
            // 计算等比例缩放的高度
            float aspectRatio = (float)original.Height / original.Width;
            int targetHeight = (int)(targetWidth * aspectRatio);

            // 创建新的缩放后的图像
            Bitmap resizedBitmap = new Bitmap(targetWidth, targetHeight);
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                // 设置高质量的插值模式
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, targetWidth, targetHeight);
            }

            return resizedBitmap;
        }

        /// <summary>
        /// 去除图像噪点
        /// </summary>
        private static Bitmap RemoveNoise(Bitmap original)
        {
            Bitmap denoisedBitmap = new Bitmap(original.Width, original.Height);

            for (int i = 1; i < original.Width - 1; i++)
            {
                for (int j = 1; j < original.Height - 1; j++)
                {
                    // 简单的中值滤波
                    Color[] neighbors = new Color[9];
                    int index = 0;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            neighbors[index++] = original.GetPixel(i + x, j + y);
                        }
                    }

                    // 排序
                    Array.Sort(neighbors, (a, b) => a.R.CompareTo(b.R));

                    // 取中值
                    denoisedBitmap.SetPixel(i, j, neighbors[4]);
                }
            }

            // 处理边界
            for (int i = 0; i < original.Width; i++)
            {
                denoisedBitmap.SetPixel(i, 0, original.GetPixel(i, 0));
                denoisedBitmap.SetPixel(i, original.Height - 1, original.GetPixel(i, original.Height - 1));
            }

            for (int j = 0; j < original.Height; j++)
            {
                denoisedBitmap.SetPixel(0, j, original.GetPixel(0, j));
                denoisedBitmap.SetPixel(original.Width - 1, j, original.GetPixel(original.Width - 1, j));
            }

            return denoisedBitmap;
        }
    }
}