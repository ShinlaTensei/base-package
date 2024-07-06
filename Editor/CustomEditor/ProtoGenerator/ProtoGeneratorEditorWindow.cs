using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Base.Helper;
using NUnit.Framework;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Debug = UnityEngine.Debug;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Base.Editor
{
    public class ProtoGeneratorEditorWindow : OdinEditorWindow
    {
        [MenuItem("PFramework/Proto Generator")]
        private static void OpenWindow()
        {
            m_window = GetWindow<ProtoGeneratorEditorWindow>("Proto Generator");
            m_window.minSize = new Vector2(1000f, 200f);
            m_window.maxSize = new Vector2(1000f, 1000f);
        }

        private static ProtoGeneratorEditorWindow m_window;

        private string m_sourceProtoPath;
        private string m_destinationPath;
        private IList<string> m_protoFullPath = new List<string>();
        private IList<bool> m_selectedProtoToGenerate = new List<bool>();

        protected override void OnImGUI()
        {
            base.OnImGUI();

            DrawPathSelectable();
            IList<string> protoFilename = LoadAllProtoInPath(m_sourceProtoPath);
            DrawProtoFiles(protoFilename);
        }

        private void DrawPathSelectable()
        {
            Rect sourcePathRect = new Rect(10, 15f, position.width - 25f, 20f);
            m_sourceProtoPath =
                SirenixEditorFields.FolderPathField(sourcePathRect, new GUIContent("Source Proto"), m_sourceProtoPath, "Assets", true, true);
            Rect destinationPathRect = new Rect(10f, 50f, position.width - 25f, 20f);
            m_destinationPath =
                SirenixEditorFields.FolderPathField(destinationPathRect, new GUIContent("Destination"), m_destinationPath, "Assets", true, true);
        }

        private IList<string> LoadAllProtoInPath(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                return new List<string>();
            }

            m_protoFullPath = Directory.GetFiles(sourcePath, "*.proto", SearchOption.AllDirectories);
            m_protoFullPath = m_protoFullPath.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();

            if (m_protoFullPath.Count <= 0)
            {
                return new List<string>();
            }

            IList<string> protoFileName = new List<string>();
            for (int i = 0; i < m_protoFullPath.Count; i++)
            {
                protoFileName.Add(Path.GetRelativePath(m_sourceProtoPath, m_protoFullPath[i]));
            }

            return protoFileName;
        }

        private IList<string> LoadAllDestinationInPath(string destinationPath)
        {
            if (string.IsNullOrEmpty(destinationPath))
            {
                return new List<string>();
            }

            IList<string> destinationName = new List<string>();
            destinationName = Directory.GetDirectories(destinationPath).ToList();
            return destinationName;
        }

        private void DrawListProto(IList<string> protoFiles, string searchStr)
        {
            if (!string.IsNullOrEmpty(searchStr))
            {
                protoFiles = protoFiles.Where(str => str.Contains(searchStr)).ToList();
            }

            SirenixEditorGUI.BeginVerticalList(true, true);
            for (int i = 0; i < protoFiles.Count; i++)
            {
                SirenixEditorGUI.BeginListItem();
                SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);
                GUILayout.Label(protoFiles[i], GUILayout.Width(250f));
                GUILayout.FlexibleSpace();
                if (m_selectedProtoToGenerate.Count <= i)
                {
                    m_selectedProtoToGenerate.Insert(i, false);
                }

                m_selectedProtoToGenerate[i] = EditorGUILayout.Toggle(m_selectedProtoToGenerate[i]);
                SirenixEditorGUI.EndHorizontalPropertyLayout();
                SirenixEditorGUI.EndListItem();
            }

            SirenixEditorGUI.EndVerticalList();
        }

        private Vector2 m_scrollPosition;
        private string m_searchString = string.Empty;

        private void DrawProtoFiles(IList<string> protoFiles)
        {
            if (protoFiles.Count <= 0)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(15f, 120f, 300f, position.height - 150f));
            SirenixEditorGUI.BeginToolbarBoxHeader();
            m_searchString = SirenixEditorGUI.ToolbarSearchField(m_searchString);
            SirenixEditorGUI.EndToolbarBoxHeader();
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, true, GUILayout.MaxHeight(position.height - 200f));
            DrawListProto(protoFiles, m_searchString);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (GUI.Button(new Rect(position.width - 200f, position.height - 100f, 100f, 50f), "Generate"))
            {
                GenerateProto();
            }
        }

        private async Task GenerateProto()
        {
            if (string.IsNullOrEmpty(m_destinationPath))
            {
                return;
            }

            if (m_selectedProtoToGenerate.Count > 0)
            {
                IList<string> arg = new List<string>();
                for (int i = 0; i < m_selectedProtoToGenerate.Count; i++)
                {
                    if (m_selectedProtoToGenerate[i])
                    {
                        string relativePath = Path.GetRelativePath(m_sourceProtoPath, m_protoFullPath[i]);
                        if (relativePath.Contains(Path.DirectorySeparatorChar))
                        {
                            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        }

                        arg.Add(relativePath);
                    }
                }

                IDictionary<string, IList<string>> mapSubfoldersProto = new Dictionary<string, IList<string>>();
                foreach (var protoPath in arg)
                {
                    string[] split = protoPath.Split(Path.AltDirectorySeparatorChar);
                    string subFolder = string.Join(Path.AltDirectorySeparatorChar, split, 0, split.Length - 1);
                    if (!Directory.Exists(Path.Combine(m_destinationPath, subFolder)))
                    {
                        Directory.CreateDirectory(Path.Combine(m_destinationPath, subFolder));
                    }

                    if (!mapSubfoldersProto.ContainsKey(subFolder))
                    {
                        mapSubfoldersProto[subFolder] = new List<string>();
                    }

                    if (mapSubfoldersProto.TryGetValue(subFolder, out IList<string> listProto))
                    {
                        listProto.AddIfNotContains(protoPath);
                    }
                }

                foreach (var kvp in mapSubfoldersProto)
                {
                    await StartProcess(m_sourceProtoPath, Path.Combine(m_destinationPath, kvp.Key), kvp.Value.ToArray());
                }
            }
        }

        private async Task StartProcess(string protoSourcePath, string destinationPath, params string[] arg)
        {
            string tempBatchFile = Path.GetTempFileName() + ".bat";

            await File.WriteAllTextAsync(tempBatchFile,
                $"protoc --proto_path={protoSourcePath} --csharp_out={destinationPath} {string.Join(" ", arg)}");

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{tempBatchFile}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                LoadUserProfile = true,
                WorkingDirectory = PathUtility.GetProjectPath()
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.Log($"Proto Generate Output: {e.Data}");
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.LogError($"Proto Generate Error: {e.Data}");
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
        }
    }
}