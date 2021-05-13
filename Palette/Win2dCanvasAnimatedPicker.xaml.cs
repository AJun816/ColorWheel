using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Palette
{
    public sealed partial class Win2dCanvasAnimatedPicker : UserControl
    {
        readonly Vector2 _centerVector = new Vector2() { X = 200, Y = 200 };
        readonly float _radiusMax = 120;
        readonly float _radiusMin = 70;
        readonly float _radiusCenter = 40;
        readonly float _radiusGetColor = 20;

        byte Argb_A = 255;
        bool _isGetColor;
        Vector2 _getColorPointer = new Vector2(294, 198);
        Color centercolors = new Color() { A = 255, R = 255, G = 0, B = 0 };

        List<Color> _wheelColors = new List<Color>();

        public Win2dCanvasAnimatedPicker()
        {
            this.InitializeComponent();
            CreateWheelColors();
        }

        /// <summary>
        /// 创建颜色
        /// </summary>
        private void CreateWheelColors()
        {
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, 255, i, 0));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, i, 255, 0));
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, 0, 255, i));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, 0, i, 255));
            for (byte i = 0; i < 255; i++) _wheelColors.Add(Color.FromArgb(Argb_A, i, 0, 255));
            for (byte i = 255; i > 0; i--) _wheelColors.Add(Color.FromArgb(Argb_A, 255, 0, i));
        }


        private void canvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            float centerX = _centerVector.X;
            float centerY = _centerVector.Y;
            float radiusMax = _radiusMax;
            float radiusMin = _radiusMin;

            // 创建路径变量                                              
            int colorCount = _wheelColors.Count;     // 颜色数量
            Double angel = 360.0 / colorCount;      // 计算夹角(注：计算参数必须为浮点数，否则结果为0)
            Double rotate = 0;                      // 起始角度
            float pointX, pointY;                   // 缓存绘图路径点       

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
                if (d < 0) d = d + 360;
                if (d == r) centercolors = color;

                using (CanvasPathBuilder path = new CanvasPathBuilder(sender))
                {
                    path.BeginFigure(point1);
                    path.AddLine(point3);
                    path.EndFigure(CanvasFigureLoop.Open);

                    using (CanvasGeometry apple = CanvasGeometry.CreatePath(path))
                    {
                        args.DrawingSession.DrawGeometry(apple, color);
                    }
                }
            });

            centercolors.A = Argb_A;
            args.DrawingSession.FillCircle(_centerVector, _radiusCenter, centercolors);
            args.DrawingSession.DrawCircle(_getColorPointer, _radiusGetColor, Colors.Wheat);            
        }


        private async void canvasAnimatedControl_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                sliderColor.Color = Color.FromArgb(centercolors.A, centercolors.R, centercolors.G, centercolors.B);         
            });
        }

       
        private void slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
     
            Argb_A = (Byte)slider.Value;
            _isGetColor = false;
        }

        private void canvasAnimatedControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = true;

            PointerPoint pressedPointer = e.GetCurrentPoint(canvasAnimatedControl);
            if (_isGetColor)
            {
                PointerPoint pointer = e.GetCurrentPoint(canvasAnimatedControl);
                Vector2 vector = new Vector2();
                vector.X = (float)pointer.Position.X;
                vector.Y = (float)pointer.Position.Y;
                Vector2 vector2 = Vector2.Normalize(vector - _centerVector);

                _getColorPointer = _centerVector + vector2 * 95;
            }
        }

        private void canvasAnimatedControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (_isGetColor)
            {
                PointerPoint pointer = e.GetCurrentPoint(canvasAnimatedControl);
                Vector2 vector = new Vector2();
                vector.X = (float)pointer.Position.X;
                vector.Y = (float)pointer.Position.Y;
                Vector2 vector2 = Vector2.Normalize(vector - _centerVector);
                _getColorPointer = _centerVector + vector2 * 95;
            }
        }

        private void canvasAnimatedControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = false;

        }

        private void canvasAnimatedControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _isGetColor = false;
        }


        private void canvasAnimatedControll_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvasAnimatedControl.RemoveFromVisualTree();
            this.canvasAnimatedControl = null;
        }
    }
}
