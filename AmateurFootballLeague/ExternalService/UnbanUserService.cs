using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using Quartz;

namespace AmateurFootballLeague.ExternalService
{
    [DisallowConcurrentExecution]
    public class UnbanUserService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public UnbanUserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                DateTime currentDate = DateTime.Today;
                TimeSpan totalTime = DateTime.Now.TimeOfDay;
                if(totalTime.Hours >= 17)
                {
                    currentDate = currentDate.AddDays(1);
                }

                IUserService userService = scope.ServiceProvider.GetService<IUserService>()!;
                ISendEmailService sendEmailService = scope.ServiceProvider.GetService<ISendEmailService>()!;

                List<User> listUser = userService.GetList().Where(u => u.DateUnban!.Value.CompareTo(currentDate) <= 0 && u.Status == false).ToList();
                foreach (User user in listUser)
                {
                    user.Status = true;
                    userService.UpdateAsync(user).Wait();

                    EmailForm model = new();
                    model.ToEmail = user.Email!;
                    model.Subject = "Thông báo mở khóa tài khoản tài khoản A-Football-League";
                    model.Message = "<html><head></head><body>" +
                        "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                        "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                        "</header>" +
                        "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                        "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Mở khóa tài khoản</div>" +
                        "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào <a href='mailto:" + user.Email + "'>" + user.Email + "</a>,</p>" +
                        "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                        "Tài khoản của bạn đã được mở khóa.</p>" +
                        "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                        "Hãy truy cập vào trang chủ AFL bằng liên kết bên dưới.</p>" +

                        "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                        "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                        "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                        "</div>" +
                        "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                        "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                        "</p>" +
                        "</footer></body></html>";
                    sendEmailService.SendEmail(model).Wait();
                }
            }

            return Task.CompletedTask;
        }
    }
}
