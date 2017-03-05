using System;
using System.Diagnostics;
using Mono.Cecil;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Artefacts
{
    [DebuggerDisplay("Event: {Name}")]
    [Serializable]
    public class BfEvent : BfMember, IDisposable
    {
        private EventBools _eventBools;

        [NonSerialized] private EventDefinition _eventDefinition;
        private string _eventId;

        internal BfEvent(BfCache cache, EventDefinition eventDef, BfType type)
            : base(cache, eventDef, type)
        {
            _eventDefinition = eventDef;

            EventType = _cache.GetBfType(_eventDefinition.EventType);

            _typesUsed.Add(EventType);
            _typesUsed.AddRange(_cache.GetTypeCollection(_eventDefinition.EventType));
            _typesUsed.Clear();

            IsInternal = _eventDefinition.AddMethod.IsAssembly;
            IsProtected = _eventDefinition.AddMethod.IsFamily;
            IsPrivate = _eventDefinition.AddMethod.IsPrivate;
            IsPublic = _eventDefinition.AddMethod.IsPublic;
            IsStatic = _eventDefinition.AddMethod.IsStatic;

            _cache.Events.Add(this);
        }

        internal BfEvent()
        {
        }

        public string EventId
        {
            // ReSharper disable once UnusedMember.Global
            get { return _eventId; }
            set { _eventId = value; }
        }

        private BfType EventType { get; }

        private bool IsPublic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_eventBools & EventBools.IsPublic) == EventBools.IsPublic; }
            set
            {
                if (value)
                    _eventBools |= EventBools.IsPublic;
                else
                    _eventBools &= ~EventBools.IsPublic;
            }
        }

        private bool IsInternal
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_eventBools & EventBools.IsInternal) == EventBools.IsInternal; }
            set
            {
                if (value)
                    _eventBools |= EventBools.IsInternal;
                else
                    _eventBools &= ~EventBools.IsInternal;
            }
        }

        private bool IsProtected
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_eventBools & EventBools.IsProtected) == EventBools.IsProtected; }
            set
            {
                if (value)
                    _eventBools |= EventBools.IsProtected;
                else
                    _eventBools &= ~EventBools.IsProtected;
            }
        }

        private bool IsPrivate
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_eventBools & EventBools.IsPrivate) == EventBools.IsPrivate; }
            set
            {
                if (value)
                    _eventBools |= EventBools.IsPrivate;
                else
                    _eventBools &= ~EventBools.IsPrivate;
            }
        }

        private bool IsStatic
        {
            // ReSharper disable once UnusedMember.Local
            get { return (_eventBools & EventBools.IsStatic) == EventBools.IsStatic; }
            set
            {
                if (value)
                    _eventBools |= EventBools.IsStatic;
                else
                    _eventBools &= ~EventBools.IsStatic;
            }
        }

        public void Dispose()
        {
            _eventDefinition = null;
        }

        public override string ToString()
        {
            return $"IEvent: {FullName}";
        }

        [Flags]
        private enum EventBools : byte
        {
            IsPublic = (byte) 1,
            IsInternal = (byte) 2,
            IsProtected = (byte) 4,
            IsPrivate = (byte) 8,
            IsStatic = (byte) 16
        }
    }
}