using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeFlow
{
    [DBName("GENMANUA")]
    public class ManuaCode : Manual
    {
        [DBName("tipo")]
        private string tipo;

        [DBName("modulo")]
        private string modulo;

        [DBName("codmodul")]
        private Guid codmodul;

        [DBName("parameter")]
        private string parameter;

        [DBName("lang")]
        private string lang;

        [DBName("feature")]
        private string feature;

        [DBName("codfeature")]
        private Guid codfeature;

        [DBName("file")]
        private string file;

        [DBName("order")]
        private double order;

        [DBName("system")]
        private int system;

        [DBName("inhib")]
        private int inhib;

        public static string BEGIN_MANUAL = "BEGIN_MANUALCODE_CODMANUA:";
        public static string END_MANUAL = "END_MANUALCODE";

        private void init()
        {
            this.Code = "";
            this.Codfeature = Guid.Empty;
            this.Feature = "";
            this.Codmodul = Guid.Empty;
            this.Modulo = "";
            this.Plataform = "";
            this.Parameter = "";
            this.ManualFile = "";
            this.Order = .0f;
            this.Inhib = 0;
            this.System = 0;
            this.GenioUser = "";
        }

        public ManuaCode()
        {
            init();
        }

        public ManuaCode(Guid codmanua, string code)
        {
            init();
            this.CodeId = codmanua;
            this.Code = code;
        }
        public ManuaCode(string code)
        {
            init();
            this.Code = code;
        }

        [DBName("CODMANUA")]
        public override Guid CodeId { get => codeID; set => codeID = value; }
        [DBName("CORPO")]
        public override string Code { get => corpo; set => corpo = FixSetCurrentIndex(value); }
        public string Modulo { get => modulo; set => modulo = value; }
        public string Parameter { get => parameter; set => parameter = value; }
        public override string Lang { get => lang; set => lang = value; }
        public string Feature { get => feature; set => feature = value; }
        public string ManualFile { get => file; set => file = value; }
        public double Order { get => order; set => order = value; }
        public int System { get => system; set => system = value; }
        public int Inhib { get => inhib; set => inhib = value; }
        public Guid Codfeature { get => codfeature; set => codfeature = value; }
        public Guid Codmodul { get => codmodul; set => codmodul = value; }
        public string TipoRotina { get => tipo; set => tipo = value; }
        public override string TipoCodigo { get => "Manual"; }
        public override string Tag { get => Parameter; }
        public override string Tipo { get => TipoRotina; }

        private string FixSetCurrentIndex(string code)
        {
            return Regex.Replace(code,
                "((pDoc)(\\-\\>|\\.)){0,1}([0-9a-zA-Z_]*)(\\-\\>|\\.)(SetCurrentIndex)\\s*(\\()(\\s*INX_([_0-9a-zA-Z]\\s*)+\\s*)(\\))\\s*(;)\\s*(\\/\\/)*\\s*(\\[(([0-9a-zA-Z_]|\\s|(->))*\\]))",
                "$1$4$5$6$7$13$10$11",
                RegexOptions.Multiline);
        }

        private static Regex reg = new Regex(@"(Plataforma:)\s*(\w)*\s*(\|)\s*(Tipo:)\s*(\w)*\s*(\|)\s*(Modulo:)\s*(\w)*\s*(\|)\s*(Parametro:)\s*(\w)*\s*(\|)\s*(Ficheiro:)\s*(\w)*\s*(\|)\s*(Ordem:)\s*([+-]?([0-9]*[.])?[0-9]+)", RegexOptions.Compiled);

        public static List<IManual> GetManualCode(string vscode)
        {
            List<IManual> codeList = new List<IManual>();
            string remainig = vscode ?? "", plat = "", tipo = "", modulo = "", param = "", fich = "", ordem = "";
            do
            {
                int b = remainig.IndexOf(BEGIN_MANUAL);
                if (b == -1)
                    break;

                int platEnd = remainig.LastIndexOf(Utils.Util.NewLine, b);
                if (platEnd > -1)
                {
                    int platBegin = remainig.LastIndexOf(Utils.Util.NewLine, platEnd) + Utils.Util.NewLine.Length;
                    if (platBegin != -1)
                    {
                        Match match = reg.Match(remainig.Substring(platBegin, platEnd - platBegin));
                        if (match.Success)
                        {
                            plat = match.Groups[2].Value;
                            tipo = match.Groups[5].Value;
                            modulo = match.Groups[8].Value;
                            param = match.Groups[11].Value;
                            fich = match.Groups[14].Value;
                            ordem = match.Groups[17].Value;
                        }
                    }
                }
                IManual m = ParseText<ManuaCode>(BEGIN_MANUAL, END_MANUAL, remainig, out remainig);
                if (m != null)
                {
                    codeList.Add(m);
                    ManuaCode manua = (ManuaCode)m;
                    manua.Plataform = plat;
                    manua.TipoRotina = tipo;
                    manua.Modulo = modulo;
                    manua.Parameter = param;
                    manua.ManualFile = fich;
                    Double.TryParse(ordem, out double tmp);
                    manua.Order = tmp;
                }
            } while (remainig.Length != 0);

            return codeList;
        }

        public override bool Update(Profile profile)
        {
            bool result = false;

            lock (PackageOperations.lockObject)
            {
                if (!profile.GenioConfiguration.ConnectionIsOpen())
                    profile.GenioConfiguration.OpenConnection();

                if (profile.GenioConfiguration.ConnectionIsOpen())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("UPDATE GENMANUA SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODMANUA = @CODMANUA");
                        cmd.Parameters.AddWithValue("@CORPO", this.Code);
                        cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
                        cmd.Parameters.AddWithValue("@OPERMUDA", PackageOperations.ActiveProfile.GenioConfiguration.Username);
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                        cmd.ExecuteNonQuery();

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return result;
        }

        public bool Insert(Profile profile)
        {
            lock (PackageOperations.lockObject)
            {
                if (!profile.GenioConfiguration.ConnectionIsOpen())
                    profile.GenioConfiguration.OpenConnection();

                if (profile.GenioConfiguration.ConnectionIsOpen())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("INSERT INTO GENMANUA (CODMANUA, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, CORPO, LANG, ORDEM, CODCARAC, CODMODUL, ISSISTEM, NEGCARAC, CARAC, DATACRIA, OPERCRIA, ZZSTATE) "
                        + "VALUES (@CODMANUA, @PLATAFOR, @TIPO, @MODULO, @PARAMETR, @FICHEIRO, @CORPO, @LANG, @ORDEM, @CODCARAC, @CODMODUL, @ISSISTEM, @NEGCARAC, @CARAC, @DATACRIA, @OPERCRIA, 0)");

                        cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
                        cmd.Parameters.AddWithValue("@PLATAFOR", this.Plataform);
                        cmd.Parameters.AddWithValue("@TIPO", this.Tag);
                        cmd.Parameters.AddWithValue("@MODULO", this.Modulo);
                        cmd.Parameters.AddWithValue("@PARAMETR", this.Parameter);
                        cmd.Parameters.AddWithValue("@FICHEIRO", this.ManualFile);
                        cmd.Parameters.AddWithValue("@CORPO", this.Code);
                        cmd.Parameters.AddWithValue("@LANG", this.Lang);
                        cmd.Parameters.AddWithValue("@ORDEM", this.Order);
                        cmd.Parameters.AddWithValue("@CODCARAC", this.Codfeature);
                        cmd.Parameters.AddWithValue("@CODMODUL", this.Codmodul);
                        cmd.Parameters.AddWithValue("@ISSISTEM", this.System);
                        cmd.Parameters.AddWithValue("@NEGCARAC", this.Inhib);
                        cmd.Parameters.AddWithValue("@CARAC", this.Feature);
                        cmd.Parameters.AddWithValue("@DATACRIA", DateTime.Now);
                        cmd.Parameters.AddWithValue("@OPERCRIA", this.GenioUser);
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                    return cmd.ExecuteNonQuery() != 0;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return false;
        }

        public new static ManuaCode GetManual(Guid codmanua, Profile profile)
        {
            ManuaCode man = null;
            SqlDataReader reader = null;
            lock (PackageOperations.lockObject)
            {
                if (!profile.GenioConfiguration.ConnectionIsOpen())
                    profile.GenioConfiguration.OpenConnection();

                if (profile.GenioConfiguration.ConnectionIsOpen())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("SELECT CODMANUA, CORPO, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM FROM GENMANUA WHERE CODMANUA = '{0}'", codmanua);
                        cmd.CommandType = global::System.Data.CommandType.Text;
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                        reader = cmd.ExecuteReader();
                        man = new ManuaCode("");
                        if (reader.HasRows)
                        {
                            reader.Read();
                            man.CodeId = reader.GetGuid(0);
                            man.Code = reader.GetString(1);
                            man.Plataform = reader.GetString(2);
                            man.TipoRotina = reader.GetString(3);
                            man.Modulo = reader.GetString(4);
                            man.Parameter = reader.GetString(5);
                            man.ManualFile = reader.GetString(6);
                            man.Lang = reader.GetString(7);
                            man.Order = reader.GetDouble(8);
                            man.CodeTransformKeyValue();
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

            return man;
        }

        public void CompareDB(Profile profile)
        {
            ManuaCode bd = GetManual(CodeId, profile);

            Compare(this, bd);
        }

        public override void ShowSVNLog(Profile profile, string systemName)
        {
            try
            {
                OpenSVNLog($"{profile.GenioConfiguration.CheckoutPath + "\\ManualCode\\" + "MAN" + this.ManualFile + "." + systemName}");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public override string ToString()
        {
            string str = BEGIN_MANUAL + this.CodeId.ToString() + Utils.Util.NewLine;
            str += this.Code;
            str += Utils.Util.NewLine;
            str += END_MANUAL;

            return str;
        }
        
        public static List<ManuaCode> Search(Profile profile, string texto, int limitBodySize = 80, bool caseSensitive = false, bool wholeWord = false)
        {
            List<ManuaCode> results = new List<ManuaCode>();
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
                    string manuaQuery = String.Format("SELECT CODMANUA, {0}, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM FROM GENMANUA WHERE CORPO LIKE @TERM", tmp);

                    string search = "%" + texto + "%";
                    if (caseSensitive)
                        manuaQuery += " COLLATE Latin1_General_BIN;";
                    if (wholeWord)
                        search = $"%[ ]{texto}[ ]%";

                    cmd.CommandText = manuaQuery;
                    cmd.CommandType = global::System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@TERM", search);
                    cmd.Connection = profile.GenioConfiguration.SqlConnection;

                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid codmanua = reader.GetGuid(0);
                            string corpo = reader.GetString(1);

                            ManuaCode man = new ManuaCode(codmanua, corpo)
                            {
                                Plataform = reader.GetString(2),
                                TipoRotina = reader.GetString(3),
                                Modulo = reader.GetString(4),
                                Parameter = reader.GetString(5),
                                ManualFile = reader.GetString(6),
                                Lang = reader.GetString(7),
                                Order = reader.GetDouble(8)
                            };
                            man.CodeTransformKeyValue();
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
    }
}
