using Plugin.Settings;
using Plugin.Settings.Abstractions;
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
        private const string SavedSecurityTokenKey = "SavedSecurityToken";

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string SavedSecurityToken
        {
            get { return AppSettings.GetValueOrDefault<string>(SavedSecurityTokenKey, null); }
            set { AppSettings.AddOrUpdateValue<string>(SavedSecurityTokenKey, value); }
        }

        public static string HomePageView
        {
            get { return AppSettings.GetValueOrDefault<string>(HomePageViewKey, null); }
            set { AppSettings.AddOrUpdateValue<string>(HomePageViewKey, value); }
        }
    }
}
