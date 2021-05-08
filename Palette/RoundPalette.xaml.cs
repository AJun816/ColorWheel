using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Palette
{
    public sealed partial class RoundPalette : UserControl
    {
        public RoundPalette()
        {
            this.InitializeComponent();
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            
                // 创建绘图容器
                Grid colorWheel = new Grid() { Name = "xColorWheel" };
                // 生成色环表
                List<Color> wheelColors = new List<Color>();
                for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(0, 255, i, 0));
                for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(0, i, 255, 0));
                for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(0, 0, 255, i));
                for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(0, 0, i, 255));
                for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(0, i, 0, 255));
                for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(0, 255, 0, i));
                // 创建路径变量
                int colorCount = wheelColors.Count;     // 颜色数量
                Double angel = 360.0 / colorCount;      // 计算夹角(注：计算参数必须为浮点数，否则结果为0)
                Double rotate = 0;                      // 起始角度
                Double pointX, pointY;                  // 缓存绘图路径点
                Double centerX = 20;
                Double centerY = 20;
                Double radiusMax = 30;
                Double radiusMin = 40;
            wheelColors.ForEach((color) =>
            {
                pointX = centerX + radiusMax * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * Math.Sin(rotate * Math.PI / 180);
                Point point0 = new Point(pointX, pointY);
                Vector2 vector0 = new Vector2((float)pointX, (float)pointY);
                pointX = centerX + radiusMin * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * Math.Sin(rotate * Math.PI / 180);
                Point point1 = new Point(pointX, pointY);
                Vector2 vector1 = new Vector2((float)pointX, (float)pointY);
                rotate += angel;
                pointX = centerX + radiusMin * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * Math.Sin(rotate * Math.PI / 180);
                Point point2 = new Point(pointX, pointY);
                Vector2 vector2 = new Vector2((float)pointX, (float)pointY);
                pointX = centerX + radiusMax * Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * Math.Sin(rotate * Math.PI / 180);
                Point point3 = new Point(pointX, pointY);
                Vector2 vector3 = new Vector2((float)pointX, (float)pointY);

                PathFigure wheelPath = new PathFigure() { IsClosed = true, StartPoint = point0 };
                wheelPath.Segments.Add(new LineSegment() { Point = point1 });
                wheelPath.Segments.Add(new ArcSegment() { Point = point2, SweepDirection = SweepDirection.Clockwise, Size = new Size(radiusMin, radiusMin) });
                wheelPath.Segments.Add(new LineSegment() { Point = point3 });
                wheelPath.Segments.Add(new ArcSegment() { Point = point0, Size = new Size(radiusMax, radiusMax) });

                args.DrawingSession.DrawLine(vector0, vector1, color);
                args.DrawingSession.FillCircle(vector0.X, vector0.Y, (float)radiusMax, color);


                    //PathGeometry wheelPaths = new PathGeometry();
                    //PathFigureCollection pathFigures = new PathFigureCollection();
                    //pathFigures.Add(wheelPath);
                    //wheelPaths.Figures = pathFigures;
                    //colorWheel.Children.Add(new Path() { StrokeThickness = 0, Fill = new SolidColorBrush(color), Data = wheelPaths });
                });


            //args.DrawingSession.FillCircle(200, 150, 70, Colors.Black);

            //args.DrawingSession.FillCircle(200, 150, 40, Colors.Orange);

            //args.DrawingSession.DrawCircle(200, 150, 15, Colors.White);


            using (var canvasPathBuilder = new CanvasPathBuilder(args.DrawingSession))
            {
                // 这里可以画出 Path 或写出文字 lindexi.github.io
                canvasPathBuilder.BeginFigure(1, 1);
                canvasPathBuilder.AddLine(300, 300);
                canvasPathBuilder.AddLine(1, 300);
                canvasPathBuilder.EndFigure(CanvasFigureLoop.Closed);
                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(canvasPathBuilder), Colors.Gray, 2);
            }

        }

       


    }

}
