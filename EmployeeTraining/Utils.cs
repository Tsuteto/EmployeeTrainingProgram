using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace EmployeeTraining
{
    public static class Utils
    {
        public static T FindResourceByName<T>(string name) where T : UnityEngine.Object
            => (T)Resources.FindObjectsOfTypeAll(typeof(T)).First(r => r.name == name);
        
        public static string ToBoxInfo(this Box box)
        {
            if (Plugin.Instance.Settings.RestockerLog)
            {
                if (box.HasProducts)
                {
                    return $"[{box.Product.name} x{box.Data.ProductCount}]";
                }
                else
                {
                    return "[EMPTY]";
                }
            }
            else
            {
                return "";
            }
        }

        public static string ToBoxStackInfo(this IEnumerable<Box> list)
        {
            return list.Select(b => b.ToBoxInfo()).Join(delimiter: "");
        }
    }

    public class PrivateFld<T>
    {
        private readonly FieldInfo fld;
        private readonly string name;

        public PrivateFld(Type type, string name)
        {
            this.name = name;
            try
            {
                this.fld = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get field {name} from {type.Name}", e);
            }
        }

        public object Instance {
            private get; set;
        }

        public T Value {
            get {
                //Plugin.LogDebug($"Get value of {name} obj={fld}, instance={Instance}");
                return (T)fld.GetValue(this.Instance);
            }
            set {
                fld.SetValue(this.Instance, value);
            }
        }
    }

    public class PrivateFldStatic<T>
    {
        private readonly FieldInfo fld;
        private readonly string name;

        public PrivateFldStatic(Type type, string name)
        {
            this.name = name;
            try
            {
                this.fld = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get field {name} from {typeof(T).Name}", e);
            }
        }

        public T GetValue(object instance) => (T)fld.GetValue(instance);
        public void SetValue(object instance, T value) => fld.SetValue(instance, value);
    }

    public class PrivateProp<T>
    {
        private readonly PropertyInfo prop;
        private readonly string name;

        public PrivateProp(Type type, string name)
        {
            this.name = name;
            this.prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public object Instance {
            private get; set;
        }

        public T Value {
            get {
                if (prop.CanRead)
                {
                    //Plugin.LogDebug($"Get property of {name} obj={prop}, instance={Instance}");
                    return (T)prop.GetValue(this.Instance);
                }
                else
                {
                    throw new Exception($"Unable to get value from {name}");
                }
            }
            set {
                if (prop.CanWrite)
                {
                    prop.SetValue(this.Instance, value);
                }
                else
                {
                    throw new Exception($"Unable to set value to {name}");
                }
            }
        }
    }

    public class PrivatePropStatic<V>
    {
        internal readonly PropertyInfo prop;
        internal readonly string name;

        public PrivatePropStatic(Type type, string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            this.name = name;
            try
            {
                this.prop = type.GetProperty(name, flags);
                if (this.prop == null)
                {
                    throw new Exception($"Property {name} not found in {type.Name}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get property {name} from {type.Name}", e);
            }
        }

        public V Get(object instance) {
            if (prop.CanRead)
            {
                return (V)prop.GetValue(instance);
            }
            else
            {
                throw new Exception($"Unable to get value from {name}");
            }
        }

        public void Set(object instance, V value) {
            if (prop.CanWrite)
            {
                prop.SetValue(instance, value);
            }
            else
            {
                throw new Exception($"Unable to set value to {name}");
            }
        }
    }


    public class PrivateMtd
    {
        internal readonly MethodInfo mtd;
        internal readonly string name;

        public PrivateMtd(Type type, string name, params Type[] args)
        {
            this.name = name;
            try
            {
                this.mtd = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get method {name} from {type.Name}", e);
            }
        }

        public object Instance {
            internal get; set;
        }

        public void Invoke(params object[] args)
        {
            //Plugin.LogDebug($"Invoke mtd={name}, info={mtd}, instance={Instance}");
            try
            {
                this.mtd.Invoke(this.Instance, args);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to invoke {mtd.Name} from {mtd.GetType().Name}", e);
            }
        }
    }

    public class PrivateMtd<T> : PrivateMtd
    {
        public PrivateMtd(Type type, string name, params Type[] args) : base(type, name, args)
        {
        }

        new public T Invoke(params object[] args)
        {
            //Plugin.LogDebug($"Invoke mtd={name}, info={mtd}, instance={Instance}");
            try
            {
                return (T)this.mtd.Invoke(this.Instance, args);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to invoke {mtd.Name} from {mtd.GetType().Name}", e);
            }
        }
    }

    public class PrivateMtdStatic
    {
        internal readonly MethodInfo mtd;
        internal readonly string name;

        public PrivateMtdStatic(Type type, string name, params Type[] args)
        {
            this.name = name;
            try
            {
                this.mtd = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get method {name} from {type.Name}", e);
            }
        }

        public void Invoke(object instance, params object[] args)
        {
            //Plugin.LogDebug($"Invoke mtd={name}, info={mtd}, instance={Instance}");
            try
            {
                this.mtd.Invoke(instance, args);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to invoke {mtd.Name} from {mtd.GetType().Name}", e);
            }
        }
    }

    public class PrivateMtdStatic<T> : PrivateMtdStatic
    {
        public PrivateMtdStatic(Type type, string name, params Type[] args) : base(type, name, args)
        {
        }

        new public T Invoke(object instance, params object[] args)
        {
            //Plugin.LogDebug($"Invoke mtd={name}, info={mtd}, instance={Instance}");
            try
            {
                return (T)this.mtd.Invoke(instance, args);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to invoke {mtd.Name} from {mtd.GetType().Name}", e);
            }
        }
    }

}