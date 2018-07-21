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
// Create Time         :    2017/7/16 15:38:21
// Update Time         :    2017/7/16 15:38:21
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierTPP
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // 禁止系统休眠
            FKBaseUtils.FKSystemEnviSettingHelper.PreventSystemSleep(true);
            // 创建日志对象
            FKLog.FKLogImp.GetInstance.SetCommandEditor(this.textBox_Cmd);
            FKLog.FKLogImp.GetInstance.SetRichTextBox(this.Log_richTextBox);
        }

        #region ==== 控件消息 ====

        /// <summary>
        /// Form 启动初始化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            LOGGER.LOG(FKLog.FKLogImp.ENUM_LogLevel.eLogLevel_Debug, "Debug", 0);
            LOGGER.LOG(FKLog.FKLogImp.ENUM_LogLevel.eLogLevel_Info, "Info", 0);
            LOGGER.LOG(FKLog.FKLogImp.ENUM_LogLevel.eLogLevel_Warning, "Warning", 0);
            LOGGER.LOG(FKLog.FKLogImp.ENUM_LogLevel.eLogLevel_Error, "Error", 0);

            this.Text = GetAppDescption();
        }

        #endregion ==== 控件消息 ====

        #region ==== 功能函数 ====

        /// <summary>
        /// 获取App信息
        /// </summary>
        /// <returns></returns>
        private string GetAppDescption()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly
                 .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                 .OfType<AssemblyDescriptionAttribute>()
                 .FirstOrDefault();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = "unknown version";
            if (fvi != null)
                version = "v" + fvi.FileVersion;
            string description = "FKCashierNode";
            if (descriptionAttribute != null)
                description = descriptionAttribute.Description;
            string strInfo = description + version;
            return strInfo;
        }

        #endregion ==== 功能函数 ====
    }
}
