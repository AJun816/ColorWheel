using Windows.UI;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Palette
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.FillCircle(200, 150, 100, Colors.Green);
            args.DrawingSession.DrawCircle(200,150,80,Colors.Blue);

            args.DrawingSession.FillCircle(200, 150, 70, Colors.Black);

            args.DrawingSession.FillCircle(200, 150, 40, Colors.Orange);

            args.DrawingSession.DrawCircle(200, 150, 15, Colors.White);


            //using (var canvasPathBuilder = new CanvasPathBuilder(args.DrawingSession))
            //{
            //    // 这里可以画出 Path 或写出文字 lindexi.github.io
            //    canvasPathBuilder.BeginFigure(1, 1);
            //    canvasPathBuilder.AddLine(300, 300);
            //    canvasPathBuilder.AddLine(1, 300);
            //    canvasPathBuilder.EndFigure(CanvasFigureLoop.Closed);
            //    args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(canvasPathBuilder), Colors.Gray, 2);
            //}

        }
    }
}
