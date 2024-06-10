#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VNLauncher.FuntionalClasses
{
    internal class LocalColorAcquirer
    {
        private ResourceDictionary dictionary;
        public LocalColorAcquirer()
        {
            dictionary = new ResourceDictionary
            {
                Source = new Uri("/VNLauncher;component/Themes/FrontColor.xaml", UriKind.Relative)
            };
        }
        public Object GetColor(String resourceName)
        {
            return dictionary[resourceName];
        }
    }
}
