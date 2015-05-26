using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.Utilities
{
    public static class Settings
    {
        private const string HomePageViewKey = "HomePageView";
        private const string HomePageViewDefault = null;

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string HomePageView
        {
            get { return AppSettings.GetValueOrDefault<string>(HomePageViewKey, HomePageViewDefault); }
            set { AppSettings.AddOrUpdateValue<string>(HomePageViewKey, value); }
        }
    }
}
