using CodeFlow.GenioManual;
using CodeFlow.GenioOperations;
using CodeFlow.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeFlow.ManualOperations
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
        protected string localFileName = "";
        protected ManualMatch localMatch = new ManualMatch();
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
        
        public virtual string Code { get => corpo; set => corpo = value; }
        
        public string Plataform { get => plataform; set => plataform = value; }
        
        public string CreatedBy { get => createdBy; set => createdBy = value; }
        
        public DateTime LastChangeDate { get => lastChangeDate; set => lastChangeDate = value; }
        
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }

        public virtual string GetCodeExtension(Profile p)
        {
            GenioPlataform plat = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Find(x => x.ID.Equals(Plataform));
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

                return Code.Replace("\r\n", String.Empty).Replace("\n", String.Empty).Replace("\r", String.Empty).Replace(lineSeparator, String.Empty).Replace(paragraphSeparator, String.Empty).Replace("\t", String.Empty);
            }
        }
        public string ShortOneLineCode(int max = 300)
        {
            if (OneLineCode.Length < max)
                max = OneLineCode.Length;

            return OneLineCode.Substring(0, max);
        }
        public string ChangedBy { get => changedBy; set => changedBy = value; }
        public static IManual GetManual(Type t, Guid g, Profile profile)
        {
            IManual man = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { profile, g }) as IManual;
            return man;
        }
        public void CompareDB(Profile profile)
        {
            IManual bd = GetManual(GetType(), CodeId, profile);
            Compare(bd, this);
        }
        public IManual MergeDB(Profile profile)
        {
            IManual bd = GetManual(GetType(), CodeId, profile);
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

                string lname = "Genio copy";
                string rname = "Working copy";

                File.WriteAllText(tmpBD, database.Code);
                File.WriteAllText(tmpOld, database.Code);
                File.WriteAllText(tmpCode, local.Code);


                Process merge = new Process();
                if (!String.IsNullOrEmpty(PackageOperations.Instance.UseCustomTool))
                {
                    string tool = PackageOperations.Instance.UseCustomTool
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

                var finalCode = File.ReadAllText(tmpFinal);
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
                throw new Exception(string.Format(Properties.Resources.ErrorMerge, e.Message));
            }
        }
        public static List<IManual> SearchDatabase(Profile profile, string texto, bool caseSensitive = false, bool wholeWord = false, string plataform = "")
        {
            List<IManual> results = new List<IManual>();
            string searchTerms = texto.Replace("[", "[[]");
            results.AddRange(ManuaCode.Search(profile, searchTerms, caseSensitive, wholeWord, plataform));
            results.AddRange(CustomFunction.Search(profile, searchTerms, caseSensitive, wholeWord, plataform));
            return results;
        }
        public string CodeTransformKeyValue()
        {
            string c = Code;
            foreach (KeyValuePair<Int32, byte> entry in SpecialChars)
            {
                c = Code.Replace((char)entry.Key, (char)entry.Value);
            }

            return c;
        }
        public string CodeTransformValueKey()
        {
            string c = Code;
            foreach (KeyValuePair<Int32, byte> entry in SpecialChars)
            {
                c = Code.Replace((char)entry.Value, (char)entry.Key);
            }

            return c;
        }

        public void ShowSVNLog(Profile profile)
        {
            try
            {
                Process merge = new Process();
                merge.StartInfo.FileName = "TortoiseProc.exe ";
                merge.StartInfo.Arguments = $"/command:log /path:{GetFilePath(profile)}";
                merge.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                merge.Start();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(Properties.Resources.ErrorSVN, e.Message));
            }
        }
        public void Blame(Profile profile)
        {
            try
            {
                Process merge = new Process();
                merge.StartInfo.FileName = "TortoiseProc.exe ";
                merge.StartInfo.Arguments = $"/command:blame /path:{GetFilePath(profile)}";
                merge.Start();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(Properties.Resources.ErrorSVN, e.Message));
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
        public ManualMatchProvider GetMatchProvider()
        {
            // Get instance of the attribute.
            return Attribute.GetCustomAttribute(this.GetType(), typeof(ManualMatchProvider)) as ManualMatchProvider;

        }
        public static string FixSetCurrentIndex(string code)
        {
            return Regex.Replace(code, "(INX_[_0-9a-zA-Z]*)(.*)\\s*(\\/\\/)(\\s*)(\\[FNTX\\s*([0-9a-zA-Z_]|\\s|->)*\\])",
                "/$5/$2$3$4$5",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }
        public abstract bool Update(Profile profile);
        public abstract bool Create(Profile profile);
        public abstract bool Delete(Profile profile);
        public abstract string GetFilePath(Profile profile);
        public abstract string FormatCode(string extension);
        public abstract bool MatchAndFix(string upperLine);
        public abstract string Lang { get; set; }
        public abstract string Tag { get; }
        public abstract string TipoCodigo { get; }
        public abstract string Tipo { get; }
        public string LocalFileName { get => localMatch.LocalFileName; }
        public ManualMatch LocalMatch { get => localMatch; set => localMatch = value; }
        public string FullFileName { get => localMatch.FullFileName; }
    }
}
