
using LozyeFramework.Lua.LuaHeaders;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaStringProxy : ILuaProxy<string>
	{
		Type _type;
		int _luaType;
		Encoding _luaEncoding;

		public LuaStringProxy(Type type, int luaType, Encoding encoding)
		{
			_type = type;
			_luaType = luaType;
			_luaEncoding = encoding;
		}

		public Type type => _type;
		public int luaType => _luaType;
		public string peek(IntPtr _luaState, int idx)
		{
			var ptr = LuaJIT.lua_tolstring(_luaState, idx, out var len);
			if (ptr == IntPtr.Zero) return null;
			int length = (int)len;
			if (length == 0) return string.Empty;
			byte[] output = new byte[length];
			Marshal.Copy(ptr, output, 0, length);
			return _luaEncoding.GetString(output);
		}
		public void push(IntPtr _luaState, string value)
		{
			if (value == null) LuaJIT.lua_pushnil(_luaState);
			else LuaJIT.lua_pushliteral(_luaState, _luaEncoding.GetBytes(value));
		}
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (string)value);
	}

	class LuaNilProxy : ILuaProxy<object>
	{
		Type _type;
		int _luaType;

		public LuaNilProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;
		public int luaType => _luaType;
		public object peek(IntPtr _luaState, int idx) => null;
		public void push(IntPtr _luaState, object value) => LuaJIT.lua_pushnil(_luaState);
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, value);
	}

	class LuaBooleanProxy : ILuaProxy<bool>
	{
		Type _type;
		int _luaType;

		public LuaBooleanProxy(Type type, int luaType)
		{
			_type = type;
			_luaType = luaType;
		}
		public Type type => _type;
		public int luaType => _luaType;

		public bool peek(IntPtr _luaState, int idx) => LuaJIT.lua_toboolean(_luaState, idx) == 1;
		public void push(IntPtr _luaState, bool value) => LuaJIT.lua_pushboolean(_luaState, value ? 1 : 0);
		public object rawpeek(IntPtr _luaState, int idx) => peek(_luaState, idx);
		public void rawpush(IntPtr _luaState, object value) => push(_luaState, (bool)value);
	}

}
