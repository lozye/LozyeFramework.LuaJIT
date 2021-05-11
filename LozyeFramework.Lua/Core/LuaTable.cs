using System;

namespace LozyeFramework.Lua
{
	/// <summary>LUA Table/Metatable</summary>
	public interface LuaTable : IDisposable
	{
		/// <summary>获取对应路径的值</summary>
		T Get<T>(string path);
		/// <summary>获取对应索引的值</summary>
		T Get<T>(int path);
		/// <summary>对应路径赋值</summary>
		void Set<T>(string path, T value);
		/// <summary>对应索引赋值</summary>
		void Set<T>(int path, T value);
		/// <summary>获取对应路径的方法</summary>
		T GetFunction<T>(string path) where T : Delegate;
		/// <summary>设置对应路径的方法</summary>
		void SetFunction<T>(string path, T value) where T : Delegate;
		/// <summary>获取原始Luaref值</summary>
		LuaRef LuaReference { get; }
		/// <summary>获取当前table的长度</summary>
		int Length();
	}
}
