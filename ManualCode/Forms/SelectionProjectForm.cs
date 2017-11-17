using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlow.SolutionOperations;

namespace CodeFlow.Forms
{
    public partial class ProjectSelectionForm : Form
    {
        private List<GenioProjectProperties> savedFiles;
        private GenioSolutionProperties solution;
        public bool Result { get; set; }
        public List<IManual> ExportCode { get; set; }
        public Dictionary<Guid, List<ManuaCode>> ConflictCode { get; set; }

        public ProjectSelectionForm(List<GenioProjectProperties> saved)
        {
            InitializeComponent();
            solution = GenioSolutionProperties.ParseSolution(PackageOperations.DTE, true);
            savedFiles = saved;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshSavedFiles()
        {
            treeProjects.Nodes.Clear();
            foreach (GenioProjectProperties genioProject in savedFiles)
            {
                TreeNode node = new TreeNode(genioProject.ProjectName);
                node.Tag = genioProject;
                treeProjects.Nodes.Add(node);

                foreach (GenioProjectItem item in genioProject.ProjectFiles)
                {
                    TreeNode itemNode = new TreeNode(item.ItemName);
                    itemNode.Tag = item;
                    node.Nodes.Add(itemNode);
                }
            }
        }
        private void RefreshFullSolution()
        {
            treeProjects.Nodes.Clear();
            foreach (GenioProjectProperties genioProject in solution.GenioProjects)
            {
                TreeNode node = new TreeNode(genioProject.ProjectName);
                node.Tag = genioProject;
                treeProjects.Nodes.Add(node);
            }
        }

        private void SelectionProjectForm_Load(object sender, EventArgs e)
        {
            if (GenioSolutionProperties.SavedFiles.Count == 0)
            {
                chkSavedFiles.Checked = false;
                chkSavedFiles.Enabled = false;
            }
            else
            {
                chkSavedFiles.Checked = true;
                chkSavedFiles.Enabled = true;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            List<GenioProjectProperties> projects = new List<GenioProjectProperties>();
            foreach (TreeNode node in treeProjects.Nodes)
            {
                if(node.Tag is GenioProjectProperties && !node.Checked)
                {
                    GenioProjectProperties proj = node.Tag as GenioProjectProperties;
                    List<GenioProjectItem> items = new List<GenioProjectItem>();

                    foreach (TreeNode level2 in node.Nodes)
                        if (level2.Tag is GenioProjectItem && level2.Checked)
                            items.Add(level2.Tag as GenioProjectItem);

                    if (items.Count > 0)
                        projects.Add(new GenioProjectProperties(proj.GenioProject, items));
                }
                else if (node.Tag is GenioProjectProperties && node.Checked)
                {
                    GenioProjectProperties project = (GenioProjectProperties)node.Tag;
                    projects.Add(project);
                }
            }

            ProjectsAnalyzer analyzer = new ProjectsAnalyzer(projects);
            analyzer.Analyze();
            Result = true;
            ExportCode = analyzer.ToExport;
            ConflictCode = analyzer.ManualConflict;
            this.Close();
        }

        private void chkSavedFiles_CheckedChanged(object sender, EventArgs e)
        {
            if(chkSavedFiles.Checked)
                RefreshSavedFiles();
            else
                RefreshFullSolution();
        }

        private void treeProjects_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }

        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }
    }
}
