using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;

namespace consist.RapidEntity.Customizations.IDEHelpers
{
    public static class SolutionExplorerHelper
    {
        public const string DataSourceFormat = "CF_VSSTGPROJECTITEMS";

        private static EnvDTE80.DTE2 activeIDE;

        public static void SetActiveIDE(ClassDiagram classDiagram)
        {
            activeIDE = classDiagram.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
        }

        public static IEnumerable<Project> GetSolutionProjects()
        {
            EnvDTE.Solution solution = activeIDE.Solution;

            foreach ( EnvDTE.Project project in solution.Projects )
            {
                if ( project.Object is SolutionFolder )
                {
                    foreach (Project current in GetSolutionProjects( project ))
                    {
                        yield return current;
                    }
                }
                else if( project is Project )
                {
                    yield return project;
                }
            }
        }

        /// <summary>
        /// Solution folders are not visible on the file system.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static IEnumerable<Project> GetSolutionProjects( Project mainProject )
        {            
            foreach( ProjectItem item in mainProject.ProjectItems )
            {
                if ( item.Object is SolutionFolder )
                {
                    foreach (Project project in GetSolutionProjects((Project)item.Object))
                    {
                        yield return project;
                    }
                }
                else if ( item.Object is Project )
                {
                    yield return item.Object as Project;
                }
            }
        }

        public static string GetSolutionProjectsBySolutionItemPath(string directoryPath)
        {
            foreach (Project project in GetSolutionProjects())
            {
                if(directoryPath.Contains(Path.GetDirectoryName( project.FullName )))
                    return project.FullName;
            }

            return string.Empty;
        }

        public static string GetProjectNameByFullName(string fullName)
        {
            foreach (EnvDTE.Project project in GetSolutionProjects())
            {
                if (project.FullName == fullName)
                    return project.Name;
            }

            return string.Empty;
        }

        public static string GetProjectDefaultNamespace(string fullName)
        {
            foreach (EnvDTE.Project project in GetSolutionProjects())
            {
                if (project.FullName == fullName)
                {
                    return project.Properties.Item("DefaultNamespace").Value.ToString();
                }
            }

            return string.Empty;
        }

        public static void AddDirectory(string projectName, string folderName)
        {
            Project project = GetSolutionProject(projectName);
            ProjectItem item = GetProjectItemByNameItem(project, folderName);

            if(null == item)
                project.ProjectItems.AddFolder(folderName, Constants.vsProjectItemKindPhysicalFolder);
        }

        public static void AddFromFileCopy(string projectName, string folderName, DirectoryInfo directoryInfo)
        {
            Project item = GetSolutionProject( projectName );

            foreach ( FileInfo file in directoryInfo.GetFiles() )
            {
                item.ProjectItems.AddFromFile(file.FullName);
            }            
        }

        public static void AddFromFileBasedOnFileExtension( string projectName , string folderName , DirectoryInfo directoryInfo , string extension , bool shouldEmbedd )
        {
            Project item = GetSolutionProject( projectName );

            foreach( FileInfo file in directoryInfo.GetFiles() )
            {
                if (file.Extension.Contains(extension))
                {
                    ProjectItem projectItem = item.ProjectItems.AddFromFile( file.FullName );

                    if (shouldEmbedd)
                    {
                        projectItem.Properties.Item("BuildAction").Value = 3;
                    }
                }
            }
        }

        public static void AddToProject(string projectName, string folderName, string fromDirectory)
        {
            ProjectItem item = GetProjectItemByNameItem(GetSolutionProject(projectName), folderName);
            item.ProjectItems.AddFromDirectory(fromDirectory);
        }

        public static Project GetSolutionProject(string projectName)
        {
            foreach (EnvDTE.Project project in GetSolutionProjects())
            {
                if (project.Name == projectName)
                    return project;
            }

            return null;
        }

        private static ProjectItem GetProjectItemByNameItem(Project project, string itemName)
        {
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.Name == itemName)
                {
                    return item;
                }
            }

            return null;
        }

        public static void ViewCode(string projectName, string itemPath)
        {
            Project project = GetSolutionProject(projectName);

            if (project.IsNull())
                return;

            foreach (ProjectItem item in project.ProjectItems)
            {
                if (IsExactProjectItem(item, itemPath))
                {
                    item.Open(Constants.vsViewKindCode);
                }

                ProjectItem innerItem = GetProjectItem(item, itemPath);

                if (!innerItem.IsNull())
                {
                   Window window = innerItem.Open(Constants.vsViewKindCode);
                   window.Activate();
                }
            }
        }

        public static ProjectItem GetProjectItemByFullPath(Project project, string path)
        {            
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (IsExactProjectItem(item, path))
                    return item;

                ProjectItem innerItem = GetProjectItem(item, path);

                if (!innerItem.IsNull())
                {
                    return innerItem;
                }
            }

            return null;
        }

        private static ProjectItem GetProjectItem(ProjectItem projectItem, string fullFilePath)
        {
            foreach (ProjectItem item in projectItem.ProjectItems)
            {
                if (IsExactProjectItem(item, fullFilePath))
                    return item;
                else
                {
                    ProjectItem innerItem = GetProjectItem(item, fullFilePath);
                    if (!innerItem.IsNull())
                        return innerItem;
                }
            }

            return null;
        }

        public static bool IsExactProjectItem(ProjectItem item, string projectItemFullPath)
        {
            foreach (Property property in item.Properties)
            {
                if (property.Name == "FullPath")
                {
                    if (property.Value.ToString() == projectItemFullPath)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static ProjectItem GetProjectItem(string projectName, string projectItemName)
        {
            Project project = GetSolutionProject(projectName);
            return GetProjectItemByNameItem(project, projectItemName);            
        }

        public static void DeleteFolderItems(string projectName, string folderName)
        {
            Project project = GetSolutionProject(projectName);
            ProjectItem projectItem = GetProjectItemByNameItem(project, folderName);

            if (null != projectItem)
            {
                foreach (ProjectItem item in projectItem.ProjectItems)
                {
                    item.Delete();
                    //item.Remove();
                }
            }
        }

        public static void RefereshProject(string projectName)
        {
            EnvDTE.Solution solution = activeIDE.Solution;

            foreach (EnvDTE.Project project in GetSolutionProjects())
            {
                if (project.Name == projectName)
                {
                    //project.DTE.ExecuteCommand("View.Refresh");
                }
            }
        }

        private static string MakeValidName(string typeName)
        {
            return Regex.Replace(typeName, "[^a-zA-Z]", "");
        }
    }
}
