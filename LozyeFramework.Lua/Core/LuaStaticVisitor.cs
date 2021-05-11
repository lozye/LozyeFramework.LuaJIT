using LozyeFramework.Lua.LuaHeaders;
using LozyeFramework.Lua.LuaProxys;
using System;

namespace LozyeFramework.Lua
{
	static class LuaStaticVisitor
	{
		public static T Get<T>(IntPtr _luaState, LuaRef luaPtr, string path)
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy<T>.Instance;
			var children = string.IsNullOrEmpty(path) ? new string[0] : path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				for (int i = 0; i < children.Length; i++)
					LuaJIT.lua_getfield(_luaState, -1, children[i]);
				return proxy.peek(_luaState, -1);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public static void Set<T>(IntPtr _luaState, LuaRef luaPtr, string path, T value)
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy<T>.Instance;
			var children = path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				for (int i = 0; i < children.Length - 1; i++)
					LuaJIT.lua_getfield(_luaState, -1, children[i]);
				proxy.push(_luaState, value);
				LuaJIT.lua_setfield(_luaState, -2, children[children.Length - 1]);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public static T GetFunction<T>(IntPtr _luaState, LuaRef luaPtr, string path) where T : Delegate
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy.Instance.Function;
			var children = string.IsNullOrEmpty(path) ? new string[0] : path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				for (int i = 0; i < children.Length; i++)
					LuaJIT.lua_getfield(_luaState, -1, children[i]);
				if (!LuaJIT.lua_isfunction(_luaState, -1)) throw new Exception("not function");
				var idx = LuaJIT.luaL_ref(_luaState, LuaJIT.LUA_REGISTRYINDEX);
				return proxy.peek<T>(_luaState, idx);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}

		}
		public static void SetFunction<T>(IntPtr _luaState, LuaRef luaPtr, string path, T value) where T : Delegate
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy.Instance.Function;
			var children = string.IsNullOrEmpty(path) ? null : path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				if (children == null)
				{
					proxy.push(_luaState, value);
					LuaJIT.lua_replace(_luaState, -2);
				}
				else
				{
					LuaJIT.lua_pushref(_luaState, (int)luaPtr);
					for (int i = 0; i < children.Length - 1; i++)
						LuaJIT.lua_getfield(_luaState, -1, children[i]);
					proxy.push<T>(_luaState, value);
					LuaJIT.lua_setfield(_luaState, -2, children[children.Length - 1]);
				}
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public static T Get<T>(IntPtr _luaState, LuaRef luaPtr, int path)
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy<T>.Instance;

			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				LuaJIT.lua_pushinteger(_luaState, path);
				LuaJIT.lua_gettable(_luaState, -2);
				return proxy.peek(_luaState, -1);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public static void Set<T>(IntPtr _luaState, LuaRef luaPtr, int path, T value)
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var proxy = LuaProxy<T>.Instance;

			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				LuaJIT.lua_pushinteger(_luaState, path);
				proxy.push(_luaState, value);
				LuaJIT.lua_settable(_luaState, -3);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}

		public static int Length(IntPtr _luaState, LuaRef luaPtr)
		{
			if (luaPtr == LuaRef.Zero) throw new NullReferenceException();
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_pushref(_luaState, (int)luaPtr);
				return (int)LuaJIT.lua_objlen(_luaState, -1);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}

		}
	}
}
