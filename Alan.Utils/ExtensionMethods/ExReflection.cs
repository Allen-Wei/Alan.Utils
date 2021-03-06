﻿using System;
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
        /// <param name="properties">属性列表表达式</param>
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
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="current">当前对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="defWhenFail">当获取失败时的值</param>
        /// <returns></returns>
        public static TOut ExGetPropValue<TOut>(this object current,
            string propertyName,
            TOut defWhenFail = default(TOut))
        {
            try
            {
                if (current == null) return defWhenFail;
                var property = current.GetType().GetProperties().FirstOrDefault(p => p.CanRead && p.Name == propertyName);
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
        /// <param name="current">当前对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns>是否设置成功</returns>
        public static bool ExSetPropValue(this object current, string propertyName, object value)
        {
            if (current == null) return false;

            var property = current.GetType().GetProperties().FirstOrDefault(prop => prop.CanWrite && prop.Name == propertyName);
            if (property == null) return false;
            property.SetValue(current, value, null);
            return true;
        }


        public static T ExToModel<T>(this Dictionary<string, object> dict)
            where T : new()
        {
            var model = new T();
            typeof(T)
               .GetProperties()
               .Where(property => property.CanWrite)
               .ToList()
               .ForEach(property => property.SetValue(model, dict[property.Name], null));

            return model;
        }

        public static IEnumerable<T> ExToModels<T>(this IEnumerable<Dictionary<string, object>> dicts)
            where T : new()
        {
            return dicts.Select(dict => dict.ExToModel<T>());
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
            return current.ExToListDictionary(eachCallback: (cur, dict) => dict);
        }

        /// <summary>
        /// 将对象数组映射到字典列表
        /// </summary>
        /// <typeparam name="T">当前对象的类型</typeparam>
        /// <param name="current">当前对象数组</param>
        /// <param name="eachCallback">回调</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ExToListDictionary<T>(this IEnumerable<T> current,
            Action<Dictionary<string, object>> eachCallback)
        {
            return current.ExToListDictionary(eachCallback: (cur, dict) => eachCallback(dict));
        }

        /// <summary>
        /// 将对象数组映射到字典列表
        /// </summary>
        /// <typeparam name="T">当前对象的类型</typeparam>
        /// <param name="current">当前对象数组</param>
        /// <param name="eachCallback">回调</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ExToListDictionary<T>(this IEnumerable<T> current,
            Action<T, Dictionary<string, object>> eachCallback)
        {
            var dicts = new List<Dictionary<string, object>>();
            current.ExForEach(obj =>
                        {
                            var dict = obj.ExToDictionary();
                            eachCallback(obj, dict);
                            dicts.Add(dict);
                        });
            return dicts;
        }

        /// <summary>
        /// 将对象数组的属性映射到字典列表
        /// </summary>
        /// <typeparam name="TInput">对象类型</typeparam>
        /// <typeparam name="TOutput">输出类型</typeparam>
        /// <param name="current">当前对象数组</param>
        /// <param name="eachCallback">每个字典的操作</param>
        /// <returns></returns>
        public static List<TOutput> ExToListDictionary<TInput, TOutput>(this IEnumerable<TInput> current,
           Func<TInput, Dictionary<string, object>, TOutput> eachCallback)
        {
            return current.Select(cur => eachCallback(cur, cur.ExToDictionary())).ToList();
        }


        /// <summary>
        /// 收集对象所有可读属性的值列表
        /// </summary>
        /// <typeparam name="TOutput">对象类型</typeparam>
        /// <param name="current">当前待收集对象</param>
        /// <param name="fail">当转换失败时的代替值</param>
        /// <param name="filter">过滤操作</param>
        /// <returns></returns>
        public static IEnumerable<TOutput> ExGetPropValues<TOutput>(this object current, TOutput fail, Func<PropertyInfo, bool> filter = null)
        {
            var properties = current.GetType().GetProperties().Where(property => property.CanRead);
            if (filter != null) properties = properties.Where(filter);

            return properties.Select(property => current.ExGetPropValue<TOutput>(property.Name, fail));
        }

        /// <summary>
        /// 收集对象所有的属性名称
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static string[] ExGetPropNames(this object current)
        {
            return current.GetType().GetProperties().Select(property => property.Name).ToArray();
        }

        /// <summary>
        /// 收集泛型T的所有属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] ExGetPropNames<T>()
        {
            return typeof(T).GetProperties().Select(property => property.Name).ToArray();
        }
    }
}
