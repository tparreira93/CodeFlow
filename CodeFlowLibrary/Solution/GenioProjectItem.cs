namespace CodeFlowLibrary.Solution
{
    public class GenioProjectItem
    {
        public string ItemName { get; }
        public string ItemPath { get; }

        public GenioProjectItem(string itemName, string itemPath)
        {
            ItemName = itemName;
            ItemPath = itemPath;
        }
    }
}
