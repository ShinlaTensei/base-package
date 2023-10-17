using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Base.Logging;
using Base.Pattern;
using TMPro;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Base.Helper
{
    public static class UtilsClass
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                    using (client.OpenRead("http://unity3d.com")) 
                        return true; 
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNetworkReachable()
        {
            return Application.internetReachability is not NetworkReachability.NotReachable;
        }
        public static string FormatMoney(int money, int decPlace = 2)
        {
            string result = String.Empty;
            float place = Mathf.Pow(10f, decPlace);

            string[] abbrev = {"K", "M", "B", "T"};
            string str = (money < 0) ? "-" : "";
            float size;

            money = Mathf.Abs(money);

            for (int i = abbrev.Length - 1; i >= 0; --i)
            {
                size = Mathf.Pow(10, (i + 1) * 3);
                if (size <= money)
                {
                    money = (int) (Mathf.Floor(money * place / size) / place);
                    if ((money == 1000) && (i < abbrev.Length - 1))
                    {
                        money = 1;
                        i += 1;
                    }

                    result = money + abbrev[i];
                    break;
                }
            }

            return str + result;
        }

        /// <summary>
        /// Convert money string with format x.xxx.xxx or x,xxx,xxx to Float
        /// </summary>
        /// <param name="moneyStr"></param>
        /// <returns>money in float</returns>
        public static uint ConvertMoneyToNumber(string moneyStr)
        {
            var resultStr = moneyStr.Split('.');
            if (resultStr.Length <= 1)
            {
                resultStr = moneyStr.Split(',');
            }

            string joinString = string.Join("", resultStr);
            return uint.Parse(joinString);
        }

        /// <summary>
        /// Format a number to string with comma seperated format.
        /// </summary>
        /// <param name="money"></param>
        /// <returns>money in string</returns>
        public static string FormatStringCommaSeparated(uint money)
        {
            object o = money;
            return $"{o:#,##0.##}";
        }

        public static string ToOrdinalString(this long number)
        {
            if (number < 0) return number.ToString();
            long rem = number % 100;
            if (rem >= 11 && rem <= 13) return number + "th";

            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        public static string ToOrdinalString(this int number)
        {
            return ((long) number).ToOrdinalString();
        }

        public static Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
        {
            Vector3 direction = target - source;
            float h = direction.y;
            direction.y = 0;
            float distance = direction.magnitude;
            float a = angle * Mathf.Deg2Rad;
            direction.y = distance * Mathf.Tan(a);
            distance += h / Mathf.Tan(a);

            // calculate velocity
            float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return velocity * direction.normalized;
        }

        /// <summary>
        /// Returns true if the target value is between a and b ( both exclusive ). 
        /// To include the limits values set the "inclusive" parameter to true.
        /// </summary>
        public static bool IsBetween(float target, float a, float b, bool inclusive = false)
        {
            if (b > a)
                return (inclusive ? target >= a : target > a) && (inclusive ? target <= b : target < b);
            else
                return (inclusive ? target >= b : target > b) && (inclusive ? target <= a : target < a);
        }

        /// <summary>
        /// Returns true if the target value is between a and b ( both exclusive ). 
        /// To include the limits values set the "inclusive" parameter to true.
        /// </summary>
        public static bool IsBetween(int target, int a, int b, bool inclusive = false)
        {
            if (b > a)
                return (inclusive ? target >= a : target > a) && (inclusive ? target <= b : target < b);
            else
                return (inclusive ? target >= b : target > b) && (inclusive ? target <= a : target < a);
        }

        public static bool IsCloseTo(Vector3 input, Vector3 target, float tolerance)
        {
            return Vector3.Distance(input, target) <= tolerance;
        }

        public static bool IsCloseTo(float input, float target, float tolerance)
        {
            return Mathf.Abs(target - input) <= tolerance;
        }

        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value) {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static bool IsInCameraView(Camera cam, Vector3 toCheck)
        {
            Vector3 point = cam.WorldToViewportPoint(toCheck);

            if (point.z < 0) return false;

            if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1) return true;

            return false;
        }
        
        public static Vector2 GetSizeOfText(TMP_Text tmpText, float fontSize, string text, float maxWidthInUI = 0)
        {
            if (tmpText.overflowMode != TextOverflowModes.Overflow || !tmpText.enableWordWrapping) return Vector2.zero;
            
            TMP_FontAsset font = tmpText.font;
            float lineHeight = font.faceInfo.lineHeight;
            float biggestHeight = 1f;
            float biggestWidth = 1f;
            float spaceWidth = font.faceInfo.tabWidth * 2f;
            float width = 0;
            int spacingCount = 0;

            foreach (var c in text)
            {
                if (c == ' ')
                {
                    spacingCount++;
                    continue;
                }
                if (font.HasCharacter(c))
                {
                    TMP_FontUtilities.SearchForCharacter(font, (uint)Char.ConvertToUtf32(c.ToString(), 0), out TMP_Character character);
                    width += character.glyph.metrics.width;
                    if (biggestHeight < character.glyph.metrics.height)
                    {
                        biggestHeight = character.glyph.metrics.height;
                    }

                    if (biggestWidth < character.glyph.metrics.width)
                    {
                        biggestWidth = character.glyph.metrics.width;
                    }
                }
            }

            float textWidth = (width + (spacingCount * spaceWidth)) * fontSize / font.faceInfo.pointSize;
            float textHeight = 0;
            if (maxWidthInUI > 0)
            {
                int numberOfLine = Mathf.CeilToInt(textWidth / maxWidthInUI);
                textHeight = lineHeight * numberOfLine * fontSize / font.faceInfo.pointSize;
                textWidth = maxWidthInUI;
            }
            else
            {
                textHeight = lineHeight * fontSize / font.faceInfo.pointSize;
            }

            PDebug.InfoFormat("[Utilities] Get Text Size: {0}", new Vector2(textWidth, textHeight));
            return new Vector2(textWidth, textHeight);
        }

        #region Animator

        /// <summary>
        /// Gets the current clip effective length, that is, the original length divided by the playback speed. The length value is always positive, regardless of the speed sign. 
        /// It returns false if the clip is not valid.
        /// </summary>
        public static bool GetCurrentClipLength(this Animator animator, ref float length)
        {
            if (animator.runtimeAnimatorController == null) return false;

            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (clipInfo.Length == 0) return false;

            float clipLength = clipInfo[0].clip.length;
            float speed = animator.GetCurrentAnimatorStateInfo(0).speed;

            length = Mathf.Abs(clipLength / speed);

            return true;
        }
        
        public static bool GetCurrentClipLength(this Animator animator, int layerIndex, ref float length)
        {
            if (animator.runtimeAnimatorController == null) return false;

            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);

            if (clipInfo.Length == 0) return false;

            float clipLength = clipInfo[0].clip.length;
            float speed = animator.GetCurrentAnimatorStateInfo(0).speed;

            length = Mathf.Abs(clipLength / speed);

            return true;
        }

        public static bool MatchTarget(this Animator animator, Vector3 targetPosition, Quaternion targetRotation, AvatarTarget avatarTarget, float startNormalizedTime,
            float targetNormalizedTime)
        {
            if (animator.runtimeAnimatorController == null) return false;

            if (animator.isMatchingTarget) return false;

            if (animator.IsInTransition(0)) return false;

            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1f);

            animator.MatchTarget(targetPosition, targetRotation, avatarTarget, weightMask, startNormalizedTime, targetNormalizedTime);

            return true;
        }

        public static bool MatchTarget(this Animator animator, Vector3 targetPosition, AvatarTarget avatarTarget, float startNormalizedTime, float targetNormalizedTime)
        {
            if (animator.runtimeAnimatorController == null) return false;

            if (animator.isMatchingTarget) return false;

            if (animator.IsInTransition(0)) return false;

            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 0f);

            animator.MatchTarget(targetPosition, Quaternion.identity, avatarTarget, weightMask, startNormalizedTime, targetNormalizedTime);

            return true;
        }

        public static bool MatchTarget(this Animator animator, Transform target, AvatarTarget avatarTarget, float startNormalizedTime, float targetNormalizedTime)
        {
            if (animator.runtimeAnimatorController == null) return false;

            if (animator.isMatchingTarget) return false;

            if (animator.IsInTransition(0)) return false;

            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1f);

            animator.MatchTarget(target.position, target.rotation, avatarTarget, weightMask, startNormalizedTime, targetNormalizedTime);

            return true;
        }

        public static bool MatchTarget(this Animator animator, Transform target, AvatarTarget avatarTarget, float startNormalizedTime, float targetNormalizedTime,
            MatchTargetWeightMask weightMask)
        {
            if (animator.runtimeAnimatorController == null) return false;

            if (animator.isMatchingTarget) return false;

            if (animator.IsInTransition(0)) return false;

            animator.MatchTarget(target.position, target.rotation, AvatarTarget.Root, weightMask, startNormalizedTime, targetNormalizedTime);

            return true;
        }

        #endregion

        #region Component

        
        /// <summary>
        /// Gets a "target" component within a particular branch (inside the hierarchy). The branch is defined by the "branch root object", which is also defined by the chosen 
        /// "branch root component". The returned component must come from a child of the "branch root object".
        /// </summary>
        /// <param name="callerComponent"></param>
        /// <param name="includeInactive">Include inactive objects?</param>
        /// <typeparam name="T1">Target component type.</typeparam>	
        /// <returns>The target component.</returns>
        public static T1 GetComponentInBranch<T1>(this Component callerComponent, bool includeInactive = true) where T1 : Component
        {
            return callerComponent.GetComponentInBranch<T1, T1>(includeInactive);
        }
        
        /// <summary>
        /// Gets a "target" component within a particular branch (inside the hierarchy). The branch is defined by the "branch root object", which is also defined by the chosen 
        /// "branch root component". The returned component must come from a child of the "branch root object".
        /// </summary>
        /// <param name="callerComponent"></param>
        /// <param name="includeInactive">Include inactive objects?</param>
        /// <typeparam name="T1">Branch root component type.</typeparam>
        /// <typeparam name="T2">Target component type.</typeparam>
        /// <returns>The target component.</returns>
        public static T2 GetComponentInBranch<T1, T2>(this Component callerComponent, bool includeInactive = true) 
            where T1 : Component where T2 : Component
        {
            T1[] rootComponents = callerComponent.transform.root.GetComponentsInChildren<T1>(includeInactive);

            if (rootComponents.Length == 0)
            {
                Debug.LogWarning($"Root component: No objects found with {typeof(T1).Name} component");
                return null;
            }

            for (int i = 0; i < rootComponents.Length; i++)
            {
                T1 rootComponent = rootComponents[i];

                // Is the caller a child of this root?
                if (!callerComponent.transform.IsChildOf(rootComponent.transform) && !rootComponent.transform.IsChildOf(callerComponent.transform)) continue;

                T2 targetComponent = rootComponent.GetComponentInChildren<T2>(includeInactive);

                if (targetComponent == null) continue;

                return targetComponent;
            }

            return null;
        }

        public static TComponent GetOrAddComponent<TComponent>(this Component source) where TComponent : Component
        {
            return source.GetComponent<TComponent>() ?? source.AddComponent<TComponent>();
        }

        public static T AddComponent<T>(this Component source) where T : Component
        {
            return source.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject source) where T : Component
        {
            return source.GetComponent<T>() ?? source.AddComponent<T>();
        }

        public static void SetActive<T>(this T component, bool value) where T : Component
        {
            component.gameObject.SetActive(value);
        }

        #endregion

        #region Asset

        public static void LoadAssetSprite(string keyId, Image component)
        {
            ServiceLocator.Get<AddressableManager>().LoadSpriteAsync(keyId, sprite =>
                                                                                   {
                                                                                       if (sprite != null && component != null)
                                                                                       {
                                                                                           component.sprite = sprite;
                                                                                       }
                                                                                   });
        }

        #endregion
    }
    
    /// <summary>
    /// This attribute is used to represent a string value
    /// for a value in an enum.
    /// </summary>
    public class StringValueAttribute : System.Attribute {

        #region Properties

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value) {
            this.StringValue = value;
        }

        #endregion

    }
}
