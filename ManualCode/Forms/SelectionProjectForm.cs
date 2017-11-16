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
        private GenioSolutionProperties solutionProperties;
        public bool Result { get; set; }
        public List<IManual> ExportCode { get; set; }
        public Dictionary<Guid, List<ManuaCode>> ConflictCode { get; set; }

        public ProjectSelectionForm(GenioSolutionProperties solution)
        {
            InitializeComponent();

            solutionProperties = solution;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshSavedFiles()
        {
            foreach (GenioProjectProperties genioProject in solutionProperties.GenioProjects)
            {
                TreeNode node = new TreeNode(genioProject.ProjectName);
                node.Tag = genioProject;

                foreach (GenioProjectItem item in genioProject.ProjectFiles)
                {
                    TreeNode itemNode = new TreeNode(item.ItemName);
                    itemNode.Tag = item;
                    node.Nodes.Add(itemNode);
                }
                treeProjects.Nodes.Add(node);
            }
            chkSavedFiles.Checked = true;
        }
        private void RefreshFullSolution()
        {
            foreach (GenioProjectProperties genioProject in solutionProperties.GenioProjects)
            {
                TreeNode node = new TreeNode(genioProject.ProjectName);
                node.Tag = genioProject;
                treeProjects.Nodes.Add(node);
            }

            chkSavedFiles.Checked = false;
        }

        private void SelectionProjectForm_Load(object sender, EventArgs e)
        {
            if (GenioSolutionProperties.SavedFiles.Count == 0)
            {
                RefreshFullSolution();
                chkSavedFiles.Checked = false;
                chkSavedFiles.Enabled = false;
            }
            else
            {
                RefreshSavedFiles();
                chkSavedFiles.Checked = true;
                chkSavedFiles.Enabled = true;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            List<GenioProjectProperties> projects = new List<GenioProjectProperties>();
            foreach (TreeNode node in treeProjects.Nodes)
            {
                if(node.Tag is GenioProjectProperties)
                {
                    GenioProjectProperties project = (GenioProjectProperties)node.Tag;
                    projects.Add(project);
                }
            }

            if (projects.Count == 0)
                return;

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
    }
}
