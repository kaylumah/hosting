# XUnit Memberdata


        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { "# Test", string.Empty };
            // yield return new object[] { string.Empty, string.Empty };

            // yield return new object[] { "*foo*", "<p><em>foo</em></p>\n" };

            // yield return new object[] { "# This is the header", "<h1 id=\"this-is-the-header\">This is the header</h1>\n" };
            // yield return new object[] { "## This is the header", "<h2 id=\"this-is-the-header\">This is the header</h2>\n" };
            // yield return new object[] { "### This is the header", "<h3 id=\"this-is-the-header\">This is the header</h3>\n" };
            // yield return new object[] { "#### This is the header", "<h4 id=\"this-is-the-header\">This is the header</h4>\n" };
            // yield return new object[] { "##### This is the header", "<h5 id=\"this-is-the-header\">This is the header</h5>\n" };
            // yield return new object[] { "###### This is the header", "<h6 id=\"this-is-the-header\">This is the header</h6>\n" };

            // // 6 Inlines
            // yield return new object[] { "`foo`", "<p><code>foo</code></p>\n" };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        //[InlineData("", "")]
        // [InlineData("*Hello*", "")]
        public void Test1(string input, string expected)
        {
            var actual = _fixture.Render.Render(input);
            Assert.Equal(expected, actual);
        }
