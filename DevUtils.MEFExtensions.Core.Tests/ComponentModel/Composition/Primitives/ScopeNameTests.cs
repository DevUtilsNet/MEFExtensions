using System;
using System.Linq;
using DevUtils.MEFExtensions.Core.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.MEFExtensions.Core.Tests.ComponentModel.Composition.Primitives
{
	/// <summary>
	/// Summary description for ScopeNameTests
	/// </summary>
	[TestClass]
	public class ScopeNameTests
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void ScopeNameNameTest01()
		{
			Assert.IsNull(new ScopeName(null).Name);
			Assert.AreEqual("1", new ScopeName("1").Name);
			Assert.AreEqual("2", new ScopeName("1/2").Name);
			Assert.AreEqual("3", new ScopeName("1/2/3").Name);
			Assert.AreEqual(string.Empty, new ScopeName(string.Empty).Name);
		}

		[TestMethod]
		public void ScopeNameNamesTest01()
		{
			Assert.IsNotNull(new ScopeName(null).Names);
			Assert.AreEqual(1, new ScopeName("1").Names.Length);
			Assert.AreEqual(2, new ScopeName("1/2").Names.Length);
			Assert.AreEqual(3, new ScopeName("1/2/3").Names.Length);
			Assert.AreEqual(1, new ScopeName(string.Empty).Names.Length);
		}

		[TestMethod]
		public void ScopeNameNamesTest02()
		{
			Assert.IsNotNull(new ScopeName(null).Names);
			Assert.IsTrue(new ScopeName("1").Names.SequenceEqual(new [] {"1"}));
			Assert.IsTrue(new ScopeName("1/2").Names.SequenceEqual(new[] { "1", "2" }));
			Assert.IsTrue(new ScopeName("1/2/3").Names.SequenceEqual(new[] { "1", "2", "3" }));
			Assert.IsTrue(new ScopeName(string.Empty).Names.SequenceEqual(new[] { string.Empty }));
		}

		[TestMethod]
		public void ScopeNameFullNameTest01()
		{
			Assert.IsNull(new ScopeName(null).FullName);
			Assert.AreEqual("1", new ScopeName("1").FullName);
			Assert.AreEqual("1/2", new ScopeName("1/2").FullName);
			Assert.AreEqual(string.Empty, new ScopeName(string.Empty).FullName);
		}

		[TestMethod]
		public void ScopeNameCombainTest01()
		{
			Assert.IsNull(new ScopeName(null).Combain().FullName);
			Assert.IsNull(new ScopeName(null).Combain(null).FullName);
			Assert.AreEqual("1/2", new ScopeName("1").Combain("2").FullName);
			Assert.AreEqual(string.Empty, new ScopeName(string.Empty).Combain().FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombainTest02()
		{
			Assert.IsNull(new ScopeName(null).Combain(null, null).FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombainTest03()
		{
			Assert.IsNull(new ScopeName(null).Combain("1/2").FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombain04()
		{
			Assert.AreEqual(string.Empty, new ScopeName(null).Combain(string.Empty).FullName);
		}

		[TestMethod]
		public void ScopeNameCombainBeforeTest01()
		{
			Assert.IsNull(new ScopeName(null).CombainBefore().FullName);
			Assert.IsNull(new ScopeName(null).CombainBefore(null).FullName);
			Assert.AreEqual("2/1", new ScopeName("1").CombainBefore("2").FullName);
			Assert.AreEqual(string.Empty, new ScopeName(string.Empty).CombainBefore().FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombainBeforeTest02()
		{
			Assert.IsNull(new ScopeName(null).CombainBefore(null, null).FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombainBeforeTest03()
		{
			Assert.IsNull(new ScopeName(null).CombainBefore("1/2").FullName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ScopeNameCombainBeforeTest04()
		{
			Assert.AreEqual(string.Empty, new ScopeName(null).CombainBefore(string.Empty).FullName);
		}

		[TestMethod]
		public void ScopeNameEqualsTest01()
		{
			Assert.AreEqual(new ScopeName("1"), new ScopeName("1"));
			Assert.AreEqual(new ScopeName("1"), new ScopeName("*"));
			Assert.AreEqual(new ScopeName("1/2"), new ScopeName("1/2"));
			Assert.AreNotEqual(new ScopeName("1/2"), new ScopeName("*"));
			Assert.AreNotEqual(new ScopeName("2/1"), new ScopeName("1/2"));
			Assert.AreEqual(new ScopeName("*/*/*"), new ScopeName("*/*/*"));
			Assert.AreEqual(new ScopeName("0/1/2/3/4"), new ScopeName("**"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("*/2/3/4"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("1/*/3/4"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("1/2/3/*"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("*/*/*/*"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("**/2/3/4"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("1/**/3/4"));
			Assert.AreEqual(new ScopeName("1/2/3/4"), new ScopeName("1/2/3/**"));
			Assert.AreEqual(new ScopeName("**/**/**"), new ScopeName("**/**/**"));
			Assert.AreEqual(new ScopeName("0/1/2/3/4"), new ScopeName("**/2/3/4"));
			Assert.AreEqual(new ScopeName("0/1/2/3/4"), new ScopeName("0/**/3/4"));
			Assert.AreEqual(new ScopeName("0/1/2/3/4"), new ScopeName("0/1/2/**"));
			Assert.AreEqual(new ScopeName("0/**/3/*"), new ScopeName("0/1/2/3/4"));
			Assert.AreNotEqual(new ScopeName("0/1/2/3/4"), new ScopeName("0/**/3/5"));
			Assert.AreNotEqual(new ScopeName("0/1/2/3/4"), new ScopeName("**/0/1/2/3/4"));
			Assert.AreNotEqual(new ScopeName("0/1/2/3/4"), new ScopeName("0/1/2/3/4/**"));
		}
	}
}