using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Relationships
    {
        public int Id { get; set; }
        public int? FatherId { get; set; }
        public int? MotherId { get; set; }
        public int ChildId { get; set; }

        public virtual Persons Father { get; set; }
        public virtual Persons Mother { get; set; }
        public virtual Persons Child { get; set; }


    }
}
