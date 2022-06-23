using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteCall.Resources.CustomControls.Loading
{
    public class LoadingCustomControl: Control
    {
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingCustomControl),
                new PropertyMetadata(false));


        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingCustomControl),
                new PropertyMetadata(100.0));


        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingCustomControl),
                new PropertyMetadata(1.0));


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingCustomControl),
                new PropertyMetadata(Brushes.Black));


        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingCustomControl),
                new PropertyMetadata(PenLineCap.Flat));


        public static readonly DependencyProperty MainTextProperty =
            DependencyProperty.Register("MainText", typeof(string), typeof(LoadingCustomControl),
                new PropertyMetadata("Loading..."));


        static LoadingCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingCustomControl),
                new FrameworkPropertyMetadata(typeof(LoadingCustomControl)));
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

