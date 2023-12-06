using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Checkers.Commands
{
    class MouseBehavior
    {
        public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached(
            "MouseDownCommand",
            typeof (ICommand),
            typeof (MouseBehavior),
            new FrameworkPropertyMetadata(
                new PropertyChangedCallback(MouseDownCommandChanged))
            );

        public static readonly DependencyProperty MouseDownCommandParameterProperty = DependencyProperty.RegisterAttached(
            "MouseDownCommandParameter",
            typeof(object),
            typeof(MouseBehavior),
            new FrameworkPropertyMetadata());

        private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            element.MouseLeftButtonDown += new MouseButtonEventHandler(element_MouseDown);
        }

        static void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetMouseDownCommand(element);
            object parameter = GetMouseDownCommandParameter(element);
            command.Execute(parameter);
        }

        public static void SetMouseDownCommand(UIElement element, ICommand value, object parameter)
        {
            element.SetValue(MouseDownCommandProperty, value);
            element.SetValue(MouseDownCommandParameterProperty,parameter);
        }

        public static ICommand GetMouseDownCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseDownCommandProperty);
        }

        public static object GetMouseDownCommandParameter(UIElement element)
        {
            return element.GetValue(MouseDownCommandParameterProperty);
        }
    }
}
