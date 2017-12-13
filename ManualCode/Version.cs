using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    public class Version
    {
        private int patch = 0;
        private int update = 0;
        private int major = 0;

        public Version(string version)
        {
            string[] ver = version.Split('.');
            if(ver.Length == 3)
            {
                Patch = Int32.Parse(ver[2]);
                Update = Int32.Parse(ver[1]);
                Major = Int32.Parse(ver[0]);
            }
        }

        public Version(int major, int update, int patch)
        {
            this.Patch = patch;
            this.Update = update;
            this.Major = major;
        }

        public int Patch { get => patch; set => patch = value; }
        public int Update { get => update; set => update = value; }
        public int Major { get => major; set => major = value; }

        public bool IsBefore(Version v)
        {
            if (Major < v.Major)
                return true;
            if (Major == v.Major && Update < v.Update)
                return true;
            if (Major == v.Major && Update == v.Update && Patch < v.Patch)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Major.ToString() + "." + Update.ToString() + "." + Patch.ToString();
        }
    }
}
