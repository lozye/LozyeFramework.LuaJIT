using LozyeFramework.Lua.LuaHeaders;
using System;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaPointerProxy : ILuaProxy<IntPtr>
	{
		Type _type;
		int _luaType;

		public LuaPointerProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;
		public int luaType => _luaType;

		public IntPtr peek(IntPtr _luaState, int idx) => LuaJIT.lua_topointer(_luaState, idx);
		public void push(IntPtr _luaState, IntPtr value) => throw new NotImplementedException("only peek support");
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => throw new NotImplementedException("only peek support");
	}
}
