
using LozyeFramework.Lua.LuaHeaders;
using System;

namespace LozyeFramework.Lua.LuaProxys
{

	class LuaNumberProxy : ILuaProxy<double>
	{
		Type _type;
		int _luaType;

		public LuaNumberProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;

		public int luaType => _luaType;

		public double peek(IntPtr _luaState, int idx) => LuaJIT.lua_tonumber(_luaState, idx);

		public void push(IntPtr _luaState, double value) => LuaJIT.lua_pushnumber(_luaState, value);

		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (double)value);
	}

	class LuaSigleProxy : ILuaProxy<Single>
	{
		Type _type;
		int _luaType;

		public LuaSigleProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;

		public int luaType => _luaType;

		public float peek(IntPtr _luaState, int idx) => (float)LuaJIT.lua_tonumber(_luaState, idx);

		public void push(IntPtr _luaState, float value) => LuaJIT.lua_pushnumber(_luaState, value);
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (float)value);

	}

	class LuaInt32Proxy : ILuaProxy<int>
	{

		Type _type;
		int _luaType;

		public LuaInt32Proxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}

		public Type type => _type;

		public int luaType => _luaType;

		public int peek(IntPtr _luaState, int idx) => (int)LuaJIT.lua_tointeger(_luaState, idx);

		public void push(IntPtr _luaState, int value) => LuaJIT.lua_pushinteger(_luaState, value);
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (int)value);
	}

	class LuaInt64Proxy : ILuaProxy<long>
	{
		Type _type;
		int _luaType;

		public LuaInt64Proxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;
		public int luaType => _luaType;
		public long peek(IntPtr _luaState, int idx) => LuaJIT.lua_tointeger(_luaState, idx);
		public void push(IntPtr _luaState, long value) => LuaJIT.lua_pushinteger(_luaState, value);
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (long)value);
	}
}
