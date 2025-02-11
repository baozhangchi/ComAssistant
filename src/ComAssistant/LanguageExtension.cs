using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ComAssistant
{
    internal class LanguageExtension
    {
        public string Key { get; set; }

        // 当 ResourceDictionary 中的资源发生变化时，动态更新
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            // 当前语言或文化
            string culture = Lang.Resources.Culture.Name; // 或者可以根据需要获取当前语言设置

            // 加载资源文件
            var resourceManager = new ResourceManager("ComAssistant.Lang.Resources", typeof(LanguageExtension).Assembly);

            // 获取当前语言的资源字符串
            var resourceValue = resourceManager.GetString(Key, new CultureInfo(culture));
            return resourceValue ?? Key; // 如果找不到对应的值，返回默认的 Key
        }
    }
}
