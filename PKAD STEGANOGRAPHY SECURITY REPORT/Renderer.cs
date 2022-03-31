using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Linq;

namespace PKAD_STEGANOGRAPHY_SECURITY_REPORT
{
    public class Renderer
    {
        private int width = 0, height = 0;
        private double totHeight = 1000;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private string batchID = "";
        private List<BallotData> data = null;
        private Dictionary<string, int> precinct_map = null;

        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        Image redfingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "red_finger.png"));
        Image yellowfingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "yellow_finger.png"));
        private Dictionary<int, Color> colorDic = null;

        public Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public int getDataCount()
        {
            if (this.data == null) return 0;
            else return this.data.Count;
        }
        public int getPrecinctCount()
        {
            if (this.precinct_map == null) return 0;
            else return this.precinct_map.Count();
        }
        public List<BallotData> getData()
        {
            return this.data;
        }
        public void setChatData(List<BallotData> data, Dictionary<string, int> precinct_map, string batchID)
        {
            this.data = data;
            this.batchID = batchID;
            this.precinct_map = precinct_map;
            var sorted_precinct_map = precinct_map.OrderByDescending(o => o.Value);

            colorDic = new Dictionary<int, Color>();
            Random rnd = new Random();
            for (int i = 0; i< sorted_precinct_map.Count(); i++)
            {
                int count = sorted_precinct_map.ElementAt(i).Value;
                if (!colorDic.ContainsKey(count))
                {
                    colorDic[count] = Color.FromArgb(rnd.Next(50, 255), rnd.Next(50, 255), rnd.Next(50, 255));
                }
            }

        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }

        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)((a.X + 20) * px);
            res.Y = (int)((1000 - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)((p.X + 20) * px);
            res.Y = (int)((1000 - p.Y) * px);
            return res;
        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }
        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        public void drawString(Font font, Color brushColor, string content, Point o)
        {
            o = convertCoord(o);
            SolidBrush drawBrush = new SolidBrush(brushColor);
            gfx.DrawString(content, font, drawBrush, o.X, o.Y);
        }

        public void drawString(Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }
        public void drawPolygon(Pen pen, PointF[] curvePoints)
        {
            for (int i = 0; i< curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.DrawPolygon(pen, curvePoints);
        }
        public void drawPolygon(Pen pen, Point[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.DrawPolygon(pen, curvePoints);
        }
        public void fillPolygon(Brush brush, PointF[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.FillPolygon(brush, curvePoints);
        }
        public void fillPolygon(Brush brush, Point[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.FillPolygon(brush, curvePoints);
        }
        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }

        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle, string content = "")
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);

            o.X += size.Width / 2;
            o.Y -= size.Height / 2;
            float radius = size.Width * 0.3f;
            o.X += (int)(radius * Math.Cos(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            o.Y -= (int)(radius * Math.Sin(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            content += "\n" + string.Format("{0:F}%", sweepAngle * 100.0f / 360.0f);
            drawString(o, content, 9);
        }
        public void drawFilledCircle(Brush brush, Point o, Size size)
        {
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);

            gfx.FillEllipse(brush, rect);
        }
        public void drawRoundedRectangle(Pen pen, Rectangle rect, int borderRadius)
        {

            using (GraphicsPath path = RoundedRect(rect, borderRadius))
            {
                SmoothingMode initMode = gfx.SmoothingMode;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.DrawPath(pen, path);
                gfx.SmoothingMode = initMode;
            }
        }
        public GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {

            //Convert Rectangle Coord
            bounds.Location = convertCoord(bounds.Location);
            double px = height / totHeight;
            bounds.Width = (int)(bounds.Width * px);
            bounds.Height = (int)(bounds.Height * px);


            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }


        public void draw(int chatIndex)
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }

            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }


            Font textFont = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point);
            Font textNumberFont = new Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Point);
            Font titleNumberFont = new Font("Arial", 30, FontStyle.Bold, GraphicsUnit.Point);
            Font titleFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Point);
            Font descFont = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font bold_descFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);



            Color grayColor = Color.FromArgb(84, 84, 84);
            Color yellowColor = Color.FromArgb(255, 222, 90);

            Brush grayBrush = new SolidBrush(grayColor);
            Brush yellowBrush = new SolidBrush(yellowColor);


            Point point1 = new Point(20, 910);
            Point point2 = new Point(1650, 910);
            Point point3 = new Point(1670, 890);
            Point point4 = new Point(1670, 850);
            Point point5 = new Point(20, 850);

            drawPolygon(new Pen(Color.Black, 5), new Point[] { point1, point2, point3, point4, point5 });
            fillPolygon(yellowBrush, new Point[] { point1, point2, point3, point4, point5 });

            point1 = new Point(0, 990);
            point2 = new Point(1570, 990);
            point3 = new Point(1570, 910);
            point4 = new Point(1650, 910);
            point5 = new Point(1650, 870);
            Point point6 = new Point(0, 870);
            Point[] curvePoints = { point1, point2, point3, point4, point5, point6 };

            drawPolygon(new Pen(Color.Black, 5), new Point[] { point1, point2, point4, point5, point6 }); 
            fillPolygon(grayBrush, new Point[]  { point1, point2, point4, point5, point6});

            drawLine(point2, point3, Color.Black, 3);
            drawLine(point4, point3, Color.Black, 3);


            if (this.data == null) return;

            drawCenteredString("PKAD STEGANOGRAPHY\nSECURITY REPORT", new Rectangle(0, 980, 600, 120), Brushes.White, titleFont);
            string[] tmp = batchID.Split("/");
            string idstring = "";
            if (tmp.Length >= 2)
            {
                if (tmp[tmp.Length - 2].Trim().ToLower().StartsWith("pallet")) {
                    idstring = tmp[tmp.Length - 2] + "/" + tmp[tmp.Length - 1];
                }
            }
            drawCenteredString(idstring , new Rectangle(800, 980, 800, 120), Brushes.White, titleFont);

            //////////////////Draw Confirmed 3 circles//////////////////////////////////////////////////
            ///

            drawFilledCircle(yellowBrush, new Point(600, 965), new Size(30, 30));
            drawFilledCircle(yellowBrush, new Point(650, 965), new Size(30, 30));
            drawFilledCircle(yellowBrush, new Point(630, 920), new Size(30, 30));
            //////////////////////////////////////////////////////////////////////////////////////////////

            int initHeight = 830;
            fillRectangle(grayColor, new Rectangle(20, initHeight, 1650, 300));

            fillRectangle(Color.White, new Rectangle(30, initHeight-10, 130, 80));
            drawCenteredString("#-BAL", new Rectangle(20, initHeight, 150, 100), Brushes.Black, titleFont);
            drawCenteredString("EV", new Rectangle(170, initHeight, 100, 100), Brushes.White, titleFont);


            fillRectangle(Color.White, new Rectangle(280, initHeight-10, 80, 80));
            drawCenteredString("ED", new Rectangle(270, initHeight, 100, 100), Brushes.Black, titleFont);
            drawCenteredString("FD", new Rectangle(370, initHeight, 100, 100), Brushes.White, titleFont);
            drawCenteredString("CONFIRMED", new Rectangle(550, initHeight, 300, 100), Brushes.White, titleFont);


            /////////////////////////Draw 3 yellow circle////////////////////////////////////////////////
            ///
            drawFilledCircle(yellowBrush, new Point(500, initHeight - 20), new Size(20, 20));
            drawFilledCircle(yellowBrush, new Point(500, initHeight - 60), new Size(20, 20));
            drawFilledCircle(yellowBrush, new Point(540, initHeight - 45), new Size(20, 20));
            ///////////////////////////////////////////////////////////
            ///


            initHeight = 730;
            
            drawCenteredString(data.Count.ToString() , new Rectangle(20, initHeight, 150, 100), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(170, initHeight - 10 , 100, 80));

            int count = data.Where(item => item.type == "EV").Count();
            double percent = 0;
            percent = count * 100 / data.Count();
            drawCenteredString(string.Format("{0:N0}%", percent), new Rectangle(170, initHeight - 10, 100, 80), Brushes.Black, titleFont);
            count = data.Where(item => item.type == "ED").Count();

            percent = count * 100 / data.Count();
            drawCenteredString(string.Format("{0:N0}%", percent), new Rectangle(270, initHeight, 100, 100), Brushes.White, titleFont);


            fillRectangle(Color.White, new Rectangle(370, initHeight - 10, 100, 80));
            count = data.Where(item => item.type == "FD").Count();
            percent = count * 100 / data.Count();
            drawCenteredString(string.Format("{0:N0}%", percent), new Rectangle(370, initHeight - 10, 100, 80), Brushes.Black, titleFont);

            ////////////////////////////////////////////////////////////////////
            fillRectangle(Color.White, new Rectangle(530, initHeight - 10, 140, 80));
            count = data.Where(item => item.mic.ToLower() == "confirmed").Count();
            drawCenteredString(count.ToString(), new Rectangle(530, initHeight - 10, 140, 80), Brushes.Black, titleFont);

            fillRectangle(Color.White, new Rectangle(690, initHeight - 10, 140, 80));
            count = data.Where(item => item.mic.ToLower() == "undetected").Count();
            percent = count * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(690, initHeight - 10, 140, 80), Brushes.Black, titleFont);


            fillRectangle(Color.White, new Rectangle(850, initHeight - 10, 150, 80));

            Dictionary<string, int> ooc_map = new Dictionary<string, int>();
            int max_ooc = 0;
            foreach (var item in data)
            {
                int iooc = 0;
                tmp = item.ooc.Split("_");
                if (tmp.Length >=2 )
                {
                    if ( int.TryParse(tmp[tmp.Length - 1], out iooc) )
                    {
                        max_ooc = iooc > max_ooc ? iooc : max_ooc;                        
                    }
                    if (ooc_map.ContainsKey(item.ooc))
                        ooc_map[item.ooc]++;
                    else ooc_map[item.ooc] = 1;
                }
            }
            drawCenteredString(string.Format("{0}%", max_ooc), new Rectangle(850, initHeight - 10, 150, 80), Brushes.Black, titleFont);
            drawCenteredString("HI - OoC", new Rectangle(850, initHeight + 70, 150, 80), Brushes.White, textFont);

            //fillRectangle(Color.White, new Rectangle(600, initHeight + 30, 200, 120));

            //Fill white rectangle for yellow finger image;
            fillRectangle(Color.White, new Rectangle(1010, initHeight + 100, 200, 190));            

            fillRectangle(Color.White, new Rectangle(1220, initHeight + 90, 440, 80));
            drawCenteredString("CALIBRATION SIGNATURE\nVARIANCE ANOMOLIES", new Rectangle(1220, initHeight + 90, 440, 80), Brushes.Black, textFont);
            
           
            drawCenteredString(string.Format("{0} IDENTIFIED", ooc_map.Count()), new Rectangle(1220, initHeight, 440, 100), Brushes.White, titleFont);
            if (ooc_map.Count() >= 2)
                drawImg(yellowfingerImg, new Point(1020, initHeight + 90), new Size(180, 170));

            initHeight = 630;
            fillRectangle(Color.White, new Rectangle(30, initHeight - 10, 250, 80));

            var sorted_precinct_map = precinct_map.OrderByDescending(o => o.Value);

            drawCenteredString(string.Format("{0} PRECINCTS", sorted_precinct_map.Count()), new Rectangle(30, initHeight - 10, 250, 80), Brushes.Black, textFont);
            int max_cnt = 0;
            Dictionary<string, int> count_map = new Dictionary<string, int>();
            count_map["N"] = data.Where(item => item.letter == "N").Count(); max_cnt = count_map["N"] > max_cnt ? count_map["N"] : max_cnt;
            count_map["P"] = data.Where(item => item.letter == "P").Count(); max_cnt = count_map["P"] > max_cnt ? count_map["P"] : max_cnt;
            count_map["W"] = data.Where(item => item.letter == "W").Count(); max_cnt = count_map["W"] > max_cnt ? count_map["W"] : max_cnt;
            count_map["Z"] = data.Where(item => item.letter == "Z").Count(); max_cnt = count_map["Z"] > max_cnt ? count_map["Z"] : max_cnt;
            count_map["NA"] = data.Where(item => item.letter == "NA").Count(); max_cnt = count_map["NA"] > max_cnt ? count_map["NA"] : max_cnt;

            
            drawCenteredString("N", new Rectangle(280, initHeight - 10, 60, 80), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(340, initHeight - 10, 160, 80));
            
            drawCenteredString(count_map["N"].ToString(), new Rectangle(340, initHeight - 10, 60, 80), max_cnt == count_map["N"] ? Brushes.Green : Brushes.Black, textFont);
            percent = count_map["N"] * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(390, initHeight - 10, 100, 80), max_cnt == count_map["N"] ? Brushes.Green : Brushes.Black, textNumberFont);


            drawCenteredString("P", new Rectangle(500, initHeight - 10, 60, 80), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(560, initHeight - 10, 160, 80));
            drawCenteredString(count_map["P"].ToString(), new Rectangle(560, initHeight - 10, 60, 80), max_cnt == count_map["P"] ? Brushes.Green : Brushes.Black, textFont);
            percent = count_map["P"] * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(620, initHeight - 10, 100, 80), max_cnt == count_map["P"] ? Brushes.Green : Brushes.Black, textNumberFont);


            drawCenteredString("W", new Rectangle(720, initHeight - 10, 60, 80), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(780, initHeight - 10, 160, 80));
            drawCenteredString(count_map["W"].ToString(), new Rectangle(780, initHeight - 10, 60, 80), max_cnt == count_map["W"] ? Brushes.Green : Brushes.Black, textFont);
            percent = count_map["W"] * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(840, initHeight - 10, 100, 80), max_cnt == count_map["W"] ? Brushes.Green : Brushes.Black, textNumberFont);


            drawCenteredString("Z", new Rectangle(940, initHeight - 10, 60, 80), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(1000, initHeight - 10, 160, 80));
            count = data.Where(item => item.letter == "Z").Count();
            drawCenteredString(count_map["Z"].ToString(), new Rectangle(1000, initHeight - 10, 60, 80), max_cnt == count_map["Z"] ? Brushes.Green : Brushes.Black, textFont);
            percent = count_map["Z"] * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(1060, initHeight - 10, 100, 80), max_cnt == count_map["Z"] ? Brushes.Green : Brushes.Black, textNumberFont);

            drawCenteredString("X", new Rectangle(1160, initHeight - 10, 60, 80), Brushes.White, titleFont);
            fillRectangle(Color.White, new Rectangle(1220, initHeight - 10, 160, 80));
            drawCenteredString(count_map["NA"].ToString(), new Rectangle(1220, initHeight - 10, 60, 80), max_cnt == count_map["NA"] ? Brushes.Green : Brushes.Black, textFont);
            percent = count_map["NA"] * 100 / (double)data.Count();
            drawCenteredString(string.Format("{0:N2}%", percent), new Rectangle(1280, initHeight - 10, 100, 80), max_cnt == count_map["NA"] ? Brushes.Green : Brushes.Black, textNumberFont);


            drawCenteredString("COUNTERFEIT\nDETECTION", new Rectangle(1400, initHeight, 270, 100), Brushes.White, textFont);
            fillRectangle(grayColor, new Rectangle(1400, initHeight - 100, 270, 270));
            fillRectangle(Color.White, new Rectangle(1410, initHeight - 110, 250, 250));
            drawImg(redfingerImg, new Point(1435, initHeight - 135), new Size(200, 200));
            count = data.Where(item => item.ooc.ToUpper() == "UNDETERMINED").Count();
            drawCenteredString(count.ToString(), new Rectangle(1485, initHeight - 185, 150, 150), Brushes.Black, new Font("Arial", 50, FontStyle.Bold, GraphicsUnit.Point));
            count = data.Where(item => item.letter != "N" && item.letter !="P" && item.letter != "W" && item.letter != "Z" && item.letter != "NA").Count();

            fillRectangle(grayColor, new Rectangle(1570, initHeight - 370, 100, 40));
            drawCenteredString(string.Format("NR-{0}", count), new Rectangle(1570, initHeight - 370, 100, 40), Brushes.White, textFont);

            drawImg(logoImg, new Point(1400, 200), new Size(250, 100));
            drawString(descFont, Color.Black, "© 2021 Tesla Laboratories, llc & JHP", new Point(1400, 100));


            int basex = 20, basey = initHeight - 120;
            int xstep = 330, ystep = 30;
            int prev_cnt = 0;
            for (int i = chatIndex * 64; i< Math.Min(sorted_precinct_map.Count(), chatIndex * 64 + 64); i++)
            {
                string precinct = sorted_precinct_map.ElementAt(i).Key;
                count = sorted_precinct_map.ElementAt(i).Value;
                if (count != prev_cnt) prev_cnt = count;

                int x = basex + xstep * ((i % 64) / 16);
                int y = basey - ystep * ((i % 64) % 16);
                fillRectangle(colorDic[prev_cnt], new Rectangle(x, y + 5, xstep, ystep + 1));
                drawString(Color.Black,  new Point(x, y), precinct, 10);
                percent = count * 100 / (double)data.Count();
                drawString(Color.Black, new Point(x + 200, y), string.Format("{0} - {1:N2}%", count, percent), 10);
            }


            int chartCount = 1;
            chartCount = chartCount + sorted_precinct_map.Count() / 64;
            if (sorted_precinct_map.Count() % 64 == 0) chartCount--;

            if (chartCount >= 2)
                drawCenteredString(string.Format("{0} of {1}", chatIndex + 1, chartCount), new Rectangle(1000, 65, 400, 100), Brushes.Red, titleFont);

        }
    }
}
