using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormUI.FieldObjects;
using FormUI.Predictions;
using FormUICore.Predictions;

namespace FormUI.Infrastructure
{
    public static class PredictionSettings
    {
        public static IDictionary<PredictionType, CheckBox> Checkboxes { get; set; } = new Dictionary<PredictionType, CheckBox>();

        public static void Init(PredictionType type, CheckBox checkbox)
        {
            Checkboxes[type] = checkbox;
        }

        public static void SetVisible(PredictionType type, bool value)
        {
            if (Checkboxes.ContainsKey(type))
            {
                Checkboxes[type].Checked = value;
            }
        }

        public static bool GetVisible(PredictionType type)
        {
            if (Checkboxes.ContainsKey(type))
            {
                return Checkboxes[type].Checked;
            }

            return false;
        }
    }
}
