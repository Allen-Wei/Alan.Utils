using Alan.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Alan.Utils.ExtensionMethods
{
    public static class ExReflection
    {

        #region Override map

        /// <summary>
        /// 重写属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="callback">嵌套循环里的回调 (TDestination Property, TSource Property)</param>
        public static void ExOverride<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            Action<PropertyInfo, PropertyInfo> callback)
        {
            if (source == null || destination == null) return;
            var sourceProperties = source.GetType().GetProperties().ToList();
            var destinationProperties = destination.GetType().GetProperties().ToList();

            sourceProperties.ForEach(srcProp =>
            {
                destinationProperties.ForEach(dstProp =>
                {
                    callback(dstProp, srcProp);
                });
            });
        }

        /// <summary>
        /// 重写属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="callback">判断是否应该重写的回调</param>
        public static void ExOverride<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            Func<PropertyInfo, PropertyInfo, bool> callback)
        {
            destination.ExOverride(source, (dstProp, srcProp) =>
            {
                if (callback(dstProp, srcProp))
                {
                    dstProp.SetValue(destination, srcProp.GetValue(source, null), null);
                }
            });
        }


        /// <summary>
        ///  简单重写属性(根据属性名和属性类型匹配)
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        public static void ExOverrideSimple<TDestination, TSource>(this TDestination destination, TSource source)
        {
            destination.ExOverrideSimple(source, (dstProp, srcProp) => true);
        }




        /// <summary>
        /// 简单重写属性(根据属性名和属性类型匹配和callback匹配) 
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="callback">条件回调 (TDestination Property, TSource Property)</param>
        public static void ExOverrideSimple<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            Func<PropertyInfo, PropertyInfo, bool> callback)
        {
            destination.ExOverride(source, (dstProp, srcProp) =>
                srcProp.GetValue(source, null) != null &&
                dstProp.Name == srcProp.Name &&
                dstProp.CanWrite &&
                callback(dstProp, srcProp));
        }

        /// <summary>
        /// 重写值排除指定的属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="excludes">需要排除的属性名称</param>
        public static void ExOverrideExclude<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            params string[] excludes)
        {
            destination.ExOverrideSimple(source,
                (dstProp, srcProp) => excludes.All(excludeProp => excludeProp.ToLower() != dstProp.Name.ToLower()));
        }

        /// <summary>
        /// 重写值排除指定的属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="properties">需要排除的属性列表表达式</param>
        public static void ExOverrideExclude<TDestination, TSource>( 
            this TDestination destination, 
            TSource source, 
            Expression<Func<TDestination, object>> properties)
        {
            var names = CollectNames(properties);
            destination.ExOverrideExclude(source, names);
        }

        /// <summary>
        /// 重写值包含指定的属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="includes">包含的属性名</param>
        public static void ExOverrideInclude<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            params string[] includes)
        {
            destination.ExOverrideSimple(source,
                (dstProp, srcProp) => includes.Any(include => include.ToLower() == dstProp.Name.ToLower()));
        }


        /// <summary>
        /// 重写值包含指定的属性
        /// </summary>
        /// <typeparam name="TDestination">目标类型(this的类型)</typeparam>
        /// <typeparam name="TSource">值来源类型</typeparam>
        /// <param name="destination">目标对象(被重写的对象)</param>
        /// <param name="source">值来源对象</param>
        /// <param name="expression">属性列表表达式</param>
        public static void ExOverrideInclude<TDestination, TSource>(
            this TDestination destination,
            TSource source,
            Expression<Func<TDestination, object>> properties)
        {
            var names = CollectNames(properties);
            destination.ExOverrideInclude(source, names);
        }

        /// <summary>
        /// 通过Lambda表达式获取传递进来的属性的名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string[] CollectNames<T>(Expression<Func<T, object>> expression)
        {
            var exp = expression.Body as NewExpression;
            if (exp == null) return new string[0];
            var names = exp.Members.Select(mem => mem.Name).ToArray();
            return names;
        }
        #endregion


        /// <summary>
        /// 动态获取对象属性值
        /// </summary>
        /// <typeparam name="TCurrent">当前对象类型</typeparam>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="defWhenFail">当获取失败时的值</param>
        /// <returns></returns>
        public static TOut ExGetPropValue<TCurrent, TOut>(this TCurrent current,
            string propertyName,
            TOut defWhenFail = default(TOut))
        {
            try
            {
                if (current == null) return defWhenFail;
                var property = current.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
                if (property == null) return defWhenFail;

                var value = property.GetValue(current, null);
                return value.ExChangeType(defWhenFail);
            }
            catch
            {
                return defWhenFail;
            }
        }

        /// <summary>
        /// 动态设置对象的值
        /// </summary>
        /// <typeparam name="TCurrent">当前对象类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        public static bool ExSetPropValue<TCurrent>(this TCurrent current, string propertyName, object value)
        {
            var property = current.GetType().GetProperties().FirstOrDefault(prop => prop.Name == propertyName);
            if (property == null) return false;
            property.SetValue(current, value, null);
            return true;
        }



        /// <summary>
        /// 将一个对象转换成字典
        /// </summary>
        /// <typeparam name="TCurrent">当前对象类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <returns>字典</returns>
        public static Dictionary<string, object> ExToDictionary<TCurrent>(this TCurrent current)
        {
            return current.ExToDictionary(null);
        }

        /// <summary>
        /// 将一个对象转换成字典, 并附加额外的字典数据
        /// </summary>
        /// <typeparam name="TCurrent">当前对象类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <param name="append">额外的字典数据</param>
        /// <returns>字典</returns>
        public static Dictionary<string, object> ExToDictionary<TCurrent>(
            this TCurrent current,
            Dictionary<string, object> append)
        {
            if (current == null) return new Dictionary<string, object>();
            var query = from property in current.GetType().GetProperties()
                        where property.CanRead
                        let Value = property.GetValue(current, null)
                        select new { property.Name, Value };
            var dicts = new Dictionary<string, object>();
            query.ToList().ForEach(nv => dicts.Add(nv.Name, nv.Value));
            if (append != null)
            {
                append.ExForEach(appendDict =>
                {
                    if (!dicts.ContainsKey(appendDict.Key))
                    {
                        dicts.Add(appendDict.Key, appendDict.Value);
                    }
                });
            }

            return dicts;
        }

        /// <summary>
        /// 将数组转换成字典列表
        /// </summary>
        /// <typeparam name="TCurrent"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> ExToListDictionary<TCurrent>(this IEnumerable<TCurrent> current)
        {
            return current.Select(obj => obj.ExToDictionary()).ToList();
        }

    }
}
