

using LozyeFramework.Lua.LuaHeaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaProxy
	{
		Encoding _luaEncoding;
		IReadOnlyList<ILuaProxy> _luaProxies;
		ILuaProxy<string> _luaString;
		ILuaProxy<object> _luaNull;
		ILuaProxy<LuaRef> _luaReference;
		ILuaFunctionProxy _luaFunction;
		private LuaProxy(Encoding luaEncoding)
		{
			_luaEncoding = luaEncoding;
			ILuaProxy luaString;
			_luaProxies = new ILuaProxy[] {
					new LuaBooleanProxy(typeof(bool),LuaJIT.LUA_TBOOLEAN),
					new LuaReferenceProxy(typeof(LuaRef),LuaJIT.LUA_TLIGHTUSERDATA),
					new LuaNumberProxy(typeof(double),LuaJIT.LUA_TNUMBER),
					new LuaSigleProxy(typeof(float),LuaJIT.LUA_TNONE),
					new LuaInt32Proxy(typeof(int),LuaJIT.LUA_TNONE),
					new LuaInt64Proxy(typeof(long),LuaJIT.LUA_TNONE),
					luaString=new LuaStringProxy(typeof(string),LuaJIT.LUA_TSTRING,_luaEncoding),
					_luaReference=new LuaReferenceProxy(typeof(LuaRef),LuaJIT.LUA_TTABLE),
					new LuaReferenceProxy(typeof(LuaRef),LuaJIT.LUA_TFUNCTION),
					new LuaReferenceProxy(typeof(LuaRef),LuaJIT.LUA_TUSERDATA),
					new LuaReferenceProxy(typeof(LuaRef),LuaJIT.LUA_TTHREAD),
					new LuaTableProxy(typeof(LuaTable),LuaJIT.LUA_TNONE),
					new LuaPointerProxy(typeof(IntPtr),LuaJIT.LUA_TNONE)
			};
			_luaString = (ILuaProxy<string>)luaString;
			_luaNull = new LuaNilProxy(typeof(object), LuaJIT.LUA_NULL);
			_luaFunction = new LuaFunctionProxy(LuaJIT.LUA_TNONE, this);
		}
		private static readonly Lazy<LuaProxy> lazyInstance = new Lazy<LuaProxy>(() => new LuaProxy(Encoding.UTF8));
		public static LuaProxy Instance => lazyInstance.Value;
		public ILuaProxy<T> Get<T>()
		{
			var type = typeof(T);
			for (int i = 0; i < _luaProxies.Count; i++)
				if (_luaProxies[i].type == type) return (ILuaProxy<T>)_luaProxies[i];
			throw new NotSupportedException("luaporxy not support this type");
		}
		public ILuaProxy Get(Type type)
		{
			for (int i = 0; i < _luaProxies.Count; i++)
				if (_luaProxies[i].type == type) return _luaProxies[i];
			throw new NotSupportedException("luaporxy not support this type");
		}
		public ILuaProxy Get(int luaType)
		{
			for (int i = 0; i < _luaProxies.Count; i++)
				if (_luaProxies[i].luaType == luaType) return _luaProxies[i];
			throw new NotSupportedException("luaporxy not support this type");
		}
		public ILuaProxy<object> Null => _luaNull;
		public ILuaProxy<string> String => _luaString;
		public ILuaProxy<LuaRef> Reference => _luaReference;
		public ILuaFunctionProxy Function => _luaFunction;
		public byte[] GetBytes(string text) => _luaEncoding.GetBytes(text);
		public string GetString(byte[] text) => _luaEncoding.GetString(text);
	}

	class LuaProxy<T>
	{
		private static readonly Lazy<ILuaProxy<T>> lazyInstance = new Lazy<ILuaProxy<T>>(() => LuaProxy.Instance.Get<T>());
		public static ILuaProxy<T> Instance => lazyInstance.Value;
	}

}
