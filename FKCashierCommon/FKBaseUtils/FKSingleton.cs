/* 
 * WRANING: These codes below is far away from bugs with the god and his animal protecting
 *                  _oo0oo_                   ┏┓　　　┏┓
 *                 o8888888o                ┏┛┻━━━┛┻┓
 *                 88" . "88                ┃　　　　　　　┃ 　
 *                 (| -_- |)                ┃　　　━　　　┃
 *                 0\  =  /0                ┃　┳┛　┗┳　┃
 *               ___/`---'\___              ┃　　　　　　　┃
 *             .' \\|     |# '.             ┃　　　┻　　　┃
 *            / \\|||  :  |||# \            ┃　　　　　　　┃
 *           / _||||| -:- |||||- \          ┗━┓　　　┏━┛
 *          |   | \\\  -  #/ |   |          　　┃　　　┃神兽保佑
 *          | \_|  ''\---/''  |_/ |         　　┃　　　┃永无BUG
 *          \  .-\__  '-'  ___/-. /         　　┃　　　┗━━━┓
 *        ___'. .'  /--.--\  `. .'___       　　┃　　　　　　　┣┓
 *     ."" '<  `.___\_<|>_/___.' >' "".     　　┃　　　　　　　┏┛
 *    | | :  `- \`.;`\ _ /`;.`/ - ` : | |   　　┗┓┓┏━┳┓┏┛
 *    \  \ `_.   \_ __\ /__ _/   .-` /  /   　　　┃┫┫　┃┫┫
 *=====`-.____`.___ \_____/___.-`___.-'=====　　　┗┻┛　┗┻┛ 
 *                  `=---='　　　
 *          佛祖保佑       永无BUG
 */
// =============================================================================== 
// Author              :    Frankie.W
// Create Time         :    2017/7/13 16:04:35
// Update Time         :    2017/7/13 16:04:35
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Reflection;
using System.Threading;
// ===============================================================================
namespace FKBaseUtils
{
    ///Usage:
    /// class MyClass : FKSingleton<MySingleton>
    /// {
    ///     private const string HelloWorldMessage = "Hello World - from MySingleton";
    /// 
    ///     public string HelloWorld { get; private set; }
    /// 
    ///     // 请必须使用私有构造！
    ///     private MyClass()
    ///     {
    ///         HelloWorld = HelloWorldMessage;
    ///     }
    /// }
    /// 
    /// var MyClassObj = MyClass.GetInstance;
    /// MyClassObj.HelloWorld();
    public abstract class FKSingleton<T> where T : class
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            if (!Array.Exists(ctors, (ci) => ci.GetParameters().Length == 0))
            {
                // 找不到私有构造
                throw new FKConstructorNotFoundException("Non-public ctor() note found.");
            }
            var ctor = Array.Find(ctors, (ci) => ci.GetParameters().Length == 0);

            // 执行构造
            return ctor.Invoke(new object[] { }) as T;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public static T GetInstance
        {
            get { return _instance.Value; }
        }
    }

    public class FKConstructorNotFoundException : Exception
    {
        private const string ConstructorNotFoundMessage = "Singleton<T> derived types require a non-public default constructor.";
        public FKConstructorNotFoundException() : base(ConstructorNotFoundMessage) { }
        public FKConstructorNotFoundException(string auxMessage) : base(String.Format("{0} - {1}", ConstructorNotFoundMessage, auxMessage)) { }
        public FKConstructorNotFoundException(string auxMessage, Exception inner) : base(String.Format("{0} - {1}", ConstructorNotFoundMessage, auxMessage), inner) { }
    }
}