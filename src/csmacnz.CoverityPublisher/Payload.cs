namespace csmacnz.CoverityPublisher
{
    public class Payload
    {
        public string FileName { get; set; }
        public string RepositoryName { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public bool SubmitToCoverity { get; set; }
    }
}