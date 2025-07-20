using DatabaseOperationsWithEFCore.Repository.Services;
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseOperationsWithEFCore.Repository.Implementations
{
    public class FilterImplementation<T> : IFilterService<T>
    {
        public IQueryable<T?> ApplyFilterOn(IQueryable<T?> queryOn, string? columnName, string? filterKeyWord)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(filterKeyWord))
            {
                return queryOn;
            }

            else
            {
                /* Get the property info for the specified column name */
                var propertyInfo = typeof(T).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo is null || propertyInfo.PropertyType != typeof(string))
                {
                    return queryOn;
                }

                /* Create expression: x => x.PropertyName != null && x.PropertyName.Contains(filterKeyword) */
                var parameter = Expression.Parameter(typeof(T), "x");

                /* Access the property dynamically */
                var property = Expression.Property(parameter, propertyInfo);

                /* Check for null */
                var nullCheck = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

                /* Contains method */
                var containsMethod = typeof(string).GetMethod(name: "Contains", types: new[] { typeof(string), typeof(StringComparison) }) ?? throw new InvalidOperationException($"The Contains method was not found on the string type.");

                /* Ensure the Contains method is found */
                var containsCall = Expression.Call(instance: property, method: containsMethod, arg0: Expression.Constant(filterKeyWord), arg1: Expression.Constant(StringComparison.OrdinalIgnoreCase));

                /* Combine null check and contains */
                var combinedExpression = Expression.AndAlso(nullCheck, containsCall);

                /* Create the lambda expression */
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

                return queryOn.Where(lambda); 
            }
        }
    }
}
