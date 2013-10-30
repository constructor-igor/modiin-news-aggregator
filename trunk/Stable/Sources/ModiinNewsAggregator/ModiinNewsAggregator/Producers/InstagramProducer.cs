using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
                string author = data.caption.from.username;
                string fileUrl = data.images.standard_resolution.url;

                string location_latitude = null;
                string location_longitude = null;
                try
                {
                    location_latitude = data.location.latitude;
                    location_longitude = data.location.longitude;
                }
                catch
                {
                    Trace.WriteLine("location not found for media {0}", caption);
                }

                var mediaData = new InstagramMedia { Caption = caption, Author = author, Link = link, Filter = filter, FileUrl = fileUrl, Latitude = location_latitude, Longitude = location_longitude};
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
        public bool Empty { get { return !MediaList.Any(); } }
        public string Text { get; private set; }
        public readonly IList<InstagramMedia> MediaList;

        public InstagramMediaDataMessage(IList<InstagramMedia> mediaList)
        {
            Text = "";
            MediaList = mediaList;            
            //int randomMedia = random.Next(0, MediaList.Count - 1);
            //InstagramMedia media = MediaList[randomMedia];
            if (MediaList.Count > 0)
            {
                InstagramMedia media = MediaList[0];
                Text = String.Format("{0}: {1}", media.Caption, media.Link);
            }
        }
        public override string ToString()
        {
            return Text;
        }
    }

    public class InstagramMedia
    {
        public string Caption { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Filter { get; set; }
        public string FileUrl { get; set; }       
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}