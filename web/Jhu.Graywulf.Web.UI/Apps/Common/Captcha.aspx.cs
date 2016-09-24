using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Imaging;

namespace Jhu.Graywulf.Web.UI.Apps.Common
{
    public class Captcha : Page
    {
        private static readonly Pen[] RandomPens =
        {
            Pens.DarkBlue, Pens.DarkCyan, Pens.DarkGoldenrod, Pens.DarkGray,
            Pens.DarkGreen, Pens.DarkKhaki, Pens.DarkMagenta, Pens.DarkOliveGreen,
            Pens.DarkOrange, Pens.DarkOrchid, Pens.DarkRed, Pens.DarkSalmon,
            Pens.DarkSeaGreen, Pens.DarkSlateBlue, Pens.DarkSlateGray, Pens.DarkTurquoise,
            Pens.DarkViolet,
        };

        private static readonly Brush[] RandomBrushes =
        {
            Brushes.DarkBlue, Brushes.DarkCyan, Brushes.DarkGoldenrod, Brushes.DarkGray,
            Brushes.DarkGreen, Brushes.DarkKhaki, Brushes.DarkMagenta, Brushes.DarkOliveGreen,
            Brushes.DarkOrange, Brushes.DarkOrchid, Brushes.DarkRed, Brushes.DarkSalmon,
            Brushes.DarkSeaGreen, Brushes.DarkSlateBlue, Brushes.DarkSlateGray, Brushes.DarkTurquoise,
            Brushes.DarkViolet,
        };

        protected override void OnLoad(EventArgs e)
        {
            Response.ContentType = Constants.CaptchaMimeType;
            Response.Expires = -1;

            var width = int.Parse(Request.QueryString["width"]);
            var height = int.Parse(Request.QueryString["height"]);
            var digits = int.Parse(Request.QueryString["digits"]);

            var rnd = new Random();
            var code = "";

            for (int i = 0; i < digits; i++)
            {
                code += rnd.Next(10).ToString();
            }
            Session[Constants.SessionCaptcha] = code;

            using (var bmp = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.White);

                    // --- Draw random lines

                    for (int i = 0; i < 10; i++)
                    {
                        float a = rnd.Next(height);
                        float b = rnd.Next(height);
                        var pen = rnd.Next(RandomPens.Length);

                        g.DrawLine(RandomPens[pen], 0, a, width, b);
                    }

                    // --- Draw text

                    using (var f = new Font("Arial", (float)(0.4 * height)))
                    {
                        for (int i = 0; i < digits; i++)
                        {
                            var s = g.MeasureString(code[i].ToString(), f);
                            var brush = rnd.Next(RandomBrushes.Length);

                            float x = (float)(width * 0.1 + width * 0.8 / digits * (i + 0.5)) - s.Width / 2;
                            float y = (float)height / 2 - s.Height / 2;

                            float a = (float)(-30.0 + 60.0 * rnd.NextDouble());

                            g.TranslateTransform(x, y);
                            g.RotateTransform(a);

                            g.DrawString(code[i].ToString(), f, RandomBrushes[brush], 0, 0);

                            g.ResetTransform();
                        }
                    }
                }

                bmp.Save(Response.OutputStream, ImageFormat.Jpeg);
            }

            Response.End();
        }
    }
}
