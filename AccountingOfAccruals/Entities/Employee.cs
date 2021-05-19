
namespace AccountingOfAccruals.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public int Salary { get; set; }

        public override string ToString()
            => FullName;        
    }
}
