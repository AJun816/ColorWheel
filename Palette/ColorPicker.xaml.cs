using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


namespace Palette
{
    public sealed partial class ColorPicker : UserControl
    {

        #region Win32API
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        #endregion
        public Path WheelPath { get; set; }

        public ColorPicker()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 绘制色环
        /// </summary>
        /// <param name="centerX">圆心坐标X点</param>
        /// <param name="centerY">圆心坐标Y点</param>
        /// <param name="radiusMax">外径</param>
        /// <param name="radiusMin">内径</param>
        private void DrawColorWheel(Double centerX, Double centerY, Double radiusMax, Double radiusMin, byte a)
        {
            // 创建绘图容器
            Grid colorWheel = new Grid() { Name = "xColorWheel" };
            colorWheel.Children.Clear();
            // 生成色环表
            List<Color> wheelColors = new List<Color>();
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(a, 255, i, 0));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(a, i, 255, 0));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(a, 0, 255, i));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(a, 0, i, 255));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(a, i, 0, 255));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(a, 255, 0, i));
            // 创建路径变量
            int colorCount = wheelColors.Count;     // 颜色数量
            Double angel = 360.0 / colorCount;      // 计算夹角(注：计算参数必须为浮点数，否则结果为0)
            Double rotate = 0;                      // 起始角度
            Double pointX, pointY;                  // 缓存绘图路径点
                                                    // 计算绘图路径
            wheelColors.ForEach((color) =>
            {
                pointX = centerX + radiusMax * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * Math.Sin(rotate * Math.PI / 180);
                Point point0 = new Point(pointX, pointY);
                pointX = centerX + radiusMin * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * Math.Sin(rotate * Math.PI / 180);
                Point point1 = new Point(pointX, pointY);
                rotate += angel;
                pointX = centerX + radiusMin * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * Math.Sin(rotate * Math.PI / 180);
                Point point2 = new Point(pointX, pointY);
                pointX = centerX + radiusMax * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * Math.Sin(rotate * Math.PI / 180);
                Point point3 = new Point(pointX, pointY);

                PathFigure wheelPath = new PathFigure() { IsClosed = true, StartPoint = point0 };
                wheelPath.Segments.Add(new LineSegment() { Point = point1 });
                wheelPath.Segments.Add(new ArcSegment() { Point = point2, SweepDirection = SweepDirection.Clockwise, Size = new Size(radiusMin, radiusMin) });
                wheelPath.Segments.Add(new LineSegment() { Point = point3 });
                wheelPath.Segments.Add(new ArcSegment() { Point = point0, Size = new Size(radiusMax, radiusMax) });

                PathGeometry wheelPaths = new PathGeometry();
                PathFigureCollection pathFigures = new PathFigureCollection();
                pathFigures.Add(wheelPath);
                wheelPaths.Figures = pathFigures;
                WheelPath = new Path() { StrokeThickness = 0, Fill = new SolidColorBrush(color), Data = wheelPaths };
                colorWheel.Children.Add(WheelPath);
            });
            colorWheel.HorizontalAlignment = HorizontalAlignment.Center;
            colorWheel.VerticalAlignment = VerticalAlignment.Center;
            colorWheel.AllowDrop = true;
            grid.Children.Add(colorWheel);

            Canvas.SetLeft(ellipse, centerX / 2 - 5);
            Canvas.SetTop(ellipse, centerY / 2);

            Point point = new Point() { X = centerX, Y = centerY };
            double redius = (radiusMax - radiusMin) / 2 + radiusMin;
            var p = GetPoints(point, (int)redius, 100);

            Canvas.SetLeft(BlackEllipse, p[0].X - 15);
            Canvas.SetTop(BlackEllipse, p[0].Y - 15);
        }

        Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);
            ellipse.Margin = new Thickness(left, top, 0, 0);
            return ellipse;
        }
        /// <summary>
        /// 获取圆周坐标
        /// </summary>
        /// <param name="pointCenter">中心坐标</param>
        /// <param name="r">半径</param>
        /// <param name="count">等分分数</param>
        /// <returns></returns>
        private Point[] GetPoints(Point pointCenter, int r, int count)
        {
            Point[] point = new Point[count];
            for (int i = 0; i < count; i++)
            {
                point[i].X = (int)(r * Math.Cos((i + 1) * 360 / count * Math.PI / 180)) + pointCenter.X;
                point[i].Y = (int)(r * Math.Sin((i + 1) * 360 / count * Math.PI / 180)) + pointCenter.Y;
            }
            return point;
        }

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            double radius = 100;
            byte type = (Byte)slider.Value;
            DrawColorWheel(50, 60, radius, radius * 0.6, type);
        }

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            UIElementCollection g = grid.Children;
            foreach (var item in g)
            {
                if (item is Grid)
                {
                    grid.Children.Remove(item);
                }
            }
            double radius = 100;
            byte type = (Byte)slider.Value;
            DrawColorWheel(50, 60, radius, radius * 0.6, type);
        }

        private void BlackEllipse_DragDelta(object sender, Windows.UI.Xaml.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Thumb thumb = sender as Thumb;
            //Point point = new Point() { X = 50, Y = 60 };
            //double redius = (100 - 60) / 2 + 60;
            //var p = GetPoints(point, (int)redius, 500);

            //int h = (int)Math.Abs(e.HorizontalChange);
            //int v = (int)Math.Abs(e.VerticalChange);

            //Canvas.SetLeft(thumb, e.HorizontalChange);
            //Canvas.SetTop(thumb, e.VerticalChange);

            ////if (h > v && h > 0)
            ////{
            ////    Canvas.SetLeft(thumb, p[h].X - 15);
            ////    Canvas.SetTop(thumb, p[h].Y - 15);
            ////}
            ////else if (v > h && v > 0)
            ////{
            ////    Canvas.SetLeft(thumb, p[v].X - 15);
            ////    Canvas.SetTop(thumb, p[v].Y - 15);
            ////}

            //Color color = GetColor((int)p[v].X - 15,(int)p[v].Y - 15);
            ////Color color = GetColor(900, 583);
            //ellipse.Fill = new SolidColorBrush(color);
        }

        /// <summary>
        /// 获取对应坐标的颜色值
        /// </summary>
        /// <param name="x">鼠标相对于显示器的坐标X</param>
        /// <param name="y">鼠标相对于显示器的坐标Y</param>
        /// <returns></returns>
        public static Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((Byte)pixel, (Byte)pixel, (Byte)pixel, (Byte)pixel);
            return color;
        }

        private void BlackEllipse_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Point point = new Point() { X = 50, Y = 60 };
            double redius = (100 - 60) / 2 + 60;
            var p = GetPoints(point, (int)redius, 500);

            int h = 0;
            int v = 0;

             h += (int)e.HorizontalChange;
             v += (int)e.VerticalChange;

            //Canvas.SetLeft(thumb,- h);
            //Canvas.SetTop(thumb, -v);

            if (h > v && h > 0)
            {
                Canvas.SetLeft(thumb, p[h].X - 15);
                Canvas.SetTop(thumb, p[h].Y - 15);
            }
            else if (v > h && v > 0)
            {
                Canvas.SetLeft(thumb, p[v].X - 15);
                Canvas.SetTop(thumb, p[v].Y - 15);
            }

            //Color color = GetColor((int)p[v].X - 15, (int)p[v].Y - 15);
            Color color = GetColor(900, 583);
            ellipse.Fill = new SolidColorBrush(color);
        }
    }
}
