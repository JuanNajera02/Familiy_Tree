using System;
using System.Collections.Generic;

namespace Entities.Entities
{
    public class Persons
    {
 
        public int Id { get; set; }

        public string PersonName { get; set; }

        public int? PartnerId { get; set; }

        public virtual Persons Partner { get; set; }

        public virtual ICollection<Persons> Partners { get; set; }

        public virtual ICollection<Relationships> Children { get; set; }

        public virtual ICollection<Relationships> Parents { get; set; }
    }
}
