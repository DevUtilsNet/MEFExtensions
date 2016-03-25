using System;
using System.Collections.Generic;

namespace DevUtils.MEFExtensions.Core.Collections.Generic.Extensions
{
	/// <summary> An enumerable extensions. </summary>
	public static class EnumerableExtensions
	{
		/// <summary> An IEnumerable&lt;T&gt; extension method that applies an operation to all items in
		/// this collection. </summary>
		///
		/// <typeparam name="T"> Generic type parameter. </typeparam>
		/// <param name="source"> The source to act on. </param>
		/// <param name="action"> The action. </param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}