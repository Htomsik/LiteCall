using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LoadingCustom
{
    public class LoadingCustom : Control
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingCustom),
                new PropertyMetadata(false));


        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingCustom),
                new PropertyMetadata(100.0));


        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingCustom),
                new PropertyMetadata(1.0));


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingCustom),
                new PropertyMetadata(Brushes.Black));


        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingCustom),
                new PropertyMetadata(PenLineCap.Flat));


        public static readonly DependencyProperty MainTextProperty =
            DependencyProperty.Register("MainText", typeof(string), typeof(LoadingCustom),
                new PropertyMetadata("Loading..."));


        static LoadingCustom()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingCustom),
                new FrameworkPropertyMetadata(typeof(LoadingCustom)));
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