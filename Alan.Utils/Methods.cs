using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alan.Utils
{
    /// <summary>
    /// 实用方法
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// 获取指定长度的随机数
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns></returns>
        public static string GenerateRandom(int length)
        {
            var guidCount = (length / 32) + 1;
            var random = new StringBuilder();
            for (var i = 0; i < guidCount; i++)
            {
                random.Append(Guid.NewGuid().ToString().Replace("-", ""));
            }
            var result = random.ToString().Substring(0, length);
            return result;
        }
    }
}
