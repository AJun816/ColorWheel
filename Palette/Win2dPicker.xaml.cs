using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Windows.Foundation;
using Windows.System.Threading;
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
        Point _pressedPointer = new Point();

        List<Color> wheelColors = new List<Color>();

        System.Timers.Timer timer = new System.Timers.Timer(50);   //实例化Timer类，设置间隔时间为10000毫秒；   
   

    public Win2dPicker()
        {
            this.InitializeComponent();
            CreateWheelColors();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(canvasInvalidate); //到达时间的时候执行事件；   
            timer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            timer.Enabled = true;
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

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
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
                color.A = Argb_A;
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

            Color centercolor =  wheelColors[_pointGetColor];
            centercolor.A = Argb_A;
            args.DrawingSession.DrawCircle(_centerVector, _radiusCenter, centercolor);
            args.DrawingSession.FillCircle(_centerVector, _radiusCenter, centercolor);
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



        private void canvasInvalidate(object source, System.Timers.ElapsedEventArgs e)
        {
            canvasControl.Invalidate();
        }

        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Argb_A = (Byte)slider.Value;
            _isGetColor = false;
            canvasControl.Invalidate();
        }

        private void canvasControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            PointerPoint pressedPointer = e.GetCurrentPoint(canvasControl);
            _pressedPointer = new Point(pressedPointer.Position.X,pressedPointer.Position.Y);
            timer.Start();

            _isGetColor = true;
        }

        private void canvasControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (_isGetColor)
            {
                PointerPoint pointer = e.GetCurrentPoint(canvasControl);

                double x = canvasControl.Width / wheelColors.Count;
                double y = canvasControl.Height / wheelColors.Count;
                int i = wheelColors.Count;

                if (_pointGetColor>i)
                {
                    _pointGetColor = 0;
                }
                else if (_pointGetColor <0)
                {
                    _pointGetColor = i;
                }
          

                if (_pressedPointer.X > pointer.Position.X && _pressedPointer.X - pointer.Position.X>x)
                {
                    //向左
                    //  double ratioLeft =(_pressedPointer.X - pointer.Position.X) / 400/ x;
                    if (_pointGetColor > i)
                    {
                        _pointGetColor = 0;
                    }
                    else if (_pointGetColor < 0)
                    {
                        _pointGetColor = i;
                    }

                    _pointGetColor --;
                   
                    //_pointGetColor = (int)ratioLeft;
                    //canvasControl.Invalidate();
                }
                else if (_pressedPointer.X < pointer.Position.X && pointer.Position.X - _pressedPointer.X>x)
                {
                    //向右
                    //double ratioRight = (pointer.Position.X - _pressedPointer.X) / 400 / x;

                    if (_pointGetColor > i)
                    {
                        _pointGetColor = 0;
                    }
                    else if (_pointGetColor < 0)
                    {
                        _pointGetColor = i;
                    }

                    _pointGetColor++;
                    //_pointGetColor = (int)ratioRight;
                    //canvasControl.Invalidate();
                }
                else if (_pressedPointer.Y < pointer.Position.Y && pointer.Position.Y - _pressedPointer.Y>y)
                {
                    //向上
                    // double ratioTop = (pointer.Position.Y - _pressedPointer.Y) / 400 / y;

                    if (_pointGetColor > i)
                    {
                        _pointGetColor = 0;
                    }
                    else if (_pointGetColor < 0)
                    {
                        _pointGetColor = i;
                    }


                    _pointGetColor--;
                    //_pointGetColor = (int)ratioTop;
                    //canvasControl.Invalidate();
                }
                else 
                {
                    //向下
                    // double ratioBottom = (_pressedPointer.Y- pointer.Position.Y) / 400 / y;

                    if (_pointGetColor > i)
                    {
                        _pointGetColor = 0;
                    }
                    else if (_pointGetColor < 0)
                    {
                        _pointGetColor = i;
                    }

                    _pointGetColor++;
                    //_pointGetColor = (int)ratioBottom;
                   //canvasControl.Invalidate();
                }
            }
        }

        private void canvasControl_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            timer.Stop();
            _isGetColor = false;
        }

        private void canvasControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            timer.Stop();
            _isGetColor = false;
        }
    }
}
