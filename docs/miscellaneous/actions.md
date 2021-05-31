https://github.com/dotnet/arcade/issues/2984

https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2019

https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk?view=vs-2019

https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2019

/usr/local/share/dotnet/sdk/5.0.101/Sdks

https://stackoverflow.com/questions/56176534/creating-a-custom-sdk-in-net-core

https://reynders.co/simplify-your-dotnet-with-custom-sdks/

https://devblogs.microsoft.com/dotnet/the-evolving-infrastructure-of-net-core/ 


https://docs.github.com/en/actions/learn-github-actions/managing-complex-workflows#creating-dependent-jobs

https://dev.to/pierresaid/deploy-node-projects-to-github-pages-with-github-actions-4jco

https://github.com/maxhamulyak/GithubActionTests/blob/main/.github/workflows/dotnet.yml


https://dev.to/kurtmkurtm/testing-net-core-apps-with-github-actions-3i76



https://github.com/actions/starter-workflows/blob/main/ci/dotnet.yml

https://github.com/actions/starter-workflows/blob/main/ci/dotnet-desktop.yml

      - uses: actions/checkout@v2
        with:
          fetch-depth: 0 # avoid shallow clone so nbgv can do its work.

env:
  BuildConfig: Release
  SolutionFile: MySolution.sln

      - name: Build with dotnet
        run: dotnet build
              --configuration ${{ env.BuildConfig }}
              /p:Version=${{ steps.nbgv.outputs.AssemblyVersion }}

      - name: Test with dotnet
        run: dotnet test ${{ env.SolutionFile }}




—
https://www.michalbialecki.com/2020/01/30/pimp-your-repo-with-github-actions/
    - name: Build with dotnet
      run: dotnet build --configuration Release
    - name: Run unit tests
      run: dotnet test --no-build --configuration Release


———

steps:
        # Checkout and Setup .NET Core
      - uses: actions/checkout@v2

      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '5.0.100'

      - name: Install dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Test
        run: dotnet test --no-restore --verbosity normal

      - name: dotnet publish
        run: dotnet publish --configuration Release --output 'dotnetcorewebapp'

https://dev.to/kurtmkurtm/testing-net-core-apps-with-github-actions-3i76

-----------------

https://docs.github.com/en/free-pro-team@latest/actions/guides

https://docs.github.com/en/free-pro-team@latest/actions/managing-workflow-runs

https://docs.github.com/en/free-pro-team@latest/actions/learn-github-actions
https://docs.github.com/en/free-pro-team@latest/github/working-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site

https://dev.to/pierresaid/deploy-node-projects-to-github-pages-with-github-actions-4jco

https://github.com/SethClydesdale/sethclydesdale.github.io

https://dev.to/kurtmkurtm/testing-net-core-apps-with-github-actions-3i76

