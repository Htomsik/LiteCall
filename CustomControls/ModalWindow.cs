using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomControls;

public class ModalWindow : ContentControl
{
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register("IsOpen", typeof(bool), typeof(ModalWindow),
            new PropertyMetadata(false));


    public static readonly DependencyProperty CornerProperty =
        DependencyProperty.Register("Corner", typeof(CornerRadius), typeof(ModalWindow),
            new PropertyMetadata(new CornerRadius(10)));

    static ModalWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalWindow),
            new FrameworkPropertyMetadata(typeof(ModalWindow)));

        BackgroundProperty.OverrideMetadata(typeof(ModalWindow),
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