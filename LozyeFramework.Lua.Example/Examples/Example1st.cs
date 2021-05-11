
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LozyeFramework.Lua;


namespace LozyeFramework.Lua.Example.Examples
{

	public class Example1st : IExample
	{

		private Example1st() { }
		private static readonly Lazy<Example1st> lazyInstance = new Lazy<Example1st>(() => new Example1st());
		public static IExample Instance => lazyInstance.Value;

		public void Run(string[] args)
		{
			using (var lua = new LuaEngine())
			{
				var script = @"c={firstname='test',lastname='test_1',age=18}";
				lua.Execute(script);
				var firstname = lua.Get<string>("c.firstname");
				var age = lua.Get<int>("c.age");

				Debug.Assert("test" == firstname);
				Debug.Assert(18 == age);
			}


		}

	}
}
