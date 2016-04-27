using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives
{
	/// <summary> A scope name. </summary>
	public struct ScopeName
	{
		/// <summary> The scope separator. </summary>
		public const char ScopeSeparator = '/';

		/// <summary> The scope separator string. </summary>
		public static readonly string ScopeSeparatorStr = string.Empty + ScopeSeparator;

		private readonly int _hashCode;

		/// <summary> Gets the name. </summary>
		///
		/// <value> The name. </value>
		public string Name
		{
			get
			{
				if (Names.Length == 0)
				{
					return null;
				}

				var ret = Names[Names.Length - 1];
				return ret;
			}
		}

		/// <summary> Gets the name of the full. </summary>
		///
		/// <value> The name of the full. </value>
		public string FullName { get; }

		/// <summary> Gets the names. </summary>
		///
		/// <value> The names. </value>
		public string[] Names
		{
			get
			{
				if (FullName == null)
				{
					return new string[0];
				}
				var ret = FullName.Split(ScopeSeparator);
				return ret;
			}
		}

		/// <summary> Constructor. </summary>
		///
		/// <param name="fullName"> Name of the full. </param>
		public ScopeName(string fullName)
		{
			FullName = fullName;
			_hashCode = FullName?.Replace('*', ' ').GetHashCode() ?? 0;
		}

		internal static void CheckSingleScopeName(string scopeName)
		{
			if (string.IsNullOrEmpty(scopeName))
			{
				throw new ArgumentException("The Scope name cannot be null or empty", nameof(scopeName));
			}

			if (scopeName.IndexOf(ScopeSeparator) != -1)
			{
				throw new ArgumentException("Not a single scope name", nameof(scopeName));
			}
		}

		internal static string CombainInternal(params string[] scopes)
		{
			var ret = CombainInternal((IEnumerable<string>)scopes);
			return ret;
		}

		internal static string CombainInternal(IEnumerable<string> scopes)
		{
			var ret = string.Join(ScopeSeparatorStr, scopes);
			return ret;
		}

		/// <summary> Combains the given scopes. </summary>
		///
		/// <param name="scopes"> A variable-length parameters list containing scopes. </param>
		///
		/// <returns> A ScopeName. </returns>
		[Pure]
		public ScopeName Combain(params string[] scopes)
		{
			if (scopes == null || scopes.Length == 0)
			{
				return this;
			}

			foreach (var item in scopes)
			{
				CheckSingleScopeName(item);
			}

			var ret = new ScopeName(CombainInternal(Names.Concat(scopes)));
			return ret;
		}

		/// <summary> Combain before. </summary>
		///
		/// <exception cref="ArgumentException">	Thrown when one or more arguments have unsupported or
		/// 																			illegal values. </exception>
		///
		/// <param name="scopes"> A variable-length parameters list containing scopes. </param>
		///
		/// <returns> A ScopeName. </returns>
		[Pure]
		public ScopeName CombainBefore(params string[] scopes) 
		{
			if (scopes == null || scopes.Length == 0)
			{
				return this;
			}

			foreach (var item in scopes)
			{
				CheckSingleScopeName(item);
			}

			var ret = new ScopeName(string.Join(ScopeSeparatorStr, scopes.Concat(Names)));
			return ret;
		}

		private static bool ScopeEquals(string scope1, string scope2)
		{
			var enum1 = (scope1 ?? string.Empty).Split(ScopeSeparator).Cast<string>().GetEnumerator();
			var enum2 = (scope2 ?? string.Empty).Split(ScopeSeparator).Cast<string>().GetEnumerator();

			var move1 = enum1.MoveNext();
			var move2 = enum2.MoveNext();

			var doubleStarts = false;
			while (move1 && move2)
			{
				if (enum1.Current != enum2.Current)
				{
					if (doubleStarts)
					{
						move2 = enum2.MoveNext();
						continue;
					}

					var start = false;
					if (string.Equals(enum2.Current, "*") || (doubleStarts = string.Equals(enum2.Current, "**")))
					{
						var tmp = enum1;
						enum1 = enum2;
						enum2 = tmp;
						start = true;
					}

					if (start || string.Equals(enum1.Current, "*") || (doubleStarts = string.Equals(enum1.Current, "**")))
					{
						move1 = enum1.MoveNext();
						if (doubleStarts && !move1)
						{
							return true;
						}
						move2 = enum2.MoveNext();

						continue;
					}

					return false;
				}

				doubleStarts = false;

				move1 = enum1.MoveNext();
				move2 = enum2.MoveNext();
			}

			var ret = move1 == move2;
			return ret;
		}

		#region Overrides of ValueType

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			return FullName;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			var ret = _hashCode;
			return ret;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false. 
		/// </returns>
		/// <param name="obj">The object to compare with the current instance. </param>
		public override bool Equals(object obj)
		{
			if (obj is ScopeName)
			{
				var ret = ScopeEquals(FullName, ((ScopeName)obj).FullName);
				return ret;
			}

			return false;
		}

		#endregion

		/// <summary> Equality operator. </summary>
		///
		/// <param name="scopeName1"> The first scope name. </param>
		/// <param name="scopeName2"> The second scope name. </param>
		///
		/// <returns> The result of the operation. </returns>
		public static bool operator == (ScopeName scopeName1, ScopeName scopeName2)
		{
			var ret = scopeName1.Equals(scopeName2);
			return ret;
		}

		/// <summary> Inequality operator. </summary>
		///
		/// <param name="scopeName1"> The first scope name. </param>
		/// <param name="scopeName2"> The second scope name. </param>
		///
		/// <returns> The result of the operation. </returns>
		public static bool operator != (ScopeName scopeName1, ScopeName scopeName2)
		{
			var ret = !(scopeName1 == scopeName2);
			return ret;
		}

		/// <summary> Addition operator. </summary>
		///
		/// <param name="scopeName">			 Name of the scope. </param>
		/// <param name="singleScopeName"> Name of the single scope. </param>
		///
		/// <returns> The result of the operation. </returns>
		public static ScopeName operator / (ScopeName scopeName, string singleScopeName)
		{
			var ret = scopeName.Combain(singleScopeName);
			return ret;
		}

		/// <summary> Addition operator. </summary>
		///
		/// <param name="singleScopeName"> Name of the single scope. </param>
		/// <param name="scopeName">			 Name of the scope. </param>
		///
		/// <returns> The result of the operation. </returns>
		public static ScopeName operator / (string singleScopeName, ScopeName scopeName)
		{
			var ret = scopeName.CombainBefore(singleScopeName);
			return ret;
		}

		/// <summary> Addition operator. </summary>
		///
		/// <param name="left">  The left. </param>
		/// <param name="right"> The right. </param>
		///
		/// <returns> The result of the operation. </returns>
		public static ScopeName operator / (ScopeName left, ScopeName right)
		{
			var ret = left.Combain(right.Names);
			return ret;
		}
	}
}