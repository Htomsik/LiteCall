using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModalCutom
{
    public class ModalCustom : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ModalCustom),
                new PropertyMetadata(false));


        public static readonly DependencyProperty CornerProperty =
            DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(ModalCustom),
                new PropertyMetadata(new CornerRadius(10)));

        static ModalCustom()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalCustom),
                new FrameworkPropertyMetadata(typeof(ModalCustom)));

            BackgroundProperty.OverrideMetadata(typeof(ModalCustom),
                new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public CornerRadius Corner
        {
            get => (CornerRadius)GetValue(CornerProperty);
            set => SetValue(CornerProperty, value);
        }

        private static object CreateDefaultBackground()
        {
            return new SolidColorBrush(Colors.Black)
            {
                Opacity = 0.3
            };
        }
    }
}