using LozyeFramework.Lua.LuaHeaders;
using System;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaTableProxy : ILuaProxy<LuaTable>
	{
		Type _type;
		int _luaType;

		public LuaTableProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;
		public int luaType => _luaType;
		public LuaTable peek(IntPtr _luaState, int idx)
		{
			if (idx != -1) LuaJIT.lua_pushvalue(_luaState, idx);
			var n = LuaJIT.luaL_ref(_luaState, LuaJIT.LUA_REGISTRYINDEX);
			return new LuaTableLite(_luaState, n);
		}
		public void push(IntPtr _luaState, LuaTable value)
		{
			if (value == null) LuaJIT.lua_pushnil(_luaState);
			else LuaJIT.lua_rawgeti(_luaState, LuaJIT.LUA_REGISTRYINDEX, (int)value.LuaReference);
		}
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (LuaTable)value);
	}
}
