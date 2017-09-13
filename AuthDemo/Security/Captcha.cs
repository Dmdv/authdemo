using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace AuthDemo.Security
{
    public class Captcha
    {
        private const int Height = 80;
        private const int Width = 190;

        public byte[] Generate(string captcha)
        {
            var random = new Random();

            int[] backgroundNoiseColor = {150, 150, 150};
            int[] textColor = {0, 0, 0};

            Graphics graphics;

            using (var outputBitmap = CreateOutputBitmap(Width, Height, out graphics))
            {
                //Create a Drawing area
                var rectangleF = new RectangleF(0, 0, Width, Height);

                var brush = DrawBackground(random);

                graphics.FillRectangle(brush, rectangleF);

                var oMatrix = new Matrix();

                for (var i = 0; i <= captcha.Length - 1; i++)
                {
                    oMatrix.Reset();

                    var iChars = captcha.Length;
                    var x = Width / (iChars + 1) * i;
                    var y = Height / 2;

                    //Rotate text Random
                    oMatrix.RotateAt(random.Next(-40, 40), new PointF(x, y));
                    graphics.Transform = oMatrix;

                    graphics.DrawString
                    (
                        captcha.Substring(i, 1),
                        RandomFont(random),
                        RandomColor(random),
                        x,
                        random.Next(10, 40)
                    );

                    graphics.ResetTransform();
                }

                using (var memoryStream = new MemoryStream())
                {
                    outputBitmap.Save(memoryStream, ImageFormat.Png);
                    return memoryStream.GetBuffer();
                }
            }
        }

        private static SolidBrush RandomColor(Random random)
        {
            return new SolidBrush(Color.FromArgb(random.Next(0, 100), random.Next(0, 100), random.Next(0, 100)));
        }

        private static Font RandomFont(Random random)
        {
            int[] fontEmSizes = {15, 20, 25, 30, 35};

            string[] fontNames =
            {
                "Comic Sans MS",
                "Arial",
                "Times New Roman",
                "Georgia",
                "Verdana",
                "Geneva"
            };

            FontStyle[] fontStyles =
            {
                FontStyle.Bold,
                FontStyle.Italic,
                FontStyle.Regular,
                FontStyle.Strikeout,
                FontStyle.Underline
            };

            return new Font(fontNames[random.Next(fontNames.Length - 1)],
                fontEmSizes[random.Next(fontEmSizes.Length - 1)],
                fontStyles[random.Next(fontStyles.Length - 1)]);
        }

        private static Brush DrawBackground(Random random)
        {
            HatchStyle[] aHatchStyles =
            {
                HatchStyle.BackwardDiagonal, HatchStyle.Cross,
                HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal,
                HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical,
                HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross,
                HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid,
                HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,
                HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard,
                HatchStyle.LargeConfetti, HatchStyle.LargeGrid,
                HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal,
                HatchStyle.LightUpwardDiagonal, HatchStyle.LightVertical,
                HatchStyle.Max, HatchStyle.Min, HatchStyle.NarrowHorizontal,
                HatchStyle.NarrowVertical, HatchStyle.OutlinedDiamond,
                HatchStyle.Plaid, HatchStyle.Shingle, HatchStyle.SmallCheckerBoard,
                HatchStyle.SmallConfetti, HatchStyle.SmallGrid,
                HatchStyle.SolidDiamond, HatchStyle.Sphere, HatchStyle.Trellis,
                HatchStyle.Vertical, HatchStyle.Wave, HatchStyle.Weave,
                HatchStyle.WideDownwardDiagonal, HatchStyle.WideUpwardDiagonal, HatchStyle.ZigZag
            };

            //Draw background (Lighter colors RGB 100 to 255)
            return new HatchBrush(
                aHatchStyles[random.Next(aHatchStyles.Length - 1)],
                Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255)),
                Color.White);
        }

        private static Bitmap CreateOutputBitmap(int iWidth, int iHeight, out Graphics oGraphics)
        {
            var outputBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            oGraphics = Graphics.FromImage(outputBitmap);
            oGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            return outputBitmap;
        }
    }
}