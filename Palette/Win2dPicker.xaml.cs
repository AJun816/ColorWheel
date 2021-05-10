using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Palette
{
    public sealed partial class Win2dPicker : UserControl
    {
        private readonly Vector2 _centerVector = new Vector2() { X = 200, Y = 200 };
        private readonly float _radiusMax = 120;
        private readonly float _radiusMin = 70;
        private readonly float _radiusCenter = 40;
        private readonly float _radiusGetColor = 20;

        byte Argb_A = 255;
        int _pointGetColor;
        bool _isGetColor;

        List<Color> wheelColors = new List<Color>();

        public Win2dPicker()
        {
            this.InitializeComponent();



            CreateWheelColors();
            CreateWheelColors();
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            float centerX = _centerVector.X;
            float centerY = _centerVector.Y;
            float radiusMax = _radiusMax;
            float radiusMin = _radiusMin;


            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, 255, i, 0));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, i, 255, 0));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, 0, 255, i));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, 0, i, 255));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, i, 0, 255));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, 255, 0, i));
            // 创建路径变量                                              
            int colorCount = wheelColors.Count;     // 颜色数量
            Double angel = 360.0 / colorCount;      // 计算夹角(注：计算参数必须为浮点数，否则结果为0)
            Double rotate = 0;                      // 起始角度
            float pointX, pointY;                  // 缓存绘图路径点

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
                args.DrawingSession.DrawGeometry(apple, color);
            });

            Vector2 point = new Vector2() { X = centerX, Y = centerY };
            float redius = (radiusMax - radiusMin) / 2 + radiusMin;
            Vector2[] p = GetPoints(point, (int)redius, wheelColors.Count);

            args.DrawingSession.DrawCircle(_centerVector, _radiusCenter, wheelColors[_pointGetColor]);
            args.DrawingSession.FillCircle(_centerVector, _radiusCenter, wheelColors[_pointGetColor]);
            args.DrawingSession.DrawCircle(p[_pointGetColor], _radiusGetColor, Colors.Wheat);
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

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Argb_A = (Byte)slider.Value;
            canvasControl.Invalidate();
        }

        private void canvasControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = true;
        }

        private void canvasControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (_isGetColor)
            {
                PointerPoint p = e.GetCurrentPoint(canvasControl);

                double x = canvasControl.Width / wheelColors.Count;
                double y = canvasControl.Height / wheelColors.Count;

                if (p.Position.X > p.Position.Y)
                {
                    _pointGetColor = (int)(p.Position.X / x);
                    canvasControl.Invalidate();
                }
                else
                {
                    _pointGetColor = (int)(p.Position.Y / y);
                    canvasControl.Invalidate();
                }
            }

        }

        private void canvasControl_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = false;
        }
    }
}
