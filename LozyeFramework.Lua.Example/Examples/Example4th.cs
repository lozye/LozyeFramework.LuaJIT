using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LozyeFramework.Lua.Example.Examples
{
	class Example4th : IExample
	{

		private Example4th() { }
		private static readonly Lazy<Example4th> lazyInstance = new Lazy<Example4th>(() => new Example4th());
		public static IExample Instance => lazyInstance.Value;
		public void Run(string[] args)
		{
			using (var lua = new LuaEngine())
			{
				var script = @"
						ffi=require('ffi');
						stringlist= ffi.typeof('const char* [?]');
						doublelist=ffi.typeof('double [?]');
						stringarray=stringlist(30, {'a','bb','ccc','eee'});
						doublearray=doublelist(30, {1.1,2.2,3.3});";

				lua.Execute(script);


				var stringarray = LuaArray.From(lua.Get<IntPtr>("stringarray"), 30);
				var doublearray = LuaArray.From<double>(lua.Get<IntPtr>("doublearray"), 30);

				var stringarray_2 = lua.Get<LuaTable>("stringarray");
				var doublearray_2 = lua.Get<LuaTable>("doublearray");


				stringarray[2] = "fffff";
				doublearray[4] = 44.5;			

				var s_2 = stringarray_2.Get<string>(2);//this must be fail , jit-string use  ffi.string(stringarray[2]) to get
				var d_4 = doublearray_2.Get<double>(4);

				var s_2_2 = lua.Evaluate<string>("return ffi.string(stringarray[2])");
				var d_4_2 = lua.Evaluate<double>("return doublearray[4]");

				Debug.Assert(s_2_2 == "fffff");
				Debug.Assert(s_2 == null);
				Debug.Assert(d_4_2 == d_4);
				Debug.Assert(d_4_2 == 44.5);


				lua.Execute("stringarray[2]='sssssssssss'");
				lua.Execute("doublearray[4]=88.8;");

				var s_2_3 = stringarray[2];
				var d_4_3 = doublearray[4];

				stringarray_2.Set<string>(2, "yyyyyyyyyy");
				doublearray_2.Set<double>(4, 99);

				var s_2_4 = stringarray[2];
				var d_4_4 = doublearray[4];

				var s_2_5 = lua.Evaluate<string>("return ffi.string(stringarray[2])");
				var d_4_5 = lua.Evaluate<double>("return doublearray[4]");

				Debug.Assert(s_2_4 == "yyyyyyyyyy");
				Debug.Assert(d_4_4 == d_4_5);
				Debug.Assert(s_2_5 == s_2_4);


				lua.Execute("stringarray=null;doublearray=null;");

				stringarray_2.Dispose();
				doublearray_2.Dispose();
				stringarray.Dispose();
				doublearray.Dispose();

			}
		}
	}
}
