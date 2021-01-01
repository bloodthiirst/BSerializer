namespace BSerializer.UnitTest.Model
{
    public class Person : IPerson
    {
        public int age;
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public Person Parent { get; set; }
    }
}
