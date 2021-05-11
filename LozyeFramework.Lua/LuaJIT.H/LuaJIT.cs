using System;
using System.Runtime.InteropServices;
using System.Security;
using charptr_t = System.IntPtr;
using lua_Integer = System.Int64;
using lua_Number = System.Double;
using lua_State = System.IntPtr;
using luaL_Buffer = System.IntPtr;
using size_t = System.UIntPtr;
using voidptr_t = System.IntPtr;

namespace LozyeFramework.Lua.LuaHeaders
{
	/*
	 * #####################
	 * # Version LuaJIT-2.0.5
	 * # https://luajit.org/
	 * #####################
	 */
	/// <summary>
	/// LuaJIT
	/// </summary>
	[SuppressUnmanagedCodeSecurity]
	public static class LuaJIT
	{
		public const string DLLNAME = "lua51.dll";

		public const int LUAJIT_MODE_MASK = 0xFF;
		public const int LUAJIT_MODE_OFF = 0x0;
		public const int LUAJIT_MODE_ON = 0x100;
		public const int LUAJIT_MODE_FLUSH = 0x200;

		/*-- basic types --*/
		public const int LUA_TNONE = -1;
		public const int LUA_TNIL = 0;
		public const int LUA_NULL = 0;
		public const int LUA_TBOOLEAN = 1;
		public const int LUA_TLIGHTUSERDATA = 2;
		public const int LUA_TNUMBER = 3;
		public const int LUA_TSTRING = 4;
		public const int LUA_TTABLE = 5;
		public const int LUA_TFUNCTION = 6;
		public const int LUA_TUSERDATA = 7;
		public const int LUA_TTHREAD = 8;
		/*-- minimum Lua stack available to a C function --*/
		public const int LUA_MINSTACK = 20;
		/*-- garbage-collection function and options --*/
		public const int LUA_GCSTOP = 0;
		public const int LUA_GCRESTART = 1;
		public const int LUA_GCCOLLECT = 2;
		public const int LUA_GCCOUNT = 3;
		public const int LUA_GCCOUNTB = 4;
		public const int LUA_GCSTEP = 5;
		public const int LUA_GCSETPAUSE = 6;
		public const int LUA_GCSETSTEPMUL = 7;
		/*-- Event codes --*/
		public const int LUA_HOOKCALL = 0;
		public const int LUA_HOOKRET = 1;
		public const int LUA_HOOKLINE = 2;
		public const int LUA_HOOKCOUNT = 3;
		public const int LUA_HOOKTAILRET = 4;
		/*--  Event masks --*/
		public const int LUA_MASKCALL = (1 << LUA_HOOKCALL);
		public const int LUA_MASKRET = (1 << LUA_HOOKRET);
		public const int LUA_MASKLINE = (1 << LUA_HOOKLINE);
		public const int LUA_MASKCOUNT = (1 << LUA_HOOKCOUNT);

		/*-- pseudo-indices --*/
		public const int LUA_REGISTRYINDEX = (-10000);
		public const int LUA_ENVIRONINDEX = (-10001);
		public const int LUA_GLOBALSINDEX = (-10002);
		public const int LUA_MULTRET = (-1);

		/*-- thread status; 0 is OK --*/
		public const int LUA_YIELD = 1;
		public const int LUA_ERRRUN = 2;
		public const int LUA_ERRSYNTAX = 3;
		public const int LUA_ERRMEM = 4;
		public const int LUA_ERRERR = 5;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct luaL_Reg
		{
			public charptr_t name;
			public lua_CFunction func;
		}


		public enum LUAJIT_MODE : int
		{
			LUAJIT_MODE_ENGINE,     /* Set mode for whole JIT engine. */
			LUAJIT_MODE_DEBUG,      /* Set debug mode (idx = level). */
			LUAJIT_MODE_FUNC,       /* Change mode for a function. */
			LUAJIT_MODE_ALLFUNC,        /* Recurse into subroutine protos. */
			LUAJIT_MODE_ALLSUBFUNC, /* Change only the subroutines. */
			LUAJIT_MODE_TRACE,      /* Flush a compiled trace. */
			LUAJIT_MODE_WRAPCFUNC = 0x10,   /* Set wrapper mode for C function calls. */
			LUAJIT_MODE_MAX
		}


		/// <summary>Control the JIT engine</summary>
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int luaJIT_setmode(lua_State L, int idx, int mode);

		/*--
		 * #######################################
		 * ######          LUA_API          ###### 
		 * #######################################
		 * ---*/

		/*-- state manipulation/miscellaneous functions --*/
		#region STATE
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_State lua_newstate(lua_Alloc f, voidptr_t ud);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_close(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_State lua_newthread(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_CFunction lua_atpanic(lua_State L, lua_CFunction panicf);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_State luaL_newstate();

		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_error(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_next(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_concat(lua_State L, int n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Alloc lua_getallocf(lua_State L, ref voidptr_t ud);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_setallocf(lua_State L, lua_Alloc f, voidptr_t ud);
		#endregion

		/*-- basic stack manipulation --*/
		#region BASICSTACK
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int lua_gettop(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_settop(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushvalue(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_remove(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_insert(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_replace(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int lua_checkstack(lua_State L, int sz);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_xmove(lua_State from, lua_State to, int n);
		#endregion

		/*-- access functions (stack -> C) --*/
		#region ACCESSFUNCTIONS
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_isnumber(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_isstring(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_iscfunction(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_isuserdata(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_type(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static string lua_typename(lua_State L, int tp);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_equal(lua_State L, int idx1, int idx2);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_rawequal(lua_State L, int idx1, int idx2);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_lessthan(lua_State L, int idx1, int idx2);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Number lua_tonumber(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Integer lua_tointeger(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_toboolean(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static charptr_t lua_tolstring(lua_State L, int idx, out size_t len);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static size_t lua_objlen(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_CFunction lua_tocfunction(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_touserdata(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_State lua_tothread(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static voidptr_t lua_topointer(lua_State L, int idx);
		#endregion

		/* ----------------
		 * push functions (C -> stack)
		 * get functions (Lua -> stack)
		 * set functions (stack -> Lua)
		 * `load' and `call' functions (load and run Lua code)
		 * coroutine functions
		 * ----------------*/
		#region FUNCTIONS
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushnil(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushnumber(lua_State L, lua_Number n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushinteger(lua_State L, lua_Integer n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static charptr_t lua_pushlstring(lua_State L, byte[] s, size_t l);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void lua_pushstring(lua_State L, string s);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushcclosure(lua_State L, lua_CFunction fn, int n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushboolean(lua_State L, int b);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_pushlightuserdata(lua_State L, voidptr_t p);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_pushthread(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_gettable(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void lua_getfield(lua_State L, int idx, string k);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_rawget(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_rawgeti(lua_State L, int idx, int n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_createtable(lua_State L, int narr, int nrec);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static voidptr_t lua_newuserdata(lua_State L, size_t sz);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_getmetatable(lua_State L, int objindex);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_getfenv(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_settable(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void lua_setfield(lua_State L, int idx, string k);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_rawset(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_rawseti(lua_State L, int idx, int n);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_setmetatable(lua_State L, int objindex);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_setfenv(lua_State L, int idx);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void lua_call(lua_State L, int nargs, int nresults);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_pcall(lua_State L, int nargs, int nresults, int errfunc);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_cpcall(lua_State L, lua_CFunction func, voidptr_t ud);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int lua_load(lua_State L, lua_Reader reader, voidptr_t dt, string chunkname);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_dump(lua_State L, lua_Writer writer, voidptr_t data);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_yield(lua_State L, int nresults);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_resume(lua_State L, int narg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_status(lua_State L);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int lua_gc(lua_State L, int what, int data);
		#endregion

		/*-- some useful macros --*/
		#region USEFULMACROS
		public static void lua_pop(lua_State L, int n) => lua_settop(L, -n - 1);
		public static void lua_newtable(lua_State L) => lua_createtable(L, 0, 0);
		public static void lua_register(lua_State L, string n, lua_CFunction f) { lua_pushcfunction(L, f); lua_setglobal(L, n); }
		public static void lua_pushcfunction(lua_State L, lua_CFunction f) => lua_pushcclosure(L, f, 0);
		public static void lua_strlen(lua_State L, int i) => lua_objlen(L, i);
		public static bool lua_isfunction(lua_State L, int n) => lua_type(L, n) == LUA_TFUNCTION;
		public static bool lua_istable(lua_State L, int n) => lua_type(L, n) == LUA_TTABLE;
		public static bool lua_islightuserdata(lua_State L, int n) => lua_type(L, n) == LUA_TLIGHTUSERDATA;
		public static bool lua_isnil(lua_State L, int n) => lua_type(L, n) == LUA_TNIL;
		public static bool lua_isboolean(lua_State L, int n) => lua_type(L, n) == LUA_TBOOLEAN;
		public static bool lua_isthread(lua_State L, int n) => lua_type(L, n) == LUA_TTHREAD;
		public static bool lua_isnone(lua_State L, int n) => lua_type(L, n) == LUA_TNONE;
		public static bool lua_isnoneornil(lua_State L, int n) => (lua_type(L, (n)) <= 0);
		public static lua_State lua_pushliteral(lua_State L, byte[] s) => lua_pushlstring(L, s, (size_t)s.Length);
		public static void lua_setglobal(lua_State L, string s) => lua_setfield(L, LUA_GLOBALSINDEX, s);
		public static void lua_getglobal(lua_State L, string s) => lua_getfield(L, LUA_GLOBALSINDEX, s);
		public static byte[] lua_tostring(lua_State L, int i)
		{
			var ptr = lua_tolstring(L, i, out var len);
			if (ptr == IntPtr.Zero) return null;
			int length = (int)len;
			byte[] output = new byte[length];
			if (length == 0) return output;
			Marshal.Copy(ptr, output, 0, length);
			return output;
		}
		public static lua_State lua_open() => luaL_newstate();
		public static void lua_getregistry(lua_State L) => lua_pushvalue(L, LUA_REGISTRYINDEX);
		public static int lua_getgccount(lua_State L) => lua_gc(L, LUA_GCCOUNT, 0);
		public static void lua_pushref(lua_State L, int n) => lua_rawgeti(L, LUA_REGISTRYINDEX, n);

		#endregion

		/*--
		 * #######################################
		 * ######        LUALIB_API         ###### 
		 * #######################################
		 * ---*/

		#region LUALIB_API 
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void luaL_openlib(lua_State L, string libname, luaL_Reg[] l, int nup);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void luaL_register(lua_State L, string libname, luaL_Reg[] l);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_getmetafield(lua_State L, int obj, string e);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_callmeta(lua_State L, int obj, string e);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_typerror(lua_State L, int narg, string tname);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_argerror(lua_State L, int numarg, string extramsg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static charptr_t luaL_checklstring(lua_State L, int numArg, out size_t l);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static charptr_t luaL_optlstring(lua_State L, int numArg, string def, out size_t l);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Number luaL_checknumber(lua_State L, int numArg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Number luaL_optnumber(lua_State L, int nArg, lua_Number def);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Integer luaL_checkinteger(lua_State L, int numArg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static lua_Integer luaL_optinteger(lua_State L, int nArg, lua_Integer def);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_checkstack(lua_State L, int sz, charptr_t msg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_checktype(lua_State L, int narg, int t);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_checkany(lua_State L, int narg);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int luaL_newmetatable(lua_State L, charptr_t tname);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static voidptr_t luaL_checkudata(lua_State L, int ud, charptr_t tname);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_where(lua_State L, int lvl);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_error(lua_State L, byte[] fmt);

		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int luaL_checkoption(lua_State L, int narg, charptr_t def, charptr_t[] lst);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static int luaL_ref(lua_State L, int t);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_unref(lua_State L, int t, int _ref);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_loadfile(lua_State L, string filename);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_loadbuffer(lua_State L, byte[] buff, size_t sz, string name);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_loadstring(lua_State L, string s);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static charptr_t luaL_gsub(lua_State L, string s, string p, string r);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static charptr_t luaL_findtable(lua_State L, int idx, string fname, int szhint);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_fileresult(lua_State L, int stat, string fname);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_execresult(lua_State L, int stat);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_loadfilex(lua_State L, string filename, charptr_t mode);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static int luaL_loadbufferx(lua_State L, byte[] buff, size_t sz, string name, string mode);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void luaL_traceback(lua_State L, lua_State L1, string msg, int level);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_buffinit(lua_State L, luaL_Buffer B);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static charptr_t luaL_prepbuffer(luaL_Buffer B);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void luaL_addlstring(luaL_Buffer B, string s, size_t l);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public extern static void luaL_addstring(luaL_Buffer B, string s);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_addvalue(luaL_Buffer B);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_pushresult(luaL_Buffer B);
		[DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
		public extern static void luaL_openlibs(lua_State L);
		#endregion


		#region USEFULMACROS
		public static void luaL_argcheck(lua_State L, bool cond, int numarg, string extramsg)
		{
			if (cond) luaL_argerror(L, (numarg), (extramsg));
		}
		public static lua_State luaL_checkstring(lua_State L, int n) => luaL_checklstring(L, n, out _);
		public static lua_State luaL_optstring(lua_State L, int n, string d) => luaL_optlstring(L, n, d, out _);
		public static lua_Integer luaL_checkint(lua_State L, int n) => (int)luaL_checkinteger(L, n);
		public static lua_Integer luaL_optint(lua_State L, int n, int d) => (int)luaL_optinteger(L, n, d);
		public static lua_Integer luaL_checklong(lua_State L, int n) => luaL_checkinteger(L, n);
		public static lua_Integer luaL_optlong(lua_State L, int n, int d) => luaL_optinteger(L, n, d);
		public static string luaL_typename(lua_State L, int i) => lua_typename(L, lua_type(L, i));

		public static bool luaL_dostring(lua_State L, byte[] s, string name)
		{
			if (luaL_loadbufferx(L, s, (size_t)s.Length, name, null) != 0) return false;
			return lua_pcall(L, 0, LUA_MULTRET, 0) == 0;
		}
		public static void luaL_getmetatable(lua_State L, string n) => lua_getfield(L, LUA_REGISTRYINDEX, n);
		public static T luaL_opt<T>(lua_State L, Func<lua_State, int, T> f, int n, T d)
		{
			if (lua_isnoneornil(L, n))
				return d;
			return f(L, n);
		}
		#endregion







	}

	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public delegate int lua_CFunction(lua_State L);
	/// <summary>prototype for memory-allocation functions</summary>
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public delegate voidptr_t lua_Alloc(voidptr_t ud, voidptr_t ptr, size_t osize, size_t nsize);
	/// <summary>functions that read/write blocks when loading/dumping Lua chunks</summary>
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public delegate string lua_Reader(lua_State L, voidptr_t ud, size_t sz);
	/// <summary>functions that read/write blocks when loading/dumping Lua chunks</summary>
	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public delegate int lua_Writer(voidptr_t ud, voidptr_t p, size_t sz, voidptr_t nsize);
}
