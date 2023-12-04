#region Header
// Date: 15/11/2023
// Created by: Huynh Phong Tran
// File name: CheatUI.cs
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Cheat;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using UnityEngine;

namespace Base
{
    public class CheatUI : UIView
    {
        [SerializeField] private TMP_Dropdown m_dropdown;

        private CheatService m_cheatService;
        private int          m_currentIndex = 0;
        protected override void Awake()
        {
            m_cheatService = ServiceLocator.Get<CheatService>();
        }

        protected override void Start()
        {
            TaskRunner.Start(StartInitializeService());
        }

        private async Task StartInitializeService()
        {
            m_cheatService.Init();
            
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