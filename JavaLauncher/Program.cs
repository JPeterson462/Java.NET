using System;
using System.IO;
using net.sf.jni4net;
using net.sf.jni4net.jni;
using net.sf.jni4net.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JavaLauncher
{
    public class Configuration
    {
        public string Jre { get; set; }
        public string Lib { get; set; }
        public string[] Options { get; set; }
        public string Class { get; set; }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            string config = File.ReadAllText("config.json");
            Configuration configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(config, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            BridgeSetup setup = new BridgeSetup()
            {
                //JavaHome = Directory.GetCurrentDirectory() + "/" + configuration.Jre,
                JavaHome = configuration.Jre,
                Verbose = true
            };
            foreach (string option in configuration.Options)
            {
                setup.AddJVMOption(option);
            }
            setup.AddAllJarsClassPath(Directory.GetCurrentDirectory() + "/" + configuration.Lib);
            JNIEnv env = Bridge.CreateJVM(setup);
            java.lang.Class clazz = java.lang.Class.forName(configuration.Class);
            env.CallStaticVoidMethod(clazz, clazz.getDeclaredMethod("main", new java.lang.Class[] { java.lang.String._class }).GetMethodId(), new Value[0]);
        }
    }
}
