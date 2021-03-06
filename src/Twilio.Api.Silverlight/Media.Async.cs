﻿using RestSharp;
using RestSharp.Extensions;
using System;

namespace Twilio
{
    public partial class TwilioRestClient
    {

        /// <summary>
        /// Get the details for a specific Media instance.
        /// </summary>
        public void GetMedia(string mediaSid, Action<Media> callback)
        {
            var request = new RestRequest();
            request.Resource = "Accounts/{AccountSid}/Media/{MediaSid}.json";
            request.AddUrlSegment("MediaSid", mediaSid);
            ExecuteAsync<Media>(request, (response) => callback(response));
        }

        /// <summary>
        /// Retrieve a list of Media objects with no list filters
        /// </summary>
        public void ListMedia(Action<MediaResult> callback)
        {
            ListMedia(new MediaListRequest(), callback);
        }

        /// <summary>
        /// Return a filtered list of Media objects. The list includes paging
        /// information.
        /// </summary>
        public void ListMedia(MediaListRequest options, Action<MediaResult> callback)
        {
            var request = new RestRequest();
            request.Resource = "Accounts/{AccountSid}/Media.json";
            AddMediaListOptions(options, request);
            ExecuteAsync<MediaResult>(request, (response) => callback(response));
        }

        /// <summary>
        /// List all media for a particular message
        /// </summary>
        /// <param name="messageSid">The message sid to filter on</param>
        public void ListMessageMedia(string messageSid, MediaListRequest options, Action<MediaResult> callback)
        {
            var request = new RestRequest();
            request.Resource = "Accounts/{AccountSid}/Messages/{MessageSid}/Media.json";
            request.AddUrlSegment("MessageSid", messageSid);
            AddMediaListOptions(options, request);
            ExecuteAsync<MediaResult>(request, (response) => callback(response));
        }

        /// <summary>
        /// Add the options to the request
        /// </summary>
        private void AddMediaListOptions(MediaListRequest options, RestRequest request)
        {
            if (options.ParentSid.HasValue()) request.AddParameter("ParentSid", options.ParentSid);
            // Construct the date filter
            if (options.DateCreated.HasValue)
            {
                var dateCreatedParameterName = GetParameterNameWithEquality(options.DateCreatedComparison, "DateCreated");
                request.AddParameter(dateCreatedParameterName, options.DateCreated.Value.ToString("yyyy-MM-dd"));
            }

            // Paging options
            if (options.PageNumber.HasValue) request.AddParameter("Page", options.PageNumber.Value);
            if (options.Count.HasValue) request.AddParameter("PageSize", options.Count.Value);
        }

        /// <summary>
        /// Delete the specified media instance. Makes a DELETE request to a 
        /// Media Instance resource.
        /// </summary>
        /// <param name="mediaSid">The Sid of the media to delete</param>
        public void DeleteMedia(string mediaSid, Action<DeleteStatus> callback)
        {
            var request = new RestRequest(Method.DELETE);
            request.Resource = "Accounts/{AccountSid}/Media/{MediaSid}.json";

            request.AddParameter("MediaSid", mediaSid, ParameterType.UrlSegment);

            ExecuteAsync(request, (response) => { callback(response.StatusCode == System.Net.HttpStatusCode.NoContent ? DeleteStatus.Success : DeleteStatus.Failed); });
        }
    }
}
