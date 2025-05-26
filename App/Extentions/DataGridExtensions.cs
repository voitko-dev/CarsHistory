using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

public static class DataGridExtensions
{
    public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
    {
        if (row != null)
        {
            DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
            if (presenter != null)
            {
                return presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
            }
        }
        return null;
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is T)
                return (T)child;
            
            T childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }
}