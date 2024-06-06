using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Base.Helper;
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

        private IList<string> LoadAllProtoInPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new List<string>();
            }

            m_protoFullPath = Directory.GetFiles(path, "*.proto", SearchOption.AllDirectories);

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

        private void DrawProtoFiles(in IList<string> protoFiles)
        {
            if (protoFiles.Count <= 0)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(15f, 120f, 300f, position.height - 80f));
            SirenixEditorGUI.BeginVerticalList();
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
            GUILayout.EndArea();

            if (GUI.Button(new Rect(position.width - 200f, position.height - 100f, 100f, 50f), "Generate"))
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
                            arg.Add(Path.GetRelativePath(m_sourceProtoPath, m_protoFullPath[i]));
                        }
                    }

                    _ = StartProcess(m_sourceProtoPath, m_destinationPath, arg.ToArray());
                }
            }
        }

        private async Task StartProcess(string protoSourcePath, string destinationPath, params string[] arg)
        {
            string tempBatchFile = Path.GetTempFileName() + ".bat";
            
            await File.WriteAllTextAsync(tempBatchFile, $"protoc --proto_path={protoSourcePath} --csharp_out={destinationPath} {string.Join(" ", arg)}");
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{tempBatchFile}\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
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