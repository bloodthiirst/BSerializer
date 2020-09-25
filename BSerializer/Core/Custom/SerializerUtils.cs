using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BSerializer.Core.Custom
{
    public class SerializerUtils
    {
        /// <summary>
        /// Converts a property getter to a delegate
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Func<object, object> GetterToDelegate(MethodInfo methodInfo)
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
        public static Action<object, object> SetterToDelegate(MethodInfo methodInfo)
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

    }
}
