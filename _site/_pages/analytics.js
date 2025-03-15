(function() {
    let googleAnalyticsID = window.GA_ID;
    if (googleAnalyticsID !== undefined) {
        window.dataLayer = window.dataLayer || [];
        function gtag(){ dataLayer.push(arguments); }

        gtag('js', new Date());
        gtag('config', googleAnalyticsID);

        window.setGtagConsentStatus = function(mode, status) {
            if (mode !== "update" && mode !== "default") {
                throw new Error(`Mode ${mode} is not supported!`);
            }
            if (status !== "denied" && status !== "granted") {
                throw new Error(`Status ${status} is not supported!`);
            }
            gtag('consent', mode, {
                'ad_storage': status,
                'ad_user_data': status,
                'ad_personalization': status,
                'analytics_storage': status
            });
        };

        if (window.getCookieConsent && window.getCookieConsent() === "granted") {
            window.setGtagConsentStatus("default", "granted");
        } else {
            window.setGtagConsentStatus("default", "denied");
        }
    }
})();