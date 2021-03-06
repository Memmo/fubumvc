using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;

namespace FubuCore.Configuration
{
    public class SettingsRequestData : IRequestData
    {
        private readonly SettingsStep _environmentStep;
        private readonly SettingsStep _packageStep;
        private readonly SettingsStep _coreStep;
        private readonly SettingsStep[] _steps;


        public SettingsRequestData(IEnumerable<ISettingsData> settingData)
        {
            _environmentStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.environment));
            _packageStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.package));
            _coreStep = new SettingsStep(settingData.Where(x => x.Category == SettingCategory.core));

            _steps = new []{_environmentStep, _packageStep, _coreStep};
        }

        public object Value(string key)
        {
            object returnValue = null;

            Value(key, o => returnValue = o);

            return (returnValue ?? string.Empty).ToString();
        }

        public bool Value(string key, Action<object> callback)
        {
            return _steps.Any(x => x.Value(key, callback));
        }

        public bool HasAnyValuePrefixedWith(string key)
        {
            return _steps.Any(x => x.HasAnyValuePrefixedWith(key));
        }

        public static SettingsRequestData For(params ISettingsData[] data)
        {
            return new SettingsRequestData(data);
        }

        public class SettingsStep
        {
            private readonly IEnumerable<ISettingsData> _settingData;

            public SettingsStep(IEnumerable<ISettingsData> settingData)
            {
                _settingData = settingData;
            }

            public bool HasAnyValuePrefixedWith(string key)
            {
                return _settingData.Any(x => x.AllKeys.Any(k => k.StartsWith(key)));
            }

            public bool Value(string key, Action<object> callback)
            {
                var data = _settingData.FirstOrDefault(x => x.Has(key));
                if (data == null) return false;

                callback(data.Get(key));

                return true;
            }
        }
    }
}