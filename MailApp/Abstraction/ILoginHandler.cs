using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailApp.Services;

namespace MailApp.Abstraction
{
    /// <summary>
    /// 登陆处理器
    /// </summary>
    public interface ILoginHandler
    {
        public void OnLoginStarted();
        public void OnLoginCompleted(IMailService mailService);
    }
}
