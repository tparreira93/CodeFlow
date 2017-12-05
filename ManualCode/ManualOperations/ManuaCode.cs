using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using CodeFlow.Utils;
using System.Text;
namespace CodeFlow
{
    [DBName("GENMANUA")]
    public class ManuaCode : Manual
    {
        [DBName("tipo")]
        private string tipo = "";

        [DBName("modulo")]
        private string modulo = "";

        [DBName("codmodul")]
        private Guid codmodul = Guid.Empty;

        [DBName("parameter")]
        private string parameter = "";

        [DBName("lang")]
        private string lang = "";

        [DBName("feature")]
        private string feature = "";

        [DBName("codfeature")]
        private Guid codfeature = Guid.Empty;

        [DBName("file")]
        private string file = "";

        [DBName("order")]
        private double order = 0;

        [DBName("system")]
        private int system = 0;

        [DBName("inhib")]
        private int inhib = 0;

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
            this.CreatedBy = "";
        }

        public ManuaCode()
        {
            init();
        }

        public ManuaCode(Guid codmanua, string code)
        {
            this.CodeId = codmanua;
            this.Code = code;
        }
        public ManuaCode(string code)
        {
            this.Code = code;
        }

        [DBName("CODMANUA")]
        public override Guid CodeId { get => codeID; set => codeID = value; }
        [DBName("CORPO")]
        public override string Code { get => corpo; set => corpo = value; }
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

        #region LocalOperations
        private static Regex reg = new Regex(@"(Plataforma:)\s*(\w*)\s*(\|)\s*(Tipo:)\s*(\w*)\s*(\|)\s*(Modulo:)\s*(\w*)\s*(\|)\s*(Parametro:)\s*(\w*)\s*(\|)\s*(Ficheiro:)\s*(\w*)\s*(\|)\s*(Ordem:)\s*([+-]?([0-9]*[.])?[0-9]+)", RegexOptions.Compiled);
        public static string FixSetCurrentIndex(string code)
        {
            return Regex.Replace(code, "(INX_[_0-9a-zA-Z]*)(.*)\\s*(\\/\\/)(\\s*)(\\[FNTX\\s*([0-9a-zA-Z_]|\\s|->)*\\])", 
                "/$5/$2$3$4$5", 
                RegexOptions.Multiline | RegexOptions.Compiled);
        }
        public static List<IManual> GetManualCode(string vscode, string localFileName = "")
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
                    if (platBegin != -1 && platEnd - platBegin > 0)
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
                    m.LocalFileName = localFileName;
                    codeList.Add(m);
                    ManuaCode manua = (ManuaCode)m;
                    manua.Plataform = plat;
                    manua.TipoRotina = tipo;
                    manua.Modulo = modulo;
                    manua.Parameter = param;
                    manua.ManualFile = fich;
                    Double.TryParse(ordem, out double tmp);
                    manua.Order = tmp;
                    manua.Code = FixSetCurrentIndex(manua.Code);
                }
            } while (remainig.Length != 0);

            return codeList;
        }
        public override void ShowSVNLog(Profile profile, string systemName)
        {
            try
            {
                OpenSVNLog($"{profile.GenioConfiguration.CheckoutPath + "\\ManualCode\\" + "MAN" + this.Plataform + this.ManualFile + "." + systemName}");
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public string FormatCode(string extension)
        {
            string str = FormatComment(extension, BEGIN_MANUAL + this.CodeId.ToString()) + Utils.Util.NewLine;
            str += this.Code;
            str += Utils.Util.NewLine;
            str += FormatComment(extension, END_MANUAL);

            return str;
        }
        public override string ToString()
        {
            string str = BEGIN_MANUAL + this.CodeId.ToString() + Utils.Util.NewLine;
            str += this.Code;
            str += Utils.Util.NewLine;
            str += END_MANUAL;

            return str;
        }
        #endregion

        #region DatabaseOperations
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
                        string c = Util.ConverToDOSLineEndings(CodeTransformKeyValue());

                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("UPDATE GENMANUA SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODMANUA = @CODMANUA");
                        cmd.Parameters.AddWithValue("@CORPO", c);
                        cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
                        cmd.Parameters.AddWithValue("@OPERMUDA", PackageOperations.GetActiveProfile().GenioConfiguration.GenioUser);
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                        cmd.ExecuteNonQuery();

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        profile.GenioConfiguration.CloseConnection();
                    }
                }
            }

            return result;
        }
        public bool Insert(Profile profile)
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
                        if(CodeId.Equals(Guid.Empty))
                            this.CodeId = Guid.NewGuid();

                        string c = Util.ConverToDOSLineEndings(CodeTransformValueKey());

                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("INSERT INTO GENMANUA (CODMANUA, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, CORPO, LANG, ORDEM, CODCARAC, CODMODUL, ISSISTEM, NEGCARAC, CARAC, DATACRIA, OPERCRIA, ZZSTATE) "
                        + "VALUES (@CODMANUA, @PLATAFOR, @TIPO, @MODULO, @PARAMETR, @FICHEIRO, @CORPO, @LANG, @ORDEM, @CODCARAC, @CODMODUL, @ISSISTEM, @NEGCARAC, @CARAC, @DATACRIA, @OPERCRIA, 0)");

                        cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
                        cmd.Parameters.AddWithValue("@PLATAFOR", this.Plataform);
                        cmd.Parameters.AddWithValue("@TIPO", this.TipoRotina);
                        cmd.Parameters.AddWithValue("@MODULO", this.Modulo);
                        cmd.Parameters.AddWithValue("@PARAMETR", this.Parameter);
                        cmd.Parameters.AddWithValue("@FICHEIRO", this.ManualFile);
                        cmd.Parameters.AddWithValue("@CORPO", c);
                        cmd.Parameters.AddWithValue("@LANG", this.Lang);
                        cmd.Parameters.AddWithValue("@ORDEM", this.Order);
                        cmd.Parameters.AddWithValue("@CODCARAC", this.Codfeature);
                        cmd.Parameters.AddWithValue("@CODMODUL", this.Codmodul);
                        cmd.Parameters.AddWithValue("@ISSISTEM", this.System);
                        cmd.Parameters.AddWithValue("@NEGCARAC", this.Inhib);
                        cmd.Parameters.AddWithValue("@CARAC", this.Feature);
                        cmd.Parameters.AddWithValue("@DATACRIA", DateTime.Now);
                        cmd.Parameters.AddWithValue("@OPERCRIA", profile.GenioConfiguration.GenioUser);
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                        result = cmd.ExecuteNonQuery() != 0;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        profile.GenioConfiguration.CloseConnection();
                    }
                }
            }
            return result;
        }
        public static ManuaCode GetManual(Profile profile, Guid codmanua)
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
                        cmd.CommandText = String.Format("SELECT CODMANUA, CORPO, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM, OPERCRIA, OPERMUDA, DATACRIA, DATAMUDA FROM GENMANUA WHERE CODMANUA = @CODMANUA");
                        cmd.Parameters.AddWithValue("@CODMANUA", codmanua);
                        cmd.CommandType = global::System.Data.CommandType.Text;
                        cmd.Connection = profile.GenioConfiguration.SqlConnection;

                        reader = cmd.ExecuteReader();
                        man = new ManuaCode("");
                        if (reader.HasRows)
                        {
                            reader.Read();
                            man.CodeId = reader.SafeGetGuid(0);
                            man.Code = Util.ConverToDOSLineEndings(reader.SafeGetString(1));
                            man.Plataform = reader.SafeGetString(2);
                            man.TipoRotina = reader.SafeGetString(3);
                            man.Modulo = reader.SafeGetString(4);
                            man.Parameter = reader.SafeGetString(5);
                            man.ManualFile = reader.SafeGetString(6);
                            man.Lang = reader.SafeGetString(7);
                            man.Order = reader.SafeGetDouble(8);
                            man.CreatedBy = reader.SafeGetString(9);
                            man.ChangedBy = reader.SafeGetString(10);
                            man.CreationDate = reader.SafeGetDateTime(11);
                            man.LastChangeDate = reader.SafeGetDateTime(12);

                            man.Code = man.CodeTransformKeyValue();
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
                        profile.GenioConfiguration.CloseConnection();
                    }
                }
            }

            return man;
        }
        public static List<ManuaCode> Search(Profile profile, string texto, bool caseSensitive = false, bool wholeWord = false, string plataform = "")
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
                    string manuaQuery = String.Format("SELECT CODMANUA, @RESULT_LINE FOUND_LINE, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM, OPERCRIA, OPERMUDA, DATACRIA, DATAMUDA "
                                                    + "FROM GENMANUA WHERE (' ' + CORPO + ' ') LIKE @TERM @CASESENSITIVE");

                    if (plataform.Length != 0)
                    {
                        manuaQuery += " AND PLATAFOR = @PLATAFORM";
                        cmd.Parameters.AddWithValue("@PLATAFORM", plataform);
                    }

                    string search = "%" + texto + "%";
                    string result_line = "RIGHT(LEFT(CORPO, @AFTER_NEWLINE), @BEFORE_NEWLINE)";
                    string after_newline = "CHARINDEX(CHAR(13), CORPO, PATINDEX(@TERM, CORPO @CASESENSITIVE))";
                    string before_newline = "CHARINDEX(CHAR(13), REVERSE(LEFT(CORPO, @AFTER_NEWLINE)), 2)";
                    result_line = result_line.Replace("@BEFORE_NEWLINE", before_newline).Replace("@AFTER_NEWLINE", after_newline);
                    manuaQuery = manuaQuery.Replace("@RESULT_LINE", result_line);

                    if (caseSensitive)
                        manuaQuery = manuaQuery.Replace("@CASESENSITIVE", "COLLATE Latin1_General_BIN");
                    else
                        manuaQuery = manuaQuery.Replace("@CASESENSITIVE", "");

                    if (wholeWord)
                        search = $"%[^a-z]{texto}[^a-z]%";

                    cmd.CommandText = manuaQuery;
                    cmd.CommandType = global::System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@TERM", search);
                    cmd.Connection = profile.GenioConfiguration.SqlConnection;

                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid codmanua = reader.SafeGetGuid(0);
                            string corpo = reader.SafeGetString(1);

                            ManuaCode man = new ManuaCode(codmanua, Util.ConverToDOSLineEndings(corpo))
                            {
                                Plataform = reader.SafeGetString(2),
                                TipoRotina = reader.SafeGetString(3),
                                Modulo = reader.SafeGetString(4),
                                Parameter = reader.SafeGetString(5),
                                ManualFile = reader.SafeGetString(6),
                                Lang = reader.SafeGetString(7),
                                Order = reader.SafeGetDouble(8),
                                CreatedBy = reader.SafeGetString(9),
                                ChangedBy = reader.SafeGetString(10),
                                CreationDate = reader.SafeGetDateTime(11),
                                LastChangeDate = reader.SafeGetDateTime(12)
                            };
                            man.Code = man.CodeTransformKeyValue();
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
                        profile.GenioConfiguration.CloseConnection();
                    }
                }
            }

            return results;
        }
        #endregion
    }
}
