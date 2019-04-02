using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Text;
using CodeFlowLibrary;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Helpers;

namespace CodeFlowLibrary.GenioCode
{
    [ManualMatchProvider("BEGIN_MANUALCODE_CODMANUA:", "END_MANUALCODE")]
    public class ManuaCode : Manual
    {
        private string tipo = "";
        private string modulo = "";
        private Guid codmodul = Guid.Empty;
        private string parameter = "";
        private string lang = "";
        private string feature = "";
        private Guid codfeature = Guid.Empty;
        private string file = "";
        private double order = 0;
        private int system = 0;
        private int inhib = 0;

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
        
        public override Guid CodeId { get => codeID; set => codeID = value; }
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

        public override string GetFilePath(Profile profile)
        {
            return $"{profile.GenioConfiguration.CheckoutPath + "\\ManualCode\\" + "MAN" + this.Plataform + this.ManualFile + "." + profile.GenioConfiguration.SystemInitials}";
        }
        public override bool MatchAndFix(string upperLine)
        {
            Match match = reg.Match(upperLine);
            if (match.Success)
            {
                Plataform = match.Groups[2].Value;
                TipoRotina = match.Groups[5].Value;
                Modulo = match.Groups[8].Value;
                Parameter = match.Groups[11].Value;
                ManualFile = match.Groups[14].Value;
                Double.TryParse(match.Groups[17].Value, out double tmp);
                Order = tmp;
            }
            if(PackageOperations.Instance.FixIndexes)
                Code = FixSetCurrentIndex(Code);

            return match.Success;
        }
        public override string FormatCode(string extension)
        {
            string str = FormatComment(extension, GetMatchProvider().MatchBeginnig + this.CodeId.ToString()) + Helpers.NewLine;
            str += this.Code;
            str += Helpers.NewLine;
            str += FormatComment(extension, GetMatchProvider().MatchEnd);

            return str;
        }
        public override string ToString()
        {
            string str = GetMatchProvider().MatchBeginnig + this.CodeId.ToString() + Helpers.NewLine;
            str += this.Code;
            str += Helpers.NewLine;
            str += GetMatchProvider().MatchEnd;

            return str;
        }
        #endregion

        #region DatabaseOperations
        public override bool Update(Profile profile)
        {
            bool result = false;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    string c = PackageOperations.Instance.ForceDOSLine ? Helpers.ConverToDOSLineEndings(CodeTransformKeyValue()) : Code;

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format("UPDATE GENMANUA SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODMANUA = @CODMANUA");
                    cmd.Parameters.AddWithValue("@CORPO", c);
                    cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
                    cmd.Parameters.AddWithValue("@OPERMUDA", profile.GenioConfiguration.GenioUser);
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

            return result;
        }
        public override bool Create(Profile profile)
        {
            bool result = false;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    if(CodeId.Equals(Guid.Empty))
                        this.CodeId = Guid.NewGuid();

                    string c = PackageOperations.Instance.ForceDOSLine ? Helpers.ConverToDOSLineEndings(CodeTransformValueKey()) : CodeTransformValueKey();

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
            return result;
        }
        public override bool Delete(Profile profile)
        {
            bool result = false;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format("DELETE FROM GENMANUA WHERE CODMANUA = @CODMANUA");

                    cmd.Parameters.AddWithValue("@CODMANUA", this.CodeId);
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
            return result;
        }
        public static ManuaCode GetManual(Profile profile, Guid codmanua)
        {
            ManuaCode man = null;
            SqlDataReader reader = null;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format("SELECT CODMANUA, CORPO, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM, OPERCRIA, OPERMUDA, DATACRIA, DATAMUDA FROM GENMANUA WHERE CODMANUA = @CODMANUA");
                    cmd.Parameters.AddWithValue("@CODMANUA", codmanua);
                    cmd.CommandType = global::System.Data.CommandType.Text;
                    cmd.Connection = profile.GenioConfiguration.SqlConnection;

                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        man = new ManuaCode("");
                        man.CodeId = reader.SafeGetGuid(0);
                        man.Code = PackageOperations.Instance.ForceDOSLine ? Helpers.ConverToDOSLineEndings(reader.SafeGetString(1)) : reader.SafeGetString(1);
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

            return man;
        }
        public static List<IManual> Search(Profile profile, string texto, bool caseSensitive = false, bool wholeWord = false, string plataform = "")
        {
            List<IManual> results = new List<IManual>();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            if (profile.GenioConfiguration.OpenConnection())
            {
                string manuaQuery = String.Format("SELECT CODMANUA, @RESULT_LINE FOUND_LINE, PLATAFOR, TIPO, MODULO, PARAMETR, FICHEIRO, LANG, ORDEM, OPERCRIA, OPERMUDA, DATACRIA, DATAMUDA "
                                                + "FROM GENMANUA WHERE ((' ' + CORPO + ' ') LIKE @TERM OR (' ' + PARAMETR + ' ') LIKE @TERM) @CASESENSITIVE");

                if (plataform.Length != 0)
                {
                    manuaQuery += " AND PLATAFOR = @PLATAFORM";
                    cmd.Parameters.AddWithValue("@PLATAFORM", plataform);
                }

                string search = "%" + texto + "%";
                string resultLine = "LEFT(RIGHT(LEFT(CORPO, COALESCE(NULLIF(@AFTER_NEWLINE, 0), LEN(CORPO))), COALESCE(NULLIF(@BEFORE_NEWLINE, 0), LEN(CORPO))), 400)";
                string afterNewline = "CHARINDEX(CHAR(13), CORPO, PATINDEX(@TERM, CORPO @CASESENSITIVE) + 2)";
                string beforeNewline = "CHARINDEX(CHAR(13), REVERSE(LEFT(CORPO, @AFTER_NEWLINE)), 2)";
                resultLine = resultLine.Replace("@BEFORE_NEWLINE", beforeNewline).Replace("@AFTER_NEWLINE", afterNewline);
                manuaQuery = manuaQuery.Replace("@RESULT_LINE", resultLine);

                manuaQuery = manuaQuery.Replace("@CASESENSITIVE", caseSensitive ? "COLLATE Latin1_General_BIN" : "");

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

                        ManuaCode man = new ManuaCode(codmanua, PackageOperations.Instance.ForceDOSLine ? Helpers.ConverToDOSLineEndings(corpo) : corpo)
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

            return results;
        }
        #endregion
    }
}
