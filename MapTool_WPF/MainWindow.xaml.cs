using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapTool_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        NameValueCollection MapTypes = ConfigurationManager.GetSection("MapTypesManagerSetting") as NameValueCollection;

        NameValueCollection MapTypeFilePaths = ConfigurationManager.GetSection("MapTypesFilePathSetting") as NameValueCollection;

        public MainWindow()
        {
            InitializeComponent();

            if (MapTypes != null)
            {
                MenuItems = new ObservableCollection<MenuItemViewModel>();

                foreach (var typeKey in MapTypes.AllKeys)
                {
                    string typeValue = MapTypes.GetValues(typeKey).FirstOrDefault();

                    string dirPath = MapTypeFilePaths.GetValues(typeKey).FirstOrDefault();
                    if (LoadType.边界线.ToString() == typeValue)
                        MenuItems.Add(new MenuItemViewModel()
                        {
                            Header = typeValue,
                            Type = typeValue,
                            IsActive = true,
                            DirectionaryPath = dirPath
                        });
                    else
                        MenuItems.Add(new MenuItemViewModel(typeValue, dirPath));
                }
            }

            this.DataContext = this;
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            string suriFormat = "http://r2.tiles.ditu.live.com/tiles/r{AjlkoMbsDkP0oK5kXwM0PkZfUqaAKw-0T8DXZsl4br7JPc95hPJZTrsyGBKGJ1i1}.png?g=41";
            Microsoft.Maps.MapControl.WPF.TileSource tileSource = new Microsoft.Maps.MapControl.WPF.TileSource(suriFormat);
            Microsoft.Maps.MapControl.WPF.MapTileLayer tileLayer = new Microsoft.Maps.MapControl.WPF.MapTileLayer(); //初始化一个图层
            tileLayer.TileSource = tileSource;
            tileLayer.Opacity = 0.9;
            this.myMap.Children.Add(tileLayer);
            this.myMap.Mode = new Microsoft.Maps.MapControl.WPF.MercatorMode();
        }

        public void LoadPolyline(List<Location> locations)
        {
            MapPolyline polyline = new MapPolyline();
            polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polyline.StrokeThickness = 1;
            polyline.Opacity = 0.7;
            polyline.Locations = new LocationCollection();
            locations.ForEach(item => { polyline.Locations.Add(item); });

            myMap.Children.Add(polyline);
            myMap.SetView(locations, myMap.Margin, myMap.Heading);
        }
        public void LoadPolygon(List<Location> locations)
        {
            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            polygon.StrokeThickness = 1;
            polygon.Opacity = 0.7;
            polygon.Locations = new LocationCollection();
            locations.ForEach(item => { polygon.Locations.Add(item); });
            myMap.SetView(locations, myMap.Margin, myMap.Heading);
            myMap.Children.Add(polygon);
        }

        private void MenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            myMap.Children.Clear();
            var context = (sender as Button).DataContext as MenuItemViewModel;

            switch (context.Type)
            {
                case "边界":
                    LoadPolyline(context.MapLocations);
                    break;
                case "湖泊":
                    LoadPolyline(context.MapLocations);
                    break;
                case "岛屿":
                    LoadPolygon(context.MapLocations);
                    break;
                case "边界线":
                    var boundaries = Configurations.GetMenuItems(context.DirectionaryPath, null).Select(x => x.MapLocations).ToList();
                    boundaries.ForEach(item =>
                    {
                        LoadPolyline(item);
                    });
                    break;
            }

        }
    }
}
