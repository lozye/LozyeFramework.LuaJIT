---
title: LozyeFramework.Lua
tags: c# cshape lua luajit ptr-access
---

# LozyeFramework.Lua
luajit wapper for cshape


####    ========== SUPPORT TYPES  ==========    
(bool)      lua_toboolean       lua_pushboolean     
(LuaRef)    luaL_ref            lua_rawgeti         
(double)    lua_tonumber        lua_pushnumber      
(float)     lua_tonumber        lua_pushnumber      
(int)       lua_tointeger       lua_pushinteger     
(long)      lua_tointeger       lua_pushinteger     
(string)    lua_tolstring       lua_pushliteral     
(LuaTable)  luaL_ref            lua_rawgeti         
(IntPtr)    lua_topointer       --          

####    ========== SUPPORT Function  ==========       
(Delegate)  lua_pcall			  lua_pushcfunction   
[PS: Delegate need use GetFunction/SetFunction]     



####    ==========      Example         ==========  

+ How to start and execute a lua script
``` c#
using (var lua = new LuaEngine())
{
    var script = @"c={firstname='test',lastname='test_1',age=18}";
    lua.Execute(script);
    var firstname = lua.Get<string>("c.firstname");
    var age = lua.Get<int>("c.age");

    Debug.Assert("test" == firstname);
    Debug.Assert(18 == age);
}
```
+ How to get/set function
```c#
public static int Sum(int a, int b, int c) => a + b + c;

using (var lua = new LuaEngine())
{
    lua.SetFunction<Func<int, int, int, int>>("sum", Sum);
    var sum = lua.Evaluate<int>("return sum(1,6,7)");
    var fn2 = lua.GetFunction<Func<int, int, int, int>>("sum");
    var sum2 = fn2(2, 3, 4);

    Debug.Assert(sum2 == Sum(2, 3, 4));

    lua.Execute("getsum=function(a,b,c) return a+b+c; end;");
    var fn3 = lua.GetFunction<Func<int, int, int, int>>("getsum");
    var sum3 = fn3(2, 3, 4);

    Debug.Assert(sum3 == Sum(2, 3, 4));

}
```

+ Example for LuaRef & LuaTable
```c#
using (var lua = new LuaEngine())
{
    var script = @"c={firstname='test',lastname='test_1',age=18}";
    lua.Execute(script);
    var luaref = lua.Get<LuaRef>("c");
    var firstname = lua.Get<string>(luaref, "firstname");
    var age = lua.Get<int>(luaref, "age");



    lua.Set<string>(luaref, "firstname", "lili");
    var firstname2 = lua.Evaluate<string>("return c.firstname");

    Debug.Assert(firstname2 == "lili");

    using (var luatable = lua.Get<LuaTable>("c"))
    {
        var age2 = luatable.Get<int>("age");
        var firstname3 = luatable.Get<string>("firstname");
        luatable.Set<string>("firstname", "by LuaTable");
        var firstname4 = lua.Evaluate<string>("return c.firstname");

        Debug.Assert(firstname4 == "by LuaTable");
    }
}

```

+ c array by ffi and visit by c# & lua
```c#
using (var lua = new LuaEngine())
{
    var script = @"
            ffi=require('ffi');
            stringlist= ffi.typeof('const char* [?]');
            doublelist=ffi.typeof('double [?]');
            stringarray=stringlist(30, {'a','bb','ccc','eee'});
            doublearray=doublelist(30, {1.1,2.2,3.3});";

    lua.Execute(script);


    var stringarray = LuaArray.From(lua.Get<IntPtr>("stringarray"), 30);
    var doublearray = LuaArray.From<double>(lua.Get<IntPtr>("doublearray"), 30);

    var stringarray_2 = lua.Get<LuaTable>("stringarray");
    var doublearray_2 = lua.Get<LuaTable>("doublearray");


    stringarray[2] = "fffff";
    doublearray[4] = 44.5;			

    var s_2 = stringarray_2.Get<string>(2);//this must be fail , jit-string use  ffi.string(stringarray[2]) to get
    var d_4 = doublearray_2.Get<double>(4);

    var s_2_2 = lua.Evaluate<string>("return ffi.string(stringarray[2])");
    var d_4_2 = lua.Evaluate<double>("return doublearray[4]");

    Debug.Assert(s_2_2 == "fffff");
    Debug.Assert(s_2 == null);
    Debug.Assert(d_4_2 == d_4);
    Debug.Assert(d_4_2 == 44.5);


    lua.Execute("stringarray[2]='sssssssssss'");
    lua.Execute("doublearray[4]=88.8;");

    var s_2_3 = stringarray[2];
    var d_4_3 = doublearray[4];

    stringarray_2.Set<string>(2, "yyyyyyyyyy");
    doublearray_2.Set<double>(4, 99);

    var s_2_4 = stringarray[2];
    var d_4_4 = doublearray[4];

    var s_2_5 = lua.Evaluate<string>("return ffi.string(stringarray[2])");
    var d_4_5 = lua.Evaluate<double>("return doublearray[4]");

    Debug.Assert(s_2_4 == "yyyyyyyyyy");
    Debug.Assert(d_4_4 == d_4_5);
    Debug.Assert(s_2_5 == s_2_4);


    lua.Execute("stringarray=null;doublearray=null;");

    stringarray_2.Dispose();
    doublearray_2.Dispose();
    stringarray.Dispose();
    doublearray.Dispose();

}
```