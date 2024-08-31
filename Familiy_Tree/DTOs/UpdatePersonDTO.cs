namespace Familiy_Tree.DTOs
{
    public class UpdatePersonDTO
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int? PartnerId { get; set; }
        public List<int>? ChildrenIds { get; set; }
    }
}
