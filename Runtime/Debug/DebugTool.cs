using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Base.Helper;
using Base.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class DebugTool : BaseMono
    {
        [SerializeField] private FloatingButton floatingButton;
        [SerializeField] private GameObject debugContent;
        [SerializeField] private DebugMono functionObj;

        [SerializeField] private TMP_Dropdown functionDropdown;
        [SerializeField] private TMP_Dropdown groupDropdown;
        [SerializeField] private Transform functionParamContainer;

        [SerializeField] private InputParamUI inputParamUI;
        [SerializeField] private SelectorParamUI selectorParamUI;
        [SerializeField] private TMP_Text m_buildText;

        private bool _init = false;
        private int _functionIndex = 0;
        private int _groupIndex = 0;

        private List<GameDebugProperty> _debugProperties = new List<GameDebugProperty>();
        private List<GameDebugSceneMethod> _debugSceneMths = new List<GameDebugSceneMethod>();
        private Dictionary<SceneName, List<GameDebugSceneMethod>> _sceneMethods = new Dictionary<SceneName, List<GameDebugSceneMethod>>();
        private List<string> _renderGroup = new List<string>();

        private object[] _parameters;

        protected override void Start()
        {
            #if CHEAT_ENABLE
            base.Start();
            functionDropdown.onValueChanged.AddListener(OnFunctionValueChanged);
            groupDropdown.onValueChanged.AddListener(OnGroupValueChanged);
            floatingButton.OnClick += OpenDebugUI;
            #else
            Active = false;
            #endif
        }

        private void OnDestroy()
        {
            floatingButton.OnClick -= OpenDebugUI;
        }

        private void OpenDebugUI()
        {
            if (!_init) Init(functionObj);
            
            FilterByScene();
            Render();
            
            debugContent.SetActive(true);
            floatingButton.Active = false;
        }

        private void Init(MonoBehaviour mono)
        {
            _sceneMethods.Clear();
            _debugProperties.Clear();

            {
                // Method
                List<MethodInfo> mths = mono.GetType().GetMethods()
                    .Where(e => e.GetCustomAttributes(typeof(DebugActionAttribute), false).Length > 0).ToList();

                int count = mths.Count;
                for (int i = 0; i < count; ++i)
                {
                    MethodInfo methodInfo = mths[i];
                    object[] attributes = methodInfo.GetCustomAttributes(false);
                    DebugActionAttribute debugActionAttribute = null;

                    foreach (var attr in attributes)
                    {
                        debugActionAttribute = attr as DebugActionAttribute;
                        if (debugActionAttribute != null) break;
                    }
                    
                    if (debugActionAttribute == null || !debugActionAttribute.IsEnable) { continue; }

                    if (!_sceneMethods.TryGetValue(debugActionAttribute.ActionScene, out List<GameDebugSceneMethod> methods))
                    {
                        methods = new List<GameDebugSceneMethod>();
                        _sceneMethods[debugActionAttribute.ActionScene] = methods;
                    }
                    
                    methods.Add(new GameDebugSceneMethod
                    {
                        SceneName = debugActionAttribute.ActionScene,
                        Attribute = debugActionAttribute,
                        Method = methodInfo
                    });
                }
            }

            {
                // Properties
                List<PropertyInfo> infos = mono.GetType().GetProperties()
                    .Where(e => e.GetCustomAttributes(typeof(DebugInfoAttribute), true).Length > 0).ToList();
                for (int i = 0; i < infos.Count; ++i)
                {
                    PropertyInfo propertyInfo = infos[i];
                    object[] attrs = propertyInfo.GetCustomAttributes(false);
                    DebugInfoAttribute debugInfoAttribute = null;
                    foreach (var attr in attrs)
                    {
                        debugInfoAttribute = attr as DebugInfoAttribute;
                        if (debugInfoAttribute != null) break;
                    }

                    if (debugInfoAttribute == null) { continue; }

                    GameDebugProperty debugProperty = new GameDebugProperty
                    {
                        Attribute = debugInfoAttribute,
                        Property = propertyInfo
                    };
                    
                    _debugProperties.Add(debugProperty);
                }
                
                _debugProperties.Sort((a,b) => a.Attribute.Order - b.Attribute.Order);
            }

            _init = true;
        }

        private void FilterByScene()
        {
            _debugSceneMths.Clear();
            string sceneName = GetOpenScene();

            Enum.TryParse(sceneName, false, out SceneName scene);

            foreach (var pair in _sceneMethods)
            {
                if (pair.Key == scene || pair.Key == SceneName.AnyScene)
                {
                    _debugSceneMths.AddRange(pair.Value);
                }
            }
            
            _debugSceneMths.Sort((a, b) =>
            {
                int compareTo = string.Compare(a.Attribute.ActionGroup, b.Attribute.ActionGroup, StringComparison.Ordinal);
                if (compareTo != 0) { return compareTo; }

                compareTo = a.Attribute.Priority - b.Attribute.Priority;
                if (compareTo != 0) { return compareTo; }

                compareTo = string.Compare(a.Attribute.ActionName, b.Attribute.ActionName, StringComparison.Ordinal);
                return compareTo;
            });
            
            functionDropdown.ClearOptions();
            List<string> cheatNames = new List<string>();
            foreach (var sceneMethod in _debugSceneMths)
            {
                cheatNames.Add($"<b>[{sceneMethod.Attribute.ActionGroup}]</b> {sceneMethod.Attribute.ActionName}");
            }
            functionDropdown.AddOptions(cheatNames);
            functionDropdown.value = _functionIndex;
        }

        private void Render()
        {
            functionParamContainer.DestroyAllChildren();

            int index = _functionIndex;
            if (_debugSceneMths.Count > index)
            {
                GameDebugSceneMethod sceneMethod = _debugSceneMths[index];
                Attribute attribute = sceneMethod.Attribute;
                ParameterInfo[] paramsInfo = sceneMethod.Method.GetParameters();
                int[] paramData = sceneMethod.ParamIndex ?? new int[paramsInfo.Length];

                _parameters = new object[paramsInfo.Length];
                sceneMethod.ParamIndex = paramData;

                for (int i = 0; i < paramsInfo.Length; ++i)
                {
                    int temp = i;
                    ParameterInfo info = paramsInfo[i];
                    SelectAttribute selectAttribute = info.GetCustomAttribute<SelectAttribute>();
                    _parameters[i] = info.DefaultValue;
                    if (selectAttribute != null) // Parameter that can be select from dropdown menu
                    {
                        int             lastIndex = paramData[i];
                        SelectorParamUI input     = Instantiate(selectorParamUI);
                        Transform inputTrans = input.transform;
                        inputTrans.SetParent(functionParamContainer.transform);
                        inputTrans.localScale    = Vector3.one;
                        inputTrans.localPosition = Vector3.zero;
                        input.gameObject.name         = "DropDown - " + info.Name;
                        input.gameObject.SetActive(true);
                        input.InitUI(info.Name, info.ParameterType.Name, info.DefaultValue);
                        input.MainInput.ClearOptions();
                        input.MainInput.onValueChanged.RemoveAllListeners();

                        MethodInfo m = functionObj.GetType().GetMethod(selectAttribute.MethodName);
                        if (m != null)
                        {
                            List<string> options  = new List<string>();
                            IList        options_ = (IList)m.Invoke(functionObj, null);

                            if (options_ != null)
                            {
                                foreach (object option in options_) { options.Add(option.ToString()); }
                            }

                            input.MainInput.AddOptions(options);
                            if (input.MainInput.options.Count > input.MainInput.value)
                            {
                                string text = input.MainInput.options[input.MainInput.value].text;
                                _parameters[i] = Convert(text, info.ParameterType);
                            }

                            if (lastIndex < options.Count)
                            {
                                _parameters[i]         = Convert(options[lastIndex], info.ParameterType);
                                input.MainInput.value = lastIndex;
                            }
                        }
                        input.MainInput.onValueChanged.AddListener(id =>
                        {
                            if (input.MainInput.options.Count > id)
                            {
                                string text = input.MainInput.options[id].text;
                                _parameters[temp]   = Convert(text, info.ParameterType);
                                paramData[temp] = id;
                            }
                        });
                    }
                    else
                    {
                        InputParamUI input = Instantiate(inputParamUI);
                        Transform inputTrans = input.transform;
                        inputTrans.SetParent(functionParamContainer.transform);
                        inputTrans.localScale    = Vector3.one;
                        inputTrans.localPosition = Vector3.zero;
                        input.gameObject.name         = "Input - " + info.Name;
                        input.gameObject.SetActive(true);
                        input.InitUI(info.Name, info.ParameterType.Name, info.DefaultValue);
                        input.MainInput.onValueChanged.RemoveAllListeners();
                        input.Toggle1.onValueChanged.RemoveAllListeners();
                        input.MainInput.onValueChanged.AddListener(value =>
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                _parameters[temp]        = Activator.CreateInstance(info.ParameterType);
                                input.MainInput.text = _parameters[temp].ToString();
                                return;
                            }
                            if (Convert(value, info.ParameterType, out object result))
                            {
                                _parameters[temp] = result;
                            }
                            else
                            {
                                input.MainInput.text = _parameters[temp].ToString();
                            }
                        });
                        input.Toggle1.onValueChanged.AddListener(value =>
                        {
                            _parameters[temp] = value;
                        });
                    }
                }
            }

            if (_debugProperties.Count > 0)
            {
                string buildTextText = string.Empty;
                for (int i = 0; i < _debugProperties.Count; i++)
                {
                    GameDebugProperty property = _debugProperties[i];
                    string            color    = property.Attribute.Color;
                    string            text     = color == null ? $"{property.Attribute.Name}: {property.Property.GetValue(functionObj, null)}" 
                        : $"{property.Attribute.Name}: <color={color}>{property.Property.GetValue(functionObj, null)}</color>";

                    if (i == _debugProperties.Count - 1)
                    {
                        buildTextText += text;
                    }
                    else
                    {
                        buildTextText += text + "\n";
                    }
                }
                if (m_buildText && !string.IsNullOrEmpty(buildTextText))
                {
                    m_buildText.text = buildTextText;
                }
                Canvas.ForceUpdateCanvases();
            }
        }

        private string GetOpenScene()
        {
            string sceneName = String.Empty;
            for (int i = 1; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.isLoaded) sceneName = scene.name;
            }

            return sceneName;
        }

        private void OnFunctionValueChanged(int index)
        {
            functionDropdown.value = index;
            _functionIndex = index;
            
            Render();
        }

        private void OnGroupValueChanged(int index)
        {
            groupDropdown.value = index;
            _groupIndex = index;
            
            FilterByScene();
        }

        public void OnSubmitClick()
        {
            if (_functionIndex < _debugSceneMths.Count)
            {
                GameDebugSceneMethod method = _debugSceneMths[_functionIndex];
                MethodInfo methodInfo = method.Method;
                PDebug.DebugFormat("CHEAT: calling {method}", methodInfo.Name);
                methodInfo.Invoke(functionObj, _parameters);
            }
        }
        
        #region static
        public static object Convert(string value, Type conversionType)
        {
            if (conversionType.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(conversionType, value);
            }

            return System.Convert.ChangeType(value, conversionType);
        }
        public static bool Convert(string value, Type conversionType, out object result)
        {
            result = null;
            if (conversionType.GetTypeInfo().IsEnum)
            {
                try
                {
                    result = Enum.Parse(conversionType, value);
                    return true;
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            try
            {
                result = System.Convert.ChangeType(value, conversionType);
                return true;
            }
            catch (Exception e)
            {
                // ignored
            }
            return false;
        }
        #endregion //static
    }

    public enum SceneName
    {
        AnyScene = 0,
        ManagerScene = 1,
    }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DebugActionAttribute : Attribute
    {
        public readonly SceneName ActionScene;
        public readonly string    ActionGroup;
        public readonly string    ActionName;
        public readonly int       Priority;
        public readonly bool      IsEnable = true;
        
        public DebugActionAttribute(string actionName, SceneName scene, int priority = 0, bool isEnable = true)
        {
            ActionName  = actionName;
            ActionGroup = "Misc";
            Priority    = priority;
            IsEnable    = isEnable;
            ActionScene = scene;
        }

        public DebugActionAttribute(string actionName, string actionGroup, SceneName scene)
        {
            ActionName  = actionName;
            ActionGroup = actionGroup;
            ActionScene = scene;
        }

        public DebugActionAttribute(string actionName, string actionGroup, SceneName scene, int priority = 0, bool isEnable = true)
        {
            ActionName  = actionName;
            ActionGroup = actionGroup;
            Priority    = priority;
            IsEnable    = isEnable;
            ActionScene = scene;
        }
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SelectAttribute : Attribute
    {
        private int _selectIndex;

        public int SelectIndex
        {
            get => _selectIndex;
            set => _selectIndex = value;
        }
        
        public readonly string MethodName;

        public SelectAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class DebugInfoAttribute : Attribute
    {
        public string Name  = null;
        public string Color = null;
        public int    Order = 0;

        public DebugInfoAttribute(string name, string color, int order = 0)
        {
            Name  = name;
            Color = color;
            Order = order;
        }
        public DebugInfoAttribute(string name, int order = 0)
        {
            Name  = name;
            Color = null;
            Order = order;
        }
    }
    
    public class GameDebugSceneMethod
    {
        public SceneName            SceneName;
        public MethodInfo           Method;
        public DebugActionAttribute Attribute;
        public int[]                ParamIndex;
    }
    public class GameDebugProperty
    {
        public PropertyInfo       Property;
        public DebugInfoAttribute Attribute;
    }
}

