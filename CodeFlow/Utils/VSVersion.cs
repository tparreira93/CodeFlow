using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.Utils
{
    // StackOverflow: Detect the Visual Studio version inside a VSPackage
    // https://stackoverflow.com/questions/11082436/detect-the-visual-studio-version-inside-a-vspackage
    public static class VSVersion
    {
        static readonly object mLock = new object();
        static Version mVsVersion;
        static Version mOsVersion;

        public static Version FullVersion
        {
            get
            {
                lock (mLock)
                {
                    if (mVsVersion == null)
                    {
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                        if (File.Exists(path))
                        {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

                            string verName = fvi.ProductVersion;

                            for (int i = 0; i < verName.Length; i++)
                            {
                                if (!char.IsDigit(verName, i) && verName[i] != '.')
                                {
                                    verName = verName.Substring(0, i);
                                    break;
                                }
                            }
                            mVsVersion = new Version(verName);
                        }
                        else
                            mVsVersion = new Version(0, 0); // Not running inside Visual Studio!
                    }
                }

                return mVsVersion;
            }
        }

        public static Version OSVersion
        {
            get { return mOsVersion ?? (mOsVersion = Environment.OSVersion.Version); }
        }

        public static bool VS2012OrLater
        {
            get { return FullVersion >= new Version(11, 0); }
        }

        public static bool VS2010OrLater
        {
            get { return FullVersion >= new Version(10, 0); }
        }

        public static bool VS2008OrOlder
        {
            get { return FullVersion < new Version(9, 0); }
        }

        public static bool VS2005
        {
            get { return FullVersion.Major == 8; }
        }

        public static bool VS2008
        {
            get { return FullVersion.Major == 9; }
        }

        public static bool VS2010
        {
            get { return FullVersion.Major == 10; }
        }

        public static bool VS2012
        {
            get { return FullVersion.Major == 11; }
        }

        public static bool VS2013
        {
            get { return FullVersion.Major == 12; }
        }

        public static bool VS2015
        {
            get { return FullVersion.Major == 14; }
        }

        public static bool VS2017
        {
            get { return FullVersion.Major == 15; }
        }

        public static bool VS2019
        {
            get { return FullVersion.Major == 16; }
        }
    }
}
