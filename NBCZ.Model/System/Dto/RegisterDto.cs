namespace NBCZ.Model.System.Dto
{
    public class RegisterDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Uuid { get; set; } = "";
        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { get; set; }
    }
}