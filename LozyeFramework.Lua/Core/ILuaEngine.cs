
using System;
using System.Collections.Generic;

namespace LozyeFramework.Lua
{
	public interface ILuaEngine : IDisposable
	{

		IntPtr LuaState { get; }
		/// <summary>执行一个脚本，T为响应类型</summary>
		T Evaluate<T>(string script);
		/// <summary>执行一段脚本</summary>
		bool Execute(string script);
		/// <summary>显示Lua堆栈信息</summary>
		IEnumerable<string> Stack();
		/// <summary>释放对应对象</summary>
		bool UnReference(LuaRef luaPtr);
		/// <summary>获取Table对应的索引</summary>
		T Get<T>(LuaRef luaPtr, string path);
		/// <summary>Table对应的索引赋值</summary>
		void Set<T>(LuaRef luaPtr, string path, T value);
		/// <summary>获取Table对应的索引</summary>
		T Get<T>(LuaRef luaPtr, int path);
		/// <summary>Table对应的索引赋值</summary>
		void Set<T>(LuaRef luaPtr, int path, T value);
		/// <summary>获取对应Table的长度</summary>
		int Length(LuaRef luaPtr);
		/// <summary>获取全局方法</summary>
		T GetFunction<T>(LuaRef luaPtr, string path) where T : Delegate;
		/// <summary>获取全局方法</summary>
		void SetFunction<T>(LuaRef luaPtr, string path, T value) where T : Delegate;
		/// <summary>获取全局参数</summary>
		T Get<T>(string path);
		/// <summary>设置全局参数</summary>
		void Set<T>(string path, T value);
		/// <summary>获取全局方法</summary>
		T GetFunction<T>(string path) where T : Delegate;
		/// <summary>获取全局方法</summary>
		void SetFunction<T>(string path, T value) where T : Delegate;
	}
}
