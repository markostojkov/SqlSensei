namespace SqlSensei.Api.Storage
{
    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Server> Servers { get; set; }
        public Company(string name)
        {
            Name = name;
        }

        public Company()
        {
            Name = string.Empty;
        }
    }
}
