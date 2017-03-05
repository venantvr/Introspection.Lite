using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [DebuggerDisplay("BfType: {FullName}")]
    [Serializable]
    public class BfType
    {
        private readonly BfCache _bfCache;

        [NonSerialized]
        // ReSharper disable once InconsistentNaming
        private readonly TypeDefinition _typeDefinition;

        private TypeBools _typeBools;
        private string _typeId;

        internal BfType(BfCache cache, TypeDefinition type, BfAssembly assembly)
        {
            _typeDefinition = type;
            _bfCache = cache;

            Name = _typeDefinition.Name;
            Assembly = assembly;
            Namespace = cache.GetNamespace(this);

            IsClass = _typeDefinition.IsClass;
            IsEnum = _typeDefinition.IsEnum;
            IsValueType = _typeDefinition.IsValueType;
            IsInterface = _typeDefinition.IsInterface;

            if (_typeDefinition.IsNestedAssembly)
            {
                IsNested = true;
                IsInternal = true;
            }

            if (_typeDefinition.IsNestedFamily)
            {
                IsNested = true;
                IsProtected = true;
            }

            if (_typeDefinition.IsNestedFamilyAndAssembly)
            {
                IsNested = true;
                IsProtectedAndInternal = true;
            }

            if (_typeDefinition.IsNestedFamilyOrAssembly)
            {
                IsNested = true;
                IsProtectedOrInternal = true;
            }

            if (_typeDefinition.IsNestedPrivate)
            {
                IsNested = true;
                IsPrivate = true;
            }

            if (_typeDefinition.IsNestedPublic)
            {
                IsNested = true;
                IsPublic = true;
            }

            IsInternal = _typeDefinition.IsNotPublic;
            IsPublic = _typeDefinition.IsPublic;

            if ((!_typeDefinition.IsSealed ? 1 : (!_typeDefinition.IsAbstract ? 1 : 0)) == 0)
            {
                IsStatic = true;
            }
            else
            {
                IsSealed = _typeDefinition.IsSealed;
                IsAbstract = _typeDefinition.IsAbstract;
            }
        }

        internal BfType()
        {
        }

        public string TypeId
        {
            get { return _typeId; }
            internal set { _typeId = value; }
        }

        private BfType BaseType { get; set; }

        public FieldCollection Fields { get; } = new FieldCollection();

        public MethodCollection Methods { get; } = new MethodCollection();

        public EventCollection Events { get; } = new EventCollection();

        private TypeCollection Interfaces { get; } = new TypeCollection();

        private TypeCollection TypesUsing { get; } = new TypeCollection();

        private TypeCollection TypesUsed { get; } = new TypeCollection();

        public TypeCollection DerivedTypes { get; } = new TypeCollection();

        public BfAssembly Assembly { get; }

        public string FullName => (Namespace == null ? "" : Namespace.Name + ".") + Name;

        private bool IsAbstract
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 1) == (TypeBools) 1; }

            set
            {
                if (value)
                    _typeBools |= (TypeBools) 1;
                else
                    _typeBools &= (TypeBools) (-2 % short.MaxValue);
            }
        }

        private bool IsClass
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 2) == (TypeBools) 2; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 2;
                else
                    _typeBools &= (TypeBools) (-3 % short.MaxValue);
            }
        }

        public bool IsDelegate
        {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedMember.Global
            get { return (_typeBools & (TypeBools) 16384) == (TypeBools) 16384; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 16384;
                else
                    _typeBools &= (TypeBools) (-16385 % short.MaxValue);
            }
        }

        private bool IsEnum
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 4) == (TypeBools) 4; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 4;
                else
                    _typeBools &= (TypeBools) (-5 % short.MaxValue);
            }
        }

        public bool IsInCoreAssembly => Assembly.IsCoreAssembly;

        private bool IsInterface
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 16) == (TypeBools) 16; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 16;
                else
                    _typeBools &= (TypeBools) (-17 % short.MaxValue);
            }
        }

        private bool IsInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 64) == (TypeBools) 64; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 64;
                else
                    _typeBools &= (TypeBools) (-65 % short.MaxValue);
            }
        }

        private bool IsNested
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 2048) == (TypeBools) 2048; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 2048;
                else
                    _typeBools &= (TypeBools) (-2049 % short.MaxValue);
            }
        }

        private bool IsPrivate
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 1024) == (TypeBools) 1024; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 1024;
                else
                    _typeBools &= (TypeBools) (-1025 % short.MaxValue);
            }
        }

        private bool IsProtected
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 128) == (TypeBools) 128; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 128;
                else
                    _typeBools &= (TypeBools) (-129 % short.MaxValue);
            }
        }

        private bool IsProtectedAndInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 512) == (TypeBools) 512; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 512;
                else
                    _typeBools &= (TypeBools) (-513 % short.MaxValue);
            }
        }

        private bool IsProtectedOrInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 256) == (TypeBools) 256; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 256;
                else
                    _typeBools &= (TypeBools) (-257 % short.MaxValue);
            }
        }

        private bool IsPublic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 4096) == (TypeBools) 4096; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 4096;
                else
                    _typeBools &= (TypeBools) (-4097 % short.MaxValue);
            }
        }

        private bool IsSealed
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 8192) == (TypeBools) 8192; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 8192;
                else
                    _typeBools &= (TypeBools) (-8193 % short.MaxValue);
            }
        }

        private bool IsStatic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 32) == (TypeBools) 32; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 32;
                else
                    _typeBools &= (TypeBools) (-33 % short.MaxValue);
            }
        }

        private bool IsValueType
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_typeBools & (TypeBools) 8) == (TypeBools) 8; }
            set
            {
                if (value)
                    _typeBools |= (TypeBools) 8;
                else
                    _typeBools &= (TypeBools) (-9 % short.MaxValue);
            }
        }

        public string Name { get; }

        public BfNamespace Namespace { get; }

        public override string ToString()
        {
            return $"IType: {FullName}";
        }

        [SpecialName]
        internal TypeDefinition GetTypeDefinition()
        {
            return _typeDefinition;
        }

        internal void Populate()
        {
            if (IsInCoreAssembly)
            {
                BaseType = _bfCache.GetBfType(_typeDefinition.BaseType);

                if (BaseType != null && BaseType.FullName != "System.Object")
                {
                    BaseType.DerivedTypes.Add(this);
                }

                _typeDefinition.Interfaces.Select(t => _bfCache.GetBfType(t))
                    .Where(t => t != null)
                    .ForEach(t =>
                             {
                                 Interfaces.Add(t);
                                 t.DerivedTypes.Add(this);
                             });

                _typeDefinition.Fields.ForEach(f => Fields.Add(new BfField(_bfCache, f, this)));

                _typeDefinition.Methods.ForEach(m =>
                                                {
                                                    var bfMethod = new BfMethod(_bfCache, m, this);
                                                    Methods.Add(bfMethod);
                                                    Assembly.GetMethodsDictionary().Add(bfMethod.UniqueName, bfMethod);
                                                });

                _typeDefinition.Events.ForEach(e => Events.Add(new BfEvent(_bfCache, e, this)));

                Interfaces.Clear();
                Fields.Clear();
                Methods.Clear();
                Events.Clear();
            }
        }

        public void Commit()
        {
            if (BaseType != null && BaseType == _bfCache.Types.First(t => t.FullName == "System.Object"))
            {
                BaseType.Commit();

                Append(BaseType);

                BaseType.TypesUsed.ForEach(Append);
            }

            Interfaces.ForEach(Append);

            Fields.ForEach(t => t.TypesUsed.ForEach(Append));
            Methods.ForEach(t => t.TypesUsed.ForEach(Append));
            Events.ForEach(t => t.TypesUsed.ForEach(Append));

            TypesUsed.Clear();
        }

        private void Append(BfType bfType)
        {
            if (bfType == null) return;

            if (Assembly.IsCoreAssembly)
            {
                TypesUsed.Add(bfType);
            }

            bfType.TypesUsing.Add(this);
        }

        [Flags]
        private enum TypeBools : short
        {
        }
    }
}