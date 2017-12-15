using System;

namespace CodeFlow.Versions
{
    public class Version : IComparable
    {
        private int _build = 0;
        private int _minor = 0;
        private int _major = 0;

        public Version(string version)
        {
            string[] ver = version.Split('.');
            if(ver.Length >= 3)
            {
                Build = int.Parse(ver[2]);
                Minor = int.Parse(ver[1]);
                Major = int.Parse(ver[0]);
            }
        }

        public Version(int major, int minor, int build)
        {
            Build = build;
            Minor = minor;
            Major = major;
        }

        public int Build { get => _build; set => _build = value; }
        public int Minor { get => _minor; set => _minor = value; }
        public int Major { get => _major; set => _major = value; }

        public bool IsBefore(Version v)
        {
            if (Major < v.Major)
                return true;
            if (Major == v.Major && Minor < v.Minor)
                return true;
            if (Major == v.Major && Minor == v.Minor && Build < v.Build)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Build.ToString();
        }

        public int CompareTo(object obj)
        {
            if (obj is Version ver)
            {
                if (IsBefore(ver))
                    return -1;
                if (ver.IsBefore(this))
                    return 1;
                return 0;
            }
            return -1;
        }
    }
}
