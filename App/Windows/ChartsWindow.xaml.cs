using System.Windows;
using System.Windows.Controls;
using CarsHistory.Builders;
using CarsHistory.Items;
using OxyPlot;
using OxyPlot.Wpf;

namespace CarsHistory.Windows
{
    public partial class ChartsWindow : Window
    {
        private readonly List<Car> _filteredCars;

        public ChartsWindow(List<Car> filteredCars)
        {
            InitializeComponent();
            _filteredCars = filteredCars;
        }

        private void DisplayChart(List<KeyValuePair<string, int>> data)
        {
            // Завантажуємо BarChart на вкладку "Bar Chart"
            PlotModel barChartModel = ChartBuilder.CreateChart(ChartType.Bar, data);
            PlotView barPlotView = new PlotView
            {
                Model = barChartModel
            };
            BarChartGrid.Children.Add(barPlotView);

            // Завантажуємо LineChart на вкладку "Line Chart" (якщо потрібно)
            PlotModel lineChartModel = ChartBuilder.CreateChart(ChartType.Line, data);
            PlotView linePlotView = new PlotView
            {
                Model = lineChartModel
            };
            LineChartGrid.Children.Add(linePlotView);

            // Завантажуємо PieChart на вкладку "Pie Chart" (якщо потрібно)
            PlotModel pieChartModel = ChartBuilder.CreateChart(ChartType.Pie, data);
            PlotView piePlotView = new PlotView
            {
                Model = pieChartModel
            };
            PieChartGrid.Children.Add(piePlotView);

            // Завантажуємо ScatterChart для ціни відносно пробігу
            PlotModel scatterChartModel = ChartBuilder.CreateChart(ChartType.Scatter, data);
            PlotView scatterPlotView = new PlotView
            {
                Model = scatterChartModel
            };
            ScatterChartGrid.Children.Add(scatterPlotView); // Додано новий графік
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem? selectedItem = dataTypeComboBox.SelectedItem as ComboBoxItem;
            
            if (selectedItem == null) 
                return;
            
            string selectedDataType = selectedItem.Content.ToString();

            List<KeyValuePair<string, int>> data = new List<KeyValuePair<string, int>>();

            if (selectedDataType == "Price")
                GetCategoriesAndValues(_filteredCars, out data);
            else if (selectedDataType == "PriceVsMileage")
                DisplayPriceVsMileageChart(_filteredCars, out data);

            DisplayChart(data);
        }

        private void DisplayPriceVsMileageChart(List<Car> filteredCars, out List<KeyValuePair<string, int>> data)
        {
            data = filteredCars.Select(car => new KeyValuePair<string, int>($"{car.Mileage}", (int)car.Price)).ToList();
        }

        private void GetCategoriesAndValues(List<Car> filteredCars, out List<KeyValuePair<string, int>> data)
        {
            List<string> categories = new List<string>
            {
                "0 - 5000",
                "5001 - 10000",
                "10001 - 15000",
                "15001 - 20000",
                "20001 - 25000",
                "25001 - 30000",
                "30001 - 35000",
                "35001+",
            };

            List<int> values = new List<int>(new int[categories.Count]);

            foreach (Car car in filteredCars)
            {
                if (car.Price <= 5000) values[0]++;
                else if (car.Price <= 10000) values[1]++;
                else if (car.Price <= 15000) values[2]++;
                else if (car.Price <= 20000) values[3]++;
                else if (car.Price <= 25000) values[4]++;
                else if (car.Price <= 30000) values[5]++;
                else if (car.Price <= 35000) values[6]++;
                else values[7]++;
            }
            
            data = categories.Select((t, i) => new KeyValuePair<string, int>(t, values[i])).ToList();
        }
    }
}
