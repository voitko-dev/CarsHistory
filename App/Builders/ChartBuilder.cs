using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace CarsHistory.Builders
{
    public enum ChartType
    {
        Bar,
        Line,
        Pie,
        Scatter // Додаємо новий тип для Scatter chart
    }

    public class ChartBuilder
    {
        public static PlotModel CreateChart(ChartType chartType, List<KeyValuePair<string, int>> data)
        {
            var plotModel = new PlotModel { Title = "Chart" };

            switch (chartType)
            {
                case ChartType.Bar:
                    plotModel = CreateBarChart(data);
                    break;

                case ChartType.Line:
                    plotModel = CreateLineChart(data);
                    break;

                case ChartType.Pie:
                    plotModel = CreatePieChart(data);
                    break;

                case ChartType.Scatter: // Новий випадок для Scatter chart
                    plotModel = CreateScatterChart(data);
                    break;

                default:
                    throw new ArgumentException("Invalid chart type", nameof(chartType));
            }

            return plotModel;
        }

        // Метод для створення Scatter Chart
        private static PlotModel CreateScatterChart(List<KeyValuePair<string, int>> data)
        {
            PlotModel plotModel = new PlotModel { Title = "Price vs Mileage" };
            ScatterSeries scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, Title = "Price vs Mileage" };

            int index = 0;
            foreach (KeyValuePair<string, int> item in data)
            {
                if (!int.TryParse(item.Key, out int value))
                    value = index;
                
                scatterSeries.Points.Add(new ScatterPoint(value, item.Value));
                index++;
            }

            plotModel.Series.Add(scatterSeries);
            return plotModel;
        }

        private static PlotModel CreateBarChart(List<KeyValuePair<string, int>> data)
        {
            var plotModel = new PlotModel { Title = "Category Distribution" };

            var barSeries = new BarSeries
            {
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1,
                FillColor = OxyColors.LightSkyBlue
            };

            foreach (var item in data)
            {
                barSeries.Items.Add(new BarItem { Value = item.Value });
            }

            plotModel.Series.Add(barSeries);
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "CategoryAxis",
                ItemsSource = data.Select(t => t.Key).ToList(),
            });

            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = "ValueAxis",
                ItemsSource = data.Select(t => t.Value.ToString()).ToList()
            });

            return plotModel;
        }


        private static List<KeyValuePair<string, int>> DeleteEmptyData(List<KeyValuePair<string, int>> data)
        {
            List<int> indicesToRemove = new List<int>();
            int index = 0;

            foreach (KeyValuePair<string, int> item in data)
            {
                if (item.Value == 0)
                    indicesToRemove.Add(index);
                index++;
            }
            
            for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                int indexToRemove = indicesToRemove[i];
                data.RemoveAt(indexToRemove);
            }

            return data;
        }

        private static PlotModel CreateLineChart(List<KeyValuePair<string, int>> data)
        {
            var plotModel = new PlotModel { Title = "Data Trend" };

            var lineSeries = new LineSeries
            {
                Title = "Trend",
                Color = OxyColors.Red
            };

            data = DeleteEmptyData(data);

            foreach (KeyValuePair<string, int> item in data)
            {
                lineSeries.Points.Add(new DataPoint(data.IndexOf(item), item.Value ));
            }

            plotModel.Series.Add(lineSeries);
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "CategoryAxis",
                ItemsSource = data.Select(t => t.Key).ToList(),
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "ValueAxis",
                Minimum = 0
            });

            return plotModel;
        }

        private static PlotModel CreatePieChart(List<KeyValuePair<string, int>> data)
        {
            var plotModel = new PlotModel { Title = "Category Distribution" };

            var pieSeries = new PieSeries
            {
                AngleSpan = 360,
                StartAngle = 0
            };
            
            data = DeleteEmptyData(data);

            foreach (KeyValuePair<string, int> item in data)
            {
                pieSeries.Slices.Add(new PieSlice(item.Key, item.Value));
            }

            plotModel.Series.Add(pieSeries);
            return plotModel;
        }
    }
}
