namespace ReadyPlayerMe
{
    public struct Response
    {
        public string Text;
        public byte[] Data;
        public string LastModified;

        public Response(string text, byte[] data, string lastModified)
        {
            Text = text;
            Data = data;
            LastModified = lastModified;
        }
    }
}
