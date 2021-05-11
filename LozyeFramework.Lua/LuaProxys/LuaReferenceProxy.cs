

using LozyeFramework.Lua.LuaHeaders;
using System;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaReferenceProxy : ILuaProxy<LuaRef>
	{
		Type _type;
		int _luaType;
		int _luaIndex;
		public static readonly LuaRef NULL = LuaRef.Zero;

		public LuaReferenceProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
			_luaIndex = LuaJIT.LUA_REGISTRYINDEX;//LUA_GLOBALSINDEX
		}
		public Type type => _type;
		public int luaType => _luaType;

		public LuaRef peek(IntPtr _luaState, int idx)
		{
			if (idx != -1) LuaJIT.lua_pushvalue(_luaState, idx);
			return (LuaRef)LuaJIT.luaL_ref(_luaState, _luaIndex);
		}
		public void push(IntPtr _luaState, LuaRef value)
		{
			if (NULL.Equals(value)) LuaJIT.lua_pushnil(_luaState);
			else LuaJIT.lua_rawgeti(_luaState, _luaIndex, (int)value);
		}
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (LuaRef)value);
	}
}
