using System;

namespace LozyeFramework.Lua
{
	public interface ILuaProxy
	{
		Type type { get; }
		int luaType { get; }
		object rawpeek(IntPtr _luaState, int idx);
		void rawpush(IntPtr _luaState, object value);

	}
	public interface ILuaProxy<T> : ILuaProxy
	{
		T peek(IntPtr _luaState, int idx);
		void push(IntPtr _luaState, T value);
	}

	interface ILuaFunctionProxy : ILuaProxy
	{
		T peek<T>(IntPtr _luaState, int idx) where T : Delegate;
		void push<T>(IntPtr _luaState, T value) where T : Delegate;
	}
}
