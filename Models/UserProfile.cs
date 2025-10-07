namespace WhatsAppConsume.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
