using Microsoft.Graphics.Canvas;
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
    public sealed partial class Win2dCanvasVirtualPicker : UserControl
    {

        private readonly Vector2 _centerVector;
        private readonly float _radiusMax;
        private readonly float _radiusMin;
        private readonly float _radiusCenter;
        private readonly float _radiusGetColor;

        byte Argb_A = 255;
        int _pointGetColor;
        bool _isGetColor;
        Vector2[] _getColorPoint;

        List<Color> wheelColors = new List<Color>();

        public Win2dCanvasVirtualPicker()
        {
            this.InitializeComponent();

            _centerVector = new Vector2() { X = 200, Y = 200 };
            _radiusMax = 120;
            _radiusMin = 70;
            _radiusCenter = 40;
            _radiusGetColor = 20;

            CreateWheelColors();
        }

        private void CreateWheelColors()
        {
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, 255, i, 0));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, i, 255, 0));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, 0, 255, i));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, 0, i, 255));
            for (byte i = 0; i < 255; i++) wheelColors.Add(Color.FromArgb(Argb_A, i, 0, 255));
            for (byte i = 255; i > 0; i--) wheelColors.Add(Color.FromArgb(Argb_A, 255, 0, i));
        }

        private void canvasControl_RegionsInvalidated(Microsoft.Graphics.Canvas.UI.Xaml.CanvasVirtualControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasRegionsInvalidatedEventArgs args)
        {

            foreach (Rect region in args.InvalidatedRegions)
            {
                using (CanvasDrawingSession ds = sender.CreateDrawingSession(region))
                {
                    float centerX = _centerVector.X;
                    float centerY = _centerVector.Y;
                    float radiusMax = _radiusMax;
                    float radiusMin = _radiusMin;

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
                        ds.DrawGeometry(apple, color);
                    });

                    Vector2 point = new Vector2() { X = centerX, Y = centerY };
                    float redius = (radiusMax - radiusMin) / 2 + radiusMin;
                    _getColorPoint = GetPoints(point, (int)redius, wheelColors.Count);

                    ds.DrawCircle(_centerVector, _radiusCenter, wheelColors[_pointGetColor]);
                    ds.FillCircle(_centerVector, _radiusCenter, wheelColors[_pointGetColor]);
                    ds.DrawCircle(_getColorPoint[_pointGetColor], _radiusGetColor, Colors.Wheat);
                }
            }

 

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
            Rect rect = new Rect(0,0,300,400);
            canvasControl.Invalidate(rect);
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

              
                    Rect rect = new Rect(_getColorPoint[_pointGetColor].X-10, _getColorPoint[_pointGetColor].Y-10, _getColorPoint[_pointGetColor].Y+10 , _getColorPoint[_pointGetColor].Y+10);//矩形区域
        
                  canvasControl.Invalidate(rect);
                }
                else
                {
                    _pointGetColor = (int)(p.Position.Y / y);

                    Rect rect = new Rect(_getColorPoint[_pointGetColor].X - 10, _getColorPoint[_pointGetColor].Y - 10, _getColorPoint[_pointGetColor].Y + 10, _getColorPoint[_pointGetColor].Y + 10);//矩形区域
                    canvasControl.Invalidate(rect);
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
