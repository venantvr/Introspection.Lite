using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

[assembly: InternalsVisibleTo("Introspection.Lite.Tests")]

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [DebuggerDisplay("Method: {Name}")]
    [Serializable]
    public class BfMethod : BfMember, IDisposable
    {
        private MethodBools _methodBools;

        [NonSerialized] private MethodDefinition _methodDefinition;
        private string _methodId;
        private string _methodName;

        internal BfMethod(BfCache cache, MethodDefinition methodDef, BfType type)
            : base(cache, methodDef, type)
        {
            _methodDefinition = methodDef;
            _methodName = GetSignature(_methodDefinition);

            if (!type.IsInCoreAssembly)
                return;

            ReturnType = cache.GetBfType(methodDef.ReturnType);

            _typesUsed.AddRange(_cache.GetTypeCollection(methodDef.ReturnType));
            _typesUsed.Add(ReturnType);
            _typesUsed.AddRange(_cache.GetTypeCollection(_methodDefinition));

            if (methodDef.Body != null)
            {
                foreach (var variableDefinition in methodDef.Body.Variables)
                {
                    _typesUsed.AddRange(_cache.GetTypeCollection(variableDefinition.VariableType));
                    _typesUsed.Add(_cache.GetBfType(variableDefinition.VariableType));
                }
            }

            foreach (var parameterDefinition in methodDef.Parameters)
            {
                _typesUsed.AddRange(_cache.GetTypeCollection(parameterDefinition.ParameterType));
                _typesUsed.Add(_cache.GetBfType(parameterDefinition.ParameterType));

                ParameterTypes.AddRange(_cache.GetTypeCollection(parameterDefinition.ParameterType));
                ParameterTypes.Add(_cache.GetBfType(parameterDefinition.ParameterType));
            }

            ParameterTypes.Clear();
        }

        internal BfMethod()
        {
        }

        public string MethodId
        {
            get { return _methodId; }
            set { _methodId = value; }
        }

        private TypeCollection ParameterTypes { get; } = new TypeCollection();

        private BfType ReturnType { get; }

        private bool IsPublic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsPublic) == MethodBools.IsPublic; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsPublic;
                else
                    _methodBools &= ~MethodBools.IsPublic;
            }
        }

        private bool IsInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsInternal) == MethodBools.IsInternal; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsInternal;
                else
                    _methodBools &= ~MethodBools.IsInternal;
            }
        }

        private bool IsProtected
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsProtected) == MethodBools.IsProtected; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsProtected;
                else
                    _methodBools &= ~MethodBools.IsProtected;
            }
        }

        private bool IsProtectedOrInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsProtectedOrInternal) == MethodBools.IsProtectedOrInternal; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsProtectedOrInternal;
                else
                    _methodBools &= ~MethodBools.IsProtectedOrInternal;
            }
        }

        private bool IsProtectedAndInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsProtectedAndInternal) == MethodBools.IsProtectedAndInternal; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsProtectedAndInternal;
                else
                    _methodBools &= ~MethodBools.IsProtectedAndInternal;
            }
        }

        private bool IsPrivate
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsPrivate) == MethodBools.IsPrivate; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsPrivate;
                else
                    _methodBools &= ~MethodBools.IsPrivate;
            }
        }

        public bool IsConstructor
        {
            get { return (_methodBools & MethodBools.IsConstructor) == MethodBools.IsConstructor; }
            private set
            {
                if (value)
                    _methodBools |= MethodBools.IsConstructor;
                else
                    _methodBools &= ~MethodBools.IsConstructor;
            }
        }

        public bool IsPropertyGetter
        {
            get { return (_methodBools & MethodBools.IsPropertyGetter) == MethodBools.IsPropertyGetter; }
            private set
            {
                if (value)
                    _methodBools |= MethodBools.IsPropertyGetter;
                else
                    _methodBools &= ~MethodBools.IsPropertyGetter;
            }
        }

        public bool IsPropertySetter
        {
            get { return (_methodBools & MethodBools.IsPropertySetter) == MethodBools.IsPropertySetter; }
            private set
            {
                if (value)
                    _methodBools |= MethodBools.IsPropertySetter;
                else
                    _methodBools &= ~MethodBools.IsPropertySetter;
            }
        }

        private bool IsStatic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsStatic) == MethodBools.IsStatic; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsStatic;
                else
                    _methodBools &= ~MethodBools.IsStatic;
            }
        }

        private bool IsVirtual
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsVirtual) == MethodBools.IsVirtual; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsVirtual;
                else
                    _methodBools &= ~MethodBools.IsVirtual;
            }
        }

        private bool IsGeneric
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsGeneric) == MethodBools.IsGeneric; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsGeneric;
                else
                    _methodBools &= ~MethodBools.IsGeneric;
            }
        }

        private bool IsOperator
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsOperator) == MethodBools.IsOperator; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsOperator;
                else
                    _methodBools &= ~MethodBools.IsOperator;
            }
        }

        private bool IsIndexGetter
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsIndexGetter) == MethodBools.IsIndexGetter; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsIndexGetter;
                else
                    _methodBools &= ~MethodBools.IsIndexGetter;
            }
        }

        private bool IsIndexSetter
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsIndexSetter) == MethodBools.IsIndexSetter; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsIndexSetter;
                else
                    _methodBools &= ~MethodBools.IsIndexSetter;
            }
        }

        private bool IsEventAdder
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsEventAdder) == MethodBools.IsEventAdder; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsEventAdder;
                else
                    _methodBools &= ~MethodBools.IsEventAdder;
            }
        }

        private bool IsEventRemover
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsEventRemover) == MethodBools.IsEventRemover; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsEventRemover;
                else
                    _methodBools &= ~MethodBools.IsEventRemover;
            }
        }

        private bool IsStaticConstructor
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_methodBools & MethodBools.IsStaticConstructor) == MethodBools.IsStaticConstructor; }
            set
            {
                if (value)
                    _methodBools |= MethodBools.IsStaticConstructor;
                else
                    _methodBools &= ~MethodBools.IsStaticConstructor;
            }
        }

        public bool IsProperty => IsPropertyGetter || IsPropertySetter;

        public MethodCollection Calls { get; } = new MethodCollection();

        public MethodCollection CalledBy { get; } = new MethodCollection();

        private FieldCollection FieldSets { get; } = new FieldCollection();

        public FieldCollection FieldGets { get; } = new FieldCollection();

        [SpecialName]
        private string Signature
        {
            get
            {
                if (_methodName != null)
                {
                }

                return _methodName;
            }
        }

        public string UniqueName => Signature;

        public void Dispose()
        {
            _methodDefinition = null;
            _methodName = null;
        }

        public override string ToString()
        {
            return $"IMethod: {FullName}";
        }

        internal static string GetSignature(MethodReference methodReference)
        {
            //var sentinel = -1;
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(methodReference.ReturnType.FullName);
            stringBuilder.Append(" ");

            if (methodReference.DeclaringType != null)
            {
                stringBuilder.Append(methodReference.DeclaringType.FullName + "::");
            }

            stringBuilder.Append(methodReference.Name);

            if (methodReference.GenericParameters.Count > 0)
            {
                stringBuilder.Append("<");
            }

            for (var index = 0; index < methodReference.GenericParameters.Count; ++index)
            {
                if (index > 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(methodReference.GenericParameters[index].Name);
            }

            if (methodReference.GenericParameters.Count > 0)
            {
                stringBuilder.Append(">");
            }

            stringBuilder.Append("(");

            for (var index = 0; index < methodReference.Parameters.Count; ++index)
            {
                if (index > 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(",");

                if (methodReference.Parameters[index].ParameterType != null)
                {
                    stringBuilder.Append(methodReference.Parameters[index].ParameterType.FullName);
                }
            }

            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }

        internal void Populate()
        {
            IsConstructor = _methodDefinition.IsConstructor;
            IsVirtual = _methodDefinition.IsVirtual;
            IsGeneric = _methodDefinition.GenericParameters.Count > 0;
            IsInternal = _methodDefinition.IsAssembly;
            IsProtected = _methodDefinition.IsFamily;
            IsProtectedAndInternal = _methodDefinition.IsFamilyAndAssembly;
            IsProtectedOrInternal = _methodDefinition.IsFamilyOrAssembly;
            IsPrivate = _methodDefinition.IsPrivate;
            IsPublic = _methodDefinition.IsPublic;
            IsStatic = _methodDefinition.IsStatic;
            IsStaticConstructor = _methodDefinition.IsConstructor && _methodDefinition.IsStatic;

            PopulateSpecialNames();

            if (!_methodDefinition.HasBody) return;
            if (!_type.Assembly.IsCoreAssembly) return;

            var hashSet = new HashSet<int>();

            SequencePoint first = null;

            foreach (var instruction in _methodDefinition.Body.Instructions)
            {
                if (instruction.SequencePoint != null)
                {
                    var file = _cache.File(instruction);

                    if ((first != null ? 1 : (!file.method_4(instruction.SequencePoint) ? 1 : 0)) == 0)
                    {
                        first = instruction.SequencePoint;
                    }
                }

                switch (instruction.OpCode.Code)
                {
                    case Code.Box:
                    case Code.Unbox_Any:
                    case Code.Unbox:
                        break;
                    case Code.Jmp:
                        break;
                    case Code.Call:
                    case Code.Callvirt:
                    case Code.Newobj:
                        ProcessObject(instruction);
                        break;
                    case Code.Calli:
                        break;
                    case Code.Ldfld:
                    case Code.Ldflda:
                    case Code.Ldsfld:
                    case Code.Ldsflda:
                        ProcessField(instruction, f => f.GotByMethods);
                        break;
                    case Code.Stfld:
                    case Code.Stsfld:
                        ProcessField(instruction, f => f.SetByMethods);
                        break;
                }
                switch (instruction.OpCode.FlowControl)
                {
                    case FlowControl.Branch:
                    case FlowControl.Cond_Branch:
                        hashSet.Add(instruction.Offset);
                        continue;
                    default:
                        continue;
                }
            }

            _typesUsed.Clear();
        }

        private void ProcessObject(Instruction instruction)
        {
            var operand = instruction.Operand as MethodReference;

            if (operand != null)
            {
                var bfMethod = !(operand.DeclaringType is GenericInstanceType)
                    ? _cache.GetBfMethod(operand)
                    : GetBfMethod(operand);

                if (bfMethod != null)
                {
                    _typesUsed.Add(bfMethod.Type);
                    _typesUsed.Add(bfMethod.ReturnType);
                    _typesUsed.AddRange(bfMethod.ParameterTypes);
                    _typesUsed.AddRange(_cache.GetTypeCollection(operand));

                    Calls.Add(bfMethod);

                    bfMethod.CalledBy.Add(this);
                }
            }
        }

        private void ProcessField(Instruction instruction, Func<BfField, MethodCollection> methods)
        {
            var reference = instruction.Operand as FieldReference;
            var field = _cache.GetBfField(instruction.Operand as FieldReference);

            if (field != null)
            {
                FieldSets.Add(field);
                methods.Invoke(field).Add(this);
                _typesUsed.Add(field.Type);
                if (reference != null) _typesUsed.AddRange(_cache.GetTypeCollection(reference.FieldType));
            }
        }

        private BfMethod GetBfMethod(MethodReference methodReference)
        {
            var bfType = _cache.GetBfType(((TypeSpecification) methodReference.DeclaringType).ElementType);
            var hashSet = new HashSet<string>();

            BfMethod bfMethod = null;

            if (bfType != null)
            {
                bfType.GetTypeDefinition().GenericParameters.ForEach(g => hashSet.Add(g.FullName));

                MethodReference definition = null;
                var list = new List<MethodReference>();

                list.AddRange(bfType.GetTypeDefinition().Methods);

                foreach (var reference in list)
                {
                    var methodDefinition = (MethodDefinition) reference;

                    if (methodDefinition.Name == methodReference.Name && methodDefinition.Parameters.Count == methodReference.Parameters.Count)
                    {
                        var flag = true;

                        for (var index = 0; index < methodDefinition.Parameters.Count; ++index)
                        {
                            var parameterDefinition = methodDefinition.Parameters[index];
                            var parameterReference = methodReference.Parameters[index];

                            if (hashSet.Contains(BfCache.GetTypeReference(parameterDefinition.ParameterType).FullName))
                            {
                                if (!parameterReference.ToString().StartsWith("A_"))
                                    flag = false;
                            }
                            else if (Regex.Replace(parameterReference.ParameterType.FullName, "<[^>]*>", "") != Regex.Replace(parameterDefinition.ParameterType.FullName, "<[^>]*>", ""))
                                flag = false;
                        }

                        if (flag)
                            definition = methodDefinition;
                    }
                }

                if (definition == null)
                {
                }
                else
                {
                    GetSignature(definition); // ???
                    bfMethod = _cache.GetBfMethod(definition);
                }
            }
            return bfMethod;
        }

        private void PopulateSpecialNames()
        {
            if (!_methodDefinition.IsSpecialName)
                return;
            if (_methodDefinition.Name.StartsWith("get_Item"))
                IsIndexGetter = true;
            else if (_methodDefinition.Name.StartsWith("set_Item"))
                IsIndexSetter = true;
            if (_methodDefinition.Name.StartsWith("get_"))
                IsPropertyGetter = true;
            else if (_methodDefinition.Name.StartsWith("set_"))
                IsPropertySetter = true;
            else if (_methodDefinition.Name.StartsWith(".ctor"))
                IsConstructor = true;
            else if (_methodDefinition.Name.StartsWith(".cctor"))
                IsConstructor = true;
            else if (_methodDefinition.Name.StartsWith("add_"))
                IsEventAdder = true;
            else if (_methodDefinition.Name.StartsWith("remove_"))
                IsEventRemover = true;
            else if (_methodDefinition.Name.StartsWith("op_"))
                IsOperator = true;
            else if (_methodDefinition.IsNewSlot)
            {
                if (_methodDefinition.Name.Contains("get_Item"))
                    IsIndexGetter = true;
                else if (_methodDefinition.Name.Contains("set_Item"))
                    IsIndexSetter = true;
                if (_methodDefinition.Name.Contains("get_"))
                    IsPropertyGetter = true;
                else if (_methodDefinition.Name.Contains("set_"))
                    IsPropertySetter = true;
                else if (_methodDefinition.Name.Contains(".ctor"))
                    IsConstructor = true;
                else if (_methodDefinition.Name.Contains(".cctor"))
                    IsConstructor = true;
                else if (_methodDefinition.Name.Contains("add_"))
                    IsEventAdder = true;
                else if (_methodDefinition.Name.Contains("remove_"))
                {
                    IsEventRemover = true;
                }
                else
                {
                    if (!_methodDefinition.Name.Contains("op_"))
                        return;
                    IsOperator = true;
                }
            }
        }

        [Flags]
        private enum MethodBools
        {
            //None = 0,
            IsPublic = 1,
            IsInternal = 2,
            IsProtected = 4,
            IsProtectedOrInternal = 8,
            IsProtectedAndInternal = 16,
            IsPrivate = 32,
            IsConstructor = 64,
            IsPropertyGetter = 128,
            IsPropertySetter = 256,
            IsStatic = 512,
            IsVirtual = 1024,
            //UsesBoxing = 2048,
            //UsesUnboxing = 4096,
            IsGeneric = 8192,
            IsOperator = 32768,
            IsIndexGetter = 65536,
            IsIndexSetter = 131072,
            IsEventAdder = 262144,
            IsEventRemover = 524288,
            IsStaticConstructor = 1048576
        }
    }
}