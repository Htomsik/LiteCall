using System.Windows;
using System.Windows.Input;

namespace LiteCall.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DragPanel(object sender, MouseButtonEventArgs e)
    {
        try
        {
            Keyboard.ClearFocus();
            DragMove();
        }
        catch
        {
        }
    }

    private void MaxButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void MinButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }


    private new void LostFocus(object sender, RoutedEventArgs e)
    {
        Keyboard.ClearFocus();
    }

    private new void SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Keyboard.ClearFocus();
    }
}