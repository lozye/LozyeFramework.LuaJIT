using LozyeFramework.Lua.LuaHeaders;
using System;
using System.Threading;

namespace LozyeFramework.Lua
{
	class LuaTableLite : LuaTable
	{
		readonly IntPtr _luaState;
		readonly LuaRef _luaIndex;
		int _disposed = 0;
		public LuaRef LuaReference => _luaIndex;

		private LuaTableLite() { }
		internal LuaTableLite(IntPtr luaState, int luaIdx)
		{
			_luaState = luaState;
			_luaIndex = (LuaRef)luaIdx;
		}
		~LuaTableLite() { Dispose(); }
		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0) return;
			try { LuaJIT.luaL_unref(_luaState, LuaJIT.LUA_REGISTRYINDEX, (int)_luaIndex); } catch { }
		}
		public T Get<T>(string path) => LuaStaticVisitor.Get<T>(_luaState, _luaIndex, path);
		public void Set<T>(string path, T value) => LuaStaticVisitor.Set<T>(_luaState, _luaIndex, path, value);
		public T GetFunction<T>(string path) where T : Delegate => LuaStaticVisitor.GetFunction<T>(_luaState, _luaIndex, path);
		public void SetFunction<T>(string path, T value) where T : Delegate => LuaStaticVisitor.SetFunction<T>(_luaState, _luaIndex, path, value);
		public T Get<T>(int path) => LuaStaticVisitor.Get<T>(_luaState, _luaIndex, path);
		public void Set<T>(int path, T value) => LuaStaticVisitor.Set<T>(_luaState, _luaIndex, path, value);
		public int Length() => LuaStaticVisitor.Length(_luaState, _luaIndex);
	}
}
