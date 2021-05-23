chrome.webRequest.onBeforeRequest.addListener(
    function(details) {
         //console.log(details.url);
         return {cancel: true};
    },
    {
        urls:["*://*.dmzj.com/js/ad*",
              "*://*.dmzj.com/css/share/share_css.css",
              "*://*.dmzj1.com/js/ad*",
              "*://*.dmzj1.com/css/share/share_css.css"],
        types: ["script", "stylesheet"]
    },
    ["blocking"]
);

function Remove_page(){
    let i=document.getElementById("center_box");
    let l=i.getElementsByTagName("img").length;
    for(let t=0;t<l;t++){
        i.getElementsByTagName("img")[t].style='border: none;padding: 0px';
        i.getElementsByClassName('curr_page')[t].hidden=true;
        i.getElementsByClassName('inner_img')[t].style='';
    }
}
