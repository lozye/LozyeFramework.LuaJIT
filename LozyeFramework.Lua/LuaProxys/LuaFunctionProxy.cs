
using LozyeFramework.Lua.LuaHeaders;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LozyeFramework.Lua.LuaProxys
{
	class LuaFunctionProxy : ILuaFunctionProxy
	{
		int _luaType;
		LuaProxy _map;
		ILuaProxy<string> _luaString;

		public LuaFunctionProxy(int luaType, LuaProxy map)
		{
			_luaType = luaType;
			_map = map;
			_luaString = map.String;
		}
		public Type type => null;

		public int luaType => _luaType;


		public object rawpeek(IntPtr _luaState, int idx) => throw new NotImplementedException();
		public void rawpush(IntPtr _luaState, object value) => throw new NotImplementedException();
		public void push<T>(IntPtr _luaState, T value) where T : Delegate
		{
			LambdaWapper wapper = ToLambda(value);
			LuaJIT.lua_pushcfunction(_luaState, wapper.Function);
		}
		public LambdaWapper ToLambda<T>(T value) where T : Delegate
		{
			/*-- 必须保存LambdaWapper否则会导致垃圾回收异常 --*/
			if (LambdaWapperManager.Instance.TryContinue(value, out var wapper)) return wapper;

			var info = value.Method;
			var instance = info.IsStatic ? null : Expression.Constant(value.Target);
			var props = info.GetParameters();
			var proxies = props.Select(item => _map.Get(item.ParameterType)).ToArray();
			var resultType = info.ReturnType;

			var arguments = Expression.Parameter(typeof(object[]));
			var parameters = new Expression[props.Length];

			for (int i = 0; i < props.Length; i++)
				parameters[i] = Expression.Convert(Expression.ArrayIndex(arguments, Expression.Constant(i)), props[i].ParameterType);
			Expression exp = Expression.Call(instance, info, parameters);

			if (info.ReturnType == typeof(void)) { exp = Expression.Block(exp, Expression.Constant(null)); }
			else if (info.ReturnType.IsValueType) exp = Expression.Convert(exp, typeof(object));

			var lambda = Expression.Lambda<Func<object[], object>>(exp, arguments);
			var body = lambda.Compile();
			wapper = new LambdaWapper(body, proxies, resultType == typeof(void) ? null : _map.Get(resultType));
			LambdaWapperManager.Instance.Add(value, wapper);
			return wapper;
		}

		public T peek<T>(IntPtr _luaState, int idx) where T : Delegate
		{
			var info = typeof(T).GetMethod("Invoke");
			var props = info.GetParameters();
			var luaProxies = props.Select(item => _map.Get(item.ParameterType)).ToArray();
			var resultType = info.ReturnType;
			var resultProxy = resultType != typeof(void) ? _map.Get(resultType) : null;

			var RAWPUSH = typeof(ILuaProxy).GetMethod("rawpush");
			var RAWPEEK = typeof(ILuaProxy).GetMethod("rawpeek");
			var OBJECT_TYPE = typeof(object);
			var NEW_EXCEPTION = typeof(Exception).GetConstructor(new[] { typeof(string) });

			/*-- CONSTANT --*/
			Expression luaState = Expression.Constant(_luaState);
			Expression functionIdx = Expression.Constant(idx);
			Expression luaString = Expression.Constant(_luaString);
			Expression functionArgs = Expression.Constant(props.Length);
			Expression resultArgs = Expression.Constant(resultProxy == null ? 0 : 1);
			Expression luaResult = resultProxy != null ? Expression.Constant(resultProxy) : null;
			Expression NUMBER_NEG_ONE = Expression.Constant(-1);
			Expression NUMBER_ZERO = Expression.Constant(0);

			/*-- Variable --*/
			ParameterExpression error = Expression.Variable(typeof(int));
			ParameterExpression[] parameters = props.Select(item => Expression.Parameter(item.ParameterType)).ToArray();

			if (LuaJIT.lua_checkstack(_luaState, luaProxies.Length + 6) == 0)
				throw new Exception("lua stack overflow");

			var n = 0;
			Expression[] codes = new Expression[luaProxies.Length + 6];
			//LuaJIT.lua_pushref(_luaState,idx);
			codes[n++] = Expression.Call(null, Quick.To(LuaJIT.lua_pushref), luaState, functionIdx);

			for (int i = 0; i < parameters.Length; i++)
				codes[n++] = Expression.Call(Expression.Constant(luaProxies[i]), RAWPUSH, luaState, Expression.Convert(parameters[i], OBJECT_TYPE));
			// int error = LuaJIT.lua_pcall(_luaState, nArgs, -1, 0);
			codes[n++] = Expression.Assign(error, Expression.Call(null, Quick.To(LuaJIT.lua_pcall), luaState, functionArgs, resultArgs, NUMBER_ZERO));
			var exception = Expression.New(NEW_EXCEPTION, Expression.Call(luaString, Quick.To(_luaString.peek), luaState, NUMBER_NEG_ONE));
			codes[n++] = Expression.IfThen(Expression.NotEqual(error, NUMBER_ZERO), Expression.Throw(exception));
			if (luaResult != null) codes[n++] = Expression.Convert(Expression.Call(luaResult, Quick.To(resultProxy.rawpeek), luaState, NUMBER_NEG_ONE), resultType);
			var block = Expression.Block(new[] { error }, new ArraySegment<Expression>(codes, 0, n));
			var tryfinally = Expression.TryFinally(block, Expression.Call(null, Quick.To(LuaJIT.lua_settop), luaState, NUMBER_ZERO));
			var lambda = Expression.Lambda<T>(tryfinally, parameters);
			return lambda.Compile();
		}
	}

	public class LambdaWapper
	{
		readonly Func<object[], object> _body;
		readonly ILuaProxy[] _luaProxies;
		readonly ILuaProxy _luaResult;
		readonly int _luaResultCount;
		readonly lua_CFunction _luaFunction;

		public LambdaWapper(Func<object[], object> body, ILuaProxy[] luaProxies, ILuaProxy luaResult)
		{
			_body = body;
			_luaProxies = luaProxies;
			_luaResult = luaResult;
			_luaResultCount = _luaResult == null ? 0 : 1;
			_luaFunction = m_Function;
		}

		public ILuaProxy[] LuaProxies => _luaProxies;
		public ILuaProxy LuaResult => _luaResult;
		public int LuaResultCount => _luaResultCount;
		public lua_CFunction Function => _luaFunction;
		private int m_Function(IntPtr L)
		{
			try
			{
				int n = LuaJIT.lua_gettop(L);
				if (_luaProxies.Length != n)
					return luaL_error(L, "invalid arguments to function");

				object[] args = new object[n];
				for (int i = 1; i <= n; i++)
					args[i - 1] = _luaProxies[i - 1].rawpeek(L, i);

				var response = _body(args);
				if (_luaResultCount > 0) _luaResult.rawpush(L, response);
			}
			catch (Exception ex)
			{
				return luaL_error(L, "c# exception:" + ex.Message + "\r\n" + ex.StackTrace);
			}
			return _luaResultCount;
		}
		private int luaL_error(IntPtr L, string err)
		{
			LuaJIT.lua_pushliteral(L, Encoding.UTF8.GetBytes(err));
			return -1;
		}
	}

	static class Quick
	{
		public static MethodInfo To(Action<IntPtr, int, int> value) => value.Method;
		public static MethodInfo To(Action<IntPtr, int> value) => value.Method;
		//int lua_pcall(lua_State L, int nargs, int nresults, int errfunc)
		public static MethodInfo To(Func<IntPtr, int, int, int, int> value) => value.Method;
		public static MethodInfo To(Func<IntPtr, int, string> value) => value.Method;
		public static MethodInfo To(Func<IntPtr, int, object> value) => value.Method;
	}

	public class LambdaWapperManager
	{
		System.Collections.Generic.Dictionary<int, LambdaWapper> _map;
		private LambdaWapperManager() { _map = new System.Collections.Generic.Dictionary<int, LambdaWapper>(); }
		private static readonly Lazy<LambdaWapperManager> lazyInstance = new Lazy<LambdaWapperManager>(() => new LambdaWapperManager());
		public static LambdaWapperManager Instance => lazyInstance.Value;

		public void Add(Delegate value, LambdaWapper wapper)
		{
			var code = value.GetHashCode();
			lock (this) _map[code] = wapper;
		}
		public bool TryContinue(Delegate value, out LambdaWapper wapper) => _map.TryGetValue(value.GetHashCode(), out wapper);

	}
}
