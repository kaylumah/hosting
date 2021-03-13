const { Checker, Rules } = require('seo-html-defect-checker')
var fs = require('fs');
const glob = require('glob');

var __dirname = "../../dist";

glob(__dirname + '/**/*.html', {}, (err, files) => {
    for(var i in files)
    {
        var file = files[i];
        console.log(file);
        const htmlText = fs.readFileSync(file).toString();
        const c = new Checker(htmlText)
        c.check(Rules.definedRules.defaultRuleList, (results) => { // results is array
            // manipulate results
            console.log(results);
        })
    }
})