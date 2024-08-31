using System;
using System.Collections.Generic;

namespace Entities.Entities
{
    public class Persons
    {
        public int Id { get; set; }
        public string PersonName { get; set; }

        // Relación uno a uno con Partner
        public int? PartnerId { get; set; }
        public virtual Persons Partner { get; set; }

        // Relación uno a muchos como padre (Father)
        public virtual ICollection<Relationships> FatherRelationships { get; set; } = new List<Relationships>();

        // Relación uno a muchos como madre (Mother)
        public virtual ICollection<Relationships> MotherRelationships { get; set; } = new List<Relationships>();

        // Relación uno a muchos como hijo (Child)
        public virtual ICollection<Relationships> ChildRelationships { get; set; } = new List<Relationships>();
    }
}
