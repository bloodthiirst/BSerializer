namespace ConsoleUI.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Person Parent { get; set; }
    }
}
