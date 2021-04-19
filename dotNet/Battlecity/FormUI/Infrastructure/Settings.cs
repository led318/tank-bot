using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormUI.Infrastructure
{
    public static class Settings
    {
        private static readonly SettingsModel _default = new();
        private static SettingsModel _fromServer;

        public static SettingsModel Get
        {
            get => _fromServer ?? _default;
            set => _fromServer = value;
        }
    }
}
