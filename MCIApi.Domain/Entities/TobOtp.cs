namespace MCIApi.Domain.Entities
{
    public class TobOtp
    {
        public int Id { get; set; }
        public required string OTP { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ExpireAt { get; set; }
        public bool IsVerified { get; set; }
    }
}


