#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VNLauncher.FunctionalClasses
{
    public class LocalColorAcquirer
    {
        private ResourceDictionary dictionary;
        public LocalColorAcquirer()
        {
            dictionary = new ResourceDictionary
            {
                Source = new Uri("/VNLauncher;component/Themes/FrontColor.xaml", UriKind.Relative)
            };
        }
        public Brush GetColor(String resourceName)
        {
            return (dictionary[resourceName] as Brush)!;
        }
        public Brush GetColor(String resourceName, Byte transparency)
        {
            Brush originalBrush = (dictionary[resourceName] as Brush)!;
            originalBrush.Opacity = transparency / 255.0;
            return originalBrush;

        }

    }
}
