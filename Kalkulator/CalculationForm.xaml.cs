using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kalkulator
{
    /// <summary>
    /// Interaction logic for CalculationForm.xaml
    /// </summary>
    public partial class CalculationForm : UserControl
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private ObservableCollection<BendingForm> bendingForms = new ObservableCollection<BendingForm>();
        private uint bendingOperationCount = 0;
        private int FormIndex;
        public MainWindow parent { get; set; }

        public CalculationForm(MainWindow mw)
        {
            InitializeComponent();
            parent = mw;
            FormIndex = mw.IloscFormularzy;
            InitMaterial();
            InitThickness();
            UpdateGroupBoxName();
        }

        private void InitMaterial()
        {
            foreach (Material m in FileInit.Material)
            {
                MaterialComboBox.Items.Add(m.MaterialName);
            }

            if (MaterialComboBox.Items.Count > 0)
                MaterialComboBox.SelectedIndex = 0;
        }

        private void InitThickness()
        {
            foreach (Cutting f in FileInit.Cuttings)
            {
                ThicknessComboBox.Items.Add(f.Thickness + "mm");
            }

            if (ThicknessComboBox.Items.Count > 0)
            {
                ThicknessComboBox.SelectedIndex = 0;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (parent.IloscFormularzy > 1)
            {
                parent.IloscFormularzy--;
                parent.StackPanel.Height -= Height;
                parent.CountFormsLabel.Content = "Ilość formularzy: " + parent.IloscFormularzy;
                ((Panel)this.Parent).Children.Remove(this);
                parent.calculationForms.RemoveAt(FormIndex - 1);
                parent.ReindexForms();
            }
            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^\d+";
            //Dodac zabezpieczenie przed wprowadzeniem wiecej niz jednego przecinka
            if (Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = false;
            }
            else if (Regex.IsMatch(e.Text, @"[,]{1}"))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public void UpdateGroupBoxName(int index = -1)
        {
            if (index != -1)
            {
                FormIndex = index;
            }

            string header = "bl. ";
            header += MaterialComboBox.Text + " ";
            header += ThicknessComboBox.Text + " ";
            header += "------------ Forumlarz nr " + FormIndex;

            GroupBox.Header = header;
        }

        private void UpdateValues()
        {
            int index = MaterialComboBox.SelectedIndex;
            string type = FileInit.Material[index].MaterialType;
            float thickness = FileInit.Cuttings[ThicknessComboBox.SelectedIndex].Thickness;
            float cost = FileInit.GetCostByType(type, thickness);

            CuttingCostTextBox.Text = cost.ToString();
            MaterialCostTextBox.Text = FileInit.Material[index].MaterialPrice.ToString();
            DensityTextBox.Text = FileInit.Material[index].Density.ToString();
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            UpdateGroupBoxName();
            UpdateValues();
        }

        public float CountNodes()
        {
            float totalCost = 0;

            if (bendingForms.Count != 0)
            {
                totalCost += CountBending();
            }
            else

                totalCost += CountCutting();
            totalCost += CountMaterial();

            TotalCostLabel.Content = "Łaczny koszt: " + totalCost;

            return totalCost;
        }

        public int CountBendings()
        {
            return bendingForms.Count;
        }

        private float CountBending()
        {
            float totalBending = 0;
            int count = 0;
            foreach (BendingForm bf in bendingForms)
            {
                float bendingCost = 0;
                bendingCost = float.Parse(bf.BendingCountTextBox.Text);
                bendingCost *= float.Parse(bf.PartCountTextBox.Text);
                bendingCost *= float.Parse(bf.BendingCountTextBox.Text);
                totalBending += bendingCost;
                totalBending += 50;
                count++;
            }

            TechnologyCostLabel.Content = "Koszt technologii: " + 50 * count;
            BendingCostLabel.Content = "Koszt gięcia: " + (totalBending - (50 * count));
            return totalBending;
        }

        private float CountCutting()
        {
            float totalCutting = 0;

            { //Normalne cięcie
                float cost = 0;
                cost = float.Parse(CuttingCostTextBox.Text) * 0.9f;
                cost *= (float)Convert.ToDouble(PathTextBox.Text);
                totalCutting += cost;
            }

            { //Cięcie macro
                float cost = 0;
                cost = float.Parse(CuttingCostTextBox.Text) * 0.9f * 2f;
                cost *= float.Parse(MacroPathTextBox.Text);
                totalCutting += cost;
            }

            { //Grawer
                float cost = 0;
                cost = float.Parse(CuttingCostTextBox.Text) * 0.9f * 2f;
                cost *= float.Parse(GraverTextBox.Text);
                totalCutting += cost;
            }

            CuttingCostLabel.Content = "Koszt cięcia: " + totalCutting;

            return totalCutting;
        }

        private float CountMaterial()
        {
            float totalMaterial = 0;

            float area = (float)Convert.ToDouble(AreaTextBox.Text);
            float density = (float)Convert.ToDouble(DensityTextBox.Text);

            string buffer = ThicknessComboBox.Text.Trim('m');

            float thickness = (float)Convert.ToDouble(buffer);
            float material = (float)Convert.ToDouble(MaterialCostTextBox.Text);

            totalMaterial = area * density * thickness * material;

            MaterialCostLabel.Content = "Koszt materiału: " + totalMaterial;

            return totalMaterial;
        }

        private void AddBendingOperationButton_Click(object sender, RoutedEventArgs e)
        {
            BendingForm bf = new BendingForm();
            bendingOperationCount++;
            stackPanel.Children.Add(bf);

            stackPanel.Height += bf.Height;

            GroupBox.Height += bf.Height;
            MainUC.Height += bf.Height;
            MainGrid.Height += bf.Height;
            parent.StackPanel.Height += bf.Height;

            Thickness thickness = TechnologyCanvas.Margin;
            thickness.Top += bf.Height;
            TechnologyCanvas.Margin = thickness;

            BendingOperationCountLabel.Content = "Ilość operacji gięcia: " + bendingOperationCount;

            bendingForms.Add(bf);
        }
    }
}
