
using System;
using LozyeFramework.Lua.Example.Examples;

namespace LozyeFramework.Lua.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var raw= Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("1. platform x86 use ../LuaJIT.H/x86/lua51.dll ");
			Console.WriteLine("2. lua51.dll make by msvc2019");
			Console.WriteLine("3. only support LuaJIT 2.0.5");
			Console.ForegroundColor = raw;

			/*-
			 * 怎么启动及执行一个lua脚本
			 * [EN// How to start and execute a lua script]
			 * -*/
			Example1st.Instance.Run(args);

			/*-
			 * 怎么注册和获取方法
			 * [EN// How to get/set function]
			 * -*/
			Example2nd.Instance.Run(args);

			/*-
			 * LuaRef & LuaTable 实例
			 * [EN// case for LuaRef & LuaTable]
			 * -*/
			Example3rd.Instance.Run(args);

			/*-
			 * FFI c array 指针互调
			 * [EN// c array by ffi and visit by c# & lua ]
			 * -*/
			Example4th.Instance.Run(args);

			Console.WriteLine("======== Example ========");
			Console.ReadLine();
		}
	}
}
