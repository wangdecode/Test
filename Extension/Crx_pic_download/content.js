//发消息，激活popup
chrome.runtime.sendMessage({"message": "activate_icon"});

//监听广播
chrome.runtime.onMessage.addListener(function(request, sender, sendResponse) {
    if(request.cmd == 'start') {
        let path = request.path;
        let urls = pic_explain();
        //console.log(path, urls);
        chrome.runtime.sendMessage({
            "message": "download", "urls": urls, "path": path});
    }
    
    if(request.cmd == 'getnum') {
        let num = pic_explain().length;
        chrome.runtime.sendMessage({
            "message": "pic_num", "num": num});
    }
});

//图片解析
function pic_explain() {
    let pics = document.getElementsByTagName("img");
    if (pics.length == 0) return;
    
    let pics_url = new Array();
    for (i=0;i<pics.length;i++) {
        pics_url[i]=pics[i].src;
    }
    
    return pics_url;
}
