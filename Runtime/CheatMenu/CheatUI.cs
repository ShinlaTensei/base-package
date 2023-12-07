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

            var clickStream = Observable.EveryUpdate().Where(_ => m_inputHandler.GetTouch().Count > 0);
            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250))).Where(xs => xs.Count >= 2)
                       .Subscribe(_ => Show()).AddTo(this);
            m_parameterItemDisplayContainer.Initialize();
        }

        protected override void Start()
        {
            m_dropdown.onValueChanged.AddListener(OnValueChanged);
            TaskRunner.Start(StartInitializeService());
        }

        private async Task StartInitializeService()
        {
            m_cheatService.Init(AssemblyName);
            await UniTask.WaitUntil(() => m_cheatService.IsInitialize, cancellationToken: this.GetCancellationTokenOnDestroy());

            DrawCheatCommand();
        }

        private void DrawCheatCommand()
        {
            List<ICheatCommand>           commands    = m_cheatService.GetCheatCommands();
            List<TMP_Dropdown.OptionData> listOptions = new List<TMP_Dropdown.OptionData>();
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

            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                GenerateParameterDisplay(parameterInfos[i]);
            }
        }

        private void GenerateParameterDisplay(ParameterInfo parameterInfo)
        {
            
        }

        private void OnValueChanged(int index)
        {
            try
            {
                m_currentIndex = index;

                MethodCheatCommand command = m_cheatService.GetCheatCommands()[m_currentIndex] as MethodCheatCommand;

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

        public void ExecuteCommand()
        {
            ICheatCommand command = m_cheatService.GetCheatCommands()[m_currentIndex];
        }

        public override void Populate<T>(T viewData)
        {
            return;
        }

        public void OnExecuteCheatCommand()
        {
            List<ICheatCommand> commands = m_cheatService.GetCheatCommands();
            if (m_currentIndex >= commands.Count || m_currentIndex < 0)
            {
                PDebug.ErrorFormat("[Cheat] Index out of range when calling execute cheat");
                return;
            }

            if (commands[m_currentIndex] is MethodCheatCommand methodCheatCommand)
            {
                methodCheatCommand.Execute(CheatUtils.GetCallerInstance(methodCheatCommand.DeclaringType), null);
            }
        }

        public void OnCloseClick()
        {
            
        }
    }
}