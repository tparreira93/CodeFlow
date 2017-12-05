using CodeFlow.GenioOperations;
using CodeFlow.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeFlow
{
    public abstract class Manual : IManual
    {
        protected Guid codeID = Guid.Empty;
        protected string corpo = "";
        protected string plataform = "";
        protected string createdBy = "";
        protected string changedBy = "";
        protected DateTime creationDate = DateTime.MaxValue;
        protected DateTime lastChangeDate = DateTime.MaxValue;
        private string localFileName = "";
        public static Dictionary<Int32, byte> SpecialChars = new Dictionary<Int32, byte>
        {
            //Unicode characters mappings to extended ASCII
            //Unicode = ASCII
            [0x20AC] = (byte)'\x80',
            [0x201A] = (byte)'\x82',
            [0x0192] = (byte)'\x83',
            [0x201E] = (byte)'\x84',
            [0x2026] = (byte)'\x85',
            [0x2020] = (byte)'\x86',
            [0x2021] = (byte)'\x87',
            [0x02C6] = (byte)'\x88',
            [0x2030] = (byte)'\x89',
            [0x0160] = (byte)'\x8A',
            [0x2039] = (byte)'\x8B',
            [0x0152] = (byte)'\x8C',
            [0x017D] = (byte)'\x8E',
            [0x2018] = (byte)'\x91',
            [0x2019] = (byte)'\x92',
            [0x201C] = (byte)'\x93',
            [0x201D] = (byte)'\x94',
            [0x2022] = (byte)'\x95',
            [0x2013] = (byte)'\x96',
            [0x2014] = (byte)'\x97',
            [0x02DC] = (byte)'\x98',
            [0x2122] = (byte)'\x99',
            [0x0161] = (byte)'\x9A',
            [0x203A] = (byte)'\x9B',
            [0x0153] = (byte)'\x9C',
            [0x017E] = (byte)'\x9E',
            [0x0178] = (byte)'\x9F'
        };
        
        public abstract Guid CodeId { get; set; }

        [DBName("Corpo")]
        public virtual string Code { get => corpo; set => corpo = value; }

        [DBName("Plataform")]
        public string Plataform { get => plataform; set => plataform = value; }

        [DBName("OPERMUDA")]
        public string CreatedBy { get => createdBy; set => createdBy = value; }

        [DBName("DATAMUDA")]
        public DateTime LastChangeDate { get => lastChangeDate; set => lastChangeDate = value; }

        [DBName("DATACRIA")]
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }

        public string GetCodeExtension(Profile p)
        {
            GenioPlataform plat = PackageOperations.GetActiveProfile().GenioConfiguration.Plataforms.Find(x => x.ID.Equals(Plataform));
            string extension = plat?.TipoRotina?.Find(x => x.Identifier.Equals(Tipo))?.ProgrammingLanguage;
            return extension ?? "tmp";
        }
        public string OneLineCode
        {
            get
            {
                if (String.IsNullOrEmpty(Code))
                {
                    return Code;
                }
                string lineSeparator = ((char)0x2028).ToString();
                string paragraphSeparator = ((char)0x2029).ToString();

                return Code.Replace("\r\n", String.Empty).Replace("\n", String.Empty).Replace("\r", String.Empty).Replace(lineSeparator, String.Empty).Replace(paragraphSeparator, String.Empty);
            }
        }
        public string ShortOneLineCode(int max = 300)
        {
            if (Code.Length < max)
                max = OneLineCode.Length;

            return OneLineCode.Substring(0, max);
        }
        public string ChangedBy { get => changedBy; set => changedBy = value; }
        public void CompareDB(Profile profile)
        {
            var t = this.GetType();
            IManual bd = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { profile, this.CodeId }) as IManual;
            Compare(bd, this);
        }
        public IManual MergeDB(Profile profile)
        {
            var t = this.GetType();
            IManual bd = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { profile, this.CodeId }) as IManual;
            return MergeCompare(bd, this, true);
        }
        public static IManual Merge(IManual bd, IManual local)
        {
            return MergeCompare(bd, local, true);
        }
        public static void Compare(IManual bd, IManual local)
        {
            MergeCompare(bd, local, false);
        }
        private static IManual MergeCompare(IManual database, IManual local, bool saveRequired)
        {
            try
            {
                if (local == null 
                    || database == null)
                {
                    throw new Exception(Properties.Resources.ErrorComparing);
                }

                string tmpOld = Path.GetTempFileName();
                string tmpCode = Path.GetTempFileName();
                string tmpBD = Path.GetTempFileName();
                string tmpFinal = Path.GetTempFileName();
                string finalCode;

                string lname = "Genio copy";
                string rname = "Working copy";

                File.WriteAllText(tmpBD, database.Code);
                File.WriteAllText(tmpOld, database.Code);
                File.WriteAllText(tmpCode, local.Code);


                Process merge = new Process();
                if (!String.IsNullOrEmpty(PackageOperations.UseCustomTool))
                {
                    string tool = PackageOperations.UseCustomTool
                        .Replace("%left",   "\"" + tmpBD + "\"")
                        .Replace("%right",  "\"" + tmpCode + "\"")
                        .Replace("%result", "\"" + tmpFinal + "\"")
                        .Replace("%lname",  "\"" + lname + "\"")
                        .Replace("%rname",  "\"" + rname + "\"");

                    var parts = Regex.Matches(tool, @"[\""].+?[\""]|[^ ]+")
                                    .Cast<Match>()
                                    .Select(p => p.Value)
                                    .ToList();

                    if (parts.Count < 2)
                        throw new Exception(Properties.Resources.CustomToolError);

                    merge.StartInfo.FileName = parts[0];
                    merge.StartInfo.Arguments = tool.Substring(parts[0].Length);
                }
                else
                {
                    merge.StartInfo.FileName = "TortoiseMerge.exe";
                    merge.StartInfo.Arguments =
                        $"/base:\"{tmpOld}\" /basename:\"BASE\" " +
                        $"/theirs:\"{tmpBD}\" /theirsname:{lname} " +
                        $"/mine:\"{tmpCode}\" /minename:{rname} " +
                        $"/merged:\"{tmpFinal}\" {(saveRequired ? " / saverequiredonconflicts" : "")}";
                }

                merge.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                merge.Start();

                merge.WaitForExit();

                finalCode = File.ReadAllText(tmpFinal);
                if (String.IsNullOrEmpty(finalCode))
                    finalCode = local.Code;

                File.Delete(tmpBD);
                File.Delete(tmpOld);
                File.Delete(tmpCode);
                File.Delete(tmpFinal);

                ConstructorInfo ctor = local.GetType().GetConstructor(new Type[] { });
                IManual m = ctor.Invoke(new object[] { }) as IManual;
                Util.CopyFrom(local.GetType(), local, m);
                m.Code = finalCode;
                m.Code = m.CodeTransformValueKey();

                return m;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        protected static IManual ParseText<T>(string begin, string end, string vscode, out string remaning)
            where T : IManual, new()
        {
            int b = vscode.IndexOf(begin);
            int e = -1;
            IManual m = null;
            remaning = "";

            if (b != -1)
            {
                string s = vscode.Substring(b + begin.Length);
                int i = s.IndexOf(Util.NewLine);

                if (i == -1)
                    return m;

                string guid = s.Substring(0, i).Trim();
                guid = guid.Substring(0, 36);
                if (Guid.TryParse(guid, out Guid g))
                {
                    e = s.IndexOf(end);
                    string c = "", code = "";
                    if (e == -1)
                    {
                        int anotherB = s.IndexOf(begin, i);
                        if (anotherB != -1)
                            return m;

                        c = s.Substring(i);
                        code = c.Substring(Util.NewLine.Length);
                        remaning = "";
                    }
                    else
                    {
                        e = e - i;
                        c = s.Substring(i, e);
                        int tmp = c.LastIndexOf(Util.NewLine);
                        if (tmp != -1)
                            e = tmp;
                        else
                            e = c.Length - i - Util.NewLine.Length;
                        code = c.Substring(Util.NewLine.Length, e - Util.NewLine.Length);

                        b = vscode.IndexOf(begin, b);
                        remaning = s.Substring(i + e);

                        int anotherB = s.IndexOf(begin, i, e);
                        if (anotherB != -1)
                            return m;
                    }

                    m = new T
                    {
                        Code = Util.ConverToDOSLineEndings(code),
                        CodeId = g
                    };
                    m.Code = m.CodeTransformKeyValue();
                }
            }

            return m;
        }
        public string CodeTransformKeyValue()
        {
            string c = Code;
            foreach (KeyValuePair<Int32, byte> entry in Manual.SpecialChars)
            {
                c.Replace((char)entry.Key, (char)entry.Value);
            }

            return c;
        }
        public string CodeTransformValueKey()
        {
            string c = Code;
            foreach (KeyValuePair<Int32, byte> entry in Manual.SpecialChars)
            {
                c.Replace((char)entry.Value, (char)entry.Key);
            }

            return c;
        }
        protected void OpenSVNLog(string filePath)
        {
            try
            {
                Process merge = new Process();
                merge.StartInfo.FileName = "TortoiseProc.exe ";
                merge.StartInfo.Arguments = $"/command:log /path:{filePath}";
                merge.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                merge.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        protected string FormatComment(string extension, string str)
        {
            switch (extension)
            {
                case ".cs":
                case ".cpp":
                case ".js":
                    return "//" + str;
                case ".sql":
                    return "/*" + str + "*/";
                case ".xml":
                    return "<!--" + str + "-->";
                default:
                    return "//" + str;
            }
        }
        public abstract void ShowSVNLog(Profile profile, string systemName);
        public abstract bool Update(Profile profile);
        public abstract string Lang { get; set; }
        public abstract string Tag { get; }
        public abstract string TipoCodigo { get; }
        public abstract string Tipo { get; }
        public string LocalFileName { get => localFileName; set => localFileName = value; }
    }
}
