using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Base.Helper;
using UnityEngine;

namespace Base.Cheat
{
    public class ParameterItemDisplayContainer : BaseUI
    {
        [SerializeField] private List<InputParameterDisplay> m_inputParameterDisplays;
        [SerializeField] private List<BoolParameterDisplay>  m_boolParameterDisplays;
        [SerializeField] private List<EnumParameterDisplay>  m_enumParameterDisplays;

        private T Get<T>(List<T> array) where T : ParameterItemDisplayBase
        {
            T value = null;
            for (int i = 0; i < array.Count; ++i)
            {
                if (!array[i].Active)
                {
                    value = array[i];
                    break;
                }
            }

            return value;
        }

        private List<string> GetValueList(string parameterName)
        {
            List<string> valueList = new List<string>();

            return valueList;
        }
        
        public void Initialize()
        {
            
        }

        public void PopulateParameterDisplay(ParameterInfo[] parameterInfos)
        {
            int parameterIndex = 0;
            
            m_inputParameterDisplays.SetActiveAllComponent(false);
            m_boolParameterDisplays.SetActiveAllComponent(false);
            m_enumParameterDisplays.SetActiveAllComponent(false);

            foreach (ParameterInfo parameter in parameterInfos)
            {
                List<string> valueList = GetValueList(parameter.Name);
                
                if (valueList.Count <= 0 && (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(float) 
                                                                                    || parameter.ParameterType == typeof(double) 
                                                                                    || parameter.ParameterType == typeof(string) 
                                                                                    || parameter.ParameterType == typeof(long)))
                {
                    InputParameterDisplay inputParameterDisplay = Get(m_inputParameterDisplays);
                    if (!inputParameterDisplay) continue;
                    
                    inputParameterDisplay.Initialize(parameter);
                    inputParameterDisplay.CacheTransform.SetSiblingIndex(parameterIndex);
                }
                else if (parameter.ParameterType == typeof(bool))
                {
                    BoolParameterDisplay boolParameterDisplay = Get(m_boolParameterDisplays);
                    if (!boolParameterDisplay) continue;
                    
                    boolParameterDisplay.Initialize(parameter);
                    boolParameterDisplay.CacheTransform.SetSiblingIndex(parameterIndex);
                }
                else if (valueList.Count > 0)
                {
                    EnumParameterDisplay enumParameterDisplay = Get(m_enumParameterDisplays);
                    if (!enumParameterDisplay) continue;
                    
                    enumParameterDisplay.Initialize(parameter, valueList);
                    enumParameterDisplay.CacheTransform.SetSiblingIndex(parameterIndex);
                }
                else
                {
                    EnumParameterDisplay enumParameterDisplay = Get(m_enumParameterDisplays);
                    if (!enumParameterDisplay) continue;
                    
                    enumParameterDisplay.Initialize(parameter, parameter.ParameterType);
                    enumParameterDisplay.CacheTransform.SetSiblingIndex(parameterIndex);
                }

                parameterIndex++;
            }
        }

        public List<string> GetParameterValues()
        {
            List<string> returnValues = new List<string>();
            for (int i = 0; i < CacheTransform.childCount; ++i)
            {
                ParameterItemDisplayBase parameterItemDisplayBase = CacheTransform.GetChild(i).GetComponentInBranch<ParameterItemDisplayBase>();
                if (!parameterItemDisplayBase || !parameterItemDisplayBase.Active)
                {
                    continue;
                }

                returnValues.Add(parameterItemDisplayBase.GetValue());
            }

            return returnValues;
        }

        public void ResetDisplay()
        {
            m_inputParameterDisplays.SetActiveAllComponent(false);
            m_boolParameterDisplays.SetActiveAllComponent(false);
            m_enumParameterDisplays.SetActiveAllComponent(false);
        }
    }
}

