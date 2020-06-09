chrome.webRequest.onBeforeRequest.addListener(
    function(details) {
         //console.log(details.url);
         return {cancel: true};
    },
    {
        urls:["*://*.dmzj.com/js/ad*", "*://*.dmzj.com/css/share/share_css.css"],
        types: ["script", "stylesheet"]
    },
    ["blocking"]
);