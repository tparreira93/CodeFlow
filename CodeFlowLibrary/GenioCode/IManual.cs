using System;
using CodeFlowLibrary.Genio;

namespace CodeFlowLibrary.GenioCode
{
    /// <summary>
    /// When using this interface, it is mandatory to use attribute <seealso cref="ManualMatchProvider"/>, 
    /// otherwise <seealso cref="VSCodeManualMatcher"/> will not parse tags and they will not be higlighted
    /// </summary>
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
        string LocalFileName { get; }
        string FullFileName { get; }
        string GetCodeExtension(Profile p);
        string OneLineCode { get; }
        ManualMatch LocalMatch { get; set; }

        bool Create(Profile profile);

        bool Update(Profile profile);

        bool Delete(Profile profile);

        string CodeTransformKeyValue();

        string CodeTransformValueKey();

        void ShowSVNLog(Profile profile);
        void Blame(Profile profile);

        string ShortOneLineCode(int max = 100);
        void CompareDB(Profile profile);
        IManual MergeDB(Profile profile);
        string FormatCode(string extension);
        bool MatchAndFix(string upperLine);

        string GetFilePath(Profile profile);
    }
}
