using System;
using System.Linq.Expressions;

namespace Alan.Utils.UtililyMethods
{


    /// <summary>
    /// 获取参数/属性名 类似于 C# 6.0 中的 nameof 表达式
    /// http://stackoverflow.com/questions/301809/workarounds-for-nameof-operator-in-c-typesafe-databinding
    /// </summary>
    public class Nameof<T>
    {
        public static string Property<TProp>(Expression<Func<T, TProp>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("'expression' should be a member expression");
            return body.Member.Name;
        }
    }
}
