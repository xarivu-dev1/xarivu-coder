using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xarivu.Coder.View.Control
{
    /// <summary>
    /// Interaction logic for ToolbarButton.xaml
    /// </summary>
    public partial class ToolbarButton : UserControl
    {
        public ToolbarButton()
        {
            InitializeComponent();
        }

        #region Command Dependency Property
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),            // Property name
            typeof(ICommand),           // Property type
            typeof(ToolbarButton),      // Parent control type
            new FrameworkPropertyMetadata(
                null,                   // Default value
                OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ToolbarButton toolbarButton = source as ToolbarButton;
            toolbarButton.Button.Command = toolbarButton.Command;
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion

        #region ImageSource Dependency Property
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource),        // Property name
            typeof(ImageSource),        // Property type
            typeof(ToolbarButton),      // Parent control type
            new FrameworkPropertyMetadata(
                null,                   // Default value
                OnImageSourcePropertyChanged));

        private static void OnImageSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ToolbarButton toolbarButton = source as ToolbarButton;
            toolbarButton.Image.Source = toolbarButton.ImageSource;
            toolbarButton.Image.Visibility = toolbarButton.ImageSource != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        #endregion

        #region Text Dependency Property
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),               // Property name
            typeof(string),             // Property type
            typeof(ToolbarButton),      // Parent control type
            new FrameworkPropertyMetadata(
                null,                   // Default value
                OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ToolbarButton toolbarButton = source as ToolbarButton;
            toolbarButton.TextBlock.Text = toolbarButton.Text;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion
    }
}
