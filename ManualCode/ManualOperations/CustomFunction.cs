using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

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
            this.CodeId = CodeId;
            this.Code = code;
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
                        SqlCommand cmd = new SqlCommand
                        {
                            CommandText = String.Format("UPDATE GENIMPLS SET CORPO = @CORPO, DATAMUDA = GETDATE(), OPERMUDA = @OPERMUDA WHERE CODIMPLS = @CODIMPLS")
                        };

                        cmd.Parameters.AddWithValue("@CORPO", this.Code);
                        cmd.Parameters.AddWithValue("@CODIMPLS", this.CodeId);
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

        public new static CustomFunction GetManual(Guid codimpls, Profile profile)
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
                            CommandText = String.Format("SELECT FUNCS.CODFUNCS, IMPLS.CODIMPLS, FUNCS.NOME, IMPLS.CORPO, IMPLS.PLATAFOR, IMPLS.*" +
                                                            " FROM GENFUNCS FUNCS" +
                                                            " INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS" +
                                                            " WHERE CODIMPLS = {0}", codimpls),
                            CommandType = global::System.Data.CommandType.Text,
                            Connection = profile.GenioConfiguration.SqlConnection
                        };

                        reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            custom.Codfuncs = reader.GetGuid(0);
                            custom.CodeId = reader.GetGuid(1);
                            custom.Nome = reader.GetString(2);
                            custom.Code = reader.GetString(3);
                            custom.Plataform = reader.GetString(4);
                            custom.CodeTransformKeyValue();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return custom;
        }

        public void CompareDB(Profile profile)
        {
            CustomFunction bd = GetManual(CodeId, profile);

            Compare(this, bd);
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

        public static List<CustomFunction> Search(Profile profile, string texto, int limitBodySize = 80, bool caseSensitive = false, bool wholeWord = false)
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
                    string customFuncQuery = String.Format("SELECT IMPLS.CODIMPLS, {0}, PLATAFOR, FUNCS.NOME FROM GENFUNCS FUNCS INNER JOIN GENIMPLS IMPLS ON IMPLS.CODFUNCS = FUNCS.CODFUNCS " +
                                                                    "WHERE CORPO LIKE @TERM OR NOME LIKE @TERM", tmp);

                    string search = "%" + texto + "%";
                    if (caseSensitive)
                        customFuncQuery += " COLLATE Latin1_General_BIN;";
                    if (wholeWord)
                        search = $"%[ ]{texto}[ ]%";

                    cmd.CommandText = customFuncQuery;
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

                            CustomFunction man = new CustomFunction(codmanua, corpo);

                            man.Plataform = reader.GetString(2);
                            man.Nome = reader.GetString(3);
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

        public static List<IManual> GetManualCode(string vscode)
        {
            List<IManual> codeList = new List<IManual>();
            string remainig = vscode ?? "";
            do
            {
                IManual m = ParseText<ManuaCode>(BEGIN_MANUAL, END_MANUAL, remainig, out remainig);
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
