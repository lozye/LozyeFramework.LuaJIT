using LozyeFramework.Lua.LuaHeaders;
using LozyeFramework.Lua.LuaProxys;
using System;
using System.Collections.Generic;

namespace LozyeFramework.Lua
{
	/// <summary>
	/// LuaEngine is luajit wapper  
	/// <list type="bullet">
	/// <item>          ========== SUPPORT TYPES  ==========          </item>
	/// <item>    (bool)      lua_toboolean       lua_pushboolean     </item>
	/// <item>    (LuaRef)    luaL_ref            lua_rawgeti         </item>
	/// <item>    (double)    lua_tonumber        lua_pushnumber      </item>
	/// <item>    (float)     lua_tonumber        lua_pushnumber      </item>
	/// <item>    (int)       lua_tointeger       lua_pushinteger     </item>
	/// <item>    (long)      lua_tointeger       lua_pushinteger     </item>
	/// <item>    (string)    lua_tolstring       lua_pushliteral     </item>
	/// <item>    (LuaTable)  luaL_ref            lua_rawgeti         </item>
	/// <item>    (IntPtr)    lua_topointer       --                  </item>
	/// <item>          ========== SUPPORT Function  ==========       </item>
	/// <item>    (Delegate)  lua_pcall			  lua_pushcfunction   </item>
	/// <item>    [PS: Delegate need use GetFunction/SetFunction]     </item>
	/// </list>
	/// <example>
	/// This sample shows how to call the <see cref="Get{T}(string)"/> method.
	/// <code>test={id='1212'}</code>
	/// <code>var luaref = engine.Get&lt;LuaRef&gt;("test"); //return ref-index</code>
	/// </example>
	/// <example>
	/// This sample shows how to call the <see cref="GetFunction{T}(LuaRef, string)"/> method.
	/// <code>getdate=function(k) return tostring(k)..os.date('%Y-%m-%d %H:%M:%S');  end</code>
	/// <code>var func = engine.GetFunction&lt;Func&lt;int, string&gt;&gt;("getdate"); //return func</code>
	/// </example>
	/// Author: Lozye @ MAY2021
	/// </summary>
	public class LuaEngine : ILuaEngine
	{

		readonly IntPtr _luaState;
		LuaProxy _luaProxy;
		ILuaProxy<string> _luaString;
		const string CHUNKNAME = "chunk";

		public IntPtr LuaState { get => _luaState; }

		public LuaEngine()
		{
			_luaState = LuaJIT.luaL_newstate();
			LuaJIT.luaL_openlibs(_luaState);
			_luaProxy = LuaProxy.Instance;
			_luaString = _luaProxy.String;
		}
		public void Dispose()
		{
			LuaJIT.lua_close(_luaState);
		}
		public T Evaluate<T>(string script)
		{
			try
			{
				var success = LuaJIT.luaL_dostring(_luaState, _luaProxy.GetBytes(script), CHUNKNAME);
				if (!success) throw new Exception(_luaString.peek(_luaState, -1));
				return LuaProxy<T>.Instance.peek(_luaState, -1);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, 0);
			}
		}
		public bool Execute(string script)
		{
			try
			{
				var success = LuaJIT.luaL_dostring(_luaState, _luaProxy.GetBytes(script), CHUNKNAME);
				if (!success) throw new Exception(_luaString.peek(_luaState, -1));
				return true;
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, 0);
			}
		}
		public T Get<T>(string path)
		{
			var proxy = LuaProxy<T>.Instance;
			var children = path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_getglobal(_luaState, children[0]);
				for (int i = 1; i < children.Length; i++)
					LuaJIT.lua_getfield(_luaState, -1, children[i]);
				return proxy.peek(_luaState, -1);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public void Set<T>(string path, T value)
		{
			var proxy = LuaProxy<T>.Instance;
			var children = path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				if (children.Length == 1)
				{
					proxy.push(_luaState, value);
					LuaJIT.lua_setglobal(_luaState, children[0]);
				}
				else
				{
					LuaJIT.lua_getglobal(_luaState, children[0]);
					for (int i = 1; i < children.Length - 1; i++)
						LuaJIT.lua_getfield(_luaState, -1, children[i]);
					proxy.push(_luaState, value);
					LuaJIT.lua_setfield(_luaState, -2, children[children.Length - 1]);
				}
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public T GetFunction<T>(string path) where T : Delegate
		{
			var _luaFunction = _luaProxy.Function;
			var children = path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				LuaJIT.lua_getglobal(_luaState, children[0]);
				for (int i = 1; i < children.Length; i++)
					LuaJIT.lua_getfield(_luaState, -1, children[i]);
				if (!LuaJIT.lua_isfunction(_luaState, -1)) throw new Exception("not function");
				var idx = LuaJIT.luaL_ref(_luaState, LuaJIT.LUA_REGISTRYINDEX);
				return _luaFunction.peek<T>(_luaState, idx);
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}
		}
		public void SetFunction<T>(string path, T value) where T : Delegate
		{
			var _luaFunction = _luaProxy.Function;
			var children = path.Split('.');
			var top = LuaJIT.lua_gettop(_luaState);
			try
			{
				if (children.Length == 1)
				{
					_luaFunction.push<T>(_luaState, value);
					LuaJIT.lua_setglobal(_luaState, children[0]);
				}
				else
				{
					LuaJIT.lua_getglobal(_luaState, children[0]);
					for (int i = 1; i < children.Length - 1; i++)
						LuaJIT.lua_getfield(_luaState, -1, children[i]);
					_luaFunction.push<T>(_luaState, value);
					LuaJIT.lua_setfield(_luaState, -2, children[children.Length - 1]);
				}
			}
			finally
			{
				LuaJIT.lua_settop(_luaState, top);
			}

		}
		public bool UnReference(LuaRef luaPtr)
		{
			var ptr = (int)luaPtr;
			if (ptr > 0) LuaJIT.luaL_unref(_luaState, LuaJIT.LUA_REGISTRYINDEX, ptr);
			return true;
		}
		public T Get<T>(LuaRef luaPtr, string path) => LuaStaticVisitor.Get<T>(_luaState, luaPtr, path);
		public void Set<T>(LuaRef luaPtr, string path, T value) => LuaStaticVisitor.Set<T>(_luaState, luaPtr, path, value);
		public T Get<T>(LuaRef luaPtr, int path) => LuaStaticVisitor.Get<T>(_luaState, luaPtr, path);
		public void Set<T>(LuaRef luaPtr, int path, T value) => LuaStaticVisitor.Set<T>(_luaState, luaPtr, path, value);
		public int Length(LuaRef luaPtr) => LuaStaticVisitor.Length(_luaState, luaPtr);
		public T GetFunction<T>(LuaRef luaPtr, string path) where T : Delegate => LuaStaticVisitor.GetFunction<T>(_luaState, luaPtr, path);
		public void SetFunction<T>(LuaRef luaPtr, string path, T value) where T : Delegate => LuaStaticVisitor.SetFunction<T>(_luaState, luaPtr, path, value);
		public IEnumerable<string> Stack()
		{
			var top = LuaJIT.lua_gettop(_luaState);
			string format = "{0,-10} {1}";
			for (int idx = 1; idx <= top; idx++)
			{
				var lua_type = LuaJIT.lua_type(_luaState, idx);
				string str = null;
				switch (lua_type)
				{
					case LuaJIT.LUA_NULL: str = string.Format(format, "NIL", null); break;
					case LuaJIT.LUA_TBOOLEAN:
						str = string.Format(format, "BOOLEAN", LuaProxy<bool>.Instance.peek(_luaState, idx)); break;
					case LuaJIT.LUA_TNUMBER: str = string.Format(format, "NUMBER", LuaProxy<double>.Instance.peek(_luaState, idx)); break;
					case LuaJIT.LUA_TSTRING: str = string.Format(format, "STRING", _luaString.peek(_luaState, idx)); break;
					case LuaJIT.LUA_TTABLE: str = string.Format(format, "TABLE", "{...}"); break;
					case LuaJIT.LUA_TUSERDATA: str = string.Format(format, "USERDATA", "{...}"); break;
					case LuaJIT.LUA_TFUNCTION: str = string.Format(format, "FUNCTION", "{...}"); break;
					default: str = string.Format(format, LuaJIT.lua_typename(_luaState, lua_type).ToUpper(), "{...}"); break;
				}
				yield return string.Format("{0,-3} {1,-3} {2}", idx.ToString(), (idx - top - 1).ToString(), str);
			}
		}

	}
}
