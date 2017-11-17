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
        private string tipo = "";
        private Guid codfuncs = Guid.Empty;

        [DBName("CODIMPLS")]
        public override Guid CodeId { get => codeID; set => codeID = value; }

        [DBName("CODFUNCS")]
        public Guid Codfuncs { get => codfuncs; set => codfuncs = value; }

        [DBName("NOME")]
        public string Nome { get => nome; set => nome = value; }
        public override string Lang { get => lang; set => lang = value; }
        public override string Tag { get => tipo; set => tipo = value; }

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

        public static List<IManual> GetManualCode(string vscode)
        {
            return ParseManual(BEGIN_MANUAL, END_MANUAL, vscode);
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
