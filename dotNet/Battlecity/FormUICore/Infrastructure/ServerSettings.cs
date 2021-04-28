namespace FormUICore.Infrastructure
{
    public static class ServerSettings
    {
        private static readonly ServerSettingsModel _default = new ServerSettingsModel();
        private static ServerSettingsModel _fromServer;

        public static ServerSettingsModel Settings
        {
            get => _fromServer ?? _default;
            set => _fromServer = value;
        }
    }
}
