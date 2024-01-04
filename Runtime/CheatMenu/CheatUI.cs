#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: CheatUI.cs
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Base.Cheat;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace Base.Cheat
{
    public class CheatUI : UIView
    {
        [SerializeField] private TMP_Dropdown                  m_dropdown;
        [SerializeField] private ParameterItemDisplayContainer m_parameterItemDisplayContainer;

        private CheatService     m_cheatService;
        private InputHandler     m_inputHandler;
        private int              m_currentIndex  = 0;

        private const string AssemblyName = "Assembly-CSharp";
        
        protected override void Awake()
        {
            m_cheatService = ServiceLocator.Get<CheatService>();
            m_inputHandler = ServiceLocator.Get<InputHandler>();

            var clickStream = Observable.EveryUpdate().Where(_ => m_inputHandler.GetTouchBegan());
            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250))).Where(xs => xs.Count >= 2)
                       .Subscribe(_ => Show()).AddTo(this);
            m_parameterItemDisplayContainer.Initialize();
        }

        protected override void Start()
        {
            m_dropdown.onValueChanged.AddListener(OnValueChanged);
            TaskRunner.Start(StartInitializeService());

            Application.logMessageReceived += OnLogCallback;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Application.logMessageReceived -= OnLogCallback;
        }

        private void OnLogCallback(string condition, string stackTrace, LogType type)
        {
            if (type is LogType.Exception or LogType.Error)
            {
                int a = 0;
            }
        }

        private async Task StartInitializeService(params string[] assemblyNames)
        {
            string[] assemblies = new string[] { AssemblyName, "Unity.BaseFramework" };
            if (assemblyNames.Length > 0)
            {
                for (int i = 0; i < assemblyNames.Length; ++i)
                {
                    assemblies.AddIfNotContains(assemblyNames[i]);
                }
            }
            m_cheatService.Init(assemblies);
            await UniTask.WaitUntil(() => m_cheatService.IsInitialize, cancellationToken: this.GetCancellationTokenOnDestroy());

            DrawCheatCommand();
        }

        private void DrawCheatCommand()
        {
            List<ICheatCommand>           commands    = m_cheatService.GetCheatCommands();
            List<TMP_Dropdown.OptionData> listOptions = new List<TMP_Dropdown.OptionData>();
            listOptions.Add(new TMP_Dropdown.OptionData("<none>"));
            for (int i = 0; i < commands.Count; ++i)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                                                     {
                                                             text = $"[{commands[i].Category}] {commands[i].Name}"
                                                     };
                listOptions.Add(optionData);
            }

            m_currentIndex = 0;
            m_dropdown.ClearOptions();
            m_dropdown.AddOptions(listOptions);
            m_dropdown.SetValueWithoutNotify(m_currentIndex);
        }

        private void DrawMethodParameter(MethodCheatCommand command)
        {
            ParameterInfo[] parameterInfos = command.GetParameters();
            if (parameterInfos.Length <= 0) return;

            m_parameterItemDisplayContainer.PopulateParameterDisplay(parameterInfos);
        }
        

        private void OnValueChanged(int index)
        {
            try
            {
                m_currentIndex = index;

                if (m_currentIndex <= 0)
                {
                    // Return due to index 0 is <none> by default
                    m_parameterItemDisplayContainer.ResetDisplay();
                    return;
                }

                MethodCheatCommand command = m_cheatService.GetCheatCommands()[m_currentIndex - 1] as MethodCheatCommand;

                if (command is null)
                {
                    PDebug.ErrorFormat("[Cheat] Wrong type of Cheat command at Index {0}", index);
                    return;
                }
                
                DrawMethodParameter(command);
            }
            catch (IndexOutOfRangeException e)
            {
                PDebug.ErrorFormat("[Cheat] Index out of range: The selected index is out of range of CheatCommand list");
                m_currentIndex = 0;
            }
        }

        public override void Show()
        {
            base.Show();

            m_dropdown.value = m_currentIndex = 0;
            m_parameterItemDisplayContainer.ResetDisplay();
        }

        public override void Populate<T>(T viewData)
        {
            return;
        }

        public void OnExecuteCheatCommand()
        {
            List<ICheatCommand> commands = m_cheatService.GetCheatCommands();
            if (m_currentIndex - 1 >= commands.Count || m_currentIndex <= 0)
            {
                PDebug.ErrorFormat("[Cheat] Index out of range when calling execute cheat");
                return;
            }

            if (commands[m_currentIndex - 1] is MethodCheatCommand methodCheatCommand)
            {
                methodCheatCommand.Execute(CheatUtils.GetCallerInstance(methodCheatCommand.DeclaringType), 
                                           m_parameterItemDisplayContainer.GetParameterValues().ToArray());
            }
        }

        public void OnCloseClick()
        {
            OnExecuteCheatCommand();
            Hide();
        }
    }
}