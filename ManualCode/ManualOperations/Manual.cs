using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CodeFlow
{
    public abstract class Manual : IManual
    {
        protected Guid codeID;
        protected string corpo;
        protected string plataform;
        protected string geniouser;
        protected DateTime creationDate;
        protected DateTime lastChangeDate;
        public static Dictionary<Int32, byte> SpecialChars = new Dictionary<Int32, byte>
        {
            //Unicode characters mappings to extended ASCII
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
        public string GenioUser { get => geniouser; set => geniouser = value; }

        [DBName("DATAMUDA")]
        public DateTime LastChangeDate { get => lastChangeDate; set => lastChangeDate = value; }

        [DBName("DATACRIA")]
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        public abstract string Lang { get; set; }
        public abstract string Tag { get; set; }

        public string GetCodeExtension(Profile p)
        {
            p.GenioConfiguration.Tipos.TryGetValue(Tag, out string extension);

            return extension ?? "tmp";
        }

        public string ShortCode
        {
            get
            {
                int max = 80;
                if (Code.Length < max)
                    max = Code.Length;

                return Code.Substring(0, max).Replace(Utils.Util.NewLine, " ").Trim();
            }
        }

        public string OpenManual(EnvDTE80.DTE2 dte, Profile p)
        {
            string tmp = Path.GetTempPath() + Guid.NewGuid().ToString() + "." + GetCodeExtension(p);
            File.WriteAllText(tmp, ToString());
            dte.ItemOperations.OpenFile(tmp);
            return tmp;
        }

        public static List<IManual> SearchManual(Profile profile, string texto, int limitBodySize = 80)
        {
            List<IManual> results = new List<IManual>();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            lock (PackageOperations.lockObject)
            {
                if (!profile.GenioConfiguration.ConnectionIsOpen())
                    profile.GenioConfiguration.OpenConnection();

                if (profile.GenioConfiguration.ConnectionIsOpen())
                {
                    string tmp = String.Format("LEFT(CORPO, {0})", limitBodySize);
                    if (limitBodySize == 0)
                        tmp = "CORPO";
                    string manuaQuery = String.Format("SELECT CODMANUA, {0}, PLATAFOR, '' NOME, 'MANUAL' TIPO FROM GENMANUA WHERE CORPO LIKE @TERM", tmp); ;
                    string customFuncQuery = String.Format("SELECT IMPLS.CODIMPLS, {0}, PLATAFOR, FUNCS.NOME, 'CUSTOM' TIPO "+
                        "FROM GENFUNCS FUNCS INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS " + 
                        "WHERE CORPO LIKE @TERM OR NOME LIKE @TERM", tmp);
                    cmd.CommandText = String.Format("{0} UNION ALL {1}", manuaQuery, customFuncQuery);
                    cmd.CommandType = global::System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@TERM", "%" + texto + "%");
                    cmd.Connection = profile.GenioConfiguration.SqlConnection;

                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid codmanua = reader.GetGuid(0);
                            string corpo = reader.GetString(1);
                            string plataforma = reader.GetString(2);
                            string nome = reader.GetString(3);
                            string tipo = reader.GetString(4);

                            IManual man;
                            if (tipo.Equals("CUSTOM"))
                                man = new CustomFunction(corpo, nome);
                            else
                                man = new ManuaCode(corpo);

                            man.CodeTransformKeyValue();
                            man.CodeId = codmanua;
                            man.Code = corpo;
                            man.Plataform = plataforma;
                            results.Add(man);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }

            return results;
        }

        public static IManual Merge(IManual local, IManual bd)
        {
            return MergeCompare(local, bd, true);
        }
        public static void Compare(IManual local, IManual bd)
        {
            MergeCompare(local, bd, false);
        }

        private static IManual MergeCompare(IManual m1, IManual m2, bool saveRequired)
        {
            try
            {
                if (m1 == null 
                    || m2 == null)
                {
                    throw new Exception("Unable to merge/compare manual code!");
                }

                string tmpOld = Path.GetTempFileName();
                string tmpCode = Path.GetTempFileName();
                string tmpBD = Path.GetTempFileName();
                string tmpFinal = Path.GetTempFileName();
                string finalCode;


                File.WriteAllText(tmpBD, m2.Code);
                File.WriteAllText(tmpOld, m2.Code);
                File.WriteAllText(tmpCode, m1.Code);

                Process merge = new Process();
                merge.StartInfo.FileName = "TortoiseMerge.exe";
                merge.StartInfo.Arguments =
                    String.Format("/base:\"{0}\" /basename:\"BASE\" /theirs:\"{1}\" /mine:\"{2}\" /merged:\"{3}\" {4} {5}",
                    tmpOld, tmpBD, tmpCode, tmpFinal, saveRequired ? "/saverequired" : "", saveRequired ? "/saverequiredonconflicts" : "");
                merge.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                merge.Start();

                merge.WaitForExit();

                finalCode = File.ReadAllText(tmpFinal);

                File.Delete(tmpBD);
                File.Delete(tmpOld);
                File.Delete(tmpCode);
                File.Delete(tmpFinal);

                IManual m = new ManuaCode(m1.CodeId, finalCode);
                m.CodeTransformValueKey();

                return m;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static IManual GetManual(Guid codeID, Profile profile)
        {
            StackTrace stackTrace = new StackTrace();
            Type t = stackTrace.GetFrame(1).GetType();

            var method = t.GetMethod("GetManual");
            return (Manual)method.Invoke(null, new object[] { codeID, profile });
        }

        protected static List<IManual> ParseManual(string begin, string end, string vscode)
        {
            List<IManual> man = new List<IManual>();
            int b = vscode.IndexOf(begin);
            int e = -1;
            string nl = Utils.Util.NewLine;

            StackFrame frame = new StackFrame(1);
            var method = frame.GetMethod();
            var t = method.DeclaringType;
            if (t == null)
                return man;

            ConstructorInfo ctor = t.GetConstructor(new[] { typeof(Guid), typeof(string) });
            if (ctor == null)
                return man;

            while (b != -1)
            {
                string s = vscode.Substring(b + begin.Length);
                int i = s.IndexOf(nl);

                if (i == -1)
                    break;

                string guid = s.Substring(0, i).Trim();
                guid = guid.Substring(0, 36);
                e = s.IndexOf(end);
                string c = "", code = "";
                if (e == -1)
                {
                    c = s.Substring(i);
                    code = c.Substring(nl.Length);
                    vscode = "";
                }
                else
                {
                    e = e - i;

                    c = s.Substring(i, e);
                    int tmp = c.LastIndexOf(Utils.Util.NewLine);
                    if (tmp != -1)
                        e = tmp;
                    else
                        e = c.Length - i - nl.Length;
                    code = c.Substring(nl.Length, e - nl.Length);

                    b = vscode.IndexOf(begin, b);
                    vscode = s.Substring(i + e);
                }

                IManual m = (IManual)ctor.Invoke(new object[] { Guid.Parse(guid), code });
                m.CodeTransformValueKey();
                man.Add(m);

                b = vscode.IndexOf(begin);
            }

            return man;
        }

        public abstract bool Update(Profile profile);

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

        public void CodeTransformKeyValue()
        {
            foreach (KeyValuePair<Int32, byte> entry in Manual.SpecialChars)
            {
                Code.Replace((char)entry.Key, (char)entry.Value);
            }
        }
        public void CodeTransformValueKey()
        {
            foreach (KeyValuePair<Int32, byte> entry in Manual.SpecialChars)
            {
                Code.Replace((char)entry.Value, (char)entry.Key);
            }
        }

        public abstract void ShowSVNLog(Profile profile, string systemName);
    }
}
