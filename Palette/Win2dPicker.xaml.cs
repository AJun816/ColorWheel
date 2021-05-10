

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;

using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Palette
{
    public sealed partial class Win2dPicker : UserControl
    {
        private readonly Vector2 _centerVector;
        private readonly float _radiusMax;
        private readonly float _radiusMin;
        private readonly float _radiusCenter;
        private readonly float _radiusGetColor;


        public Win2dPicker()
        {
            this.InitializeComponent();

            _centerVector = new Vector2() { X = 200, Y = 200 };
            _radiusMax = 120;
            _radiusMin = 70;
            _radiusCenter = 40;
            _radiusGetColor = 20;
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            float centerX = _centerVector.X;
            float centerY = _centerVector.Y;
            float radiusMax = _radiusMax;
            float radiusMin = _radiusMin;
            byte a = 255;

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
            float pointX, pointY;                  // 缓存绘图路径点

            // 计算绘图路径
            wheelColors.ForEach((color) =>
            {
                pointX = centerX + radiusMin * (float)Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * (float)Math.Sin(rotate * Math.PI / 180);
                Vector2 point1 = new Vector2(pointX, pointY);
                rotate += angel;
                pointX = centerX + radiusMax * (float)Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * (float)Math.Sin(rotate * Math.PI / 180);
                Vector2 point3 = new Vector2(pointX, pointY);

                CanvasPathBuilder path = new CanvasPathBuilder(sender);
                path.BeginFigure(point1);
                path.AddLine(point3);
                path.EndFigure(CanvasFigureLoop.Open);
                CanvasGeometry apple = CanvasGeometry.CreatePath(path);
                args.DrawingSession.DrawGeometry(apple,color);

                args.DrawingSession.DrawCircle(_centerVector, _radiusCenter, color);
                args.DrawingSession.FillCircle(_centerVector, _radiusCenter, color);

              
            });
            Vector2 point = new Vector2() { X = centerX, Y = centerY };
            float redius = (radiusMax - radiusMin) / 2 + radiusMin;
            Vector2[] p = GetPoints(point, (int)redius, 100);

            args.DrawingSession.DrawCircle(p[10], _radiusGetColor, Colors.Wheat);
        }

        /// <summary>
        /// 获取圆周坐标
        /// </summary>
        /// <param name="pointCenter">中心坐标</param>
        /// <param name="r">半径</param>
        /// <param name="count">等分分数</param>
        /// <returns></returns>
        private Vector2[] GetPoints(Vector2 pointCenter, int r, int count)
        {
            Vector2[] point = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                point[i].X = (int)(r * Math.Cos((i + 1) * 360 / count * Math.PI / 180)) + pointCenter.X;
                point[i].Y = (int)(r * Math.Sin((i + 1) * 360 / count * Math.PI / 180)) + pointCenter.Y;
            }
            return point;
        }

    }
}
