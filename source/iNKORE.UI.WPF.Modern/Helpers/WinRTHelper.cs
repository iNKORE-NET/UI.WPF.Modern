using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNKORE.UI.WPF.Modern.Helpers
{
    public static class WinRTHelper
    {
        public static bool IsSupported
        {
            get
            {
                try { return Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"); }
                catch { }

                return false;
            }
        }

        public static string GetCurrentDeviceFamily()
        {
            try
            {
                if (IsSupported && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.AnalyticsInfo"))
                {
                    return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                }
            }
            catch { }

            return null;
        }

        public static DeviceFamilyType? GetCurrentDeviceFamilyType()
        {
            try
            {
                var familyName = GetCurrentDeviceFamily();

                if (!string.IsNullOrWhiteSpace(familyName))
                {
                    if (!Enum.TryParse(familyName.Replace("Windows.", string.Empty), out DeviceFamilyType parsedDeviceType))
                    {
                        parsedDeviceType = DeviceFamilyType.Other;
                    }

                    return parsedDeviceType;
                }
            }
            catch { }

            return null;
        }
    }

    public enum DeviceFamilyType
    {
        Desktop,
        Mobile,
        Other,
        Xbox
    }

}
