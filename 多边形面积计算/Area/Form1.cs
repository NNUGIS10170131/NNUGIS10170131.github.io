using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Area
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string fileName;

        //WGS84坐标系参数
        public static int a = 6378137;//长轴
        public static double b = 6356752.3142;//短轴
        public static double PI = 3.14159265;//圆周率

        //屏幕参数
        public static int width = 698;//panel的宽
        public static int height = 524;//panel的高
        Bitmap bitmap = new Bitmap(width, height);

        //江苏省的四至
        double maxJSX = 13574614.8546856;
        double maxJSX_Y = 3704087.05481275;
        double minJSX = 12952537.4951228;
        double minJSX_Y = 4101836.00894706;
        double maxJSY = 4156216.54314818;
        double maxJSY_X = 13273585.8008774;
        double minJSY = 3579743.801743;
        double minJSY_X = 13413840.1216346;
        static double eps = 0.0000000000000000001;

        List<Point_new[]> pointsArrayList = new List<Point_new[]>();//存放点数据

        public static List<double> area = new List<double>();//存放面积的数组

        struct Point_new
        {
            public double x;
            public double y;

            public Point_new(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "..\\";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //绘制江苏省地图
        private void display_Click(object sender, EventArgs e)
        {
            int i = 0, j = 0;
            bitmap = new Bitmap(width, height);
            panel1.Refresh();
            Graphics g = Graphics.FromImage(bitmap);

            double scaleX, scaleY;

            Pen my_pen = new Pen(Color.Blue, 1);

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string str = sr.ReadLine();
                    while (str != "END")
                    {
                        List<Point_new> line = new List<Point_new>();
                        while ((str = sr.ReadLine()) != "END")
                        {
                            string[] split = str.Split(',');
                            line.Add(new Point_new(Convert.ToDouble(split[0]), Convert.ToDouble(split[1])));

                        }
                        Point_new[] line_new = new Point_new[line.Count];
                        for (i = 0; i < line.Count; ++i)
                        {
                            line_new[i] = line[i];
                        }

                        pointsArrayList.Add(line_new);
                        str = sr.ReadLine();
                    }

                    scaleX = (maxJSX - minJSX) / width;
                    scaleY = (maxJSY - minJSY) / height;

                    if (scaleX > scaleY)
                    {
                        for (i = 0; i < pointsArrayList.Count; i++)
                        {
                            Point[] MapPoints = new Point[pointsArrayList[i].Length];
                            for (j = 0; j < pointsArrayList[i].Length; j++)
                            {
                                MapPoints[j].X = (int)((pointsArrayList[i][j].x - minJSX) / scaleX);
                                MapPoints[j].Y = (int)((maxJSY - pointsArrayList[i][j].y) / scaleX);
                            }
                            g.DrawLines(my_pen, MapPoints);
                        }
                    }
                    else
                    {
                        for (i = 0; i < pointsArrayList.Count; i++)
                        {
                            Point[] MapPoints = new Point[pointsArrayList[i].Length];
                            for (j = 0; j < pointsArrayList[i].Length; j++)
                            {
                                MapPoints[j].X = (int)((pointsArrayList[i][j].x - minJSX) / scaleY);
                                MapPoints[j].Y = (int)((maxJSY - pointsArrayList[i][j].y) / scaleY);
                            }
                            g.DrawLines(my_pen, MapPoints);
                        }
                    }
                    panel1.BackgroundImage = bitmap;
                    panel1.Refresh();
                }

            }
            catch (Exception ep11)
            {
                MessageBox.Show("The file cannot be read properly,because" + ep11.Message);
            }
        }

        //求平面坐标系下的地市面积
        private void button1_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            area.Clear();
            double temp = 0;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string str = sr.ReadLine();
                    while (str != "END")
                    {
                        List<Point_new> line = new List<Point_new>();
                        while ((str = sr.ReadLine()) != "END")
                        {
                            string[] split = str.Split(',');
                            line.Add(new Point_new(Convert.ToDouble(split[0]), Convert.ToDouble(split[1])));

                        }
                        Point_new[] line_new = new Point_new[line.Count];
                        for (int i = 0; i < line.Count; ++i)
                        {
                            line_new[i] = line[i];
                        }

                        pointsArrayList.Add(line_new);
                        str = sr.ReadLine();
                    }
                    for (int i = 0; i < pointsArrayList.Count; ++i)
                    {
                        if (i >= 1 && i <= 6)
                        {
                            for (int j = 1; j < pointsArrayList[i].Length; ++j)
                            {
                                if (j == pointsArrayList[i].Length - 1)
                                    temp = temp + pointsArrayList[i][j].x * (pointsArrayList[i][1].y - pointsArrayList[i][j - 1].y) / 2;
                                else
                                    temp = temp + pointsArrayList[i][j].x * (pointsArrayList[i][j + 1].y - pointsArrayList[i][j - 1].y) / 2;
                            }
                            if (i == 6)
                                area.Add(Math.Abs(temp));
                        }
                        else
                        {
                            for (int j = 1; j < pointsArrayList[i].Length; ++j)
                            {
                                if (j == pointsArrayList[i].Length - 1)
                                    temp = temp + pointsArrayList[i][j].x * (pointsArrayList[i][1].y - pointsArrayList[i][j - 1].y) / 2;
                                else
                                    temp = temp + pointsArrayList[i][j].x * (pointsArrayList[i][j + 1].y - pointsArrayList[i][j - 1].y) / 2;
                            }
                            area.Add(Math.Abs(temp));
                        }
                        temp = 0;
                    }
                }
            }
            catch (Exception ep11)
            {
                MessageBox.Show("The file cannot be read properly,because" + ep11.Message);
            }
            for (int i = 0; i < 13; ++i)
            {
                richTextBox1.Text += "第" + (i + 1) + "块地市的面积为：" + Form1.area[i].ToString() + "m²" + Environment.NewLine;
            }
        }

        //墨卡托投影反算程序，转换上述数据为经纬度坐标，并显示
        private void display1_Click(object sender, EventArgs e)
        {
            bitmap = new Bitmap(width, height);
            Graphics g1 = Graphics.FromImage(bitmap);
            Pen my_pen = new Pen(Color.Blue, 1);

            double B0 = 0;//标准纬线
            double ep1_84 = Math.Sqrt((Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(a, 2));
            double ep2_84 = Math.Pow((Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(b, 2), 2);
            double K = Math.Pow(a, 2) / b / Math.Sqrt(1 + ep2_84 * Math.Pow(Math.Cos(B0), 2)) * Math.Cos(B0);

            double LMax = reverse_Mercator(new Point_new(maxJSX, maxJSX_Y), K, ep1_84, 0).x;
            double BMax = reverse_Mercator(new Point_new(maxJSY_X, maxJSY), K, ep1_84, 0).y;
            double LMin = reverse_Mercator(new Point_new(minJSX, minJSX_Y), K, ep1_84, 0).x;
            double BMin = reverse_Mercator(new Point_new(minJSY_X, minJSY), K, ep1_84, 0).y;

            double scalex = (LMax - LMin) / width;
            double scaley = (BMax - BMin) / height;

            if (scalex > scaley)
            {
                for (int i = 0; i < pointsArrayList.Count; i++)
                {
                    Point[] screenPoints = new Point[pointsArrayList[i].Length];
                    for (int j = 0; j < pointsArrayList[i].Length; j++)
                    {
                        pointsArrayList[i][j] = reverse_Mercator(pointsArrayList[i][j], K, ep1_84, 0);
                        screenPoints[j].X = (int)((pointsArrayList[i][j].x - LMin) / scalex);
                        screenPoints[j].Y = (int)((BMax - pointsArrayList[i][j].y) / scalex);
                    }
                    g1.DrawLines(my_pen, screenPoints);
                }
            }
            else
            {
                for (int i = 0; i < pointsArrayList.Count; i++)
                {
                    Point[] screenPoints = new Point[pointsArrayList[i].Length];
                    for (int j = 0; j < pointsArrayList[i].Length; j++)
                    {
                        pointsArrayList[i][j] = reverse_Mercator(pointsArrayList[i][j], K, ep1_84, 0);
                        screenPoints[j].X = (int)((pointsArrayList[i][j].x - LMin) / scaley);
                        screenPoints[j].Y = (int)((BMax - pointsArrayList[i][j].y) / scaley);
                    }
                    g1.DrawLines(my_pen, screenPoints);
                }
            }
            panel1.BackgroundImage = bitmap;
        }

        //墨卡托投影反算程序
        Point_new reverse_Mercator(Point_new point, double K, double e, double B)
        {
            Point_new new_Point;
            double B_old = 0;
            double B_new = 0;
            double L0 = 0;

            while (true)
            {
                B_new = PI / 2 - 2 * Math.Atan(Math.Exp(-point.y / K) * Math.Exp(e / 2 *
                    Math.Log((1 - e * Math.Sin(B_old)) / (1 + e * Math.Sin(B_old)))));
                if (B_new - B_old < eps)
                    break;
                B_old = B_new;
            }
            new_Point.x = (point.x / K + L0) * 180 / PI;
            new_Point.y = B_new * 180 / PI;

            return new_Point;
        }

        private void displayJiangSuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //基于经纬度坐标计算江苏省十三个地市的面积
        private void button2_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();//清除文本框控件中的文本
            area.Clear();//移除area中的元素
            double ep1_84 = Math.Sqrt((Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(a, 2));
            double K;
            double A = 1 + 1.000000 / 2 * Math.Pow(ep1_84, 2) + 3.000000 / 8 * Math.Pow(ep1_84, 4) + 5.000000 / 16 * Math.Pow(ep1_84, 6);
            double B = 1.000000 / 6 * Math.Pow(ep1_84, 2) + 3.000000 / 16 * Math.Pow(ep1_84, 4) + 3.000000 / 16 * Math.Pow(ep1_84, 6);
            double C = 3.000000 / 80 * Math.Pow(ep1_84, 4) + 1.000000 / 16 * Math.Pow(ep1_84, 6);
            double D = 1.000000 / 112 * Math.Pow(ep1_84, 6);

            for (int i = 0; i < pointsArrayList.Count; ++i)
            {
                double[] T = new double[pointsArrayList.Count];

                if (i >= 1 && i <= 6)
                {
                    for (int j = 0; j < pointsArrayList[i].Length - 1; ++j)
                    {
                        double fi_diff = pointsArrayList[i][j].y * PI / 180;
                        double fi_m = pointsArrayList[i][j].y * PI / 180 / 2;
                        K = 2 * Math.Pow(a, 2) * (1 - Math.Pow(ep1_84, 2)) * ((pointsArrayList[i][j + 1].x
                            - pointsArrayList[i][j].x) * PI / 180);
                        T[i] += K * (A * Math.Sin(fi_diff / 2) * Math.Cos(fi_m) - B * Math.Sin(3 * fi_diff / 2) * Math.Cos(3 * fi_m) +
                            C * Math.Sin(5 * fi_diff / 2) * Math.Cos(5 * fi_m) - D * Math.Sin(7 * fi_diff / 2) * Math.Cos(7 * fi_m));
                    }
                    if (i == 6)
                        area.Add(Math.Abs(T[i]));
                }
                else
                {
                    for (int j = 0; j < pointsArrayList[i].Length - 1; ++j)
                    {
                        double fi_diff = pointsArrayList[i][j].y * PI / 180;
                        double fi_m = pointsArrayList[i][j].y * PI / 180 / 2;
                        K = 2 * Math.Pow(a, 2) * (1 - Math.Pow(ep1_84, 2)) * ((pointsArrayList[i][j + 1].x - pointsArrayList[i][j].x) * PI / 180);
                        T[i] += K * (A * Math.Sin(fi_diff / 2) * Math.Cos(fi_m) - B * Math.Sin(3 * fi_diff / 2) * Math.Cos(3 * fi_m) +
                            C * Math.Sin(5 * fi_diff / 2) * Math.Cos(5 * fi_m) - D * Math.Sin(7 * fi_diff / 2) * Math.Cos(7 * fi_m));
                    }
                    area.Add(Math.Abs(T[i]));
                }
            }
            for (int i = 0; i < 13; ++i)
            {
                richTextBox1.Text += "第" + (i + 1) + "块地市的面积为：" + Form1.area[i].ToString() + "m²" + Environment.NewLine;
            }
        }

    }
}
