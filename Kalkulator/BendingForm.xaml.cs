using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kalkulator
{
    /// <summary>
    /// Interaction logic for BendingForm.xaml
    /// </summary>
    public partial class BendingForm : UserControl
    {
        public BendingForm()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
