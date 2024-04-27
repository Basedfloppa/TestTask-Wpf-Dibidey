namespace TestTask_Dibidey.models
{
    internal class address
    {
        public Guid uuid {  get; set; }
        public string address_text { get; set; } = null!;
        public Guid abonent { get; set; }
    }
}
