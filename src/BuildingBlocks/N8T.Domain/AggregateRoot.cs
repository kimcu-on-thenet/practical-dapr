using System.Collections.Generic;

namespace N8T.Domain
{
    public abstract class AggregateRoot : EntityBase
    {
        protected AggregateRoot(IdentityBase id) : base(id) { }

        protected AggregateRoot() { }

        public HashSet<IDomainEvent> DomainEvents { get; private set; }

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            DomainEvents ??= new HashSet<IDomainEvent>();
            DomainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            DomainEvents?.Remove(eventItem);
        }
    }
}