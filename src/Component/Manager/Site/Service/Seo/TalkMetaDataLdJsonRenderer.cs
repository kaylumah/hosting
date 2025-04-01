// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [LdJsonTarget(typeof(TalkMetaData))]
    public class TalkMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public TalkMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not TalkMetaData talk)
            {
                throw new InvalidOperationException();
            }

#pragma warning disable

            PresentationDigitalDocument presentationScheme = new PresentationDigitalDocument();
            presentationScheme.Name = "Slide Deck for Modern Microservices";
            presentationScheme.Url = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.html");
            presentationScheme.EncodingFormat = "text/html";

            Place placeScheme = new Place();
            // place.Name = "Ilionx Dev Days 2023";

            Event eventScheme = new Event();
            eventScheme.Url = talk.CanonicalUri; // new Uri("https://kaylumah.nl/talks/modern-microservices.html")
            eventScheme.Name = talk.Name; // "Modern Microservices"
            eventScheme.Description =
                talk.Description; // "Talk presented at TechConf 2025 in Amsterdam about migrating .NET monoliths to cloud-native microservices."
            string keywords = string.Join(',', talk.Tags);
            eventScheme.Keywords = keywords;
            eventScheme.WorkPerformed = presentationScheme;
            eventScheme.Location = placeScheme;
            // StartDate = new DateTimeOffset(2025, 5, 21, 14, 30, 0, TimeSpan.Zero),
            // EndDate = new DateTimeOffset(2025, 5, 21, 15, 15, 0, TimeSpan.Zero)

            if (!string.IsNullOrEmpty(talk.Author) && _Authors.TryGetValue(talk.Author, out Person? personScheme))
            {
                eventScheme.Performer = personScheme;
            }

            /*
             *
               Location = new Place
               {
                   Name = "Amsterdam RAI Conference Centre",
                   Address = new PostalAddress
                   {
                       AddressLocality = "Amsterdam",
                       AddressCountry = "NL"
                   },
                   Geo = new GeoCoordinates
                   {
                       Latitude = 52.3411,
                       Longitude = 4.8884
                   }
               }
             */

            return eventScheme;
#pragma warning restore
        }
    }
}