# Structure Data

https://search.google.com/test/rich-results
https://simplifiedsearch.net/structured-data-generator/

https://github.com/RehanSaeed/Schema.NET/
https://rehansaeed.com/structured-data-using-schema-net/

 https://lotushope.co.uk/create-structured-markup-with-json-ld.html
 ```csharp
 	var person = new Person
	{
		Name = "Jonathan Purdom",
		Email = "lotushope@hotmail.com",
		Gender = Schema.NET.GenderType.Male,
		Address = new PostalAddress {
			StreetAddress = "Harwood Road",
			AddressLocality = "West Sussex",
			AddressRegion = "Littlehampton",
			PostalCode = "BN17 7AT"
		},
		Url = new Uri("https://www.lotushope.co.uk"),
		SameAs = new OneOrMany<System.Uri>
		(
			new System.Uri("http://www.facebook.com/in/jonathanpurdom"),
			new System.Uri("http://www.linkedin.com/in/jonathanpurdom")
		)
	};

	Console.WriteLine("<script type=\"application/ld+json\">");
	Console.WriteLine(person.ToHtmlEscapedString());
	Console.WriteLine("</script>");
 ```
        // https://support.google.com/webmasters/answer/2692911?hl=en
        // https://developers.google.com/search/docs/advanced/structured-data/article
        // https://developers.google.com/search/docs/advanced/structured-data
        // https://developers.google.com/search/docs/advanced/structured-data/sd-policies
        // https://schema.org/docs/full.html
        // https://schema.org/Article
        // https://schema.org/BlogPosting
        // https://schema.org/TechArticle
        // https://schema.org/Blog