using System;
using System.Collections.Generic;

namespace CodeFlow
{
    public interface IManual
    {
        Guid CodeId { get; set; }
        string Code { get; set; }
        string Plataform { get; set; }
        string GenioUser { get; set; }
        string Lang { get; set; }
        string Tag { get; set; }
        DateTime LastChangeDate { get; set; }
        DateTime CreationDate { get; set; }

        string ShortCode { get; }

        bool Update(Profile profile);

        string OpenManual(EnvDTE80.DTE2 dte, Profile p);

        void CodeTransformKeyValue();

        void CodeTransformValueKey();

        void ShowSVNLog(Profile profile, string systemName);
    }
}
