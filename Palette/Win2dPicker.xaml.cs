using Microsoft.Graphics.Canvas.Geometry;
using RGBHSL;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
        bool _isGetColor;
        Vector2 _getColorPointer = new Vector2(294,198);
        Vector2 _getColorPointer1 = new Vector2(270, 200);

        Color centercolors = new Color() { A =255,R = 255,G = 0,B =0};

        List<Color> _wheelColors = new List<Color>();
        List<HSL> _hSLs = new List<HSL>();

        ColorToHSLConverter _toHSL = new ColorToHSLConverter();
        HSLToColorConverter _toColor = new HSLToColorConverter();

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
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, 255, i, 0));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, i, 255, 0));
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, 0, 255, i));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, 0, i, 255));
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, i, 0, 255));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, 255, 0, i));

            foreach (var color in _wheelColors)
            {
                System.Drawing.Color c = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
                HSL hsl = _toHSL.Convert(c);
                _hSLs.Add(hsl);
            }
        }

        private void ChangeWheelColors(decimal hsl_L)
        {
            _wheelColors.Clear();
            foreach (var colorHSL in _hSLs)
            {
                colorHSL.L = hsl_L / 100M;

                System.Drawing.Color color = _toColor.Convert(colorHSL);
                _wheelColors.Add(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }
        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            float centerX = _centerVector.X;
            float centerY = _centerVector.Y;
            float radiusMax = _radiusMax;
            float radiusMin = _radiusMin;

            // 创建路径变量                                              
            int colorCount = _wheelColors.Count;     // 颜色数量
            Double angel = 360.0 / colorCount;      // 计算夹角(注：计算参数必须为浮点数，否则结果为0)
            Double rotate = 0;                      // 起始角度
            float pointX, pointY;                  // 缓存绘图路径点

            _wheelColors.ForEach((color) =>
            {
                color.A = Argb_A;
                pointX = centerX + radiusMin * (float)Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMin * (float)Math.Sin(rotate * Math.PI / 180);
                Vector2 point1 = new Vector2(pointX, pointY);
                rotate += angel;
                pointX = centerX + radiusMax * (float)Math.Cos(rotate * Math.PI / 180);
                pointY = centerY + radiusMax * (float)Math.Sin(rotate * Math.PI / 180);
                Vector2 point3 = new Vector2(pointX, pointY);
                               
                double d = Math.Atan2((_getColorPointer.Y - _centerVector.Y), (_getColorPointer.X - _centerVector.X)) * 180 / Math.PI;
                d = Math.Round(d);
                double r = Math.Round(rotate);
                if (d<0)d = d + 360;
                if (d == r)centercolors = color;

                CanvasPathBuilder path = new CanvasPathBuilder(sender);
                path.BeginFigure(point1);
                path.AddLine(point3);
                path.EndFigure(CanvasFigureLoop.Open);               
                CanvasGeometry apple = CanvasGeometry.CreatePath(path);  
                
                args.DrawingSession.DrawGeometry(apple, color);
            });
         
            centercolors.A = Argb_A;
            args.DrawingSession.FillCircle(_centerVector, _radiusCenter, centercolors);
            args.DrawingSession.DrawCircle(_getColorPointer, _radiusGetColor, Colors.Wheat);

            sliderColor.Color= Color.FromArgb(centercolors.A, centercolors.R, centercolors.G, centercolors.B);
        }


        private void canvasInvalidate(object source, System.Timers.ElapsedEventArgs e)
        {
            canvasControl.Invalidate();
        }


        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {            
            _isGetColor = false;
            ChangeWheelColors((decimal)slider.Value);
            canvasControl.Invalidate();
        }

        private void canvasControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = true;

            PointerPoint pressedPointer = e.GetCurrentPoint(canvasControl);            
            if (_isGetColor)
            {
                PointerPoint pointer = e.GetCurrentPoint(canvasControl);
                Vector2 vector = new Vector2();
                vector.X = (float)pointer.Position.X;
                vector.Y = (float)pointer.Position.Y;
                Vector2 vector2 = Vector2.Normalize(vector - _centerVector);
                
                _getColorPointer = _centerVector + vector2 * 95;
                _getColorPointer1 = _centerVector + vector2 * 70;
            }
            timer.Start();  
        }

        private void canvasControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (_isGetColor)
            {
                PointerPoint pointer = e.GetCurrentPoint(canvasControl);
                Vector2 vector = new Vector2();
                vector.X = (float)pointer.Position.X;
                vector.Y = (float)pointer.Position.Y;
                Vector2 vector2 = Vector2.Normalize(vector- _centerVector);
                _getColorPointer = _centerVector + vector2 * 95;
            }
        }

        private void canvasControl_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = false;
            timer.Stop();            
        }

        private void canvasControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = false;
            timer.Stop();           
        }

        private void canvasControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.canvasControl.RemoveFromVisualTree();
            this.canvasControl = null;
        }
    }
}
