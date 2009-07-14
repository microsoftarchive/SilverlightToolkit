using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace System.Windows.Controls.UnitTests
{
    public partial class NavigationCacheModeRequiredPage : Page
    {
        public NavigationCacheModeRequiredPage()
        {
            InitializeComponent();
        }

        public string GetTextBoxText()
        {
            return _textBox.Text;
        }

        public void SetTextBoxText(string newText)
        {
            _textBox.Text = newText;
        }

    }
}
