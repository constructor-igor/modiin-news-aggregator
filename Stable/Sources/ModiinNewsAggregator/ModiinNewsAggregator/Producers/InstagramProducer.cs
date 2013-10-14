using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using InstaSharp;
using InstaSharp.Model.Responses;
using ModiinNewsAggregator.Interfaces;
using Newtonsoft.Json;

namespace ModiinNewsAggregator.Producers
{
    public class InstagramProducer : IProducer
    {
        const string apiUri = "https://api.instagram.com/v1";
        const string oAuthUri = "https://api.instagram.com/oauth";
        const string redirectUri = "https://twitter.com/modiin_ru";

        private readonly InstagramConfig m_config;

        public InstagramProducer()
        {
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.FriendlyName), "instagram.config")
            };
            Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            string mClientId = customConfig.AppSettings.Settings["ClientId"].Value;
            string mClientSecret = customConfig.AppSettings.Settings["ClientSecret"].Value;

            m_config = new InstaSharp.InstagramConfig(apiUri, oAuthUri, mClientId, mClientSecret, redirectUri);
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IList<InstagramMedia> mediaList = new List<InstagramMedia>();
            var tags = new InstaSharp.Endpoints.Tags.Unauthenticated(m_config);
            MediasResponse pictures = tags.Recent("modiin");

            dynamic dyn = JsonConvert.DeserializeObject(pictures.Json);
            foreach (var data in dyn.data)
            {
                string filter = data.filter;
                string link = data.link;
                string caption = data.caption.text;
                string fileUrl = data.images.standard_resolution.url;
                var mediaData = new InstagramMedia { Caption = caption, Link = link, Filter = filter, FileUrl = fileUrl };
                mediaList.Add(mediaData);
            }
            var message = new InstagramMediaDataMessage(mediaList);
            return message;
        }
        #endregion
    }

    public class InstagramMediaDataMessage : IMessage
    {
        static readonly Random random = new Random();
        public bool Empty { get { return !m_mediaList.Any(); } }
        public string Text { get; private set; }
        readonly IList<InstagramMedia> m_mediaList;

        public InstagramMediaDataMessage(IList<InstagramMedia> mediaList)
        {            
            m_mediaList = mediaList;            
            int randomMedia = random.Next(0, m_mediaList.Count - 1);
            InstagramMedia media = m_mediaList[randomMedia];
            Text = String.Format("{0}: {1}", media.Caption, media.Link);
        }
        public override string ToString()
        {
            return Text;
        }
    }

    public class InstagramMedia
    {
        public string Caption { get; set; }
        public string Link { get; set; }
        public string Filter { get; set; }
        public string FileUrl { get; set; }
    }
}