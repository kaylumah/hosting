const webp=require('webp-converter');

const posts = [
  "../../_site/assets/images/posts/20190907/githooks",
  "../../_site/assets/images/posts/20200801/welcome",
  "../../_site/assets/images/posts/20210327/nuget-metadata",
  "../../_site/assets/images/posts/20210411/approach-to-writing-mocks",
  "../../_site/assets/images/posts/20210523/generate-csharp-client-for-openapi",
  "../../_site/assets/images/posts/20210717/decreasing-solution-build-time-with-filters",
  "../../_site/assets/images/posts/20211114/capture-logs-in-unit-tests",
  "../../_site/assets/images/posts/20211129/validated-strongly-typed-ioptions",
  "../../_site/assets/images/posts/20220131/improve-code-quality-with-bannedsymbolanalyzers",
  "../../_site/assets/images/posts/20220220/working-with-azure-sdk-for-dotnet"
];

posts.forEach(path => {
  const result = webp.cwebp(
    `${path}/cover_image.png`,
    `${path}/cover_image.webp`,
    "-q 80",
    logging="-v");
  result.then((response) => {
    console.log(response);
  });
});