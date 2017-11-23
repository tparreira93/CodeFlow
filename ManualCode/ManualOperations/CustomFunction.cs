using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CodeFlow.Utils;

namespace CodeFlow
{
    [DBName("GENIMPLS")]
    public class CustomFunction : Manual
    {
        public static string BEGIN_MANUAL = "BEGIN_CUSTOMFUNCS_CODFUNCS:";
        public static string END_MANUAL = "END_CUSTOMFUNCS";

        private string nome = "";
        private string lang = "";
        private Guid codfuncs = Guid.Empty;
        private string tiportn = "";
        private double ordem;
        private double largura;
        private double decimais;
        private string resumprm = "";

        [DBName("CODIMPLS")]
        public override Guid CodeId { get => codeID; set => codeID = value; }

        [DBName("CODFUNCS")]
        public Guid Codfuncs { get => codfuncs; set => codfuncs = value; }

        [DBName("NOME")]
        public string Nome { get => nome; set => nome = value; }
        public override string Lang { get => lang; set => lang = value; }
        public override string Tag { get => Nome; }
        public override string TipoCodigo { get => "Custom"; }
        public override string Tipo { get => ""; }
        public string Tiportn { get => tiportn; set => tiportn = value; }
        public double Ordem { get => ordem; set => ordem = value; }
        public double Largura { get => largura; set => largura = value; }
        public double Decimais { get => decimais; set => decimais = value; }
        public string Resumprm { get => resumprm; set => resumprm = value; }

        public CustomFunction()
        {
        }

        public CustomFunction(string code)
        {
            this.Code = code;
        }
        public CustomFunction(string code, string nome)
        {
            this.Code = code;
            this.Nome = nome;
        }
        public CustomFunction(Guid codeID, string code)
        {
            this.CodeId = codeID;
            this.Code = code;
        }
        public override void ShowSVNLog(Profile profile, string systemName)
        {
            try
            {
                OpenSVNLog($"{profile.GenioConfiguration.CheckoutPath + "\\ManualCode\\" + "Functions." + systemName}");
            }
            catch (Exception e)
            {
                throw e;
            }

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
                        string c = CodeTransformValueKey();

                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = String.Format("UPDATE GENIMPLS SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODIMPLS = @CODIMPLS");

                        cmd.Parameters.AddWithValue("@CORPO", c);
                        cmd.Parameters.AddWithValue("@CODIMPLS", this.CodeId);
                        cmd.Parameters.AddWithValue("@OPERMUDA", PackageOperations.ActiveProfile.GenioConfiguration.GenioUser);
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
        public static CustomFunction GetManual(Profile profile, Guid codimpls)
        {
            CustomFunction custom = new CustomFunction();
            SqlDataReader reader = null;
            lock (PackageOperations.lockObject)
            {
                if (!profile.GenioConfiguration.ConnectionIsOpen())
                    profile.GenioConfiguration.OpenConnection();

                if (profile.GenioConfiguration.ConnectionIsOpen())
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
                            custom.Codfuncs = reader.SafeGetGuid(0);
                            custom.CodeId = reader.SafeGetGuid(1);
                            custom.Nome = reader.SafeGetString(2);
                            custom.Code = reader.SafeGetString(3);
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
            }

            return custom;
        }
        public static List<CustomFunction> Search(Profile profile, string texto, int limitBodySize = 80, bool caseSensitive = false, bool wholeWord = false, string plataform = "")
        {
            List<CustomFunction> results = new List<CustomFunction>();
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
                    string customFuncQuery = String.Format("SELECT IMPLS.CODIMPLS, {0}, IMPLS.PLATAFOR, FUNCS.NOME, FUNCS.CODFUNCS," +
                                                            " FUNCS.TIPORTN, FUNCS.ORDEM, FUNCS.LARGURA, FUNCS.DECIMAIS, FUNCS.RESUMPRM, " +
                                                            " IMPLS.OPERCRIA, IMPLS.OPERMUDA, IMPLS.DATACRIA, IMPLS.DATAMUDA" +
                                                            " FROM GENFUNCS FUNCS" +
                                                            " INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS" +
                                                            " WHERE ", tmp);

                    string search = "%" + texto + "%";
                    string whr = "(' ' + CORPO + ' ') LIKE @TERM OR (' ' + NOME + ' ') LIKE @TERM";
                    if (plataform.Length != 0)
                    {
                        customFuncQuery += " (" + whr + ") AND PLATAFOR = @PLATAFORM";
                        cmd.Parameters.AddWithValue("@PLATAFORM", plataform);
                    }
                    else
                        customFuncQuery += whr;

                    if (caseSensitive)
                        customFuncQuery += " COLLATE Latin1_General_BIN;";
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

                            CustomFunction custom = new CustomFunction(codmanua, corpo);
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
            }

            return results;
        }
        public static List<IManual> GetManualCode(string vscode)
        {
            List<IManual> codeList = new List<IManual>();
            string remainig = vscode ?? "";
            do
            {
                IManual m = ParseText<CustomFunction>(BEGIN_MANUAL, END_MANUAL, remainig, out remainig);
                if (m != null)
                    codeList.Add(m);
            } while (remainig.Length != 0);

            return codeList;
        }

        public override string ToString()
        {
            string str = BEGIN_MANUAL + this.CodeId.ToString() + Utils.Util.NewLine;
            str += this.Code;
            str += Utils.Util.NewLine;
            str += END_MANUAL;

            return str;
        }
    }
}
