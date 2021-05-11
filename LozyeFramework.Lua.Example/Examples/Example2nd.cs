using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LozyeFramework.Lua.Example.Examples
{
	class Example2nd : IExample
	{

		private Example2nd() { }
		private static readonly Lazy<Example2nd> lazyInstance = new Lazy<Example2nd>(() => new Example2nd());
		public static IExample Instance => lazyInstance.Value;
		public void Run(string[] args)
		{
			using (var lua = new LuaEngine())
			{
				lua.SetFunction<Func<int, int, int, int>>("sum", Sum);
				var sum = lua.Evaluate<int>("return sum(1,6,7)");
				var fn2 = lua.GetFunction<Func<int, int, int, int>>("sum");
				var sum2 = fn2(2, 3, 4);

				Debug.Assert(sum2 == Sum(2, 3, 4));

				lua.Execute("getsum=function(a,b,c) return a+b+c; end;");
				var fn3 = lua.GetFunction<Func<int, int, int, int>>("getsum");
				var sum3 = fn3(2, 3, 4);

				Debug.Assert(sum3 == Sum(2, 3, 4));

			}
		}

		public static int Sum(int a, int b, int c) => a + b + c;
	}
}
