#region Header
// Date: 16/12/2023
// Created by: Huynh Phong Tran
// File name: AnimatorExtension.cs
#endregion

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Helper
{
    public static class AnimatorExtension
    {
        public static bool IsAnimatorInState(this Animator animator, int tagHash, int layer)
        {
            return animator != null && animator.GetCurrentAnimatorStateInfo(layer).tagHash == tagHash;
        }
        
        public static bool HasParameter(this Animator animator, int paramHash)
        {
            return animator.parameters.Any(param => param.nameHash == paramHash);
        }
        
        public static async Task WaitUntilAnimatorActive(this Animator animator)
        {
            if (animator == null || (animator.isInitialized && animator.isActiveAndEnabled))
            {
                return;
            }
            
            await TaskRunner.WaitUntil(() => animator == null || (animator.isInitialized && animator.isActiveAndEnabled));
        }
        
        public static async Task WaitUntilAnimatorActive(this Animator animator, string callerName)
        {
            if (animator == null || (animator.isInitialized && animator.isActiveAndEnabled))
            {
                return;
            }

            await TaskRunner.WaitUntil(callerName, () => animator == null || (animator.isInitialized && animator.isActiveAndEnabled));
        }

        public static void SetTriggerWhenActive(this Animator animator, int paramHash)
        {
            if (animator == null)
            {
                return;
            }

            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetTrigger(paramHash);
            }
        }

        public static void SetBoolWhenActive(this Animator animator, int paramHash, bool value)
        {
            if (animator == null)
            {
                return;
            }

            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetBool(paramHash, value);
            }
        }
        
        public static void SetIntegerWhenActive(this Animator animator, int paramHash, int value)
        {
            if (animator == null)
            {
                return;
            }

            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetInteger(paramHash, value);
            }
        }
        
        public static void SetFloatWhenActive(this Animator animator, int paramHash, float value)
        {
            if (animator == null)
            {
                return;
            }

            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetFloat(paramHash, value);
            }
        }

        public static async Task SetTriggerWhenActiveAsync(this Animator animator,string callerName, int paramHash)
        {
            if (animator == null)
            {
                return;
            }
            
            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetTrigger(paramHash);
                return;
            }

            await animator.WaitUntilAnimatorActive(callerName);

            if (animator == null)
            {
                return;
            }
            
            animator.SetTrigger(paramHash);
        }
        
        public static async Task SetBoolWhenActiveAsync(this Animator animator,string callerName, int paramHash, bool value)
        {
            if (animator == null)
            {
                return;
            }
            
            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetBool(paramHash, value);
                return;
            }

            await animator.WaitUntilAnimatorActive(callerName);

            if (animator == null)
            {
                return;
            }
            
            animator.SetBool(paramHash, value);
        }
        
        public static async Task SetIntegerWhenActiveAsync(this Animator animator,string callerName, int paramHash, int value)
        {
            if (animator == null)
            {
                return;
            }
            
            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetInteger(paramHash, value);
                return;
            }

            await animator.WaitUntilAnimatorActive(callerName);

            if (animator == null)
            {
                return;
            }
            
            animator.SetInteger(paramHash, value);
        }
        
        public static async Task SetFloatWhenActiveAsync(this Animator animator,string callerName, int paramHash, float value)
        {
            if (animator == null)
            {
                return;
            }
            
            if (animator.isInitialized && animator.isActiveAndEnabled)
            {
                animator.SetFloat(paramHash, value);
                return;
            }

            await animator.WaitUntilAnimatorActive(callerName);

            if (animator == null)
            {
                return;
            }
            
            animator.SetFloat(paramHash, value);
        }
        
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
    }
}