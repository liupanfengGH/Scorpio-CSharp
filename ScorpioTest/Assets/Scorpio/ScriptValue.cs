﻿using Scorpio.Tools;
using Scorpio.Exception;
using System;
using Scorpio.Userdata;
namespace Scorpio {
    public struct ScriptValue {
        public const string IteratorNext = "next";  //迭代器函数名字

        public const byte nullValueType = 0;        //null
        public const byte scriptValueType = 1;      //脚本变量
        public const byte doubleValueType = 2;      //double
        public const byte longValueType = 3;        //long
        public const byte trueValueType = 4;        //true
        public const byte falseValueType = 5;       //false
        public const byte stringValueType = 6;      //string
        public const byte objectValueType = 7;      //除了 double long 以外的number类型 和 枚举

        public static readonly ScriptValue[] EMPTY = new ScriptValue[0];

        public static readonly ScriptValue Null = new ScriptValue();
        public static readonly ScriptValue True = new ScriptValue(true);
        public static readonly ScriptValue False = new ScriptValue(false);
        public static readonly ScriptValue Zero = new ScriptValue((double)0);
        public static readonly ScriptValue InvalidIndex = new ScriptValue((double)-1);


        public byte valueType;
        public double doubleValue;
        public long longValue;
        public string stringValue;
        public ScriptObject scriptValue;
        public object objectValue;

        public ScriptValue(ScriptObject value) {
            this.valueType = value == null ? nullValueType : scriptValueType; ;
            this.scriptValue = value;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.objectValue = null;
        }
        public ScriptValue(bool value) {
            this.valueType = value ? trueValueType : falseValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null;
            this.objectValue = null;
        }
        public ScriptValue(double value) {
            this.valueType = doubleValueType;
            this.scriptValue = null;
            this.doubleValue = value;
            this.longValue = 0;
            this.stringValue = null;
            this.objectValue = null;
        }
        public ScriptValue(long value) {
            this.valueType = longValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = value;
            this.stringValue = null;
            this.objectValue = null;
        }
        public ScriptValue(string value) {
            this.valueType = value == null ? nullValueType : stringValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = value;
            this.objectValue = null;
        }
        public ScriptValue GetValueByIndex(int key, Script script) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValueByIndex(key);
                case doubleValueType:
                case longValueType:
                    return script.TypeNumber.GetValueByIndex(key);
                case stringValueType:
                    return script.TypeString.GetValueByIndex(key);
                case trueValueType:
                case falseValueType:
                    return script.TypeBoolean.GetValueByIndex(key);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 {key}");
            }
        }
        public ScriptValue GetValue(string key) {
            if (valueType == scriptValueType) {
                return scriptValue.GetValue(key);
            }
            throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 {key}");
        }
        public ScriptValue GetValue(string key, Script script) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case doubleValueType:
                case longValueType:
                    return script.TypeNumber.GetValue(key);
                case stringValueType:
                    return script.TypeString.GetValue(key);
                case trueValueType:
                case falseValueType:
                    return script.TypeBoolean.GetValue(key);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 {key}");
            }
        }
        public ScriptValue GetValue(object key, Script script) {
            switch (valueType) {
                case scriptValueType: return scriptValue.GetValue(key);
                case doubleValueType:
                case longValueType:
                    return script.TypeNumber.GetValue(key);
                case stringValueType:
                    return script.TypeString.GetValue(key);
                case trueValueType:
                case falseValueType:
                    return script.TypeBoolean.GetValue(key);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持获取变量 {key}");
            }
        }
        public void SetValueByIndex(int key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValueByIndex(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 {key}");
            }
        }
        public void SetValue(string key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 {key}");
            }
        }
        public void SetValue(object key, ScriptValue value) {
            if (valueType == scriptValueType) {
                scriptValue.SetValue(key, value);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持设置变量 {key}");
            }
        }
        //调用函数
        public ScriptValue call(ScriptValue thisObject, params object[] args) {
            var length = args.Length;
            var parameters = new ScriptValue[length];
            for (var i = 0; i < length; ++i) parameters[i] = CreateObject(args[i]);
            return Call(thisObject, parameters, length);
        }
        //调用无参函数
        public ScriptValue Call(ScriptValue thisObject) { return Call(thisObject, EMPTY, 0); }
        //调用函数
        public ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (valueType == scriptValueType) {
                return scriptValue.Call(thisObject, parameters, length);
            } else {
                throw new ExecutionException($"类型[{ValueTypeName}]不支持函数调用");
            }
        }
        //传入参数
        public object Value {
            get {
                switch (valueType) {
                    case doubleValueType: return doubleValue;
                    case longValueType: return longValue;
                    case nullValueType: return null;
                    case trueValueType: return true;
                    case falseValueType: return false;
                    case stringValueType: return stringValue;
                    case objectValueType: return objectValue;
                    default: return scriptValue.Value;
                }
            }
        }
        public string ValueTypeName {
            get {
                switch (valueType) {
                    case nullValueType: return "null";
                    case trueValueType:
                    case falseValueType:
                        return "Boolean";
                    case doubleValueType:
                    case longValueType:
                        return "Number";
                    case stringValueType:
                        return "String";
                    case objectValueType:
                        return objectValue.GetType().FullName;
                    default:
                        return scriptValue.ValueTypeName;
                }
            }
        }
        public int ToInt32() {
            switch (valueType) {
                case doubleValueType: return System.Convert.ToInt32(doubleValue);
                case longValueType: return System.Convert.ToInt32(longValue);
                case objectValueType: return System.Convert.ToInt32(objectValue);
                default: throw new ExecutionException($"类型[{ValueTypeName}]不支持转换为 int32");
            }
        }
        public T Get<T>() where T : ScriptObject {
            return valueType == scriptValueType ? (scriptValue as T) : null;
        }
        public ScriptObject Get() {
            return valueType == scriptValueType ? scriptValue : null;
        }
        public bool IsNull { get { return valueType == nullValueType; } }
        public bool IsTrue { get { return valueType == trueValueType; } }
        public bool IsFalse { get { return valueType == falseValueType; } }
        public bool IsNumber { get { return valueType == doubleValueType || valueType == longValueType; } }
        public bool IsString { get { return valueType == stringValueType; } }
        public bool IsScriptObject { get { return valueType == scriptValueType; } }

        public ScriptValue(sbyte value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(byte value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(short value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(ushort value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(int value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(uint value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(ulong value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }
        public ScriptValue(object value) {
            this.valueType = objectValueType;
            this.scriptValue = null;
            this.doubleValue = 0;
            this.longValue = 0;
            this.stringValue = null; 
            this.objectValue = value;
        }

        public string ToJson(bool supportKeyNumber) {
            switch (valueType) {
                case scriptValueType: return scriptValue.ToJson(supportKeyNumber);
                case doubleValueType: return doubleValue.ToString();
                case longValueType: return longValue.ToString();
                case trueValueType: return "true";
                case falseValueType: return "false";
                case stringValueType: return $"\"{stringValue.Replace("\"", "\\\"")}\"";
                case nullValueType: return "null";
                case objectValueType: return objectValue.ToString();
                default: return "";
            }
        }
        public override string ToString() {
            switch (valueType) {
                case doubleValueType: return doubleValue.ToString();
                case longValueType: return longValue.ToString();
                case trueValueType: return "true";
                case falseValueType: return "false";
                case stringValueType: return stringValue;
                case nullValueType: return "null";
                case scriptValueType: return scriptValue.ToString();
                case objectValueType: return objectValue.ToString();
                default: return "";
            }
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public override bool Equals(object obj) {
            if (obj == null) { return valueType == nullValueType; }
            if (obj is ScriptValue) {
                return Equals((ScriptValue)obj);
            } else if (obj is long) {
                return valueType == longValueType && longValue == (long)obj;
            } else if (obj is double) {
                return valueType == doubleValueType && doubleValue == (double)obj;
            } else if (obj is string) {
                return valueType == stringValueType && stringValue == (string)obj;
            } else if (obj is bool) {
                return (bool)obj ? valueType == trueValueType : valueType == falseValueType;
            } else if (obj is ScriptObject) {
                return valueType == scriptValueType && scriptValue.Value.Equals(obj);
            } else {
                return valueType == objectValueType && objectValue.Equals(obj);
            }
        }
        public bool Equals(ScriptValue value) {
            if (valueType != value.valueType) { return false; }
            switch (valueType) {
                case doubleValueType: return doubleValue == value.doubleValue;
                case longValueType: return longValue == value.longValue;
                case stringValueType: return stringValue == value.stringValue;
                case scriptValueType: return scriptValue.Equals(value);
                case objectValueType: return objectValue.Equals(value.objectValue);
                default: return true;
            }
        }



        public static bool operator ==(ScriptValue a1, ScriptValue a2) {
            return a1.Equals(a2);
        }
        public static bool operator !=(ScriptValue a1, ScriptValue a2) {
            return !a1.Equals(a2);
        }

        public static implicit operator ScriptValue(bool value) {
            return value ? True : False;
        }
        public static implicit operator ScriptValue(double value) {
            return new ScriptValue(value);
        }
        public static implicit operator ScriptValue(long value) {
            return new ScriptValue(value);
        }
        public static implicit operator ScriptValue(string value) {
            return new ScriptValue(value);
        }
        public static ScriptValue CreateObject(object value) {
            if (value == null)
                return Null;
            else if (value is ScriptValue)
                return (ScriptValue)value;
            else if (value is bool)
                return (bool)value ? True : False;
            else if (value is string)
                return new ScriptValue((string)value);
            else if (value is long)
                return new ScriptValue((long)value);
            else if (value is double)
                return new ScriptValue((double)value);
            else if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is float || value is decimal)
                return new ScriptValue(System.Convert.ToDouble(value));
            else if (value is ScriptObject)
                return new ScriptValue((ScriptObject)value);
            else if (value is Type)
                return TypeManager.GetUserdataType((Type)value);
            else if (value is Delegate)
                return new ScriptValue(new ScriptUserdataDelegate((Delegate)value));
            else if (value is Enum)
                return new ScriptValue(value);
            else if (value is Array)
                return new ScriptValue(new ScriptUserdataArray((Array)value, TypeManager.GetType(value.GetType())));
            return new ScriptValue(new ScriptUserdataObject(value, TypeManager.GetType(value.GetType())));
        }


        public bool Less(ScriptValue value) {
            if (valueType == ScriptValue.scriptValueType) {
                return scriptValue.Less(value);
            } else if (valueType == value.valueType) {
                switch (valueType) {
                    case ScriptValue.doubleValueType: return doubleValue < value.doubleValue;
                    case ScriptValue.longValueType: return longValue < value.longValue;
                }
            }
            throw new ExecutionException("【<】运算符不支持当前类型 : " + ValueTypeName);
        }
        public bool Greater(ScriptValue value) {
            if (valueType == ScriptValue.scriptValueType) {
                return scriptValue.Greater(value);
            } else if (valueType == value.valueType) {
                switch (valueType) {
                    case ScriptValue.doubleValueType: return doubleValue > value.doubleValue;
                    case ScriptValue.longValueType: return longValue > value.longValue;
                }
            }
            throw new ExecutionException("【>】运算符不支持当前类型 : " + ValueTypeName);
        }

    }
}
