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
        public const string MimeType = "image/jpeg";

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

        public static string GetUrl(int width, int height)
        {
            return String.Format(
                "~/Apps/Common/Captcha.aspx?width={0}&height={1}",
                width,
                height);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = MimeType;
            Response.Expires = -1;

            var width = int.Parse(Request.QueryString["width"]);
            var height = int.Parse(Request.QueryString["height"]);
            var digits = 8; // this is not a parameter as it could be set to 0

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
