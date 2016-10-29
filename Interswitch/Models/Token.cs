namespace Payment.Models
{
    public class Token
    {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string requestor_id { get; set; }
        public string merchant_code { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string payable_id { get; set; }
        public string payment_code { get; set; }
        public string jti { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public void setAccessToken(string token)
        {
            access_token = token;
        }
    }

}
