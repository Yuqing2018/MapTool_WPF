using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapTool_WPF
{
    public class Configurations
    {
        public static IEnumerable<MenuItemViewModel> GetMenuItems(string dirPath,string type)
        {
            List<MenuItemViewModel> result = new List<MenuItemViewModel>();

            List<string> files = Directory.GetFiles(dirPath, "*.tex").ToList();

            files.ForEach(item =>
            {
                result.Add(new MenuItemViewModel()
                {
                    Header = Path.GetFileNameWithoutExtension(item),
                    Type = type,
                    MapLocations = GetLocationCollection(item),
                    IsActive = true
                });
            });

            return result;
        }
        /// <summary>
        /// 根据文件路径读取坐标点集合
        /// </summary>
        /// <param name="filePath">tex文件路径</param>
        /// <returns></returns>
        public static List<Location> GetLocationCollection(string filePath)
        {
            var result = new List<Location>();

            using (FileStream file = File.OpenRead(filePath))
            {
                string pattern = @"\[MapLocation|\]\,";
                string pattern1 = @"[^\d.\d]"; // 正则表达式剔除非数字字符（不包含小数点.）

                using (StreamReader read = new StreamReader(file))
                {
                    var s = read.ReadToEnd();

                    var contents = Regex.Split(s, pattern, RegexOptions.IgnorePatternWhitespace);

                    contents.Where(x => !String.IsNullOrEmpty(x)).ToList().ForEach(item =>
                    {
                        var geos = Regex.Split(item, pattern1).Where(m => !String.IsNullOrEmpty(m)).Select(m => double.Parse(m)).ToArray();

                        if (2 == geos.Length)
                            result.Add(new Location() { Latitude = geos[0], Longitude = geos[1] });
                    });
                }
            }

            return result;
        }
    }

    public class MenuItemViewModel
    {
        public MenuItemViewModel() { }
        public MenuItemViewModel(string header, string dirPath)
        {
            Header = header;
            Type = header;
            IsActive = false;
            DirectionaryPath = dirPath;
            MenuItems = Configurations.GetMenuItems(dirPath, Type).ToList();
        }

        public string Header { get; set; }
        public bool IsActive { get; set; }

        public string Type { get; set; }
        public string DirectionaryPath { get; set; }

        public List<Location> MapLocations { get; set; }

        public List<MenuItemViewModel> MenuItems { get; set; }
    }

    public class LoadLocationsCommand : ICommand
    {
        private readonly Action _action;
        public LoadLocationsCommand(Action action)
        {
            _action = action;
        }
        public void Execute(object o)
        {
            _action();
        }

        public bool CanExecute(object o)
        {
            return true;
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }

    public enum LoadType
    {
        边界 = 1,
        湖泊 = 2,
        岛屿 = 3,
        边界线 = 4
    }
}
