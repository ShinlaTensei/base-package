#region Header
// Date: 12/10/2023
// Created by: Huynh Phong Tran
// File name: Attempt.cs
#endregion

using System;
using JetBrains.Annotations;

namespace Base.Helper
{
    public readonly struct Attempt<TSuccess>
    {

        private const string NO_ERROR = "";
        private const string NULL_INPUT_ERROR = "Null Input Error! Type: ";

        private readonly bool _isSuccess;
        [NotNull] private readonly TSuccess _content;
        [NotNull] private readonly string _error;

        private Attempt(TSuccess success)
        {
            _isSuccess = true;
            _content = success;
            _error = NO_ERROR;
        }

        private Attempt(string error)
        {
            _isSuccess = false;
            _content = default;
            _error = error;
        }

        public static Attempt<TSuccess> Success(TSuccess result, bool supportsNull = false) =>
            result != null || supportsNull
                ? new Attempt<TSuccess>(result)
                : Failure(NULL_INPUT_ERROR + typeof(TSuccess), true);
        public static Attempt<TSuccess> Failure(string failure, bool appendStack = false) => new(
            appendStack
                ? $"{failure}/n{Environment.StackTrace}"
                : failure);
        public static Attempt<TSuccess> Failure(Exception failure) => new Attempt<TSuccess>(failure.ToString());

        public bool IsSuccessful => _isSuccess;
        public bool IsFailure => !_isSuccess;

        public bool TryGetSuccess(out TSuccess success)
        {
            success = _content;
            return _isSuccess;
        }

        public bool TryGetError(out string error)
        {
            error = _error;
            return !_isSuccess;
        }

        public bool TryGetSuccess(out TSuccess success, out string error)
        {
            success = _content;
            error = _error;
            return _isSuccess;
        }

        public TSuccess SuccessOrDefault(TSuccess fallback = default) => TryGetSuccess(out TSuccess success) ? success : fallback;
        public string FailureOrDefault(string fallback = default) => TryGetError(out string error) ? error : fallback;

        public static explicit operator TSuccess(Attempt<TSuccess> a) => a.SuccessOrDefault();

        public static bool operator ==(Attempt<TSuccess> a, TSuccess b) =>
            a.TryGetSuccess(out TSuccess success) && Equals(success, b);

        public static bool operator !=(Attempt<TSuccess> a, TSuccess b) =>
            !a.TryGetSuccess(out TSuccess success) || !Equals(success, b);

        public Attempt<TNewSuccess> Map<TNewSuccess>([NotNull] Func<TSuccess, TNewSuccess> func) =>
            !TryGetSuccess(out TSuccess success, out string error)
                ? Attempt<TNewSuccess>.Failure(error)
                : func != null // NotNull is a nice note for developers, not an compile-time-enforcement.
                    ? Attempt<TNewSuccess>.Success(func(success))
                    : Attempt<TNewSuccess>.Failure($"Map of Attempt<{typeof(TSuccess)}> to Attempt<{typeof(TNewSuccess)}> failed. Transformation function is NULL!", true);

        public Attempt<TNewSuccess> MapPossibleException<TNewSuccess>([NotNull] Func<TSuccess, TNewSuccess> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Attempt<TNewSuccess>.Failure(e);
            }
        }

        // Why, oh, why doesn't C# allow overloading the >> Operator with other parameters than integers???
        /// <summary>
        /// Transforms an Attempt of Type TContent into an Attempt of TNewContent. Does not Stack
        /// AKA FlatMap
        /// </summary>
        public Attempt<TNewSuccess> Map<TNewSuccess>([NotNull] Func<TSuccess, Attempt<TNewSuccess>> func) =>
            !TryGetSuccess(out TSuccess success, out string error)
                ? Attempt<TNewSuccess>.Failure(error)
                : func?.Invoke(success) // NotNull is a nice note for developers, not an compile-time-enforcement.
                  ?? Attempt<TNewSuccess>.Failure($"Map of Attempt<{typeof(TSuccess)}> to Attempt<{typeof(TNewSuccess)}> failed. Transformation function is NULL!", true);


        public Attempt<TNewSuccess> MapPossibleException<TNewSuccess>([NotNull] Func<TSuccess, Attempt<TNewSuccess>> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Attempt<TNewSuccess>.Failure(e);
            }
        }

        public Attempt<TNewSuccess> Map<TNewSuccess>([NotNull] Func<TSuccess, Optional<TNewSuccess>> func) =>
            !TryGetSuccess(out TSuccess success, out string error)
                ? Attempt<TNewSuccess>.Failure(error)
                : func != null  // NotNull is a nice note for developers, not an enforcement.
                    ? (Attempt<TNewSuccess>) func(success)
                    : Attempt<TNewSuccess>.Failure($"Map of Attempt<{typeof(TSuccess)}> to Attempt<{typeof(TNewSuccess)}> failed. Transformation function is NULL!", true);

        public Attempt<TNewSuccess> MapPossibleException<TNewSuccess>(Func<TSuccess, Optional<TNewSuccess>> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Attempt<TNewSuccess>.Failure(e);
            }
        }

        public Attempt<TSuccess> DoIfPotentialException(Action<TSuccess> onSuccess, Action<string> onFailure, Action<Exception> onException = null)
        {
            try
            {
                if (TryGetSuccess(out TSuccess success, out string error))
                {
                    onSuccess?.Invoke(success);
                }
                else
                {
                    onFailure?.Invoke(error);
                }
            }
            catch (Exception e)
            {
                onException?.Invoke(e);
            }

            return this;
        }

        public Attempt<TSuccess> DoIf(Action<TSuccess> onSuccess, Action<string> onFailure)
        {
            if (TryGetSuccess(out TSuccess success, out string error))
            {
                onSuccess?.Invoke(success);
            }
            else
            {
                onFailure?.Invoke(error);
            }

            return this;
        }

        public Attempt<TSuccess> DoIfSuccess(Action<TSuccess> onSuccess)
        {
            if (TryGetSuccess(out TSuccess success))
            {
                onSuccess?.Invoke(success);
            }
            return this;
        }

        public Attempt<TSuccess> DoIfFailure(Action<string> onFailure)
        {
            if (TryGetError(out string failure))
            {
                onFailure?.Invoke(failure);
            }
            return this;
        }

        public static explicit operator Optional<TSuccess>(Attempt<TSuccess> a) =>
            a.TryGetSuccess(out TSuccess success)
                ? Optional<TSuccess>.Some(success)
                : Optional<TSuccess>.None();

        public Optional<TSuccess> ToOptional() => (Optional<TSuccess>) this;

        public static implicit operator Attempt<TSuccess>(TSuccess success) => Success(success);
    }
}