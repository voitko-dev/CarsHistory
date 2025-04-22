using System.Windows;
using System.Windows.Media;
using CarsHistory.Services;

namespace CarsHistory.Windows
{
    public partial class UserColorsWindow : Window
    {
        public UserColorsWindow()
        {
            InitializeComponent();
            LoadUserColors();
        }
        
        private void LoadUserColors()
        {
            List<UserColor> userColors = new List<UserColor>();

            foreach (var userData in UsersService.GetInstance().ColorData)
            {
                SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(userData.Value));
                userColors.Add(new UserColor { UserName = userData.Key, ColorForUser = color });
            }

            UserListView.ItemsSource = userColors;
        }
    }
    
    public class UserColor
    {
        public string UserName { get; set; }
        public Brush ColorForUser { get; set; }
    }
}