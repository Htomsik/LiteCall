using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFCustomControls
{
    public class LoadingBox : Control
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingBox),
                new PropertyMetadata(false));


        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingBox),
                new PropertyMetadata(100.0));


        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingBox),
                new PropertyMetadata(1.0));


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingBox),
                new PropertyMetadata(Brushes.Black));


        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingBox),
                new PropertyMetadata(PenLineCap.Flat));


        public static readonly DependencyProperty MainTextProperty =
            DependencyProperty.Register("MainText", typeof(string), typeof(LoadingBox),
                new PropertyMetadata("Loading..."));


        static LoadingBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingBox),
                new FrameworkPropertyMetadata(typeof(LoadingBox)));
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public double Diameter
        {
            get => (double)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public PenLineCap Cap
        {
            get => (PenLineCap)GetValue(CapProperty);
            set => SetValue(CapProperty, value);
        }

        public string MainText
        {
            get => (string)GetValue(MainTextProperty);
            set => SetValue(MainTextProperty, value);
        }
    }
}
