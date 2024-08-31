namespace Family_Tree.DTOs
{
    public class AddPersonDTO
    {
        public string PersonName { get; set; }
        public int? PartnerId { get; set; }
        public List<int>? ChildrenIds { get; set; }  
    }
}