// Copyright (c) 2020 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections;
using System.Collections.Generic;
using AccelByte.Core;
using AccelByte.Models;
using UnityEngine.Assertions;

namespace AccelByte.Server
{
    public class ServerLobbyApi
    {
        private string baseUrl;
        private IHttpClient httpClient;
        
        internal ServerLobbyApi(string baseUrl, IHttpClient httpClient)
        {
            Assert.IsNotNull(baseUrl, "Creating " + GetType().Name + " failed. Parameter baseUrl is null");
            Assert.IsNotNull(httpClient, "Creating " + GetType().Name + " failed. Parameter httpWorker is null");
            this.baseUrl = baseUrl;
            this.httpClient = httpClient;
        }
        
        public IEnumerator WritePartyStorage(string @namespace, string accessToken, PartyDataUpdateRequest data,
            string partyId, ResultCallback<PartyDataUpdateNotif> callback, Action callbackOnConflictedData = null)
        {
            Assert.IsNotNull(@namespace, nameof(@namespace) + " cannot be null");
            Assert.IsNotNull(accessToken, nameof(accessToken) + " cannot be null");
            Assert.IsNotNull(partyId, nameof(partyId) + " cannot be null");
            Assert.IsNotNull(data, nameof(data) + " cannot be null");

            var request = HttpRequestBuilder
                .CreatePut(this.baseUrl + "/v1/admin/party/namespaces/{namespace}/parties/{partyId}/attributes")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("partyId", partyId)
                .WithBearerAuth(accessToken)
                .WithBody(data.ToUtf8Json())
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParseJson<PartyDataUpdateNotif>();

            if (result.IsError && (result.Error.Code == ErrorCode.PreconditionFailed || result.Error.Code == ErrorCode.PartyStorageOutdatedUpdateData))
            {
                callbackOnConflictedData?.Invoke();
            }
            else
            {
                callback.Try(result);
            }
        }
        
        public IEnumerator GetPartyStorage(string @namespace, string accessToken, string partyID, ResultCallback<PartyDataUpdateNotif> callback)
        {
            Assert.IsNotNull(@namespace, nameof(@namespace) + " cannot be null");
            Assert.IsNotNull(accessToken, nameof(accessToken) + " cannot be null");
            Assert.IsNotNull(partyID, nameof(partyID) + " cannot be null");

            var request = HttpRequestBuilder
                .CreateGet(this.baseUrl + "/v1/admin/party/namespaces/{namespace}/parties/{partyId}")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("partyId", partyID)
                .WithBearerAuth(accessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParseJson<PartyDataUpdateNotif>();

            callback.Try(result);
        }

        public IEnumerator GetPartyDataByUserId(string @namespace, string accessToken, string userId, ResultCallback<PartyDataUpdateNotif> callback)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@namespace), "namespace cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(accessToken), "accessToken cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(userId), "userId cannot be null or empty");

            var request = HttpRequestBuilder
                .CreateGet(this.baseUrl + "/v1/admin/party/namespaces/{namespace}/users/{userId}/party")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("userId", userId)
                .WithBearerAuth(accessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParseJson<PartyDataUpdateNotif>();

            callback.Try(result);
        }

        public IEnumerator GetSessionAttribute(string @namespace, string accessToken, string userId, string key, ResultCallback<ServerGetSessionAttributeResponse> callback)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@namespace), "namespace cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(accessToken), "accessToken cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(userId), "userId cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(key), "key cannot be null or empty");

            var request = HttpRequestBuilder
                .CreateGet(this.baseUrl + "/v1/admin/player/namespaces/{namespace}/users/{userId}/attributes/{attribute}")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("userId", userId)
                .WithPathParam("attribute", key)
                .WithBearerAuth(accessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParseJson<ServerGetSessionAttributeResponse>();

            callback.Try(result);
        }

        public IEnumerator GetSessionAttributeAll(string @namespace, string accessToken, string userId, ResultCallback<GetSessionAttributeAllResponse> callback)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@namespace), "namespace cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(accessToken), "accessToken cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(userId), "userId cannot be null or empty");

            var request = HttpRequestBuilder
                .CreateGet(this.baseUrl + "/v1/admin/player/namespaces/{namespace}/users/{userId}/attributes")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("userId", userId)
                .WithBearerAuth(accessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParseJson<GetSessionAttributeAllResponse>();

            callback.Try(result);
        }

        public IEnumerator SetSessionAttribute(string @namespace, string accessToken, string userId, Dictionary<string, string> attributes, ResultCallback callback)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@namespace), "namespace cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(accessToken), "accessToken cannot be null or empty");
            Assert.IsFalse(string.IsNullOrEmpty(userId), "userId cannot be null or empty");
            Assert.IsFalse(attributes == null || attributes.Count == 0, "attributes cannot be null or empty.");

            ServerSetSessionAttributeRequest body = new ServerSetSessionAttributeRequest() { attributes = attributes };

            var request = HttpRequestBuilder
                .CreatePut(this.baseUrl + "/v1/admin/player/namespaces/{namespace}/users/{userId}/attributes")
                .WithPathParam("namespace", @namespace)
                .WithPathParam("userId", userId)
                .WithBearerAuth(accessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .WithBody(body.ToUtf8Json())
                .GetResult();

            IHttpResponse response = null;

            yield return this.httpClient.SendRequest(request, rsp => response = rsp);

            var result = response.TryParse();

            callback.Try(result);
        }
    }
}