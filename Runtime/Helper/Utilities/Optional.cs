#region Header
// Date: 12/10/2023
// Created by: Huynh Phong Tran
// File name: Optional.cs
#endregion

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Base.Helper
{
     public readonly struct Optional<TContent>
    {
        private readonly bool _hasContent;
        private readonly bool _supportNull;
        private readonly TContent _content;
        
        public bool Equals(Optional<TContent> other)
        {
            return _hasContent == other._hasContent && _supportNull == other._supportNull && EqualityComparer<TContent>.Default.Equals(_content, other._content);
        }
        public override bool Equals(object obj)
        {
            return obj is Optional<TContent> other && Equals(other);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_hasContent, _supportNull, _content);
        }

        private Optional(bool hasContent, bool supportNull, TContent content)
        {
            _hasContent = hasContent;
            _supportNull = supportNull;
            _content = content;
        }

        public static Optional<TContent> Some(TContent content, bool supportNull = false) =>
            supportNull || content != null
                ? new Optional<TContent>(true, supportNull, content)
                : None();

        public static Optional<TContent> None() => new Optional<TContent>(false, false, default);

        public bool IsSome => _hasContent;

        public bool TryGetSome(out TContent content)
        {
            if (_hasContent)
            {
                content = _content;
                return true;
            }

            content = default;
            return false;
        }

        public bool IsNone => !_hasContent;
        public bool IsNoneOrNull => !_hasContent || _content == null;
        public bool SupportNull => _supportNull;



        public TContent ValueOrDefault(TContent fallback = default) =>
            _hasContent
                ? _content
                : fallback;

        public static bool operator ==(Optional<TContent> a, TContent b) =>
            a._hasContent && Equals(a._content, b);

        public static bool operator ==(Optional<TContent> a, Optional<TContent> b) =>
            a._hasContent && b._hasContent && Equals(a._content, b._content);

        public static bool operator !=(Optional<TContent> a, TContent b) =>
            !a._hasContent || !Equals(a._content, b);

        public static bool operator !=(Optional<TContent> a, Optional<TContent> b) =>
            !a._hasContent || !b._hasContent || !Equals(a._content, b._content);

        /// <summary>
        /// Transforms an Optional of Type TContent into an Optional of TNewContent
        /// </summary>
        public Optional<TNewContent> Map<TNewContent>([NotNull] Func<TContent, TNewContent> func) =>
            func != null && _hasContent // NotNull is a nice note for developers, not an enforcement.
                ? Optional<TNewContent>.Some(func(_content), _supportNull)
                : Optional<TNewContent>.None();

        public Optional<TNewContent> MapPossibleException<TNewContent>([NotNull] Func<TContent, TNewContent> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Optional<TNewContent>.None();
            }
        }

        /// <summary>
        /// Transforms an Optional of Type TContent into an Optional of TNewContent. Does not Stack
        /// AKA FlatMap
        /// </summary>
        public Optional<TNewContent> Map<TNewContent>([NotNull] Func<TContent, Optional<TNewContent>> func) =>
            func != null && _hasContent // NotNull is a nice note for developers, not an enforcement.
                ? func(_content)
                : Optional<TNewContent>.None();


        public Optional<TNewContent> MapPossibleException<TNewContent>([NotNull] Func<TContent, Optional<TNewContent>> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Optional<TNewContent>.None();
            }
        }

        public Attempt<TNewContent> Map<TNewContent>([NotNull] Func<TContent, Attempt<TNewContent>> func) =>
            func == null // NotNull is a nice note for developers, not an enforcement.
                ? Attempt<TNewContent>.Failure($"Map of Optional<{typeof(TContent)}> to Attempt<{typeof(TNewContent)}> failed. Transformation function is NULL!", true)
                : _hasContent
                    ? func(_content)
                    : Attempt<TNewContent>.Failure($"None of Optional<{typeof(TContent)}>", true);

        public Attempt<TNewContent> MapPossibleException<TNewContent>([NotNull] Func<TContent, Attempt<TNewContent>> func)
        {
            try
            {
                return Map(func);
            }
            catch (Exception e)
            {
                return Attempt<TNewContent>.Failure(e);
            }
        }

        public Optional<TContent> DoIfPotentialException(Action<TContent> onSome, Action onNone, Action<Exception> onException = null)
        {
            try
            {
                if (_hasContent)
                {
                    onSome?.Invoke(_content);
                }
                else
                {
                    onNone?.Invoke();
                }
            }
            catch (Exception e)
            {
                onException?.Invoke(e);
            }

            return this;
        }

        public Optional<TContent> DoIf(Action<TContent> onSome, Action onNone)
        {
            if (_hasContent)
            {
                onSome?.Invoke(_content);
            }
            else
            {
                onNone?.Invoke();
            }

            return this;
        }

        public Optional<TContent> DoIfSome(Action<TContent> onSome)
        {
            if (_hasContent)
            {
                onSome?.Invoke(_content);
            }

            return this;
        }

        public Optional<TContent> DoIfNone(Action onNone)
        {
            if (!_hasContent)
            {
                onNone?.Invoke();
            }

            return this;
        }

        public static explicit operator Attempt<TContent>(Optional<TContent> o) =>
            o._hasContent
                ? Attempt<TContent>.Success(o._content)
                : Attempt<TContent>.Failure($"None of Optional<{typeof(TContent)}>", true);

        public Attempt<TContent> AsAttempt() => (Attempt<TContent>) this;

        public static implicit operator Optional<TContent>(TContent content) => Some(content);
    };
}