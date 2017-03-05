using System;
using System.Diagnostics;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [DebuggerDisplay("Field: {Name}")]
    [Serializable]
    public class BfField : BfMember, IDisposable
    {
        private FieldBools _fieldBools;

        [NonSerialized] private FieldDefinition _fieldDefinition;
        private string _fieldId;

        internal BfField(BfCache cache, FieldDefinition fieldDef, BfType type)
            : base(cache, fieldDef, type)
        {
            _fieldDefinition = fieldDef;
            IsInternal = _fieldDefinition.IsAssembly;
            IsProtected = _fieldDefinition.IsFamily;
            IsProtectedAndInternal = _fieldDefinition.IsFamilyAndAssembly;
            IsProtectedOrInternal = _fieldDefinition.IsFamilyOrAssembly;
            IsPrivate = _fieldDefinition.IsPrivate;
            IsPublic = _fieldDefinition.IsPublic;
            IsStatic = _fieldDefinition.IsStatic;
            IsConstant = _fieldDefinition.HasConstant;

            FieldType = _cache.GetBfType(_fieldDefinition.FieldType);

            _typesUsed.Add(FieldType);
            _typesUsed.AddRange(_cache.GetTypeCollection(_fieldDefinition.FieldType));
            _typesUsed.Clear();

            UniqueName = BfCache.GetKey(_fieldDefinition.DeclaringType);
        }

        internal BfField()
        {
        }

        public string FieldId
        {
            // ReSharper disable once UnusedMember.Global
            get { return _fieldId; }
            set { _fieldId = value; }
        }

        private BfType FieldType { get; }

        public MethodCollection SetByMethods { get; } = new MethodCollection();

        public MethodCollection GotByMethods { get; } = new MethodCollection();

        private bool IsPublic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsPublic) == FieldBools.IsPublic; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsPublic;
                else
                    _fieldBools &= FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsInternal) == FieldBools.IsInternal; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsInternal;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsProtected
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsProtected) == FieldBools.IsProtected; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsProtected;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsProtectedOrInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsProtectedOrInternal) == FieldBools.IsProtectedOrInternal; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsProtectedOrInternal;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsProtectedAndInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsProtectedAndInternal) == FieldBools.IsProtectedAndInternal; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsProtectedAndInternal;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsPrivate | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsPrivate
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsPrivate) == FieldBools.IsPrivate; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsPrivate;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsStatic | FieldBools.IsConstant;
            }
        }

        private bool IsStatic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsStatic) == FieldBools.IsStatic; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsStatic;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsConstant;
            }
        }

        private bool IsConstant
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_fieldBools & FieldBools.IsConstant) == FieldBools.IsConstant; }
            set
            {
                if (value)
                    _fieldBools |= FieldBools.IsConstant;
                else
                    _fieldBools &= FieldBools.IsPublic | FieldBools.IsInternal | FieldBools.IsProtected | FieldBools.IsProtectedOrInternal | FieldBools.IsProtectedAndInternal | FieldBools.IsPrivate | FieldBools.IsStatic;
            }
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private string UniqueName { get; }

        public void Dispose()
        {
            _fieldDefinition = null;
        }

        public override string ToString()
        {
            return $"IField: {FullName}";
        }

        [Flags]
        private enum FieldBools : byte
        {
            None = 0,
            IsPublic = (byte) 1,
            IsInternal = (byte) 2,
            IsProtected = (byte) 4,
            IsProtectedOrInternal = (byte) 8,
            IsProtectedAndInternal = (byte) 16,
            IsPrivate = (byte) 32,
            IsStatic = (byte) 64,
            IsConstant = (byte) 128
        }
    }
}