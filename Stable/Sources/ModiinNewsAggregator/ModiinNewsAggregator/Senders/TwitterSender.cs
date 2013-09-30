using System;
using System.Configuration;
using System.IO;
using ModiinNewsAggregator.Interfaces;
using Twitterizer;

namespace ModiinNewsAggregator.Senders
{
    public class TwitterErrorException : Exception
    {
        public TwitterResponse<TwitterStatus> Status;

        public TwitterErrorException(TwitterResponse<TwitterStatus> status)
        {
            Status = status;
        }
    }
    public class TwitterSender : ISender
    {
        private readonly string m_ConsumerKey;
        private readonly string m_ConsumerSecret;
        private readonly string m_AccessToken;
        private readonly string m_AccessTokenSecret;

        public TwitterSender()
        {
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.FriendlyName), "twitter.config")
            };
            Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            m_ConsumerKey = customConfig.AppSettings.Settings["ConsumerKey"].Value;
            m_ConsumerSecret = customConfig.AppSettings.Settings["ConsumerSecret"].Value;
            m_AccessToken = customConfig.AppSettings.Settings["AccessToken"].Value;
            m_AccessTokenSecret = customConfig.AppSettings.Settings["AccessTokenSecret"].Value;
        }
        #region ISender
        public void Send(string message)
        {
            var tokens = new OAuthTokens
            {
                ConsumerKey = m_ConsumerKey,
                ConsumerSecret = m_ConsumerSecret,
                AccessToken = m_AccessToken,
                AccessTokenSecret = m_AccessTokenSecret
            };

            TwitterResponse<TwitterStatus> tweetResponse = TwitterStatus.Update(tokens, message);

            if (tweetResponse.Result != RequestResult.Success)
            {
                throw new TwitterErrorException(tweetResponse);
            }
        }
        #endregion
    }
}