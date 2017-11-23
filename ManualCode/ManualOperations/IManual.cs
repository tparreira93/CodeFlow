using System;
using System.Collections.Generic;

namespace CodeFlow
{
    public interface IManual
    {
        Guid CodeId { get; set; }
        string Code { get; set; }
        string Plataform { get; set; }
        string Lang { get; set; }
        string Tag { get; }
        DateTime LastChangeDate { get; set; }
        DateTime CreationDate { get; set; }
        string CreatedBy { get; set; }
        string ChangedBy { get; set; }
        string TipoCodigo { get; }
        string Tipo { get; }

        string GetCodeExtension(Profile p);

        string OneLineCode { get; }

        bool Update(Profile profile);

        string CodeTransformKeyValue();

        string CodeTransformValueKey();

        void ShowSVNLog(Profile profile, string systemName);

        string ShortOneLineCode(int max = 100);
        void CompareDB(Profile profile);
    }
}
