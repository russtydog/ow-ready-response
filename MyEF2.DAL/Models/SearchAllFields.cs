using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
	public class SearchAllFields
	{
	}
	public static class IQueryableExtensions
	{
		public static IQueryable<T> SearchAllFields<T>(this IQueryable<T> query, string searchValue)
		{
			var properties = typeof(T).GetProperties()
				.Where(prop => prop.PropertyType == typeof(string));

			var parameter = Expression.Parameter(typeof(T), "x");
			var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

			var predicates = properties.Select(property =>
			{
				var propertyAccess = Expression.MakeMemberAccess(parameter, property);
				var constant = Expression.Constant(searchValue);
				var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
				var containsCall = Expression.Call(propertyAccess, containsMethod, constant);
				return Expression.AndAlso(nullCheck, containsCall);
			});

			var body = predicates.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

			var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

			return query.Where(lambda);
		}

	}

}
