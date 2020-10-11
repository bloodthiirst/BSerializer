using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class SerializerUtils
    {
        /// <summary>
        /// Converts a property getter to a delegate
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Func<object, object> PropertyGetterToDelegate(MethodInfo methodInfo)
        {
            Type classType = methodInfo.DeclaringType;
            Type returnType = methodInfo.ReturnType;

            var obj = Expression.Parameter(typeof(object), "obj");

            var bodyExpression =
                Expression.Convert(

                    Expression.Call(
                        Expression.Convert(obj, classType),
                        methodInfo)

                , typeof(object));



            // wanted result is 
            // (object)((object)instance).GetterInvoke
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                bodyExpression,
                 obj
                );

            return lambda.Compile();
        }

        /// <summary>
        /// Converts a property setter to a delegate
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Action<object, object> PropertySetterToDelegate(MethodInfo methodInfo)
        {
            Type classType = methodInfo.DeclaringType;
            Type paramType = methodInfo.GetParameters()[0].ParameterType;

            var obj = Expression.Parameter(typeof(object), "obj");
            var val = Expression.Parameter(typeof(object), "val");

            var bodyExpression =

                    Expression.Call(
                        Expression.Convert(obj, classType),
                        methodInfo,
                        Expression.Convert(val, paramType)
                        );




            // wanted result is 
            // ((object)instance).SetterInvoke((object) val)
            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                bodyExpression,
                 obj,
                 val
                );

            return lambda.Compile();
        }

        /// <summary>
        /// Converts a property getter to a delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static Func<object, object> FieldGetterToDelegate(FieldInfo fieldInfo)
        {
            Type classType = fieldInfo.DeclaringType;
            Type returnType = fieldInfo.FieldType;


            var obj = Expression.Parameter(typeof(object), "obj");

            var original = Expression.Convert(obj, classType);
            
            MemberExpression getField = Expression.MakeMemberAccess(original, fieldInfo);

            var returnExpression =Expression.Convert(getField , typeof(object));

            // wanted result is 
            // (object)((object)instance).GetterInvoke
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(
                returnExpression,
                 obj
                );

            return lambda.Compile();
        }

        /// <summary>
        /// Converts a property setter to a delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static Action<object, object> FieldSetterToDelegate(FieldInfo fieldInfo)
        {
            Type classType = fieldInfo.DeclaringType;
            Type returnType = fieldInfo.FieldType;


            var obj = Expression.Parameter(typeof(object), "obj");
            var instance = Expression.Parameter(typeof(object), "instance");

            var original = Expression.Convert(instance, classType);

            MemberExpression getField = Expression.MakeMemberAccess(original, fieldInfo);

            var assignExpression = Expression.Assign(getField, Expression.Convert(obj, returnType));

            // wanted result is 
            // (object)((object)instance).GetterInvoke
            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(
                assignExpression,
                instance,
                 obj
                );

            return lambda.Compile();
        }



        /// <summary>
        /// returns <paramref name="count"/> number of tabs
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetTabSpaces(int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append('\t');
            }
            return sb.ToString();
        }

    }
}
