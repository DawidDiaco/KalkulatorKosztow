using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Kalkulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int IloscFormularzy = 1;
        float Kwota = 0;
        public ObservableCollection<CalculationForm> calculationForms = new ObservableCollection<CalculationForm>();

        public MainWindow()
        {
            InitializeComponent();
            FileInit.Init("res/CuttingPrice.xml", "res/MaterialPrice.xml");
            CalculationForm cf = new CalculationForm(this);
            StackPanel.Children.Add(cf);
            calculationForms.Add(cf);
            VersionCheck vc = new VersionCheck();
            vc.CheckVersion();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            IloscFormularzy++;
            CalculationForm cf = new CalculationForm(this);
            StackPanel.Children.Add(cf);
            calculationForms.Add(cf);

            StackPanel.Height += cf.Height;
            CountFormsLabel.Content = "Ilość formularzy: " + IloscFormularzy;
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            Kwota = 0;
            SumUp();
        }

        public void ReindexForms()
        {
            for (int i = 0; i < calculationForms.Count; i++)
            {
                calculationForms[i].UpdateGroupBoxName(i + 1);
            }
        }

        private void SumUp()
        {
            int iloscOperacjiGiecia = 0;
            foreach (CalculationForm cf in calculationForms)
            {
                Kwota += cf.CountNodes();
                iloscOperacjiGiecia += cf.CountBendings();
            }

            if (iloscOperacjiGiecia == 0)
            {
                Kwota += 25;
            }

            CostLabel.Content = "Kwota: " + Kwota;
        }
    }
}
