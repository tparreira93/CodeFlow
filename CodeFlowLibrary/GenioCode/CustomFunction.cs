using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Helpers;

namespace CodeFlowLibrary.GenioCode
{
    [ManualMatchProvider("BEGIN_CUSTOMFUNCS_CODFUNCS:", "END_CUSTOMFUNCS")]
    public class CustomFunction : Manual
    {
        private string nome = "";
        private string lang = "";
        private Guid codfuncs = Guid.Empty;
        private string tiportn = "";
        private double ordem;
        private double largura;
        private double decimais;
        private string resumprm = "";
        
        public override Guid CodeId { get => codeID; set => codeID = value; }
        
        public Guid Codfuncs { get => codfuncs; set => codfuncs = value; }
        
        public string Nome { get => nome; set => nome = value; }
        public override string Lang { get => lang; set => lang = value; }
        public override string Tag { get => Nome; }
        public override string TipoCodigo { get => "Function"; }
        public override string Tipo { get => ""; }
        public string Tiportn { get => tiportn; set => tiportn = value; }
        public double Ordem { get => ordem; set => ordem = value; }
        public double Largura { get => largura; set => largura = value; }
        public double Decimais { get => decimais; set => decimais = value; }
        public string Resumprm { get => resumprm; set => resumprm = value; }

        public CustomFunction()
        {
        }
        public CustomFunction(Guid codeID, string code)
        {
            this.CodeId = codeID;
            this.Code = code;
        }

        #region DatabaseOperations
        /*
         * Still no support for insert for CustomFunctions
         */
        public override bool Create(Profile profile)
        {
            return false;
        }
        /*
         * Still no support for delete for CustomFunctions
         */
        public override bool Delete(Profile profile)
        {
            return false;
        }
        public override bool Update(Profile profile)
        {
            bool result = false;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    string c = PackageOperations.Instance.ForceDOSLine ? Helpers.Helpers.ConverToDOSLineEndings(CodeTransformKeyValue()) : CodeTransformKeyValue();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = String.Format("UPDATE GENIMPLS SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODIMPLS = @CODIMPLS");

                    cmd.Parameters.AddWithValue("@CORPO", c);
                    cmd.Parameters.AddWithValue("@CODIMPLS", this.CodeId);
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
        public static CustomFunction GetManual(Profile profile, Guid codimpls)
        {
            CustomFunction custom = null;
            SqlDataReader reader = null;

            if (profile.GenioConfiguration.OpenConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = String.Format("SELECT FUNCS.CODFUNCS, IMPLS.CODIMPLS, FUNCS.NOME, IMPLS.CORPO, IMPLS.PLATAFOR, " +
                                                        " FUNCS.TIPORTN, FUNCS.ORDEM, FUNCS.LARGURA, FUNCS.DECIMAIS, FUNCS.RESUMPRM, " +
                                                        " IMPLS.OPERCRIA, IMPLS.OPERMUDA, IMPLS.DATACRIA, IMPLS.DATAMUDA" +
                                                        " FROM GENFUNCS FUNCS" +
                                                        " INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS" +
                                                        " WHERE CODIMPLS = @CODIMPLS"),
                        CommandType = global::System.Data.CommandType.Text,
                        Connection = profile.GenioConfiguration.SqlConnection
                    };
                    cmd.Parameters.AddWithValue("@CODIMPLS", codimpls);

                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        custom = new CustomFunction();
                        custom.Codfuncs = reader.SafeGetGuid(0);
                        custom.CodeId = reader.SafeGetGuid(1);
                        custom.Nome = reader.SafeGetString(2);
                        custom.Code = PackageOperations.Instance.ForceDOSLine ? Helpers.Helpers.ConverToDOSLineEndings(reader.SafeGetString(3)) : reader.SafeGetString(3);
                        custom.Plataform = reader.SafeGetString(4);
                        custom.Tiportn = reader.SafeGetString(5);
                        custom.Ordem = reader.SafeGetDouble(6);
                        custom.Largura = reader.SafeGetDouble(7);
                        custom.Decimais = reader.SafeGetDouble(8);
                        custom.Resumprm = reader.SafeGetString(9);
                        custom.CreatedBy = reader.SafeGetString(10);
                        custom.ChangedBy = reader.SafeGetString(11);
                        custom.CreationDate = reader.SafeGetDateTime(12);
                        custom.LastChangeDate = reader.SafeGetDateTime(13);
                        custom.Code = custom.CodeTransformKeyValue();
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

            return custom;
        }
        public static List<IManual> Search(Profile profile, string texto, bool caseSensitive = false, bool wholeWord = false, string plataform = "")
        {
            List<IManual> results = new List<IManual>();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            if (profile.GenioConfiguration.OpenConnection())
            {
                string customFuncQuery = String.Format("SELECT IMPLS.CODIMPLS, @RESULT_LINE FOUND_LINE, IMPLS.PLATAFOR, FUNCS.NOME, FUNCS.CODFUNCS," +
                                                        " FUNCS.TIPORTN, FUNCS.ORDEM, FUNCS.LARGURA, FUNCS.DECIMAIS, FUNCS.RESUMPRM, " +
                                                        " IMPLS.OPERCRIA, IMPLS.OPERMUDA, IMPLS.DATACRIA, IMPLS.DATAMUDA" +
                                                        " FROM GENFUNCS FUNCS" +
                                                        " INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS" +
                                                        " WHERE @WHERE @CASESENSITIVE");


                string result_line = "LEFT(RIGHT(LEFT(CORPO, COALESCE(NULLIF(@AFTER_NEWLINE, 0), LEN(CORPO))), COALESCE(NULLIF(@BEFORE_NEWLINE, 0), LEN(CORPO))), 400)";
                string after_newline = "CHARINDEX(CHAR(13), CORPO, PATINDEX(@TERM, CORPO @CASESENSITIVE) + 2)";
                string before_newline = "CHARINDEX(CHAR(13), REVERSE(LEFT(CORPO, @AFTER_NEWLINE)), 2)";
                result_line =result_line.Replace("@BEFORE_NEWLINE", before_newline).Replace("@AFTER_NEWLINE", after_newline);
                customFuncQuery = customFuncQuery.Replace("@RESULT_LINE", result_line);

                string search = "%" + texto + "%";
                string whr = "(' ' + CORPO + ' ') LIKE @TERM OR (' ' + NOME + ' ') LIKE @TERM";
                if (plataform.Length != 0)
                {
                    whr = "(" + whr + ") AND PLATAFOR = @PLATAFORM";
                    cmd.Parameters.AddWithValue("@PLATAFORM", plataform);
                }

                customFuncQuery = customFuncQuery.Replace("@WHERE", whr);

                customFuncQuery = customFuncQuery.Replace("@CASESENSITIVE", caseSensitive ? "COLLATE Latin1_General_BIN" : "");

                if (wholeWord)
                    search = $"%[^a-z]{texto}[^a-z]%";

                cmd.CommandText = customFuncQuery;
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

                        CustomFunction custom = new CustomFunction(codmanua, PackageOperations.Instance.ForceDOSLine ? Helpers.Helpers.ConverToDOSLineEndings(corpo) : corpo);
                        custom.Plataform = reader.SafeGetString(2);
                        custom.Nome = reader.SafeGetString(3);
                        custom.Codfuncs = reader.SafeGetGuid(4);
                        custom.Tiportn = reader.SafeGetString(5);
                        custom.Ordem = reader.SafeGetDouble(6);
                        custom.Largura = reader.SafeGetDouble(7);
                        custom.Decimais = reader.SafeGetDouble(8);
                        custom.Resumprm = reader.SafeGetString(9);
                        custom.CreatedBy = reader.SafeGetString(10);
                        custom.ChangedBy = reader.SafeGetString(11);
                        custom.CreationDate = reader.SafeGetDateTime(12);
                        custom.LastChangeDate = reader.SafeGetDateTime(13);

                        custom.Code = custom.CodeTransformKeyValue();
                        results.Add(custom);
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

        #region LocalOperations
        public override string GetFilePath(Profile profile)
        {
            return $"{profile.GenioConfiguration.CheckoutPath + "\\ManualCode\\" + "Functions." + profile.GenioConfiguration.SystemInitials}";
        }
        public override bool MatchAndFix(string upperLine)
        {
            if (PackageOperations.Instance.FixIndexes)
                Code = FixSetCurrentIndex(Code);
            return true;
        }
        public override string ToString()
        {
            string str = GetMatchProvider().MatchBeginnig + this.CodeId.ToString() + Helpers.Helpers.NewLine;
            str += this.Code;
            str += Helpers.Helpers.NewLine;
            str += GetMatchProvider().MatchEnd;

            return str;
        }
        public override string FormatCode(string extension)
        {
            string str = FormatComment(extension, GetMatchProvider().MatchBeginnig + this.CodeId.ToString()) + Helpers.Helpers.NewLine;
            str += this.Code;
            str += Helpers.Helpers.NewLine;
            str += FormatComment(extension, GetMatchProvider().MatchEnd);

            return str;
        }
        public override string GetCodeExtension(Profile p)
        {
            return Plataform;
        }
        #endregion
    }
}
