namespace NBCZ.Common
{
    public class JwtSettings
    {
        /// <summary>
        /// token是谁颁发的
        /// </summary>
        public string Issuer { get; set; } = "NBCZ.NET";
        /// <summary>
        /// token可以给那些客户端使用
        /// </summary>
        public string Audience { get; set; } = "NBCZ.NET";

        /// <summary>
        /// 加密的key（SecretKey必须大于16个,是大于，不是大于等于）
        /// </summary>
        public string SecretKey { get; set; } = "SecretKey-ZRADMIN.NET-20210101";
        /// <summary>
        /// token时间（分）
        /// </summary>
        public int Expire { get; set; } = 1440;

        /// <summary>
        /// 刷新token时长
        /// </summary>
        public int RefreshTokenTime { get; set; } = 5;
        /// <summary>
        /// token类型
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}