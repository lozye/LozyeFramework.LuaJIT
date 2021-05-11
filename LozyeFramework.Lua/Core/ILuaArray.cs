using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LozyeFramework.Lua
{
	public interface ILuaArray<T> : IReadOnlyCollection<T>, IDisposable
	{
		T this[int index] { get; set; }
	}

	/// <summary>
	/// Arrary Access C & CShape
	/// </summary>
	public class LuaArray
	{
		/// <summary>
		/// C Access const char * Array
		/// <list type="table">
		/// <item>Array only create by ffi.new</item>
		/// </list>
		/// <example>
		/// This sample shows how to call the <see cref="From(IntPtr, int)"/> method.
		/// <code>c =ffi.new('const char* [30]', {'a','bb','ccc','eee'});;</code>
		/// <code>var ptr2 = engine.Get&lt;IntPtr&gt;("c");</code>
		/// <code>var array = LuaArray.From(ptr2, 30);</code>
		/// </example>
		/// </summary>	
		public static ILuaArray<string> From(IntPtr intPtr, int count) => new LuaStringArray(intPtr, count);

		/// <summary>
		/// C Access unmanaged-Type  Array
		/// <list type="table">
		/// <item>Array only create by ffi.new</item>
		/// </list>
		/// <example>
		/// This sample shows how to call the <see cref="From{T}(IntPtr, int)"/> method.
		/// <code>c =ffi.new('double [30]', {1,1.5,2,6.4,55});</code>
		/// <code>var ptr2 = engine.Get&lt;IntPtr&gt;("c");</code>
		/// <code>var array = LuaArray.From&lt;double&gt;(ptr2, 30);</code>
		/// </example>
		/// </summary>	
		public static ILuaArray<T> From<T>(IntPtr intPtr, int count) where T : unmanaged => new LuaUnmanagedArray<T>(intPtr, count);
	}

	unsafe class LuaUnmanagedArray<T> : ILuaArray<T> where T : unmanaged
	{
		int _count;
		IntPtr _intPtr;
		T* _pointer;
		int _dispose = 0;

		~LuaUnmanagedArray() { Dispose(); }
		public LuaUnmanagedArray(IntPtr intPtr, int count)
		{
			_intPtr = intPtr;
			_count = count;
			_pointer = (T*)_intPtr;
		}

		public T this[int index]
		{
			get
			{
				if (_dispose != 0) throw new ObjectDisposedException(nameof(LuaUnmanagedArray<T>));
				if (index < 0 || index + 1 > _count) throw new IndexOutOfRangeException();
				return *(_pointer + index);
			}
			set
			{
				if (_dispose != 0) throw new ObjectDisposedException(nameof(LuaUnmanagedArray<T>));
				if (index < 0 || index + 1 > _count) throw new IndexOutOfRangeException();
				*(_pointer + index) = value;
			}
		}

		public int Count => _count;

		public void Dispose() { _dispose++; }

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
				yield return this[i];
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	unsafe class LuaStringArray : ILuaArray<string>
	{
		int _count;
		IntPtr _intPtr;
		IntPtr* _pointer;
		Queue<IntPtr> _queue;
		int _dispose = 0;
		public LuaStringArray(IntPtr intPtr, int count)
		{
			_intPtr = intPtr;
			_count = count;
			_pointer = (IntPtr*)_intPtr;
			_queue = new Queue<IntPtr>(count);
		}
		~LuaStringArray() { Dispose(); }
		public string this[int index]
		{
			get
			{
				if (_dispose != 0) throw new ObjectDisposedException(nameof(LuaStringArray));
				if (index < 0 || index + 1 > _count) throw new IndexOutOfRangeException();
				var ptr = *(_pointer + index);
				if (ptr == IntPtr.Zero) return null;
				return MarshalNativeUtf8ToManagedString((byte*)ptr);
			}
			set
			{
				if (_dispose != 0) throw new ObjectDisposedException(nameof(LuaStringArray));
				if (index < 0 || index + 1 > _count) throw new IndexOutOfRangeException();
				IntPtr ptr = IntPtr.Zero;
				if (value != null)
				{
					ptr = AllocConvertManagedStringToNativeUtf8(value);
					_queue.Enqueue(ptr);
				}
				*(_pointer + index) = ptr;
			}
		}

		public int Count => _count;

		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref _dispose, 1, 0) != 0) return;
			var array = _queue.ToArray();
			_queue.Clear();
			_queue = null;
			for (int i = 0; i < array.Length; i++)
				try { Marshal.FreeHGlobal(array[i]); } catch { }
		}
		public IEnumerator<string> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
				yield return this[i];
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private static unsafe IntPtr AllocConvertManagedStringToNativeUtf8(string input)
		{
			fixed (char* pInput = input)
			{
				var len = Encoding.UTF8.GetByteCount(pInput, input.Length);
				var pResult = (byte*)Marshal.AllocHGlobal(len + 1).ToPointer();
				var bytesWritten = Encoding.UTF8.GetBytes(pInput, input.Length, pResult, len);
				pResult[len] = 0;
				return (IntPtr)pResult;
			}
		}

		private static unsafe string MarshalNativeUtf8ToManagedString(byte* pStringUtf8)
		{
			var len = 0;
			while (pStringUtf8[len] != 0) len++;
#if NETCOREAPP3_1
			return Encoding.UTF8.GetString(pStringUtf8, len);
#else
			if (len == 0) return string.Empty;
			var buffer = new byte[len];
			Marshal.Copy((IntPtr)pStringUtf8, buffer, 0, len);
			return Encoding.UTF8.GetString(buffer);
#endif
		}
	}
}
