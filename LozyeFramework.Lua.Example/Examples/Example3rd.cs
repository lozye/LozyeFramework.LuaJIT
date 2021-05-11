using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LozyeFramework.Lua.Example.Examples
{
	class Example3rd : IExample
	{

		private Example3rd() { }
		private static readonly Lazy<Example3rd> lazyInstance = new Lazy<Example3rd>(() => new Example3rd());
		public static IExample Instance => lazyInstance.Value;
		public void Run(string[] args)
		{
			using (var lua = new LuaEngine())
			{
				var script = @"c={firstname='test',lastname='test_1',age=18}";
				lua.Execute(script);
				var luaref = lua.Get<LuaRef>("c");
				var firstname = lua.Get<string>(luaref, "firstname");
				var age = lua.Get<int>(luaref, "age");

			

				lua.Set<string>(luaref, "firstname", "lili");
				var firstname2 = lua.Evaluate<string>("return c.firstname");

				Debug.Assert(firstname2 == "lili");

				using (var luatable = lua.Get<LuaTable>("c"))
				{
					var age2 = luatable.Get<int>("age");
					var firstname3 = luatable.Get<string>("firstname");
					luatable.Set<string>("firstname", "by LuaTable");
					var firstname4 = lua.Evaluate<string>("return c.firstname");

					Debug.Assert(firstname4 == "by LuaTable");
				}
			}
		}
	}
}
