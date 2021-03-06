﻿using System;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    public abstract class UserdataMethod {
        protected Type m_Type;                                  //所在类

        protected FunctionData[] m_Methods;                     //所有函数
        protected FunctionData[] m_StaticMethods;               //所有静态函数
        protected FunctionDataGeneric[] m_GenericMethods;       //所有模板函数
        protected int m_GenericMethodCount = 0;                 //模板函数数量
        public string MethodName { get; protected set; }        //函数名字

        public UserdataMethod(Type type, string methodName) {
            m_Type = type;
            MethodName = type.FullName + "." + methodName;
        }
        //创建一个模板函数
        public UserdataMethodGeneric MakeGenericMethod(Type[] parameters) {
            for (var i = 0; i < m_GenericMethodCount; ++i) {
                var method = m_GenericMethods[i];
                var types = method.Method.GetGenericArguments();
                var length = types.Length;
                if (length == parameters.Length) {
                    var accord = true;
                    for (var j = 0; j < length; ++j) {
                        if (!types[j].BaseType.IsAssignableFrom(parameters[j])) {
                            accord = false;
                            break;
                        }
                    }
                    if (accord) {
                        return new UserdataMethodGeneric(m_Type, MethodName, method.Method.MakeGenericMethod(parameters));
                    }
                }
            }
            throw new ExecutionException($"没有找到合适的泛型函数 : {MethodName}");
        }
        public object Call(bool isStatic, object obj, ScriptValue[] parameters, int length) {
            try {
                //如果obj为null 则调用静态函数
                var methods = isStatic ? m_StaticMethods : m_Methods;
                var methodLength = methods.Length;
                for (var i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (method.CheckNormalType(parameters, length)) {
                        return method.Invoke(obj, parameters);
                    }
                }
                for (int i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (method.CheckDefaultType(parameters, length)) {
                        return method.Invoke(obj, parameters);
                    }
                }
                for (int i = 0; i < methodLength; ++i) {
                    var method = methods[i];
                    if (method.CheckArgsType(parameters, length)) {
                        return method.Invoke(obj, parameters);
                    }
                }
                throw new ExecutionException($"类[{m_Type.ToString()}] 找不到合适的函数 [{MethodName}]");
            } catch (System.Exception e) {
                throw new ExecutionException($"类[{m_Type.ToString()}] 调用函数出错 [{MethodName}] : {e.ToString()}");
            }
        }
    }
}
