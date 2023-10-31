using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloInfestation.Service
{
    public static class RegistryHelper
    {
        public static string RegRead(string keyName, string valueName)
        {
            string retVal;
            RegistryKey key = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            key = key.OpenSubKey(keyName, true);
            retVal = (string)key.GetValue(valueName);

            return retVal;

        }
    }
}
