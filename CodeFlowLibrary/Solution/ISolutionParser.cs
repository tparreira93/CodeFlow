using System.Threading.Tasks;

namespace CodeFlowLibrary.Solution
{
    public interface ISolutionParser
    {
        Task<GenioSolutionProperties> ParseAsync();

        Task ChangeToolset2008Async();
    }
}
