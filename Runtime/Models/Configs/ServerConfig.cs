// Copyright (c) 2020-2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using AccelByte.Core;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AccelByte.Models {
    [DataContract, Preserve]
    public class ServerConfig : IAccelByteConfig
    {
        private const int defaultCacheSize = 100;
        private const int defaultCacheLifeTime = 100;
        private const int defaultAMSHeartbeatInterval = 15;
        [DataMember] public string Namespace;
        [DataMember] public string BaseUrl;
        [DataMember] public string IamServerUrl;
        [DataMember] public string DSHubServerUrl;
        [DataMember] public string DSMControllerServerUrl;
        [DataMember] public string StatisticServerUrl;
        [DataMember] public string PlatformServerUrl;
        [DataMember] public string QosManagerServerUrl;
        [DataMember] public string GameTelemetryServerUrl;
        [DataMember] public string AchievementServerUrl;
        [DataMember] public string LobbyServerUrl;
        [DataMember] public string SessionServerUrl;
        [DataMember] public string CloudSaveServerUrl;
        [DataMember] public string RedirectUri;
        [DataMember] public string MatchmakingServerUrl;
        [DataMember] public string MatchmakingV2ServerUrl;
        [DataMember] public string SeasonPassServerUrl;
        [DataMember] public string AMSServerUrl;
        [DataMember] public int AMSHeartbeatInterval = defaultAMSHeartbeatInterval;
        [DataMember] public int MaximumCacheSize = defaultCacheSize;
        [DataMember] public int MaximumCacheLifeTime = defaultCacheLifeTime;


        /// <summary>
        ///  Copy member values
        /// </summary>
        public ServerConfig ShallowCopy()
        {
            return (ServerConfig) MemberwiseClone();
        }

        /// <summary>
        ///  Assign missing config values.
        /// </summary>
        public void Expand()
        {
            if (this.BaseUrl == null) return;
            
            this.IamServerUrl = this.GetDefaultServerApiUrl(this.IamServerUrl, "/iam");

            this.DSHubServerUrl = this.GetDefaultServerApiUrl(this.DSHubServerUrl, "/dshub");
            
            this.DSMControllerServerUrl = this.GetDefaultServerApiUrl(this.DSMControllerServerUrl, "/dsmcontroller");

            this.PlatformServerUrl = this.GetDefaultServerApiUrl(this.PlatformServerUrl, "/platform");

            this.StatisticServerUrl = this.GetDefaultServerApiUrl(this.StatisticServerUrl, "/social");

            this.QosManagerServerUrl = this.GetDefaultServerApiUrl(this.QosManagerServerUrl, "/qosm");

            this.GameTelemetryServerUrl = this.GetDefaultServerApiUrl(this.GameTelemetryServerUrl, "/game-telemetry");

            this.AchievementServerUrl = this.GetDefaultServerApiUrl(this.AchievementServerUrl, "/achievement");

            this.LobbyServerUrl = this.GetDefaultServerApiUrl(this.LobbyServerUrl, "/lobby");
            
            this.SessionServerUrl = this.GetDefaultServerApiUrl(this.SessionServerUrl, "/session");

            this.CloudSaveServerUrl = this.GetDefaultServerApiUrl(this.CloudSaveServerUrl, "/cloudsave");

            this.MatchmakingServerUrl = this.GetDefaultServerApiUrl(this.MatchmakingServerUrl, "/matchmaking");
            
            this.MatchmakingV2ServerUrl = this.GetDefaultServerApiUrl(this.MatchmakingV2ServerUrl, "/match2");

            this.SeasonPassServerUrl = this.GetDefaultServerApiUrl(this.SeasonPassServerUrl, "/seasonpass");

            if (MaximumCacheSize <= 0)
            {
                AccelByteDebug.LogWarning($"Invalid maximum cache size: ${MaximumCacheSize}\n. Set to default value: {defaultCacheSize}");
                MaximumCacheSize = defaultCacheSize;
            }

            if (MaximumCacheLifeTime <= 0)
            {
                AccelByteDebug.LogWarning($"Invalid maximum cache lifetime: ${MaximumCacheLifeTime}\n. Set to default value: {defaultCacheLifeTime}");
                MaximumCacheLifeTime = defaultCacheLifeTime;
            }
        }

        /// <summary>
        ///  Remove config values that can be derived from another value.
        /// </summary>
        public void Compact()
        {
            int index;
            // remove protocol
            if ((index = this.BaseUrl.IndexOf("://")) > 0) this.BaseUrl = this.BaseUrl.Substring(index + 3);

            if (this.BaseUrl == null) return;
            string httpBaseUrl = "https://" + this.BaseUrl;

            if (this.IamServerUrl == httpBaseUrl + "/iam") this.IamServerUrl = null;

            if (this.DSHubServerUrl == httpBaseUrl + "/dshub") this.DSMControllerServerUrl = null;
            
            if (this.DSMControllerServerUrl == httpBaseUrl + "/dsmcontroller") this.DSMControllerServerUrl = null;

            if (this.PlatformServerUrl == httpBaseUrl + "/platform") this.PlatformServerUrl = null;

            if (this.StatisticServerUrl == httpBaseUrl + "/statistic") this.StatisticServerUrl = null;

            if (this.QosManagerServerUrl == httpBaseUrl + "/qosm") this.QosManagerServerUrl = null;

            if (this.GameTelemetryServerUrl == httpBaseUrl + "/game-telemetry") this.GameTelemetryServerUrl = null;

            if (this.AchievementServerUrl == httpBaseUrl + "/achievement") this.AchievementServerUrl = null;

            if (this.LobbyServerUrl == httpBaseUrl + "/lobby") this.LobbyServerUrl = null;
            
            if (this.SessionServerUrl == httpBaseUrl + "/session") this.SessionServerUrl = null;

            if (this.CloudSaveServerUrl == httpBaseUrl + "/cloudsave") this.CloudSaveServerUrl = null;

            if (this.MatchmakingServerUrl == httpBaseUrl + "/matchmaking") this.MatchmakingServerUrl = null;
            
            if (this.MatchmakingV2ServerUrl == httpBaseUrl + "/match2") this.MatchmakingV2ServerUrl = null;

            if (this.SeasonPassServerUrl == httpBaseUrl + "/seasonpass") this.SeasonPassServerUrl = null;
        }

        /// <summary>
        /// Check required config field.
        /// </summary>
        public void CheckRequiredField()
        {
            if (string.IsNullOrEmpty(this.Namespace)) throw new System.Exception("Init AccelByte SDK failed, Server Namespace must not null or empty.");

            if (string.IsNullOrEmpty(this.BaseUrl)) throw new System.Exception("Init AccelByte SDK failed, Server Base URL must not null or empty.");
        }

        public bool IsRequiredFieldEmpty()
        {
            if (string.IsNullOrEmpty(this.Namespace)) return true;

            if (string.IsNullOrEmpty(this.BaseUrl)) return true;

            return false;
        }

        /// <summary>
        /// Set services URL.
        /// </summary>
        /// <param name="specificServerUrl">The specific URL, if empty will be replaced by baseUrl+defaultUrl.</param>
        /// <param name="defaultServerUrl">The default URL, will be used if specific URL is empty.</param>
        /// <returns></returns>
        private string GetDefaultServerApiUrl(string specificServerUrl, string defaultServerUrl)
        {
            if (string.IsNullOrEmpty(specificServerUrl))
            {
                return string.Format("{0}{1}", BaseUrl, defaultServerUrl);
            }

            return specificServerUrl;
        }

        public void SanitizeBaseUrl()
        {
            string sanitizedUrl = string.Empty;
            try
            {
                sanitizedUrl = Utils.UrlUtils.SanitizeBaseUrl(this.BaseUrl);
            }
            catch (System.Exception ex)
            {
                AccelByteDebug.LogWarning("Invalid Server Config BaseUrl: " + ex.Message);
            }
            this.BaseUrl = sanitizedUrl;
        }
    }

    [DataContract, Preserve]
    public class MultiServerConfigs : IAccelByteMultiConfigs
    {
        [DataMember] public ServerConfig Development;
        [DataMember] public ServerConfig Certification;
        [DataMember] public ServerConfig Production;
        [DataMember] public ServerConfig Default;

        public void Expand()
        {
            if (Development == null)
            {
                Development = new ServerConfig();
            }
            Development.Expand();
            if (Certification == null)
            {
                Certification = new ServerConfig();
            }
            Certification.Expand();
            if (Production == null)
            {
                Production = new ServerConfig();
            }
            Production.Expand();
            if (Default == null)
            {
                Default = new ServerConfig();
            }
            Default.Expand();
        }

        IAccelByteConfig IAccelByteMultiConfigs.GetConfigFromEnvironment(SettingsEnvironment targetEnvironment)
        {
            switch (targetEnvironment)
            {
                case SettingsEnvironment.Development:
                    return Development;
                case SettingsEnvironment.Certification:
                    return Certification;
                case SettingsEnvironment.Production:
                    return Production;
                case SettingsEnvironment.Default:
                default:
                    return Default;
            }
        }
    }
}
