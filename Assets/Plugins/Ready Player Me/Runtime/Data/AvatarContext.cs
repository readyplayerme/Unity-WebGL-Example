namespace ReadyPlayerMe
{
    public class AvatarContext : Context
    {
        public string Url;
        public bool SaveInProjectFolder;
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public AvatarUri AvatarUri;
        public AvatarMetadata Metadata;
        public AvatarRenderSettings RenderSettings;
        public byte[] Bytes;
        public object Data;
        public string ParametersHash;
    }
}
