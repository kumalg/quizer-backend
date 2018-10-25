namespace quizer_backend.Services {
    public class ExternalAuthOptions {
        public double JwtExpirationInSeconds { get; internal set; }
        public string ManagementClientId { get; internal set; }
        public string ManagementClientSecret { get; internal set; }
        public object Domain { get; internal set; }
    }
}